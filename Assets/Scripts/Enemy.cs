using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed;
    
    [SerializeField] public bool isFacingLeft;
    [SerializeField] public float knockBack;
    
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        int leftFlip = isFacingLeft ? -1 : 1;
        _rb.velocity = new Vector2(speed * (leftFlip) , _rb.velocity.y);
        transform.localScale = new Vector3(-leftFlip, 1, 1);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.gameObject.CompareTag("Player")) {isFacingLeft = !isFacingLeft;}
    }
}
