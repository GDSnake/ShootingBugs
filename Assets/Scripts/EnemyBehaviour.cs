﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {

    public float Speed;
    public float ySpeedMultiplier = 0.2f;
    public float StartBlinkTimer = 0.4f;
    public int ScoreChangeKilled;
    public int GemScoreMultiplier;
    public Sprite[] Dead;

    // WaitForSeconds caching to save on garbage allocation
    private WaitForSeconds _waitForBlink;

    private bool _grounded = true;
    // Use this for initialization
    void Start() {
        _waitForBlink = new WaitForSeconds(StartBlinkTimer);
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, 0);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0)&&!GameManager._finished) {
            HitEnemy();
        }
    }

    /// <summary>
    /// This function sends a raycast from the mouse position into the world to see if hits an enemy
    /// </summary>
    private void HitEnemy() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null && hit.transform == transform) {
            if (gameObject.tag == "Guard") {
                GameManager.NumberGuards--;
                gameObject.tag = "Enemy";
            }

            if (gameObject.tag == "Gem") {
                GemHitted();
            } else if (Random.Range(0, 5) == 0) {
                GemAppearence();
            } else {
                gameObject.GetComponent<Collider2D>().enabled = false;
                Death();
            }
        }
    }

    /// <summary>
    /// Used to rotate and move a character on the Y axis to simulate going up and down the ramp
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "West" && _grounded) {
            _grounded = !_grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, 20);
            gameObject.transform.rotation = rotation;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, Speed * ySpeedMultiplier);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder++;

        } else if (other.tag == "East" && !_grounded) {
            _grounded = !_grounded;
            Quaternion rotation = Quaternion.Euler(0, 0, -20);
            gameObject.transform.rotation = rotation;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Speed, -Speed * ySpeedMultiplier);
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

        if (other.tag == "Boundary") {
            InstantDeath();
        }
    }

    /// <summary>
    /// This function destroys an enemy that is hitted on gem form and updates the score
    /// </summary>
    private void GemHitted() {
        StopCoroutine(Blinking());
        GameManager.ScoreDifference = ScoreChangeKilled * GemScoreMultiplier;
        GameManager.ShowScoreChange = true;
        GameManager.UpdateTotalScore = true;
        InstantDeath();
    }
    /// <summary>
    /// This function transforms the enemmy GO into a gem in sprite and tag
    /// </summary>
    private void GemAppearence() {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.GetComponent<SpriteRenderer>().sprite = Dead[Random.Range(1, Dead.Length)];
        gameObject.tag = "Gem";
        StartCoroutine(Blinking());
    }
    /// <summary>
    /// Kills an enemy and changes it's sprite into a squashed enemy and makes the sprite blink
    /// </summary>
    private void Death() {
        GameManager.ScoreDifference = ScoreChangeKilled;
        GameManager.ShowScoreChange = true;
        GameManager.UpdateTotalScore = true;
        gameObject.GetComponent<SpriteRenderer>().sprite = Dead[0];
        gameObject.GetComponent<SpriteRenderer>().sortingOrder++;
        StartCoroutine(Blinking());
    }

    /// <summary>
    /// Instantly kills GO
    /// </summary>
    private void InstantDeath() {
        Destroy(gameObject);
    }

    /// <summary>
    /// This coroutine simulates the GO sprite blink, by enabling and disabling it during X time and then destroys the GO
    /// </summary>
    /// <returns></returns>
    private IEnumerator Blinking() {
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return _waitForBlink;
        for (int i = 10; i > 0; i--) {
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds((float)i / 30);
        }
        Destroy(gameObject);
    }

}



