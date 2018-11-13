using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour 
{
    void OnTriggerExit2D(Collider2D other) 
    {
		this.GetComponent<Collider2D>().isTrigger = false;
	}
}
