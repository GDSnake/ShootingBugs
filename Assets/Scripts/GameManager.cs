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
    public float PathwayPositionOffset = 0.9f;
    public GameObject[] Hostages;
    public GameObject Enemy;
    /// <summary>
    /// Holds the first position of the hostage on the top right side of the screen
    /// </summary>
    public GameObject FirstHostagePosition;

    public static int SavedHostages;
    public static int DeadHostages;
    public static int NumberGuards;
    public static int ScoreDifference;
    /// <summary>
    /// Necessary to manage the ShowScore corroutine
    /// </summary>
    public static bool ShowScoreChange = false;
    public static bool UpdateTotalScore = false;

    private GameObject _currentHostageGo;
    private HostageBehaviour _hostage;
    /// <summary>
    /// Checks if a hostage has appeared, to prevent nullpointing in update
    /// </summary>
    private bool _hostageAppeared = false;
    /// <summary>
    /// Prevents continuing shooting after game ending
    /// </summary>
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
        TotalScore.text = "Score : " + _currentScore;
        if (Input.GetMouseButtonDown(0)&&!_finished)
        {
            ShootingAudio.Play();
            Bullets--;
        }
        if (SavedHostages + DeadHostages == 5) { //Checks for winning condition, at least one hostage saved
             _currentHostageGo.gameObject.transform.localScale= new Vector3(1,1,1);
            Time.timeScale = 0;
            if (SavedHostages == 5) {
                WinText.enabled = true;
            } else {
                BadEndingText.enabled = true;
            }
            WaveText.enabled = false;
            _finished = true;
            RestartButton.gameObject.SetActive(true);
        }
	    else if (Bullets == 0 || DeadHostages >= 5) { // Checks for losing condition, no hostage saved or no bullets left
            GameOverText.enabled = true;
            RestartButton.gameObject.SetActive(true);
            WaveText.enabled = false;
            Time.timeScale = 0;
            _finished = true;
        }

        UpdateBulletsText();
	    if (NumberGuards == 0&&_hostageAppeared)
	    {	         
            _hostage = _currentHostageGo.GetComponent<HostageBehaviour>();
            _hostage.SaveHostage();
	        _hostageAppeared = false;
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

    /// <summary>
    /// This function spawns an instance of the enemy GO on a random pathway
    /// </summary>
    void SpawnWaves()
    {
        float ySpawn=Random.Range(0, 5)*PathwayPositionOffset;
        
        Vector3 spawnPosition = new Vector3(OffscreenX,ySpawn+FirstY,0);
        Quaternion spawnDirection = Quaternion.identity;
        GameObject temp =Instantiate(Enemy,spawnPosition,spawnDirection);
        temp.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(5-ySpawn+1);
    }

    /// <summary>
    /// This function spawns an instance of the enemy GO as a guard on the same pathway of the hostage
    /// </summary>
    /// <param name="ySpawn">Hostage pathway</param>
    void SpawnGuard(float ySpawn) {
        

        Vector3 spawnPosition = new Vector3(OffscreenX, ySpawn + FirstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        GameObject tempGuard=Instantiate(Enemy, spawnPosition, spawnDirection);
        tempGuard.tag = "Guard";
        tempGuard.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(5-ySpawn+1);
    }

    /// <summary>
    /// This fucntion spawns an instance of a hostage GO on the ySpawn pathway
    /// </summary>
    /// <param name="ySpawn">The pathway to be spawned</param>
    void SpawnHostage(float ySpawn)
    {
        
        Vector3 spawnPosition = new Vector3(OffscreenX, ySpawn + FirstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        Instantiate(Hostages[_currentHostage], spawnPosition, spawnDirection);
        _currentHostageGo = GameObject.FindWithTag("Hostage");
        _hostage = _currentHostageGo.GetComponent<HostageBehaviour>();
        _hostage.HostagePosition = FirstHostagePosition.transform.position + (Vector3.left * (_currentHostage));
        _currentHostageGo.gameObject.GetComponent<SpriteRenderer>().sortingOrder = (int)(5-ySpawn+1);
        _hostageAppeared = true;
        _currentHostage++;
        
    }
    /// <summary>
    /// Main coroutine that spawns GameObjects
    /// </summary>
    /// <returns></returns>
    IEnumerator Spawner()
    {

        yield return new WaitForSeconds(StartWait);

        while (true) {
            WaveText.enabled = false;
            
            int enemyCount = Random.Range(2, 15); // sets the number of enemies per round randomly between 2 to 14
            int hostageSpawner = Random.Range(1, enemyCount); // randomly chooses spawn position in time for the hostage and it's guards, the enemy of that round is not spawned
            for (int i = 0; i < enemyCount; i++) {
                if (i == hostageSpawner)
                {
                    int firstGuards= Random.Range(1, 3); // sets the number of front guards to 1 or 2
                    int secondGuards = Random.Range(1, 3); // sets the number of back guards to 1 or 2
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

    /// <summary>
    /// Coroutine that shows GO point value during certain time
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowScore()
    {
        ScoreChange.enabled = true;
        yield return new WaitForSecondsRealtime(0.3f); // this causes unexpected behaviour if two or more temporary scores are shown in less than 0.3 seconds, only the first one shows
        ScoreChange.enabled = false;
        ShowScoreChange = false;
    }

}
