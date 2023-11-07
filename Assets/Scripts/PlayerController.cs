using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPunObservable
{
    public float MoveSpeed = 10f;
    public TextMeshProUGUI NickNameText;
    public Sprite Player;
    public Sprite Ghost;

    private MoveState _moveState = MoveState.Idle;
    private Rigidbody2D _rb;
    private Animator _animatorController;
    private SpriteRenderer _spriteRenderer;

    private bool _isRightPlayer;

    //private Vector2 _directionPlayer;

    private PhotonView _view;
    private bool _isDead = false;


    void Start()
    {
        //PhotonPeer.RegisterType(typeof(Vector2), 242, SerializeVector2Int, DeserializeVector2Int);

        _rb = GetComponent<Rigidbody2D>();
        _animatorController = GetComponent<Animator>();
        _view = GetComponent<PhotonView>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        NickNameText.text = _view.Owner.NickName;
        _isDead = true;
    }


    private void Update()
    {
        Walk();
    }

    private void Walk()
    {
        if (_view.IsMine)
        {

            float moveHorizontal = Input.GetAxis("Horizontal");

            float moveVertical = Input.GetAxis("Vertical");

            if (moveHorizontal == 0 && moveVertical == 0)
            {
                if (_isDead)
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
            if (_isDead)
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

    private void SetAnimation()
    {
        switch (_moveState)
            {
                case MoveState.Idle:
                    _animatorController.SetBool("Walk", false);
                    Debug.Log("IDLE");
                break;
                case MoveState.Ghosting:
                    _animatorController.SetBool("Ghosting", true);
                    break;
                case MoveState.Walk:
                    _animatorController.SetBool("Walk", true);
                    Debug.Log("WALK");
                    break;
            }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isRightPlayer);
            stream.SendNext(_isDead);
        }
        else
        {
            _isRightPlayer = (bool)stream.ReceiveNext();
            _isDead= (bool)stream.ReceiveNext();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("TRIGGER");
        if (other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("STAYTRIGGER");
            other.gameObject.GetComponent<SpriteRenderer>().color= Color.red;
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
