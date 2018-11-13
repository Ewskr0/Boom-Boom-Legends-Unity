using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBaseControl : MonoBehaviour
{
    protected CharacterMovementModel m_MovementModel;
    protected CharacterInteractionModel m_InteractionModel;

    void Awake()
    {
        m_MovementModel = GetComponent<CharacterMovementModel>();
        m_InteractionModel = GetComponent<CharacterInteractionModel>();

    }

    protected void SetDirection(Vector2 direction)
    {
        m_MovementModel.SetDirection(direction);
    }

    protected void DropBomb()
        {
            m_InteractionModel.DropBomb();
        }
}
