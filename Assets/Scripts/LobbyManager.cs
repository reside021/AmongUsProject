using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject PlayerPrefab;
    public GameObject Cinemachine;
    public Button BackButton;
    public Button StartGameBtn;
    public Button ReadyBtn;
    public TextMeshProUGUI ReadyPlayerText;

    private Dictionary<string, bool> _readyStatePlayer = new Dictionary<string, bool>();
    private bool _isReady = false;

    void Start()
    {
        SetCountPlayerText();

        AddListenersForButton();

        if (PhotonNetwork.IsMasterClient)
        {
            StartGameBtn.gameObject.SetActive(true);
            Spawn();
        }
    }

    #region Methods

    private void SetCountPlayerText()
    {
        var allPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        var readyPlayerCount = _readyStatePlayer.Where(x => x.Value == true).Count();

        ReadyPlayerText.text = $"{readyPlayerCount}/{allPlayerCount}";

        //if (allPlayerCount < 2 || readyPlayerCount < allPlayerCount)
        if (readyPlayerCount < allPlayerCount)
        {
            StartGameBtn.interactable = false;
        }
        else
        {
            StartGameBtn.interactable = true;
        }
    }

    private void AddListenersForButton()
    {
        BackButton.onClick.AddListener(Leave);
        StartGameBtn.onClick.AddListener(StartGame);
        ReadyBtn.onClick.AddListener(ReadyForGame);
    }

    private void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void Spawn()
    {
        var pos = new Vector2(Random.Range(-2, 2), Random.Range(-3, 3));
        var gameObject = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = gameObject.transform;
    }

    private void StartGame()
    {
        PhotonNetwork.DestroyAll();
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(3);
    }

    private void ReadyForGame()
    {
        _isReady = !_isReady;
        _readyStatePlayer[PhotonNetwork.NickName] = _isReady;

        if (_isReady)
            ReadyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Not Ready";
        else
            ReadyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";

        SendDataAboutState();
    }

    private void SendDataAboutState()
    {
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(42, _readyStatePlayer, options, sendOptions);
    }

    #endregion

    #region Callbacks

    public override void OnLeftRoom()
    {
        // current player left room
        SceneManager.LoadScene(1);
    }
    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
            Spawn();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SendDataAboutState();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameBtn.gameObject.SetActive(true);

            _readyStatePlayer.Remove(otherPlayer.NickName);
            SendDataAboutState();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 42)
        {
            var newReadyStatePlayer = photonEvent.CustomData as Dictionary<string, bool>;
            _readyStatePlayer = newReadyStatePlayer;
            SetCountPlayerText();
        }
    }

    #endregion
}
