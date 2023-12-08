using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class KillZoneController : MonoBehaviour
{
    private GameObject _targetForKill;
    private Button _killBtn;

    public Material PlayerMat;
    public Material OutlinePlayerMat;


    public Button KillButton
    {
        get { return _killBtn; }
        set { _killBtn = value; }

    }

    void Start()
    {
        _killBtn.onClick.AddListener(Kill);
        _killBtn.interactable = false;
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


    private void OnTriggerStay2D(Collider2D other)
    {
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
            _killBtn.interactable = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (_targetForKill == null) return;

        if (other.CompareTag("Player"))
        {
            _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
            _targetForKill = null;
            _killBtn.interactable = false;
        }
    }
}
