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
    public Button VentButton;
    public Button UseButton;
    public Button ReportButton;
    public Button SabotageButton;
    public Camera Camera;
    public Transform DeathPanel;
    public GameObject KillScene;
    public GameObject Zone;
    public GameObject DeadBodyReported;
    public Animator VotingUIAnimator;
    public Animator DeadBodyRepAnimator;

    private GameObject _player;


    public static Action<int> OnTabletOpened;

    void Start()
    {
        AddListenersForButton();

        var pos = new Vector2(UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-3, 3));

        if (!PhotonNetwork.InRoom) return;

        _player = PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);
        _player.GetComponent<PlayerController>().Camera = Camera;

        var virtualCamera = Cinemachine.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.Follow = _player.transform;

        var zone = Instantiate(Zone, _player.transform);
        zone.GetComponent<ZoneController>().KillButton = KillButton;
        zone.GetComponent<ZoneController>().VentButton = VentButton;
        zone.GetComponent<ZoneController>().UseButton = UseButton;
        zone.GetComponent<ZoneController>().ReportButton = ReportButton;
        zone.GetComponent<ZoneController>().SabotageButton = SabotageButton;

    }

    private void AddListenersForButton()
    {
        BackButton.onClick.AddListener(Leave);
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
            var killerID = (int)photonEvent.CustomData;

            StartCoroutine(DisplayDeathScreen(killerID));
            _player.GetComponent<PlayerController>().IsDead = true;
        }
        if (photonEvent.Code == 100)
        {
            var data = photonEvent.CustomData as Dictionary<string, int>;
            var finderID = data["finderID"];
            var murderedID = data["murderedID"];

            OnTabletOpened?.Invoke(finderID);



            StartCoroutine(DisplayBeforeVoting());
        }
    }

    IEnumerator DisplayBeforeVoting()
    {
        DeadBodyReported.SetActive(true);
        var lenghAnimReported = DeadBodyRepAnimator.GetCurrentAnimatorStateInfo(0).length;
        DeadBodyRepAnimator.Play(0);
        yield return new WaitForSeconds(lenghAnimReported);
        DeadBodyReported.SetActive(false);
        VotingUIAnimator.SetTrigger("OpenVotingUI");
    }

    IEnumerator DisplayDeathScreen(int killerActNum)
    {
        //var objects = GameObject.FindGameObjectsWithTag("Player");
        //var gameObject = objects.First(x => x.GetComponent<PhotonView>().ViewID == killerID);

        DeathPanel.gameObject.SetActive(true);

        var killScene = Instantiate(KillScene, DeathPanel);
        var animator = killScene.GetComponent<Animator>();
        animator.SetBool("KillAlien", true);

        var lengthAnim = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(lengthAnim);
        animator.SetBool("KillAlien", false);

        DeathPanel.gameObject.SetActive(false);
    }
}
