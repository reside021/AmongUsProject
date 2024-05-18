using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneController : MonoBehaviour
{
    private GameObject _targetForKill;
    private GameObject _targetForVent;
    private GameObject _targetForDeadBody;
    private GameObject _targerForTask;
    private Button _killBtn;
    private Button _ventBtn;
    private Button _useBtn;
    private Button _reportBtn;
    private Button _sabotageBtn;
    private LayerMask _ventLayer;
    private LayerMask _attackLayer;
    private bool _isImposter;
    private bool _isBlockKill;
    private bool _isReporting;

    private bool _isInVent
    {
        get
        {
            return transform.parent.GetComponent<PlayerController>().IsInVent;
        }

        set
        {
            transform.parent.GetComponent<PlayerController>().IsInVent = value;
        }
    }


    [SerializeField] private Material PlayerMat;
    [SerializeField] private Material OutlinePlayerMat;
    [SerializeField] private Material VentMat;
    [SerializeField] private Material OutlineVentMat;
    [SerializeField] private Material TaskElectricityMat;
    [SerializeField] private Material OutlineTaskElectricityMat;
    [SerializeField] private Material ReportMat;
    [SerializeField] private Material OutlineReportMat;


    public Button KillButton
    {
        get { return _killBtn; }
        set { _killBtn = value; }
    }

    public Button VentButton
    {
        get { return _ventBtn; }
        set { _ventBtn = value; }
    }

    public Button UseButton
    {
        get { return _useBtn; }
        set { _useBtn = value; }
    }

    public Button ReportButton
    {
        get { return _reportBtn; }
        set { _reportBtn = value; }
    }

    public Button SabotageButton
    {
        get { return _sabotageBtn; }
        set { _sabotageBtn = value; }
    }

    public static Action OnTaskUsed;


    private void OnEnable()
    {
        VentsManager.ChangeVents += ChangeVents;
        LevelManager.OnKillUnblocked += OnKillUnblocked;
        LevelManager.OnKillBlocked += OnKillBlocked;
    }

    void Start()
    {

        KillButton.onClick.AddListener(Kill);
        KillButton.interactable = false;
        VentButton.onClick.AddListener(InteractWithVent);
        VentButton.interactable = false;
        UseButton.onClick.AddListener(Use);
        UseButton.interactable = false;
        ReportButton.onClick.AddListener(Report);
        ReportButton.interactable = false;
        SabotageButton.onClick.AddListener(Sabotage);
        SabotageButton.interactable = false;

        _ventLayer = LayerMask.NameToLayer("VentZone");
        _attackLayer = LayerMask.NameToLayer("AttackZone");
        _isImposter = (bool)PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isImposter");


        VentButton.gameObject.SetActive(false);

        if (!_isImposter)
        {
            KillButton.gameObject.SetActive(false); 
            SabotageButton.gameObject.SetActive(false);
        } else
        {
            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            var sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(98, true, options, sendOptions);

            UseButton.gameObject.SetActive(false);
            SabotageButton.gameObject.SetActive(true);
        }

    }

    private void Sabotage()
    {
    }

    private void Use()
    {
        if (_isReporting)
        {
            var reportingID = transform.parent.GetComponent<PhotonView>().ViewID;

            var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            var sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(101, reportingID, options, sendOptions);
            return;
        }

        if (_targerForTask == null) return;

        _targerForTask.GetComponent<ShowTask>().Display();
    }

    private void Report()
    {
        var finderID = transform.parent.GetComponent<PhotonView>().ViewID;
        var murderedID = _targetForDeadBody.GetComponent<DeadBodyId>().ID;

        var data = new Dictionary<string, int>
        {
            { "finderID", finderID },
            { "murderedID", murderedID },
        };

        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        var sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(100, data, options, sendOptions);
    }

    private void Kill()
    {
        if (_isBlockKill) return;

        if (_targetForKill == null) return;

        var targetID = _targetForKill.GetComponent<PhotonView>().ControllerActorNr;
        var killerID = transform.parent.GetComponent<PhotonView>().ViewID;

        var options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        var sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(99, targetID, options, sendOptions);

        _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
        _targetForKill = null;
    }

    private void ChangeVents(Transform targetVents)
    {
        if (_isInVent)
        {
            var ventSource = _targetForVent.transform.parent;
            var deactivatedSource = ventSource.GetComponent<InteractWithPlayer>();
            deactivatedSource.DeactivatedUI();

            MoveToVents(targetVents);

            var activateTarget = targetVents.GetComponent<InteractWithPlayer>();
            activateTarget.ActivatedUI();
        }
    }

    private void InteractWithVent()
    {
        var ventilation = _targetForVent.transform.parent;
        var animator = ventilation.GetComponent<Animator>();
        var interactWithPLayer = ventilation.GetComponent<InteractWithPlayer>();

        if (_isInVent)
        {
            animator.SetBool("OutVent", true);
            interactWithPLayer.DeactivatedUI();
            StartCoroutine(WaitOutVentAnim(animator));
        } 
        else 
        {
            interactWithPLayer.ActivatedUI();
            PlayerMoveToVent(animator);
        }

    }

    IEnumerator WaitOutVentAnim(Animator animator)
    {
        var lengthAnim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(lengthAnim);
        animator.SetBool("OutVent", false);
        PlayerMoveOutVent();
    }


    private void PlayerMoveOutVent()
    {
        gameObject.layer = _attackLayer;
        _isInVent = false;
    }

    private void PlayerMoveToVent(Animator animator)
    {
        gameObject.layer = _ventLayer;
        MoveToVents(_targetForVent.transform);
        _isInVent = true;
        animator.SetBool("InVent", true);
        StartCoroutine(WaitInVentAnim(animator));
    }

    IEnumerator WaitInVentAnim(Animator animator)
    {
        var lengthAnim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(lengthAnim);
        animator.SetBool("InVent", false);
    }


    private void MoveToVents(Transform target)
    {
        var player = transform.parent;
        player.position = target.position + new Vector3(0.0f, 0.6f, 0.0f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("DeadBody"))
        {
            if (_targetForDeadBody == null)
            {
                _targetForDeadBody = other.gameObject;
                ReportButton.interactable = true;
            }
        }

        if (other.CompareTag("Task"))
        {
            if (_targerForTask == null)
            {
                if (_isImposter) return;
                _targerForTask = other.gameObject;
                other.gameObject.GetComponent<SpriteRenderer>().material = OutlineTaskElectricityMat;
                UseButton.interactable = true;
            }
        }

        if (other.CompareTag("Report"))
        {
            other.gameObject.GetComponent<SpriteRenderer>().material = OutlineReportMat;
            UseButton.interactable = true;
            _isReporting = true;
        }


        if (!_isImposter) return;

        if (other.CompareTag("Vent"))
        {
            if (_targetForVent == null)
            {
                _targetForVent = other.gameObject;

                other.gameObject.GetComponent<SpriteRenderer>().material = OutlineVentMat;
                VentButton.gameObject.SetActive(true);
                VentButton.interactable = true;
                SabotageButton.gameObject.SetActive(false);
            }
        }

        if (other.CompareTag("Player"))
        {
            if (_targetForKill == null)
            {
                _targetForKill = other.gameObject;
            }

            var distForCurrentTarget = Vector3.Distance(_targetForKill.transform.position, transform.position);
            var distForOtherTarget = Vector3.Distance(other.transform.position, transform.position);

            if (distForOtherTarget < distForCurrentTarget)
            {
                _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
                _targetForKill = other.gameObject;
            }
            _targetForKill.GetComponent<SpriteRenderer>().material = OutlinePlayerMat;
            KillButton.interactable = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DeadBody"))
        {
            _targetForDeadBody = null;
            ReportButton.interactable = false;
        }

        if (other.CompareTag("Task"))
        {
            if (_targerForTask == null) return;

            if (_isImposter) return;

            other.gameObject.GetComponent<SpriteRenderer>().material = TaskElectricityMat;
            _targerForTask = null;
            UseButton.interactable = false;
        }

        if (other.CompareTag("Report"))
        {
            other.gameObject.GetComponent<SpriteRenderer>().material = ReportMat;
            UseButton.interactable = false;
            _isReporting = false;
        }

        if (!_isImposter) return;

        if (other.CompareTag("Vent"))
        {
            if (_targetForVent == null) return;

            other.gameObject.GetComponent<SpriteRenderer>().material = VentMat;
            _targetForVent = null;
            VentButton.interactable = false;
            VentButton.gameObject.SetActive(false);
            SabotageButton.gameObject.SetActive(true);
        }

        if (other.CompareTag("Player"))
        {
            if (_targetForKill == null) return;

            _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
            _targetForKill = null;
            KillButton.interactable = false;
        }

    }


    private void OnKillUnblocked()
    {
        _isBlockKill = false;
    }
    private void OnKillBlocked()
    {
        _isBlockKill = true;
    }


    private void OnDisable()
    {
        VentsManager.ChangeVents -= ChangeVents;
        LevelManager.OnKillUnblocked -= OnKillUnblocked;
        LevelManager.OnKillBlocked -= OnKillBlocked;
    }

    
}
