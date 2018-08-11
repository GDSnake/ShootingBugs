using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostageBehaviour : MonoBehaviour {

    public int ScoreChangeDead;
    public int ScoreChangeSaved;
    public float MaxSize;
    public float GrowFactor;
    public float WaitTime;
    public float Speed;
    public Sprite Dead;
    public float JumpUpSpeed = 3;

    [HideInInspector]
    public Vector3 HostagePosition;

    /// <summary>
    /// Used to control climbing and descending behaviour
    /// </summary>
    private bool _grounded = true;
    /// <summary>
    /// Used to control scalling coroutine
    /// </summary>
    private bool _saved = false;
    private bool _dead = false;
    private bool _hasBlinked = false;
    private bool _hasScalled = false;
    // Use this for initialization
    void Start() {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, 0);
    }

    // Update is called once per frame
    void Update() {

        if (_hasScalled) // to stop unexpected scalling behaviour after reaching the destination on the top right corner
        {
            _hasScalled = !_hasScalled;
            transform.localScale = new Vector3(1, 1, 1);
            StopCoroutine("Scale");
        }
        if (Input.GetMouseButtonDown(0) && !_saved) {

            KillHostage();

        }
        if (_dead && (_hasBlinked || _hasScalled)) {
            Quaternion temp = Quaternion.Euler(0, 0, -90);
            gameObject.transform.rotation = temp;
            gameObject.transform.position = HostagePosition + new Vector3(0, -0.1f, 0);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        if (_saved && !_dead && gameObject.tag != "Saved") {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            float step = JumpUpSpeed * Time.deltaTime;

            // Move our position a step closer to the target.
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, HostagePosition, step);

            StartCoroutine("Scale");



            if (gameObject.transform.position == HostagePosition) {


                gameObject.GetComponent<Rigidbody2D>().simulated = false;

                gameObject.tag = "Saved";
                GameManager.SavedHostages++;
                GameManager.ScoreDifference = ScoreChangeSaved;
                GameManager.ShowScoreChange = true;
                GameManager.UpdateTotalScore = true;
            }
        }
    }

    /// <summary>
    /// Checks if an hostage is hitted by the mouse using raycasthit, if is hitted, kills the hostage
    /// </summary>
    void KillHostage() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null && hit.transform == transform && !_dead && !_saved) {

            _dead = true;
            Death();
        }
    }

    /// <summary>
    /// Used to rotate and move a character on the Y axis to simulate going up and down the ramp
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "West" && _grounded && !_saved) {

            _grounded = !_grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, 20);
            gameObject.transform.rotation = rotation;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, Speed * 0.2f);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder++;

        } else if (other.tag == "East" && !_grounded && !_saved) {
            _grounded = !_grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, -20);
            gameObject.transform.rotation = rotation;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, -Speed * 0.2f);
        }

    }

    /// <summary>
    /// Simulates the character going into horizontal ground when exiting a ramp and also destroys GO if goes out of a boundary
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "West" || other.tag == "East") {

            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            gameObject.transform.rotation = rotation;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, 0);
            if (other.tag == "East") {
                gameObject.GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }
        if (other.tag == "Boundary" && !_dead) {

            InstantDeath();
        }
    }

    /// <summary>
    /// Instantly kills the hostage GO, and displaying the dead sprite on the top right
    /// </summary>
    void InstantDeath() {
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        gameObject.GetComponent<SpriteRenderer>().sprite = Dead;
        Quaternion rotation = Quaternion.Euler(0, 0, -90);
        gameObject.transform.rotation = rotation;
        gameObject.transform.position = HostagePosition + new Vector3(0, -0.1f, 0);
        gameObject.tag = "Dead";
        _dead = true;
        GameManager.DeadHostages++;
        GameManager.ScoreDifference = ScoreChangeDead;
        GameManager.ShowScoreChange = true;
        GameManager.UpdateTotalScore = true;

    }

    /// <summary>
    /// Kills the hostage GO, changes it's sprite into the dead hostage and blinks it's body. Then displays the dead hostage on the top right
    /// </summary>
    void Death() {

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameObject.GetComponent<SpriteRenderer>().sprite = Dead;

        gameObject.tag = "Dead";
        StartCoroutine("Blinking");
        GameManager.DeadHostages++;
        GameManager.ScoreDifference = ScoreChangeDead;
        GameManager.ShowScoreChange = true;
        GameManager.UpdateTotalScore = true;



    }
    /// <summary>
    /// This function stops the hostage from moving and changes saved flag into true
    /// </summary>
    public void SaveHostage() {
        _saved = true;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);


    }
    /// <summary>
    /// This coroutine scales up and down the Hostage GO over time
    /// </summary>
    /// <returns></returns>
    IEnumerator Scale() {
        float timer = 0;

        if (_hasScalled || _dead) {
            yield break;
        }

        // We scale all axis, so they will have the same value, so we can work with a float instead of comparing vectors
        
        while (MaxSize > transform.localScale.x) {
            timer += Time.deltaTime;
            transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * GrowFactor;
            yield return null;
        }


        yield return new WaitForSecondsRealtime(WaitTime);
        // reset the timer
        timer = 0;
        while (1 < transform.localScale.x) {
            timer += Time.deltaTime;
            transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * GrowFactor;
            yield return null;
        }


        yield return new WaitForSecondsRealtime(WaitTime);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        _hasScalled = true;

    }

    /// <summary>
    /// This coroutine simulates the GO sprite blink, by enabling and disabling it during X time.
    /// The rotates and positions the dead sprite on the top right side
    /// </summary>
    /// <returns></returns>
    IEnumerator Blinking() {

        if (_hasBlinked) {
            Quaternion temp = Quaternion.Euler(0, 0, -90);
            gameObject.transform.rotation = temp;
            gameObject.transform.position = HostagePosition + new Vector3(0, -0.1f, 0);
            yield break;
        }
        yield return new WaitForSeconds(0.4f);
        for (int i = 10; i > 0; i--) {
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSecondsRealtime((float)i / 30);
        }
        Quaternion rotation = Quaternion.Euler(0, 0, -90);
        gameObject.transform.rotation = rotation;
        gameObject.transform.position = HostagePosition + new Vector3(0, -0.1f, 0);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        _hasBlinked = true;


    }

}



