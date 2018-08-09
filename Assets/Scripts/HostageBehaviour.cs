using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageBehaviour : MonoBehaviour
{
    public float maxSize;
    public float growFactor;
    public float waitTime;
    public float Speed;
    public Sprite Dead;
    public float jumpUpSpeed = 3;
   [HideInInspector] public GameObject hostagePosition;
    private bool _grounded = true;
    private int _guards;
    private bool saved = false;
	// Use this for initialization
	void Start ()
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            killHostage();
        }
	    if (saved)
	    {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            float step = jumpUpSpeed * Time.deltaTime;

            // Move our position a step closer to the target.
            gameObject.transform.localScale= new Vector3(20*Mathf.Sin(step),20*Mathf.Sin(step));
            gameObject.transform.position= Vector3.MoveTowards(gameObject.transform.position,hostagePosition.transform.position,step);
	        StartCoroutine("Scale");
	        if (gameObject.transform.position == hostagePosition.transform.position)
	        {
	            saved = !saved;
	            gameObject.GetComponent<Rigidbody2D>().simulated = false;

	            gameObject.tag = "Saved";
	        }
	    }
    }

    void FixedUpdate()
    {
      
    }
    void killHostage() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null && hit.transform == transform) {
            Death();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "West" && _grounded &&!saved)
        {

            _grounded = !_grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, 20);
            gameObject.transform.rotation = rotation;
            //gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                //gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, Speed* 0.2f);
            
        } else if (other.tag == "East" && !_grounded && !saved) {
            _grounded = !_grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, -20);
            gameObject.transform.rotation = rotation;

            //gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                //gameObject.transform.position.y - 0.5f, gameObject.transform.position.z);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, -Speed*0.2f);
        } 
    
        }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "West" || other.tag == "East")
        {
            
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.rotation = rotation;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, 0);
        }
        if (other.tag == "Boundary")
        {
            Death();
        }
    }

    void Death()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameObject.GetComponent<SpriteRenderer>().sprite = Dead;
        //Destroy(gameObject);
    }

    public void SaveHostage()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        saved = true;
        // gameObject.GetComponent<SpriteRenderer>().sprite = Dead;
    }
    IEnumerator Scale() {
        float timer = 0;

        
            // we scale all axis, so they will have the same value, 
            // so we can work with a float instead of comparing vectors
            while (maxSize > transform.localScale.x) {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }
            // reset the timer

            yield return new WaitForSeconds(waitTime);

            timer = 0;
            while (1 < transform.localScale.x) {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(waitTime);
        
    }

}



