using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public GameObject Player;
    private Rigidbody2D _rb;
    public float MoveSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        _rb = Player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal == 0 && moveVertical == 0)
        {
                _rb.velocity = Vector2.zero;
            return;
        }

        var movement = new Vector2(moveHorizontal, moveVertical);

        var move = MoveSpeed * movement.normalized;

        _rb.velocity = move;
    }
}
