using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;

public class ChatManager : MonoBehaviour, IChatClientListener
{

    ChatClient chatClient;

    public TMP_InputField TextMessageInputField;
    public Button SendMessageBtn;
    public TextMeshProUGUI CountCharacterText;
    public Transform Content;
    public GameObject MessageSenderPrefab;
    public GameObject MessageReceiverPrefab;

    private const string CHANNEL = "GLOBAL";


    void Start()
    {
        chatClient = new ChatClient(this);

        var appId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        var appVersion = PhotonNetwork.AppVersion;
        var userId = PhotonNetwork.LocalPlayer.NickName;

        chatClient.Connect(appId, appVersion, new AuthenticationValues(userId));

        TextMessageInputField.onValueChanged.AddListener(TextMessageChanged);

        SendMessageBtn.onClick.AddListener(SendMessage);
    }

    private void TextMessageChanged(string newText)
    {
        CountCharacterText.text = newText.Length.ToString();
    }

    private void SendMessage()
    {
        if (chatClient == null) return;
        if (string.IsNullOrEmpty(TextMessageInputField.text)) return;

        chatClient.PublishMessage(CHANNEL, TextMessageInputField.text);

        TextMessageInputField.text = string.Empty;
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"lvl: {level} - msg: {message}");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log(state);
    }

    public void OnConnected()
    {
        Debug.Log("Connected");
        chatClient.Subscribe(CHANNEL);
    }

    public void OnDisconnected()
    {
        chatClient.Unsubscribe(new string[] { CHANNEL });
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (var i = 0; i < senders.Length; i++)
        {
            GameObject msgPrefab = null;

            if (senders[i] == PhotonNetwork.LocalPlayer.NickName)
            {
                msgPrefab = Instantiate(MessageSenderPrefab, Content);
            } else
            {
                msgPrefab = Instantiate(MessageReceiverPrefab, Content);
            }

            if (msgPrefab != null)
            {
                msgPrefab.GetComponent<MessageManager>().SetUserName(senders[i]);
                msgPrefab.GetComponent<MessageManager>().SetMessage(messages[i] as string);
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (var channel in channels)
        {
            Debug.Log($"Subscriped to {channel}");
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        foreach (var channel in channels)
        {
            Debug.Log($"Unsubscriped from {channel}");
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user} subscriped to {channel}");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user} unsubscriped from {channel}");
    }


    void Update()
    {
        chatClient.Service();
    }

}
