using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float startWait = 1;
    public float maxSpawnWait = 3;
    public float waveWait = 8;
    public float offscreenX = -8;
    public float firstY = -2.2f;
    public GameObject[] Hostages;
    private int currentHostage = 0;
    public GameObject Enemy;
	// Use this for initialization
	void Start ()
	{
        StartCoroutine("Spawner");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void SpawnWaves()
    {
        float ySpawn=Random.Range(0, 5)*0.9f;
        
        Vector3 spawnPosition = new Vector3(offscreenX,ySpawn+firstY,0);
        Quaternion spawnDirection = Quaternion.identity;
        Instantiate(Enemy,spawnPosition,spawnDirection);
    }
    void SpawnGuard(float ySpawn) {
        

        Vector3 spawnPosition = new Vector3(offscreenX, ySpawn + firstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        Instantiate(Enemy, spawnPosition, spawnDirection);
    }

    void SpawnHostage(float ySpawn)
    {
        
        Vector3 spawnPosition = new Vector3(offscreenX, ySpawn + firstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        Instantiate(Hostages[currentHostage], spawnPosition, spawnDirection);
        currentHostage++;

    }

    IEnumerator Spawner()
    {

        yield return new WaitForSeconds(startWait);
        while (true)
        {
            int enemyCount = Random.Range(2, 20);
            int hostageSpawner = Random.Range(1, enemyCount);
            for (int i = 0; i < enemyCount; i++) {
                if (i == hostageSpawner)
                {
                    float ySpawn = Random.Range(0, 5) * 0.9f;
                    SpawnGuard(ySpawn);
                    yield return new WaitForSeconds(0.5f);
                    SpawnHostage(ySpawn);
                    yield return new WaitForSeconds(0.5f);
                    SpawnGuard(ySpawn);

                }
                else
                    SpawnWaves();
                
                yield return new WaitForSeconds(Random.Range(1,maxSpawnWait));
            }
            yield return new WaitForSeconds(waveWait);
        }
    }


}
