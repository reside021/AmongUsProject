using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPunObservable
{
    public float MoveSpeed = 10f;

    public TextMeshProUGUI NickNameText;

    public Material PlayerMat;
    public Material OutlinePlayerMat;
    public Material GhostMat;

    public Sprite PlayerSprite;
    public Sprite GhostSprite;

    public GameObject DeadBodyPrefab;

    private MoveState _moveState = MoveState.Idle;
    private Rigidbody2D _rb;
    private Animator _animatorController;
    private SpriteRenderer _spriteRenderer;

    private GameObject _targetForKill;

    private Button _killBtn;

    private PhotonView _view;

    private bool _isImposter = false;
    private bool _isRightPlayer = true;
    private bool _isDead = false;
    private bool _animOfDeath = false;


    public bool IsDead
    {
        get { return _isDead; }
        set
        {
            if (_isDead != value)
            {
                _isDead = value;
                Death();
            }

        }
    }


    void Start()
    {
        //PhotonPeer.RegisterType(typeof(Vector2), 242, SerializeVector2Int, DeserializeVector2Int);

        _rb = GetComponent<Rigidbody2D>();
        _animatorController = GetComponent<Animator>();
        _view = GetComponent<PhotonView>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        NickNameText.text = _view.Owner.NickName;

        if (SceneManager.GetActiveScene().name != "GameScene") return;

        if (!_view.IsMine) return;

        _killBtn = GameObject.Find("/Canvas/KillBtn").GetComponent<Button>();
        _killBtn.onClick.AddListener(Kill);

        if (_view.Controller.CustomProperties.ContainsKey("isImposter"))
        {
            _isImposter = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isImposter"];
        }

        if (_isImposter)
        {
            _killBtn.interactable = false;
        }
        else
        {
            _killBtn.gameObject.SetActive(false);
        }

    }

    private void Kill()
    {
        if (IsDead) return;

        if (_targetForKill == null) return;

        var actor = _targetForKill.GetComponent<PhotonView>().OwnerActorNr;

        var options = new RaiseEventOptions { TargetActors = new int[] { actor } };
        var sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(99, true, options, sendOptions);

        _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
    }


    private void Update()
    {
        Walk();
    }

    private void Walk()
    {
        if (_view.IsMine)
        {
            if (_animOfDeath) return;

            float moveHorizontal = Input.GetAxis("Horizontal");

            float moveVertical = Input.GetAxis("Vertical");

            if (moveHorizontal == 0 && moveVertical == 0)
            {
                if (IsDead)
                {
                    _animatorController.SetBool("Ghosting", true);
                } else
                {
                    _animatorController.SetBool("Walk", false);
                }
                return;
            }

            if (moveHorizontal > 0) _isRightPlayer = true;
            if (moveHorizontal < 0) _isRightPlayer = false;

            var movement = new Vector2(moveHorizontal, moveVertical);

            var move = MoveSpeed * Time.deltaTime * movement;

            transform.Translate(move);
            if (IsDead)
            {
                _animatorController.SetBool("Ghosting", true);
            } else
            {
                _animatorController.SetBool("Walk", true);
            }

        }

        if (_isRightPlayer)
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;

    }

    private void Death()
    {
        _animOfDeath = true;
        _animatorController.SetBool("Dead", true);
        var lengthAnim = _animatorController.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(WaitAnimCoroutine(lengthAnim));

    }
    IEnumerator WaitAnimCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Instantiate(DeadBodyPrefab.name, transform.position, Quaternion.identity);
        _animOfDeath = false;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isRightPlayer);
            stream.SendNext(IsDead);
        }
        else
        {
            _isRightPlayer = (bool)stream.ReceiveNext();
            IsDead = (bool)stream.ReceiveNext();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_isImposter) return;

        if (other.GetComponent<PhotonView>().IsMine) return;

        if (_targetForKill == null) return;

        if (other.gameObject.CompareTag("Player"))
        {
            _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
            _targetForKill = null;
            _killBtn.interactable = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_isImposter) return;

        if (other.GetComponent<PhotonView>().IsMine) return;

        if (other.GetComponent<PlayerController>().IsDead) return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (_targetForKill == null)
            {
                _targetForKill = other.gameObject;
            }

            var distForCurrentTarget = Vector3.Distance(_targetForKill.transform.position, _spriteRenderer.transform.position);
            var distForOtherTarget = Vector3.Distance(other.transform.position, _spriteRenderer.transform.position);

            if (distForOtherTarget < distForCurrentTarget)
            {
                _targetForKill.GetComponent<SpriteRenderer>().material = PlayerMat;
                _targetForKill = other.gameObject;
            }
            _targetForKill.GetComponent<SpriteRenderer>().material = OutlinePlayerMat;
            _killBtn.interactable = true;
        }
    }

    enum MoveState
    {
        Idle,
        Walk,
        Ghosting
    }


    //public static object DeserializeVector2Int(byte[] data)
    //{
    //    Vector2 result = new Vector2();

    //    result.x = BitConverter.ToInt32(data, 0);
    //    result.y = BitConverter.ToInt32(data, 4);

    //    return result;
    //}

    //public static byte[] SerializeVector2Int(object obj)
    //{
    //    var vector = (Vector2)obj;

    //    byte[] result = new byte[8];

    //    BitConverter.GetBytes(vector.x).CopyTo(result, 0);
    //    BitConverter.GetBytes(vector.y).CopyTo(result, 4);

    //    return result;
    //}
}
