using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackZoneController : MonoBehaviour
{
    public Material PlayerMat;
    public Material OutlinePlayerMat;
    public SpriteRenderer SpriteRenderer;


    private bool _isImposter = false;
    private GameObject _targetForKill = null;


    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _isImposter = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_isImposter) return;

        if (other.gameObject.CompareTag("AttackZone")) return;

        if (other.GetComponent<PhotonView>().IsMine) return;

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<SpriteRenderer>().material = PlayerMat;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_isImposter) return;

        if (other.gameObject.CompareTag("AttackZone")) return;

        if (other.GetComponent<PhotonView>().IsMine) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (_targetForKill == null)
            {
                _targetForKill = other.gameObject;
            }

            var distForCurrentTarget = Vector3.Distance(_targetForKill.transform.position, SpriteRenderer.transform.position);
            var distForOtherTarget = Vector3.Distance(other.transform.position, SpriteRenderer.transform.position);

            if (distForOtherTarget < distForCurrentTarget)
            {
                _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
                _targetForKill = other.gameObject;
            }
            _targetForKill.GetComponent<SpriteRenderer>().material = OutlinePlayerMat;
        }
    }
}
