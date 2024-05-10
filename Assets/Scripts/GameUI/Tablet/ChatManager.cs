using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using System;

public class ChatManager : MonoBehaviour, IChatClientListener
{

    ChatClient chatClient;

    [SerializeField] private TMP_InputField TextMessageInputField;
    [SerializeField] private Button SendMessageBtn;
    [SerializeField] private TextMeshProUGUI CountCharacterText;
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject MessageSenderPrefab;
    [SerializeField] private GameObject MessageReceiverPrefab;

    private const string CHANNEL = "GLOBAL";
    private const string GHOSTCH = "GHOST_CHANNEL";

    private string _currentChannel = CHANNEL;


    private void OnEnable()
    {
        LevelManager.OnGhosted += OnGhosted;
    }

    void Start()
    {
        chatClient = new ChatClient(this);

        var appId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;
        var appVersion = PhotonNetwork.AppVersion;
        var userId = PhotonNetwork.LocalPlayer.NickName;

        chatClient.Connect(appId, appVersion, new AuthenticationValues(userId));

        TextMessageInputField.onValueChanged.AddListener(TextMessageChanged);
        TextMessageInputField.onEndEdit.AddListener(InputOnEndEdit);

        SendMessageBtn.onClick.AddListener(SendMessage);
    }


    private void OnGhosted()
    {
        chatClient.Unsubscribe(new string[] { _currentChannel });
        _currentChannel = GHOSTCH;
        chatClient.Subscribe(_currentChannel);
    }

    private void InputOnEndEdit(string inputString)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage();
        }
    }

    private void TextMessageChanged(string newText)
    {
        CountCharacterText.text = newText.Trim().Length.ToString();
    }

    private void SendMessage()
    {
        if (chatClient == null) return;
        if (string.IsNullOrEmpty(TextMessageInputField.text)) return;
        chatClient.PublishMessage(_currentChannel, TextMessageInputField.text.Trim());

        TextMessageInputField.text = string.Empty;

        TextMessageInputField.ActivateInputField();
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
        chatClient.Subscribe(_currentChannel);
    }

    public void OnDisconnected()
    {
        chatClient.Unsubscribe(new string[] { _currentChannel });
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


    private void OnDisable()
    {
        LevelManager.OnGhosted -= OnGhosted;
    }

}
