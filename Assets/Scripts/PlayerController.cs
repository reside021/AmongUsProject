using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPunObservable
{

    private MoveState _moveState = MoveState.Idle;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private Animator _animatorController;
    private SpriteRenderer _spriteRenderer;
    private Button _killBtn;
    private Button _ventBtn;
    private PhotonView _view;
    private bool _isRightPlayer = true;
    private bool _isDead = false;
    private bool _isAnimOfDeath = false;
    private bool _isAnimSpwan = false;
    private Camera _camera;
    private LayerMask _ghostPlayerLayer;

    public float MoveSpeed = 10f;

    public TextMeshProUGUI NickNameText;


    public GameObject DeadBodyPrefab;

    public GameObject KillZone;

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

    public Camera Camera 
    {
        get { return _camera; }
        set { _camera = value; }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animatorController = GetComponent<Animator>();
        _view = GetComponent<PhotonView>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ghostPlayerLayer = LayerMask.NameToLayer("GhostPlayer");

        NickNameText.text = _view.Owner.NickName;

        StartSpawnAnim();

        if (SceneManager.GetActiveScene().name != "GameScene") return;

        if (!_view.IsMine) return;

        var _isImposter = false;

        if (_view.Controller.CustomProperties.ContainsKey("isImposter"))
        {
            _isImposter = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isImposter"];
        }

        if (_isImposter)
        {
            var _killZone = Instantiate(KillZone, transform);

            _killZone.GetComponent<KillZoneController>().KillButton = KillButton;
            _killZone.GetComponent<KillZoneController>().VentButton = VentButton;
        }
        else
        {
            KillButton.gameObject.SetActive(false);
            VentButton.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        Walk();
    }

    private void Walk()
    {
        if (_view.IsMine)
        {
            if (_isAnimSpwan) return;

            if (_isAnimOfDeath) return;

            float moveHorizontal = Input.GetAxis("Horizontal");

            float moveVertical = Input.GetAxis("Vertical");

            if (moveHorizontal == 0 && moveVertical == 0)
            {
                if (!IsDead)
                {
                    _animatorController.SetBool("Walk", false);
                    _rb.velocity = Vector2.zero;
                }
                return;
            }

            if (moveHorizontal > 0) _isRightPlayer = true;
            if (moveHorizontal < 0) _isRightPlayer = false;

            var movement = new Vector2(moveHorizontal, moveVertical);

            var move = MoveSpeed  * movement.normalized;

            //transform.Translate(move);
            _rb.velocity = move;

            if (!IsDead)
            {
                _animatorController.SetBool("Walk", true);
            }

        }

        if (_isRightPlayer)
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;

    }

    private void StartSpawnAnim()
    {
        _isAnimSpwan = true;
        _animatorController.SetBool("Spawn", true);

        var lengthAnim = _animatorController.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(StopSpawnAnim(lengthAnim));
    }

    IEnumerator StopSpawnAnim(float time)
    {
        yield return new WaitForSeconds(time);

        _animatorController.SetBool("Spawn", false);

        _rb.bodyType = RigidbodyType2D.Dynamic;

        _isAnimSpwan = false;
    }

    private void Death()
    {
        _isAnimOfDeath = true;

        _animatorController.SetBool("Dead", true);

        var lengthAnim = _animatorController.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(CreateDeadBodyCoroutine(lengthAnim));

    }


    IEnumerator CreateDeadBodyCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        ChangeVisiblePlayer();

        Instantiate(DeadBodyPrefab, transform.position, Quaternion.identity);

        _isAnimOfDeath = false;

        _animatorController.SetBool("Ghosting", true);
    }

    private void ChangeVisiblePlayer()
    {
        if (_view.IsMine)
        {
            Camera.cullingMask |= (1 << _ghostPlayerLayer);
        }
        gameObject.layer = _ghostPlayerLayer;
        transform.GetChild(0).gameObject.layer = _ghostPlayerLayer;
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

    enum MoveState
    {
        Idle,
        Walk,
        Ghosting
    }
}
