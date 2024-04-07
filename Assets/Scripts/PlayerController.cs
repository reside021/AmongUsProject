using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{
    private MoveState _moveState = MoveState.Idle;
    private Rigidbody2D _rb;
    private Animator _animatorController;
    private SpriteRenderer _spriteRenderer;
    private PhotonView _view;
    private bool _isDead;
    private bool _isInVent = false;
    private bool _isOpenUI;
    private bool _isAnimPlaying;
    private Camera _camera;
    private LayerMask _ghostPlayerLayer;
    private LayerMask _ventLayer;
    private LayerMask _playerLayer;


    public float MoveSpeed = 10f;

    public TextMeshProUGUI NickNameText;

    public GameObject VentArrow;

    public GameObject DeadBodyPrefab;

    public bool IsRightPlayer = true;
    public bool IsKicked;

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

    public bool IsInVent
    {
        get { return _isInVent; }
        set
        {
            if (_isInVent != value)
            {
                _isInVent = value;
                ChangeVentState();
            }

        }
    }

    public Camera Camera 
    {
        get { return _camera; }
        set { _camera = value; }
    }

    private void OnEnable()
    {
        LevelManager.OnOpenUI += OnOpenUI;
        KickUI.OnKickEnds += OnKickEnds;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animatorController = GetComponent<Animator>();
        _view = GetComponent<PhotonView>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _ghostPlayerLayer = LayerMask.NameToLayer("GhostPlayer");
        _ventLayer = LayerMask.NameToLayer("VentZone");
        _playerLayer = LayerMask.NameToLayer("Player");

        NickNameText.text = _view.Owner.NickName;

        BeginSpawnAnim();

    }

    private void Update()
    {
        Walk();
    }

    private void Walk()
    {
        if (_view.IsMine)
        {
            if (_isOpenUI) return;

            if (_isAnimPlaying) return;

            if (_isInVent) return;

            float moveHorizontal = Input.GetAxis("Horizontal");

            float moveVertical = Input.GetAxis("Vertical");

            if (moveHorizontal == 0 && moveVertical == 0)
            {
                _rb.velocity = Vector2.zero;
                if (!IsDead)
                {
                    _animatorController.SetBool("Walk", false);
                }
                return;
            }

            if (moveHorizontal > 0) IsRightPlayer = true;
            if (moveHorizontal < 0) IsRightPlayer = false;

            var movement = new Vector2(moveHorizontal, moveVertical);

            var move = MoveSpeed  * movement.normalized;

            _rb.velocity = move;

            if (!IsDead)
            {
                _animatorController.SetBool("Walk", true);
            }

        }

        if (IsRightPlayer)
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;

    }

    private void BeginSpawnAnim()
    {
        _isAnimPlaying = true;
        _spriteRenderer.flipX = !IsRightPlayer;
        _animatorController.SetBool("Spawn", true);

        var lengthAnim = _animatorController.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(EndSpawnAnim(lengthAnim));
    }

    IEnumerator EndSpawnAnim(float time)
    {
        yield return new WaitForSeconds(time);

        _animatorController.SetBool("Spawn", false);

        _rb.bodyType = RigidbodyType2D.Dynamic;

        _isAnimPlaying = false;
    }


    private void Death()
    {
        _rb.velocity = Vector2.zero;

        _isAnimPlaying = true;

        _animatorController.SetBool("Dead", true);

        var lengthAnim = _animatorController.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(TranslateToGhostCoroutine(lengthAnim));

    }


    IEnumerator TranslateToGhostCoroutine(float time)
    {
        yield return new WaitForSeconds(time);

        EnableGhostVisible();

        if (!IsKicked)
        {
            var deadBody = Instantiate(DeadBodyPrefab, transform.position, Quaternion.identity);
            deadBody.GetComponent<DeadBodyId>().ID = _view.ViewID;
        }

        _isAnimPlaying = false;

        _animatorController.SetBool("Ghosting", true);
    }

    private void EnableGhostVisible()
    {
        if (_view.IsMine)
        {
            Camera.cullingMask |= (1 << _ghostPlayerLayer);
        }
        ChangePlayerLayer(_ghostPlayerLayer);
    }

    private void ChangeVentState()
    {
        if (_isInVent)
        {
            _rb.velocity = Vector2.zero;
            ChangePlayerLayer(_ventLayer);
        } else
        {
            IsRightPlayer = true;
            _spriteRenderer.flipX = !IsRightPlayer;
            ChangePlayerLayer(_playerLayer);
        }
    }

    private void ChangePlayerLayer(LayerMask targetLayer)
    {
        gameObject.layer = targetLayer;
        transform.GetChild(0).gameObject.layer = targetLayer;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(IsRightPlayer);
            stream.SendNext(IsDead);
            stream.SendNext(IsInVent);
        }
        else
        {
            IsRightPlayer = (bool)stream.ReceiveNext();
            IsDead = (bool)stream.ReceiveNext();
            IsInVent = (bool)stream.ReceiveNext();
        }
    }

    private void OnOpenUI()
    {
        _rb.velocity = Vector2.zero;
        _isOpenUI = true;
    }

    private void OnKickEnds()
    {
        _isOpenUI = false;
    }

    private void OnDisable()
    {
        LevelManager.OnOpenUI -= OnOpenUI;
        KickUI.OnKickEnds -= OnKickEnds;
    }

    enum MoveState
    {
        Idle,
        Walk,
        Ghosting
    }
}
