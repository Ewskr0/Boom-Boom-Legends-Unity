using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementView : MonoBehaviour
{
    private Animator Animator;  
    private CharacterMovementModel  m_MovementModel;

	void Awake() 
    {
		Animator = GetComponent<Animator>();
        m_MovementModel = GetComponent<CharacterMovementModel>();
    }
	
	void Update() 
    {
		UpdateDirection();
	}

    void UpdateDirection()
    {
        Vector3 direction = m_MovementModel.GetDirection();
            
        if (direction != Vector3.zero)
        {   
            Animator.SetFloat("DirectionX", direction.x);
            Animator.SetFloat("DirectionY", direction.y);
                
        }
            
        Animator.SetBool("IsMoving", m_MovementModel.IsMoving());
    }
}
