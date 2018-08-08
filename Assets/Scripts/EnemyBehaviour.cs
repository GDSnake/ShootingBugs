using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{

    public float speed;
	// Use this for initialization
	void Start ()
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
