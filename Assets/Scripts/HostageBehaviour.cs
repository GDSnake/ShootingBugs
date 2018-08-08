using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageBehaviour : MonoBehaviour
{
    public float speed;
    private bool grounded = true;
	// Use this for initialization
	void Start ()
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
      
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "West" && grounded)
        {

            grounded = !grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, 20);
            gameObject.transform.rotation = rotation;
            //gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                //gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, speed* 0.2f);
            
        } else if (other.tag == "East" && !grounded) {
            grounded = !grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, -20);
            gameObject.transform.rotation = rotation;

            //gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                //gameObject.transform.position.y - 0.5f, gameObject.transform.position.z);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, -speed*0.2f);
        } 
    
        }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "West" || other.tag == "East")
        {
            
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.rotation = rotation;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
        }
        if (other.tag == "Boundary")
        {
            Destroy(gameObject);
        }
    }

}



