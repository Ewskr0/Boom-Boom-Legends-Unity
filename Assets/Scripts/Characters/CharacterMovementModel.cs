using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementModel : MonoBehaviour
{

    public float speed;
    private Vector3 m_MovementDirection;
    private Rigidbody2D m_body;

    void Awake()
    {
        m_body = GetComponent<Rigidbody2D>();
    }

    void Update () 
        {
            UpdateMovement();
        }

    void UpdateMovement ()
    {
        if (m_MovementDirection != Vector3.zero) 
        {
            m_MovementDirection.Normalize();
        }

        m_body.velocity = m_MovementDirection * speed;
    }

    public void SetDirection(Vector2 direction) 
    {
        m_MovementDirection = new Vector3(direction.x, direction.y, 0);
    }

    public Vector3 GetDirection()
    {      
        return m_MovementDirection;        
    }

    public bool IsMoving()
    {
        return m_MovementDirection != Vector3.zero;
    }
}
