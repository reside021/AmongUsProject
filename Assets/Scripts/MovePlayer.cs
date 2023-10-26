using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class MovePlayer : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public TextMeshProUGUI NickNameText;

    private MoveState _moveState = MoveState.Idle;
    private Rigidbody2D _rb;
    private Animator _animatorController;
    private bool _isRightPlayer = true;

    private PhotonView _view;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animatorController = GetComponent<Animator>(); 
        _view = GetComponent<PhotonView>();

        NickNameText.text = _view.Owner.NickName;
    }


    private void Update()
    {
        Walk();
    }

    private void Walk()
    {

        if (!_view.IsMine) return;

        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");


        if (moveHorizontal == 0 && moveVertical == 0)
        {
            Idle();
            return;
        }

        if (moveHorizontal > 0 && !_isRightPlayer)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && _isRightPlayer)
        {
            Flip();
        }

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        var move = movement * MoveSpeed * Time.deltaTime;

        transform.Translate(move);
        _animatorController.SetBool("Walk", true);

    }
    public void Idle()
    {
        _moveState = MoveState.Idle;
        _animatorController.SetBool("Walk", false);
    }

    private void Flip()
    {
        _isRightPlayer = !_isRightPlayer;
        var theScale = base.transform.localScale;
        var theNickName = NickNameText.transform.localScale;

        theScale.x *= -1;
        theNickName.x *= -1;

        transform.localScale = theScale;
        NickNameText.transform.localScale = theScale;
    }


    enum MoveState
    {
        Idle,
        Walk
    }
}
