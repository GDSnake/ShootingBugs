using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float speed;
    
    public Sprite Dead;
    private bool grounded = true;
	// Use this for initialization
	void Start ()
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
	}
	
	// Update is called once per frame
	void Update () {
        
        if (Input.GetMouseButtonDown(0)) {
            hitEnemy();
        }
    }

    void FixedUpdate()
    {
      
    }

    void hitEnemy()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        
        if (hit.collider != null && hit.transform == transform) {
            if (gameObject.tag == "Guard") {
                GameManager.NumberGuards--;
                gameObject.tag = "Enemy";
            }
            Death();            
        }
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
            Death();
        }
    }

    void Death()
    {

        StartCoroutine("Blinking");
    }

    IEnumerator Blinking()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameObject.GetComponent<SpriteRenderer>().sprite = Dead;
        yield return new WaitForSeconds(0.4f);
        for (int i = 10; i > 0; i--)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds((float)i/30);
        }
        Destroy(gameObject);
    }

}



