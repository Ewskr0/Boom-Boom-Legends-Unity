using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInteractionModel : MonoBehaviour
{
    private CircleCollider2D m_character_collider;

	[SerializeField]
	private GameObject m_bomb;
    private Vector3 m_character_positon;

    void Awake()
        {   
            m_character_collider = GetComponent<CircleCollider2D>();
        }

    public void DropBomb() 
    {   
        m_character_positon = m_character_collider.bounds.center;
		Instantiate (m_bomb, m_character_positon, Quaternion.identity);
    }
}
