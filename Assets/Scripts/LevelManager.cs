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
using System.Collections.Generic;

public class LevelManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject PlayerPrefab;
    public GameObject Cinemachine;
    public Button BackButton;
    public Button KillButton;
    public Camera Camera;
    public Transform DeathPanel;
    public GameObject PlayerDeathScreen;


    private GameObject _player;


    void Start()
    {
        AddListenersForButton();

        var pos = new Vector2(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-3, 3));
        _player = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);

        _player.GetComponent<PlayerController>().KillButton = KillButton;
        _player.GetComponent<PlayerController>().Camera = Camera;

        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = _player.transform;

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

            var killerActor = (int)photonEvent.CustomData;

            DisplayDeathScreen(killerActor);
            _player.GetComponent<PlayerController>().IsDead = true;
        }
    }


    private void DisplayDeathScreen(int killerID)
    {
        DeathPanel.gameObject.SetActive(true);
        Camera.gameObject.SetActive(false);
        var objects = GameObject.FindGameObjectsWithTag("Player");

        var gameObject = objects.First(x => x.GetComponent<PhotonView>().ViewID == killerID);

        var victim = Instantiate(PlayerDeathScreen, DeathPanel);
        victim.transform.localPosition = new Vector3(-200.0f, 0.0f, 0.0f);
        var killer = Instantiate(PlayerDeathScreen, DeathPanel);
        killer.transform.localPosition = new Vector3(200.0f, 0.0f, 0.0f);
        victim.GetComponent<Animator>().SetBool("IsKilled", true);
        killer.GetComponent<Animator>().SetBool("IsKill", true);
    }
}
