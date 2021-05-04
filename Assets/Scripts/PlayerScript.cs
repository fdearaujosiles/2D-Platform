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
    [SerializeField] Collider2D feetCollider, bodyCollider;
    
    private Rigidbody2D _rb;
    private Animator _anim;
    
    private bool _isGrounded = true;
    private bool _isFacingLeft, _isClimbing, _isHurting;
    private float _initialGravityScale;
    
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Falling = Animator.StringToHash("Falling");
    private static readonly int Climbing = Animator.StringToHash("Climbing");
    private static readonly int Running = Animator.StringToHash("Running");

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _initialGravityScale = _rb.gravityScale;
    }

    private void Update()
    {
        if (!_isHurting)
        {
            Move();
            Jump();
            Climb();
        }
    }

    private void Move()
    {
        float hAxis = CrossPlatformInputManager.GetAxis("Horizontal");
        _rb.velocity = new Vector2(speed * hAxis, _rb.velocity.y);
        
        FlipSprite(hAxis);
    }

    private void Climb()
    {
        float vAxis = CrossPlatformInputManager.GetAxis("Vertical");
        
        if (Mathf.Abs(vAxis) > Mathf.Epsilon && bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbable")))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, speed * vAxis);
            _rb.gravityScale = 0f;
            _isClimbing = true;
        }
        else if (_isClimbing && bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbable")))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
            _rb.gravityScale = 0f;
        }
        else
        {
            _rb.gravityScale = _initialGravityScale;
            _isClimbing = false;
        }
        
        
        _anim.SetBool(Climbing, _isClimbing);
        
        
    }

    private void Jump()
    {
        _isGrounded = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        _anim.SetBool(Jumping, !_isGrounded && _rb.velocity.y > 0);
        _anim.SetBool(Falling, !_isGrounded && _rb.velocity.y <= 0);

        if (_isGrounded && CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_isHurting && other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            int knockBackLeft = enemy.isFacingLeft ? -1 : 1;
            if ((enemy.isFacingLeft && enemy.transform.position.x < transform.position.x) ||
                (!enemy.isFacingLeft && enemy.transform.position.x > transform.position.x))
            {
                knockBackLeft *= -1;
            }
            
            _anim.SetTrigger(Hit);
            _rb.velocity = enemy.knockBack * new Vector2(knockBackLeft, 0.5f);
            _isHurting = true;
            StartCoroutine(StopHurting());
        }
    }

    private void FlipSprite(float hAxis)
    {
        bool isRunning = Mathf.Abs(hAxis) > Mathf.Epsilon;
        _anim.SetBool(Running, isRunning);
        if (isRunning)
        {   
            _isFacingLeft = Mathf.Sign(hAxis) > Mathf.Epsilon;
            transform.localScale = new Vector3(Mathf.Sign(hAxis), 1, 1);
        }
    }

    private IEnumerator StopHurting()
    {
        yield return new WaitForSeconds(0.5f);

        _isHurting = false;
    } 
}
