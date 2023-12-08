using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TMP_InputField NickNameInput;

    public TextMeshProUGUI LogText;

    public Button CreateRoomBtn;
    public Button JoinRoomBtn;
    public Button SaveNickNameBtn;
    public Button ExitGameBtn;
    void Start()
    {

        AddListenersForButton();

        if (PlayerPrefs.HasKey("NickName"))
            PhotonNetwork.NickName = PlayerPrefs.GetString("NickName");

        NickNameInput.text = PhotonNetwork.NickName;
    }

    private void AddListenersForButton()
    {
        CreateRoomBtn.onClick.AddListener(CreateRoom);
        JoinRoomBtn.onClick.AddListener(JoinRoom);
        SaveNickNameBtn.onClick.AddListener(SetNewNickName);
        ExitGameBtn.onClick.AddListener(ExitGame);
    }


    public void CreateRoom()
    {
        if (!string.IsNullOrWhiteSpace(createInput.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 10;
            PhotonNetwork.CreateRoom(createInput.text, roomOptions);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        LogText.text = message;
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrWhiteSpace(joinInput.text))
            PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {

        PhotonNetwork.LoadLevel(2);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LogText.text = message;
    }

    public void SetNewNickName()
    {
        string newNickName = NickNameInput.text;
        if (!string.IsNullOrWhiteSpace(newNickName))
        {
            PhotonNetwork.NickName = newNickName;
            PlayerPrefs.SetString("NickName", NickNameInput.text);
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    private void Log(string message)
    {
        Debug.Log(message);
        LogText.text += "\n";
        LogText.text += message;
    }
}
