using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Cinemachine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public GameObject Cinemachine;
    public Button BackButton;
    public Button KillButton;

    void Start()
    {
        AddListenersForButton();

        var pos = new Vector2(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-3, 3));
        var gameObject = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = gameObject.transform;
    }
    private void AddListenersForButton()
    {
        BackButton.onClick.AddListener(Leave);
        KillButton.onClick.AddListener(KillPlayer);
    }

    private void Update()
    {
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //foreach (GameObject p in players)
        //{
        //    Debug.Log(p.transform.position);
        //}
    }

    private void KillPlayer()
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

}
