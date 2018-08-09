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
    public GameObject Guard;
    public GameObject firstHostagePosition;
    private GameObject currentHostageGO;
    private bool guardAppeared = false;
    [HideInInspector] public static int NumberGuards;
	// Use this for initialization
	void Start ()
	{
        StartCoroutine("Spawner");
    }
	
	// Update is called once per frame
	void Update () {
	    if (NumberGuards == 0&&guardAppeared)
	    {
	        HostageBehaviour hostage;
            currentHostageGO = GameObject.FindWithTag("Hostage");

            
            hostage = currentHostageGO.GetComponent<HostageBehaviour>();
	        hostage.hostagePosition = firstHostagePosition;
            hostage.SaveHostage();
	        guardAppeared = !guardAppeared;
	    }
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
        GameObject tempGuard=Instantiate(Enemy, spawnPosition, spawnDirection);
        tempGuard.tag = "Guard";
    }

    void SpawnHostage(float ySpawn)
    {
        
        Vector3 spawnPosition = new Vector3(offscreenX, ySpawn + firstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        currentHostageGO=Instantiate(Hostages[currentHostage], spawnPosition, spawnDirection);
        currentHostage++;
        guardAppeared = !guardAppeared;
    }

    IEnumerator Spawner()
    {

        yield return new WaitForSeconds(startWait);
        while (true)
        {
            int enemyCount = Random.Range(2, 15);
            int hostageSpawner = Random.Range(1, enemyCount);
            for (int i = 0; i < enemyCount; i++) {
                if (i == hostageSpawner)
                {
                    int firstGuards= Random.Range(1, 3);
                    int secondGuards = Random.Range(1, 3);
                    NumberGuards = firstGuards + secondGuards;
                    float ySpawn = Random.Range(0, 5) * 0.9f;
                    for (int j = 0; j < firstGuards; j++)
                    {
                        SpawnGuard(ySpawn);
                        yield return new WaitForSeconds(0.4f);
                    }
                    
                    SpawnHostage(ySpawn);
                    
                    for (int j = 0; j < secondGuards; j++)
                    {
                        yield return new WaitForSeconds(0.4f);
                        SpawnGuard(ySpawn);
                    }
                    

                }
                else
                    SpawnWaves();
                
                yield return new WaitForSeconds(Random.Range(0.5f,maxSpawnWait));
            }
            yield return new WaitForSeconds(waveWait);
        }
    }


}
