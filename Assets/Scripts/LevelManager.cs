using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Cinemachine;
using System;

public class LevelManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public GameObject Cinemachine;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            Spawn();

    }

    public void Leave()
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
        var pos = new Vector2(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-3, 3));
        var gameObject = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = gameObject.transform;

    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
            Spawn();
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
