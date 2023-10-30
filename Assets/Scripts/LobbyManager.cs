using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public GameObject Cinemachine;
    public Button BackButton;
    public Button StartGameBtn;
    public Button ReadyBtn;

    public TextMeshProUGUI ReadyPlayerText;

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


    private void SetCountPlayerText()
    {
        var allPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        string readyPlayerCount = "0";
        ReadyPlayerText.text = $"{readyPlayerCount}/{allPlayerCount}";
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetCountPlayerText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
            StartGameBtn.gameObject.SetActive(true);


        SetCountPlayerText();
    }

    private void StartGame()
    {

    }

    private void ReadyForGame()
    {
        
    }
}
