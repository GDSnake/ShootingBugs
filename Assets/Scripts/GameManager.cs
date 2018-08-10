using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Button RestartButton;

    public Text ScoreChange;
    public Text TotalScore;
    public Text BulletsLeft;
    public Text WinText;
    public Text BadEndingText;
    public Text GameOverText;
    public Text WaveText;
 
    public AudioSource ShootingAudio;
  
    public int Bullets;
    public float StartWait = 1;
    public float MaxSpawnWait = 3;
    public float WaveWait = 8;
    public float OffscreenX = -8;
    public float FirstY = -2.2f;
    public GameObject[] Hostages;
    public GameObject Enemy;
    public GameObject Guard;
    public GameObject FirstHostagePosition;

    public static int SavedHostages;
    public static int DeadHostages;
    public static int NumberGuards;
    public static int ScoreDifference;
    public static bool ShowScoreChange = false;
    public static bool UpdateTotalScore = false;

    private GameObject _currentHostageGo;
    private HostageBehaviour _hostage;
    private bool _guardAppeared = false;
    private bool _finished = false;
    private int _currentHostage;
    private int _currentScore;
     
	// Use this for initialization
	void Start ()
	{
        ScoreChange.enabled = false;
	    TotalScore.enabled = true;
	    _currentScore = 0;
	    TotalScore.text = "Score :" + _currentScore;
	    WaveText.enabled = true;
	    WaveText.text = "Wave " + (_currentHostage + 1);
        RestartButton.gameObject.SetActive(false);
	    Time.timeScale = 1;
        _currentHostage = 0;
	    SavedHostages = 0;
	    DeadHostages = 0;
	    WinText.enabled = false;
	    GameOverText.enabled = false;
	    BadEndingText.enabled = false;
        StartCoroutine("Spawner");
    }
	
	// Update is called once per frame
	void Update () {
	    if (ShowScoreChange)
	    {
	        if (ScoreDifference > 0)
	        {
	            ScoreChange.text = "+ " + ScoreDifference;
	        }
	        else
	        {
	            ScoreChange.text = "" + ScoreDifference;
	        }
	        
	        
	        StartCoroutine("ShowScore");

	    }
        if (UpdateTotalScore) {
            _currentScore += ScoreDifference;
            UpdateTotalScore = !UpdateTotalScore;
        }
        TotalScore.text = "Score :" + _currentScore;
        if (Input.GetMouseButtonDown(0)&&!_finished)
        {
            ShootingAudio.Play();
            Bullets--;
        }
	    if (Bullets == 0||DeadHostages>=5)
	    {
            GameOverText.enabled = true;
            RestartButton.gameObject.SetActive(true);
           
            Time.timeScale = 0;
	        _finished = true;
            

	    }
	    else if (SavedHostages + DeadHostages == 5)
	    {
            
            Time.timeScale = 0;
	        if (SavedHostages == 5)
	        {
	            WinText.enabled=true;
	            
	        }
	        else
	        {
	            BadEndingText.enabled=true;
	        }
            _finished = true;
            RestartButton.gameObject.SetActive(true);
        }
        UpdateBulletsText();
	    if (NumberGuards == 0&&_guardAppeared)
	    {
	         
            


            _hostage = _currentHostageGo.GetComponent<HostageBehaviour>();
	        _hostage.HostagePosition = FirstHostagePosition.transform.position+(Vector3.left*(_currentHostage-1));
            _hostage.SaveHostage();
	        _guardAppeared = !_guardAppeared;
	    }
	}

    

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
   
    }
    void UpdateBulletsText()
    {
        BulletsLeft.text = "Bullets Left: " + Bullets;
    }
    void SpawnWaves()
    {
        float ySpawn=Random.Range(0, 5)*0.9f;
        
        Vector3 spawnPosition = new Vector3(OffscreenX,ySpawn+FirstY,0);
        Quaternion spawnDirection = Quaternion.identity;
        GameObject temp =Instantiate(Enemy,spawnPosition,spawnDirection);
        temp.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(5-ySpawn+1);
    }
    void SpawnGuard(float ySpawn) {
        

        Vector3 spawnPosition = new Vector3(OffscreenX, ySpawn + FirstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        GameObject tempGuard=Instantiate(Enemy, spawnPosition, spawnDirection);
        tempGuard.tag = "Guard";
        tempGuard.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(5-ySpawn+1);
    }

    void SpawnHostage(float ySpawn)
    {
        
        Vector3 spawnPosition = new Vector3(OffscreenX, ySpawn + FirstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        Instantiate(Hostages[_currentHostage], spawnPosition, spawnDirection);
        _currentHostageGo = GameObject.FindWithTag("Hostage");
        _hostage = _currentHostageGo.GetComponent<HostageBehaviour>();
        _hostage.HostagePosition = FirstHostagePosition.transform.position + (Vector3.left * _currentHostage);
        _currentHostageGo.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(5-ySpawn+1);
        _currentHostage++;
        _guardAppeared = !_guardAppeared;
    }

    IEnumerator Spawner()
    {

        yield return new WaitForSeconds(StartWait);

        while (true) {
            WaveText.enabled = false;
            
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
                
                yield return new WaitForSeconds(Random.Range(0.5f,MaxSpawnWait));
            }
            WaveText.enabled = true;
            WaveText.text = "Wave " + (_currentHostage + 1);
            yield return new WaitForSeconds(WaveWait);
            WaveText.enabled = false;
        }
    }

    IEnumerator ShowScore()
    {
        ScoreChange.enabled = true;
        yield return new WaitForSecondsRealtime(0.3f);
        ScoreChange.enabled = false;
        ShowScoreChange = false;
    }

}
