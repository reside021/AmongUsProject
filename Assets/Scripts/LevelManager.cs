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
using TMPro;

public class LevelManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject Cinemachine;
    [SerializeField] private Button KillButton;
    [SerializeField] private Button VentButton;
    [SerializeField] private Button UseButton;
    [SerializeField] private Button ReportButton;
    [SerializeField] private Button SabotageButton;
    [SerializeField] private Camera Camera;
    [SerializeField] private Transform DeathPanel;
    [SerializeField] private GameObject KillScene;
    [SerializeField] private GameObject Zone;
    [SerializeField] private GameObject DeadBodyReported;
    [SerializeField] private Animator VotingUIAnimator;
    [SerializeField] private Animator DeadBodyRepAnimator;
    [SerializeField] private GameObject Kick;
    [SerializeField] private Transform PlayerOnMap;
    [SerializeField] private Transform ZeroPointGlobal;
    [SerializeField] private Transform ZeroPointMap;
    [SerializeField] private Transform Tasks;
    [SerializeField] private TextMeshProUGUI TaskCount;

    private int _maxTasks = 10;
    private int _currentTask = 0;
    private GameObject _player;
    private float _coefForMap = 12.0f;

    private List<int> _impostors = new();


    private Dictionary<int, float[]> positionForSpawn = new ()
    {
        { 0, new float[4] { -4.25f, 4.0f, 2.2f, 9.7f }},
        { 1, new float[4] { -14.35f, -4.6f, -6.3f, 3.5f }},
        { 2, new float[4] { 4.7f, -4.5f, 12.0f, 3.9f }},
        { 3, new float[4] { -4.0f, -11.0f, 1.8f, -4.7f }},
    };


    public static Action<int> OnTabletOpened;
    public static Action<int, int> OnPlayerVoted;
    public static Action<string, string> OnVoteEnds;
    public static Action OnOpenUI;


    private void OnEnable()
    {
        base.OnEnable();
        TabletUI.OnKickPlayer += OnKickPlayer;
        ZoneController.OnTaskUsed += OnTaskUsed;
    }


    void Start()
    {

        var pos = GetPosSpawn();

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

    private Vector2 GetPosSpawn()
    {
        var index = UnityEngine.Random.Range(0, positionForSpawn.Count - 1);


        var x = UnityEngine.Random.Range(positionForSpawn[index][0], positionForSpawn[index][2]);
        var y = UnityEngine.Random.Range(positionForSpawn[index][1], positionForSpawn[index][3]);

        return new Vector2(x, y);
    }


    void Update()
    {
        if (_player != null)
        {
            var diffGlobal = _player.transform.position - ZeroPointGlobal.position;
            var diffMap = ZeroPointMap.localPosition + diffGlobal * _coefForMap;

            PlayerOnMap.transform.localPosition = diffMap;
        }
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
        if (photonEvent.Code == 98)
        {
            _impostors.Add(photonEvent.Sender);
        }
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
            OnOpenUI?.Invoke();
            OnTabletOpened?.Invoke(finderID);
            DeadBodyDestroy(murderedID);
            StartCoroutine(DisplayBeforeVoting());
        }
        if (photonEvent.Code == 10)
        {
            var data = (int)photonEvent.CustomData;
            var sender = photonEvent.Sender;

            OnPlayerVoted?.Invoke(data, sender);
            
        }
        if (photonEvent.Code == 51)
        {
            var data = (int)photonEvent.CustomData;
            var sender = photonEvent.Sender;


            _currentTask++;
            TaskCount.text = $"{_currentTask}/{TaskCount}";

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

    IEnumerator DisplayDeathScreen(int killerID)
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


    private void OnKickPlayer(int actNum)
    {
        var resultText = string.Empty;
        if (actNum == -1)
        {
            resultText = "No one was ejected";
        } 
        else
        {
            var nickName = PhotonNetwork.PlayerList.ToList().Find(x => x.ActorNumber == actNum).NickName;

            if (_impostors.Contains(actNum))
            {
                resultText = $"{nickName} was The Impostor";
            }
            else
            {
                resultText = $"{nickName} was not The Impostor";
            }
        }

        foreach (var view in PhotonNetwork.PhotonViewCollection)
        {
            if (!view.CompareTag("Player")) continue;

            if (view.ControllerActorNr == actNum)
            {
                view.GetComponent<PlayerController>().IsKicked = true;
                view.GetComponent<PlayerController>().IsDead = true;

                _impostors.Remove(actNum);
            }
        }

        var remainsText = $"{_impostors.Count} Impostor remains.";

        Kick.SetActive(true);

        OnVoteEnds?.Invoke(resultText, remainsText);

    }


    private void DeadBodyDestroy(int id)
    {
        var deads = GameObject.FindGameObjectsWithTag("DeadBody");

        GameObject objForDestroy = null;

        foreach (var dead in deads)
        {
            if (dead.GetComponent<DeadBodyId>().ID == id)
            {
                objForDestroy = dead;
            }
        }

        if (objForDestroy != null)
        {
            Destroy(objForDestroy);
        }
    }


    private void OnTaskUsed()
    {
        var taskCount = Tasks.childCount;

        int indexTask = UnityEngine.Random.Range(0, taskCount + 1);

        var task = Tasks.GetChild(indexTask);

        task.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        TabletUI.OnKickPlayer -= OnKickPlayer;
        ZoneController.OnTaskUsed -= OnTaskUsed;
    }
}
