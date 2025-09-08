using System;
using UnityEngine;

public class Collectibles : MonoBehaviour, ICollectible
{
    private Transform targetPosition;
    private Rigidbody2D rigidbody2D;

    bool hasTarget;
    bool wasDropped;
    float moveSpeed = 1.5f;

    public Rigidbody2D Rigidbody2D() => rigidbody2D;
    public bool HasTarget() => hasTarget;
    public bool WasDropped() => wasDropped;
    public Vector3 TargetPosition() => targetPosition.position;
    public Vector3 TargetPosition(Vector3 position) => targetPosition.position = position;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        targetPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        if (hasTarget && !wasDropped)
        {
            //targetPosition = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            Vector2 targetDirection = (targetPosition.position - transform.position).normalized;
            rigidbody2D.linearVelocity = new Vector2(targetDirection.x, targetDirection.y) * moveSpeed;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public void ActivateTracking() { hasTarget = true; }
    
    /// <summary>
    /// 
    /// </summary>
    public void DeactivateTracking() { 
        hasTarget = false;
        rigidbody2D.linearVelocity = Vector3.zero;
        wasDropped = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Spawn()
    {
        hasTarget = false;
        wasDropped = true;
    }
}