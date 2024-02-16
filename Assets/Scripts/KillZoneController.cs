using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KillZoneController : MonoBehaviour
{
    private GameObject _targetForKill;
    private GameObject _targetForVent;
    private Button _killBtn;
    private Button _ventBtn;
    private bool _inVent = false;

    private LayerMask _ventLayer;
    private LayerMask _playerLayer;

    public Material PlayerMat;
    public Material OutlinePlayerMat;
    public Material VentMat;
    public Material OutlineVentMat;


    public static Action OnMoveInVent;
    public static Action OnMoveOutVent;

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

    private void OnEnable()
    {
        VentsManager.ChangeVents += ChangeVents;
    }

    private void OnDisable()
    {
        VentsManager.ChangeVents -= ChangeVents;
    }

    void Start()
    {
        KillButton.onClick.AddListener(Kill);
        KillButton.interactable = false;
        VentButton.onClick.AddListener(InteractWithVent);
        VentButton.interactable = false;

        _ventLayer = LayerMask.NameToLayer("VentZone");
        _playerLayer = LayerMask.NameToLayer("Player");
    }

    private void Kill()
    {
        if (_targetForKill == null) return;

        var targetActor = _targetForKill.GetComponent<PhotonView>().OwnerActorNr;
        var killerID = transform.parent.GetComponent<PhotonView>().ViewID;

        var options = new RaiseEventOptions { TargetActors = new int[] { targetActor } };
        var sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(99, killerID, options, sendOptions);

        _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
    }

    private void ChangeVents(Transform targetVents)
    {
        if (_inVent)
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

        if (_inVent)
        {
            animator.SetTrigger("MoveOutVent");
            StartCoroutine(WaitForAnimation(animator));
        } 
        else 
        {
            PlayerMoveToVent();
            animator.SetTrigger("MoveInVent");
        }

    }

    IEnumerator WaitForAnimation(Animator animator)
    {
        var lengthAnim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(lengthAnim);
        PlayerMoveOutVent();
    }


    private void PlayerMoveOutVent()
    {
        OnMoveOutVent?.Invoke();

        ChangePlayerLayer(_playerLayer);

        _inVent = false;
    }

    private void PlayerMoveToVent()
    {
        OnMoveInVent?.Invoke();

        MoveToVents(_targetForVent.transform);

        ChangePlayerLayer(_ventLayer);

        _inVent = true;
    }


    private void MoveToVents(Transform target)
    {
        var player = transform.parent;
        player.position = target.position + new Vector3(0.0f, 0.6f, 0.0f);
    }

    private void ChangePlayerLayer(LayerMask targetLayer)
    {
        var player = transform.parent;
        player.gameObject.layer = targetLayer;
        player.transform.GetChild(0).gameObject.layer = targetLayer;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Vent"))
        {
            if (_targetForVent == null)
            {
                _targetForVent = other.gameObject;

                other.gameObject.GetComponent<SpriteRenderer>().material = OutlineVentMat;
                VentButton.interactable = true;
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
        if (other.CompareTag("Vent"))
        {
            other.gameObject.GetComponent<SpriteRenderer>().material = VentMat;
            _targetForVent = null;
            VentButton.interactable = false;
        }


        if (_targetForKill == null) return;

        if (other.CompareTag("Player"))
        {
            _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
            _targetForKill = null;
            KillButton.interactable = false;
        }
    }
}
