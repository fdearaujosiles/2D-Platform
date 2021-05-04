using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float speed, jumpSpeed;
    [SerializeField] Collider2D _feetCollider, _bodyCollider;
    [SerializeField] Tilemap _destructible;
    
    private Rigidbody2D _rb;
    private Animator _anim;


    private bool isGrounded = true;
    private bool isFacingLeft, climbing;
    private float initialGravityScale;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        initialGravityScale = _rb.gravityScale;
    }

    private void Update()
    {
        if (!isGrounded && _feetCollider.IsTouchingLayers(LayerMask.GetMask("Destructible")))
        {
            Vector3 hitPosition = new Vector3(transform.position.x, transform.position.y - 1, 0);
            _destructible.SetTile(_destructible.WorldToCell(hitPosition), null);
        }
        Movement();
        Jump();
        Climb();
    }

    private void Movement()
    {
        float hAxis = CrossPlatformInputManager.GetAxis("Horizontal");
        
        _rb.velocity = new Vector2(speed * hAxis, _rb.velocity.y);
        
        FlipSprite(hAxis);
    }

    private void Climb()
    {
        float vAxis = CrossPlatformInputManager.GetAxis("Vertical");

        if (Mathf.Abs(vAxis) > Mathf.Epsilon && _bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbable")))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, speed * vAxis);
            _rb.gravityScale = 0f;
            climbing = true;
        }
        else if (climbing && _bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbable")))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.gravityScale = 0f;
        }
        else
        {
            _rb.gravityScale = initialGravityScale;
            climbing = false;
        }
        
        
        _anim.SetBool("Climbing", climbing);
        
        
    }

    private void Jump()
    {
        isGrounded = _feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || _feetCollider.IsTouchingLayers(LayerMask.GetMask("Destructible"));
        _anim.SetBool("Jumping", !isGrounded && _rb.velocity.y > 0);
        _anim.SetBool("Falling", !isGrounded && _rb.velocity.y <= 0);

        if (isGrounded && CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpSpeed);
        }
    }

    private void FlipSprite(float hAxis)
    {
        bool isRunning = Mathf.Abs(hAxis) > Mathf.Epsilon;
        _anim.SetBool("Running", isRunning);
        if (isRunning)
        {   
            isFacingLeft = Mathf.Sign(hAxis) > Mathf.Epsilon;
            transform.localScale = new Vector3(Mathf.Sign(hAxis), 1, 1);
        }
    }
}
