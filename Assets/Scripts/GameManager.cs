using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float offscreenX = -8;
    public float firstY = -2.2f;
    public Vector3 spawnValues;
    public GameObject Enemy;
	// Use this for initialization
	void Start ()
	{
	    SpawnWaves();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void SpawnWaves() {
        Vector3 spawnPosition = new Vector3(spawnValues.x,spawnValues.y,spawnValues.z);
        Quaternion spawnDirection = Quaternion.identity;
        Instantiate(Enemy,spawnPosition,spawnDirection);
    }
}
