using Cinemachine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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
    public Animator LeftFireController;
    public Animator RightFireController;

    public TextMeshProUGUI text;

    private Dictionary<int, bool> _readyStatePlayer = new();
    private bool _isReady = false;
    private Dictionary<int, Vector3> positionForSpawn = new Dictionary<int, Vector3>()
    {
        { 0, new Vector3(-2.9f, 5.9f, 0.0f)},
        { 1, new Vector3(-3.7f, 5.7f, 0.0f)},
        { 2, new Vector3(-4.5f, 5.5f, 0.0f)},
        { 3, new Vector3(-5.3f, 5.3f, 0.0f)},
        { 4, new Vector3(-6.1f, 5.1f, 0.0f)},
        { 5, new Vector3(2.9f, 5.9f, 0.0f)},
        { 6, new Vector3(3.7f, 5.7f, 0.0f)},
        { 7, new Vector3(4.5f, 5.5f, 0.0f)},
        { 8, new Vector3(5.3f, 5.3f, 0.0f)},
        { 9, new Vector3(6.1f, 5.1f, 0.0f)},
    };

    void Start()
    {
        StartFireAnim();

        SetCountPlayerText();

        AddListenersForButton();

        if (PhotonNetwork.IsMasterClient)
        {
            StartGameBtn.gameObject.SetActive(true);
            Spawn();
        }

        PhotonNetwork.LocalPlayer.CustomProperties.Clear();
    }

    #region Methods

    private void StartFireAnim()
    {
        LeftFireController.Play("LeftFire");
        RightFireController.Play("RightFire");
    }

    private void SetCountPlayerText()
    {
        var allPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        var readyPlayerCount = _readyStatePlayer.Where(x => x.Value == true).Count();

        ReadyPlayerText.text = $"{readyPlayerCount}/{allPlayerCount}";

        if (allPlayerCount < 2 || readyPlayerCount < allPlayerCount)
        {
            StartGameBtn.interactable = false;
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                var players = ShufflePlayers(PhotonNetwork.PlayerList);

                RaiseEventOptions options = new();

                if (players.Length < 6)
                {
                    options.TargetActors = new int[] { players[0].ActorNumber };
                }
                else
                {
                    options.TargetActors = new int[] { players[0].ActorNumber, players[1].ActorNumber };
                }

                var sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(43, true, options, sendOptions);
            }
            
            StartGameBtn.interactable = true;
        }
    }

    private Player[] ShufflePlayers(Player[] list)
    {
        var rand = new System.Random();

        for (var i = list.Length - 1; i >= 1; i--)
        {
            var j = rand.Next(i + 1);

            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
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
        var player = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);

        var posForSpawn = GetPosSpawn();
        if (posForSpawn.x > 0) player.GetComponent<PlayerController>().IsRightPlayer = false;

        player.transform.position = posForSpawn;
        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = player.transform;
    }

    private Vector3 GetPosSpawn()
    {
        var rand = Random.Range(0, positionForSpawn.Count - 1);
        return positionForSpawn[rand];
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
        _readyStatePlayer[PhotonNetwork.LocalPlayer.ActorNumber] = _isReady;

        if (_isReady)
            ReadyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Not Ready";
        else
            ReadyBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";

        SendDataAboutState();
    }

    private void SendDataAboutState()
    {
        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        var sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(42, _readyStatePlayer, options, sendOptions);
    }

    #endregion

    #region Callbacks

    public override void OnLeftRoom()
    {
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

            _readyStatePlayer.Remove(otherPlayer.ActorNumber);
            SendDataAboutState();
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 42)
        {
            var newReadyStatePlayer = photonEvent.CustomData as Dictionary<int, bool>;
            _readyStatePlayer = newReadyStatePlayer;
            SetCountPlayerText();
        }
        if (photonEvent.Code == 43)
        {
            text.text += $"Получена рассылка ролей: {photonEvent.CustomData}\n";
            text.text += $"ActorNumber = {PhotonNetwork.LocalPlayer.ActorNumber}\n";
            var hT = new ExitGames.Client.Photon.Hashtable();
            hT["isImposter"] = (bool)photonEvent.CustomData;
            PhotonNetwork.LocalPlayer.CustomProperties = hT;
        }
    }

    #endregion
}
