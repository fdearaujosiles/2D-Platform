using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] Collider2D _collider;
    [SerializeField] Tilemap _destructible;
    
    private Rigidbody2D _rb;
    private Animator _anim;


    private bool isGrounded = true;
    private bool isFacingLeft;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isGrounded && _collider.IsTouchingLayers(LayerMask.GetMask("Destructible")))
        {
            Vector3 hitPosition = new Vector3(transform.position.x, transform.position.y - 1, 0);
            _destructible.SetTile(_destructible.WorldToCell(hitPosition), null);
        }
        Movement();
        Jump();
    }

    private void Movement()
    {
        float hAxis = CrossPlatformInputManager.GetAxis("Horizontal");
        
        _rb.velocity = new Vector2(speed * hAxis, _rb.velocity.y);
        FlipSprite(hAxis);
    }

    private void Jump()
    {
        isGrounded = _collider.IsTouchingLayers(LayerMask.GetMask("Ground")) || _collider.IsTouchingLayers(LayerMask.GetMask("Destructible"));
        _anim.SetBool("Jumping", !isGrounded && _rb.velocity.y > Mathf.Epsilon);
        _anim.SetBool("Falling", !isGrounded && _rb.velocity.y <= Mathf.Epsilon);

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
