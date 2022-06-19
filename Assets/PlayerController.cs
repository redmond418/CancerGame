using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpForce = 10;
    [SerializeField] float moveSpeed_Ground = 5;
    [SerializeField] float moveForce_Air = 4;
    [SerializeField] float pushedForce = 20;
    Rigidbody2D rigid2D;
    Collider2D colid2D;
    float direction = 0;
    float pushedDirection = 0;
    bool onGround = false;
    bool pushed = false;
    bool stop = false;
    // Start is called before the first frame update
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        colid2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Sign(Input.GetAxisRaw("Horizontal")) != Math.Sign(direction)/* && !pushed*/) stop = true;
        direction = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && onGround)
        {
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        //ê⁄ínîªíË
        bool onGroundCheck = false;
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        colid2D.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            if(contact.normal.y >= 0.707f)
            {
                onGroundCheck = true;
                if (pushed) pushed = false;
                if (pushedDirection != 0) pushedDirection = 0;
            }
        }
        if (onGround != onGroundCheck) onGround = onGroundCheck;
        //à⁄ìÆ
        if (onGround && pushedDirection == 0)
        {
            rigid2D.velocity = new Vector2(direction * moveSpeed_Ground, rigid2D.velocity.y);
            if (stop) stop = false;
                print(pushed);

        }
        else if(direction == 0 || rigid2D.velocity.x * Mathf.Sign(direction) < moveSpeed_Ground)
        {
            if (stop)
            {
                rigid2D.velocity = new Vector2(pushedDirection, rigid2D.velocity.y);
                stop = false;
            }
            else
            {
                rigid2D.AddForce(Vector2.right * direction * moveForce_Air);
                if (rigid2D.velocity.x * Math.Sign(pushedDirection) < Mathf.Abs(pushedDirection) && pushedDirection != 0)
                {
                    if ((rigid2D.velocity.x > 0) != (pushedDirection > 0)) pushedDirection = 0;
                    else pushedDirection = rigid2D.velocity.x;
                }
            }
        }
    }

    public void Push(Vector2 vector)
    {
        rigid2D.velocity = vector * pushedForce;
        pushedDirection = vector.x * pushedForce;
        print("p");
        pushed = true;
    }
}
