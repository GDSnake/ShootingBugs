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
   [HideInInspector] public Vector3 hostagePosition;
    private bool _grounded = true;
    private int _guards;
    private bool saved = false;
    private bool dead = false;

    private bool hasBlinked = false;
    private bool hadScalled = false;
	// Use this for initialization
	void Start ()
	{
	    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            
            KillHostage();
            
        }
	    if (dead&&(hasBlinked||hadScalled))
	    {
            Quaternion temp = Quaternion.Euler(0, 0, -90);
            gameObject.transform.rotation = temp;
            gameObject.transform.position = hostagePosition + new Vector3(0, -0.1f, 0);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
	    if (saved && !dead)
	    {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            float step = jumpUpSpeed * Time.deltaTime;

            // Move our position a step closer to the target.
            gameObject.transform.localScale= new Vector3(20*Mathf.Sin(step),20*Mathf.Sin(step));
            gameObject.transform.position= Vector3.MoveTowards(gameObject.transform.position,hostagePosition,step);
	        StartCoroutine("Scale");
	        if (gameObject.transform.position == hostagePosition)
	        {
	            saved = !saved;
	            gameObject.GetComponent<Rigidbody2D>().simulated = false;

	            gameObject.tag = "Saved";
	            GameManager.SavedHostages++;
	        }
	    }
    }

   
    void KillHostage() {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null && hit.transform == transform && !dead && !saved) {
            
            
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
            gameObject.GetComponent<SpriteRenderer>().sortingOrder++;

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
            if (other.tag == "East")
            {
                gameObject.GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
        if (other.tag == "Boundary"&&!dead)
        {
            
            InstantDeath();
        }
    }

    void InstantDeath()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        gameObject.GetComponent<SpriteRenderer>().sprite = Dead;
        Quaternion rotation=Quaternion.Euler(0,0,-90);
        gameObject.transform.rotation = rotation;
        gameObject.transform.position = hostagePosition + new Vector3(0, -0.1f, 0);
        gameObject.tag="Dead";
        dead = true;
        GameManager.DeadHostages++;

    }

    void Death()
    {

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameObject.GetComponent<SpriteRenderer>().sprite = Dead;
        
        gameObject.tag = "Dead";
        StartCoroutine("Blinking");
        GameManager.DeadHostages++;



    }
    public void SaveHostage()
    {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        saved = true;
       
    }
    IEnumerator Scale() {
        float timer = 0;

        if (hadScalled||dead)
        {
            yield break;
        }
            // we scale all axis, so they will have the same value, 
            // so we can work with a float instead of comparing vectors
            while (maxSize > transform.localScale.x) {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }
            // reset the timer

            yield return new WaitForSecondsRealtime(waitTime);

            timer = 0;
            while (1 < transform.localScale.x) {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

          
            yield return new WaitForSecondsRealtime(waitTime);
        hadScalled = true;

    }
    IEnumerator Blinking() {

        if (hasBlinked||dead)
        {
            Quaternion temp = Quaternion.Euler(0, 0, -90);
            gameObject.transform.rotation = temp;
            gameObject.transform.position = hostagePosition + new Vector3(0, -0.1f, 0);
            yield break;
        }
        yield return new WaitForSeconds(0.4f);
        for (int i = 10; i > 0; i--) {
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSecondsRealtime((float)i / 30);
        }
        Quaternion rotation = Quaternion.Euler(0, 0, -90);
        gameObject.transform.rotation = rotation;
        gameObject.transform.position =  hostagePosition + new Vector3(0, -0.1f, 0);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        hasBlinked = true;
        dead = true;

    }

}



