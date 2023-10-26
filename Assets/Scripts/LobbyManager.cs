using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public GameObject Cinemachine;
    public Button BackButton;

    void Start()
    {
        BackButton.onClick.AddListener(Leave);

        if (PhotonNetwork.IsMasterClient)
            Spawn();
    }

    private void Leave()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // current player left room
        SceneManager.LoadScene(1);
    }

    private void Spawn()
    {
        var pos = new Vector2(Random.Range(-2, 2), Random.Range(-3, 3));
        var gameObject = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = gameObject.transform;
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
            Spawn();
    }

    public void StartGame()
    {

    }
}
