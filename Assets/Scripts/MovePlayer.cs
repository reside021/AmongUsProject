using UnityEngine;
using Photon.Pun;
using TMPro;
using ExitGames.Client.Photon;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class MovePlayer : MonoBehaviour, IPunObservable
{
    public float MoveSpeed = 10f;
    public TextMeshProUGUI NickNameText;

    private MoveState _moveState = MoveState.Idle;
    private Rigidbody2D _rb;
    private Animator _animatorController;
    private SpriteRenderer _spriteRenderer;

    private bool _isRightPlayer;

    //private Vector2 _directionPlayer;

    private PhotonView _view;


    void Start()
    {
        //PhotonPeer.RegisterType(typeof(Vector2), 242, SerializeVector2Int, DeserializeVector2Int);

        _rb = GetComponent<Rigidbody2D>();
        _animatorController = GetComponent<Animator>(); 
        _view = GetComponent<PhotonView>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        NickNameText.text = _view.Owner.NickName;
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
                Idle();
                return;
            }

            if (moveHorizontal > 0) _isRightPlayer = true;
            if (moveHorizontal < 0) _isRightPlayer = false;
            
            var movement = new Vector2(moveHorizontal, moveVertical);

            var move = MoveSpeed * Time.deltaTime * movement;

            transform.Translate(move);
            _animatorController.SetBool("Walk", true);
        }

        if (_isRightPlayer) 
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;
    }
    public void Idle()
    {
        _moveState = MoveState.Idle;
        _animatorController.SetBool("Walk", false);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isRightPlayer);
        }
        else
        {
            _isRightPlayer = (bool)stream.ReceiveNext();
        }
    }

    enum MoveState
    {
        Idle,
        Walk
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
