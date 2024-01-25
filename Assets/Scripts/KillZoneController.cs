using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KillZoneController : MonoBehaviour
{
    private GameObject _targetForKill;
    private GameObject _targetForVent;
    private Button _killBtn;
    private Button _ventBtn;

    public Material PlayerMat;
    public Material OutlinePlayerMat;
    public Material VentMat;
    public Material OutlineVentMat;


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

    void Start()
    {
        KillButton.onClick.AddListener(Kill);
        KillButton.interactable = false;
        VentButton.onClick.AddListener(GoVent);
        VentButton.interactable = false;
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

    private void GoVent()
    {
        StartCoroutine(MoveVent());
    }

    IEnumerator MoveVent()
    {
        var ventilation = _targetForVent.transform.parent;
        var animator = ventilation.GetComponent<Animator>();
        animator.SetBool("MoveVent", true);
        var lengthAnim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        yield return new WaitForSeconds(lengthAnim);

        animator.SetBool("MoveVent", false);
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
