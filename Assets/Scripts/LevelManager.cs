using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Cinemachine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using ExitGames.Client.Photon;

public class LevelManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject PlayerPrefab;
    public GameObject Cinemachine;
    public Button BackButton;
    public Button KillButton;

    private GameObject _player;


    void Start()
    {
        AddListenersForButton();

        var pos = new Vector2(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-3, 3));
        _player = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);

        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = gameObject.transform;

    }

    private void AddListenersForButton()
    {
        BackButton.onClick.AddListener(Leave);
    }

    private void Update()
    {

    }

    public void Leave()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        // current player left room
        SceneManager.LoadScene(1);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 99)
        {
            var isDead = (bool)photonEvent.CustomData;

            if (isDead)
            {
                _player.GetComponent<PlayerController>().IsDead = true;
            }
        }
    }
}
