using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] ParticleSystem deathPar;
    [SerializeField] Transform[] foots = new Transform[2];
    [SerializeField] float footMoveThreshold = 0.5f;
    [SerializeField] float footMoveMultiplier = 0.4f;
    [SerializeField] float footMoveTime = 0.2f;
    [SerializeField] float jumpForce = 10;
    [SerializeField] float moveSpeed_Ground = 5;
    [SerializeField] float moveForce_Air = 4;
    [SerializeField] float pushedForce = 20;
    Rigidbody2D rigid2D;
    Collider2D colid2D;
    Vector2 contactPoint;
    Vector2 footPos;
    Vector2 footMoveVeloL;
    Vector2 footMoveVeloR;
    float direction = 0;
    float pushedDirection = 0;
    byte pushed = 0;
    bool canMove = true;
    bool onGround = false;
    bool stop = false;
    int movingFoot = 0;
    int rightFoot = 1;
    // Start is called before the first frame update
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        colid2D = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameManager.isPlaying && canMove)
        {
            if(direction != 0)
            {
                direction = 0;
                stop = true;
            }
            canMove = false;
        }
        if (canMove)
        {
            if (Math.Sign(Input.GetAxisRaw("Horizontal")) != Math.Sign(direction)/* && !pushed*/) stop = true;
            direction = Input.GetAxisRaw("Horizontal");
            if (Input.GetButtonDown("Jump") && onGround)
            {
                rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
                /*if (movingFoot != 0)
                {
                    DOTween.Kill(foots[movingFoot - 1]);
                    movingFoot = 0;
                }*/
            }
        }
    }

    private void FixedUpdate()
    {
        //接地判定
        bool onGroundCheck = false;
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        colid2D.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            if(contact.normal.y >= 0.707f)
            {
                onGroundCheck = true;
                contactPoint = contact.point;
                if(!onGround && movingFoot == 0) footPos = contactPoint;
                if(footMoveVeloL == Vector2.zero) footMoveVeloL = Vector2.zero;
                if(footMoveVeloR == Vector2.zero) footMoveVeloR = Vector2.zero;
                if (pushed == 3)
                {
                    pushed = 0;
                    if (pushedDirection != 0) pushedDirection = 0;
                }
            }
        }
        if (onGround != onGroundCheck)
        {
            if (!onGroundCheck)
            {
                if(movingFoot != 0)
                {
                    DOTween.Kill(foots[movingFoot - 1]);
                    movingFoot = 0;
                }
            }
            else
            {
                foots[1 - rightFoot].DOMove(transform.position + Vector3.left * 0.15f, 0.2f)
                    .SetEase(Ease.OutQuart); 
                foots[rightFoot].DOMove(transform.position + Vector3.right * 0.15f, 0.2f)
                     .SetEase(Ease.OutQuart);
            }
            onGround = onGroundCheck;
        }
        //移動
        if (pushed > 0 && pushed <= 2) pushed++;
        if (onGround && pushedDirection == 0)
        {
            rigid2D.velocity = new Vector2(direction * moveSpeed_Ground, rigid2D.velocity.y);
            if (stop) stop = false;

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
        //足アニメーション
        if(onGround)
        {
            if(Mathf.Abs(contactPoint.x - footPos.x) >= footMoveThreshold)
            {
                if (contactPoint.x - footPos.x > 0)
                {
                    movingFoot = 2 - rightFoot;
                }
                else
                {
                    movingFoot = 1 + rightFoot;
                }
                print(rightFoot);
                rightFoot = 1 - rightFoot;
                DOTween.Kill(foots[movingFoot - 1]);
                foots[movingFoot - 1].DOMoveY(0.25f * footPos.y + 0.75f * (contactPoint + rigid2D.velocity * footMoveMultiplier).y + 0.1f, footMoveTime * 0.75f)
                    .SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                    {
                        if(movingFoot > 0) foots[movingFoot - 1].DOMoveY((contactPoint + rigid2D.velocity * footMoveMultiplier).y, footMoveTime * 0.25f)
                            .SetEase(Ease.InOutCubic);
                    });
                foots[movingFoot - 1].DOMoveX((contactPoint + rigid2D.velocity * footMoveMultiplier).x, footMoveTime)
                    .SetEase(Ease.InOutQuad);
                footPos = contactPoint + rigid2D.velocity * footMoveMultiplier;
            }
        }
        else
        {
            foots[1 - rightFoot].position = Vector2.SmoothDamp(foots[1 - rightFoot].position, 
                transform.position + Vector3.left * 0.15f, ref footMoveVeloL, 0.06f, 50f, Time.fixedDeltaTime);
            foots[rightFoot].position = Vector2.SmoothDamp(foots[rightFoot].position,
                transform.position + Vector3.right * 0.15f, ref footMoveVeloR, 0.06f, 50f, Time.fixedDeltaTime);
        }
    }

    public void Push(Vector2 vector)
    {
        if (canMove)
        {
            rigid2D.velocity = vector * pushedForce;
            pushedDirection = vector.x * pushedForce;
            pushed = 1;
        }
    }

    public void Die()
    {
        gameManager.Restart(1f);
        deathPar.transform.position = transform.position;
        deathPar.Play();
        foots[0].gameObject.SetActive(false);
        foots[1].gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
