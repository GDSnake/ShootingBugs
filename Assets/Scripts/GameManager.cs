using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
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
    public int MaxRows = 5;
    public int MinEnemiesPerWave = 2;
    public int MaxEnemiesPerWave = 15;
    public int MinConsecutiveGuards = 1;
    public int MaxConsecutiveGuards = 3;
    public float StartWait = 1;
    public float MaxSpawnWait = 3;
    public float WaveWait = 8;
    public float OffscreenX = -8;
    public float FirstY = -2.2f;
    public float PathwayPositionOffset = 0.9f;
    public float TimeBetweenGuards = 0.4f;
    public float ShowScoreTimer = 0.3f;
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
    /// <summary>
    /// Prevents continuing shooting after game ending
    /// </summary>
    public static bool _finished = false;

    private GameObject _currentHostageGo;
    private HostageBehaviour _hostage;
    private int _maxHostages;

    /// <summary>
    /// Checks if a hostage has appeared, to prevent nullpointing in update
    /// </summary>
    private bool _hostageAppeared = false;
    private int _currentHostage;
    private int _currentScore;

    private readonly string _scoreText = "Score :";
    private readonly string _waveText = "Wave ";
    private readonly string _bulletText = "Bullets Left: ";

    // WaitForSeconds caching to save on garbage allocation
    private WaitForSeconds _nextWaveWait;
    private readonly WaitForSeconds _oneSecondWait = new WaitForSeconds(1);
    private WaitForSeconds _gameStartWait;
    private WaitForSeconds _timeBetweenGuards;
    private WaitForSeconds _timeToShowScore;
    // Use this for initialization
    void Start() {
        _timeToShowScore = new WaitForSeconds(ShowScoreTimer);
        _timeBetweenGuards = new WaitForSeconds(TimeBetweenGuards);
        _nextWaveWait = new WaitForSeconds(WaveWait - 1);
        _gameStartWait = new WaitForSeconds(StartWait);
        ScoreChange.enabled = false;
        TotalScore.enabled = true;
        _currentScore = 0;
        TotalScore.text = _scoreText + _currentScore;
        WaveText.enabled = true;
        WaveText.text = _waveText + (_currentHostage + 1);
        RestartButton.gameObject.SetActive(false);
        Time.timeScale = 1;
        _currentHostage = 0;
        SavedHostages = 0;
        DeadHostages = 0;
        _maxHostages = Hostages.Length;
        WinText.enabled = false;
        GameOverText.enabled = false;
        BadEndingText.enabled = false;
        _finished = false;
        StartCoroutine(Spawner());
    }

    // Update is called once per frame
    void Update() {

        UpdateUI();

        ProcessInput();

        if (!_finished)// checks if the game has ended to prevent unecessary code execution
        {
            UpdateGameLogic();
        }
    }

    private void UpdateUI() {
        if (ShowScoreChange) {
            if (ScoreDifference > 0) {
                ScoreChange.text = "+ " + ScoreDifference;
            } else {
                ScoreChange.text = "" + ScoreDifference;
            }
            StartCoroutine(ShowScore());
        }

        if (UpdateTotalScore) {
            _currentScore += ScoreDifference;
            UpdateTotalScore = !UpdateTotalScore;
            TotalScore.text = _scoreText + _currentScore;
        }
    }

    private void ProcessInput() {
        if (Input.GetMouseButtonDown(0) && !_finished) {
            ShootingAudio.Play();
            Bullets--;
            UpdateBulletsText();
        }
    }

    private void UpdateGameLogic() {
        if (SavedHostages + DeadHostages == _maxHostages) { //Checks for winning condition, at least one hostage saved 
            _currentHostageGo.gameObject.transform.localScale = new Vector3(1, 1, 1);
            Time.timeScale = 0;
            if (SavedHostages == _maxHostages) {
                WinText.enabled = true;
            } else if (DeadHostages == _maxHostages) {
                GameOverText.enabled = true;
                RestartButton.gameObject.SetActive(true);
                WaveText.enabled = false;
                Time.timeScale = 0;
            } else {
                BadEndingText.enabled = true;
            }
            WaveText.enabled = false;
            _finished = true;
            RestartButton.gameObject.SetActive(true);
        } else if (Bullets == 0) { // Checks for losing condition, no hostage saved or no bullets left
            GameOverText.enabled = true;
            RestartButton.gameObject.SetActive(true);
            WaveText.enabled = false;
            Time.timeScale = 0;
            _finished = true;
        }



        if (NumberGuards == 0 && _hostageAppeared) {
            _hostage = _currentHostageGo.GetComponent<HostageBehaviour>();
            _hostage.SaveHostage();
            _hostageAppeared = false;
        }
    }


    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    private void UpdateBulletsText() {
        BulletsLeft.text = _bulletText + Bullets;
    }

    private int SetSortingOrder(float ySpawn) {
        return (int)(MaxRows - ySpawn) + 1;
    }

    /// <summary>
    /// This function spawns an instance of the enemy GO on a random pathway
    /// </summary>
    private void SpawnWaves() {
        float ySpawn = Random.Range(0, MaxRows) * PathwayPositionOffset;

        Vector3 spawnPosition = new Vector3(OffscreenX, ySpawn + FirstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        GameObject temp = Instantiate(Enemy, spawnPosition, spawnDirection);
        temp.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(ySpawn);
    }

    /// <summary>
    /// This function spawns an instance of the enemy GO as a guard on the same pathway of the hostage
    /// </summary>
    /// <param name="ySpawn">Hostage pathway</param>
    private void SpawnGuard(float ySpawn) {


        Vector3 spawnPosition = new Vector3(OffscreenX, ySpawn + FirstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        GameObject tempGuard = Instantiate(Enemy, spawnPosition, spawnDirection);
        tempGuard.tag = "Guard";
        tempGuard.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(ySpawn);
    }

    /// <summary>
    /// This fucntion spawns an instance of a hostage GO on the ySpawn pathway
    /// </summary>
    /// <param name="ySpawn">The pathway to be spawned</param>
    private void SpawnHostage(float ySpawn) {
        Vector3 spawnPosition = new Vector3(OffscreenX, ySpawn + FirstY, 0);
        Quaternion spawnDirection = Quaternion.identity;
        Instantiate(Hostages[_currentHostage], spawnPosition, spawnDirection);
        _currentHostageGo = GameObject.FindWithTag("Hostage");
        _hostage = _currentHostageGo.GetComponent<HostageBehaviour>();
        _hostage.HostagePosition = FirstHostagePosition.transform.position + (Vector3.left * (_currentHostage));
        _currentHostageGo.gameObject.GetComponent<SpriteRenderer>().sortingOrder = SetSortingOrder(ySpawn);
        _hostageAppeared = true;
        _currentHostage++;
    }
    /// <summary>
    /// Main coroutine that spawns GameObjects
    /// </summary>
    /// <returns></returns>
    private IEnumerator Spawner() {

        yield return _gameStartWait;
 
        while (true) {
            WaveText.enabled = false;

            int enemyCount = Random.Range(MinEnemiesPerWave, MaxEnemiesPerWave); // sets the number of enemies per round randomly between min and max guards per wave
            int hostageSpawner = Random.Range(1, enemyCount); // randomly chooses spawn position in time for the hostage and it's guards, the enemy of that round is not spawned
            for (int i = 0; i < enemyCount; i++) {
                if (i == hostageSpawner) {
                    int firstGuards = Random.Range(MinConsecutiveGuards, MaxConsecutiveGuards); // sets the number of front guards between min and max consecutive guards
                    int secondGuards = Random.Range(MinConsecutiveGuards, MaxConsecutiveGuards); // sets the number of front guards between min and max consecutive guards
                    NumberGuards = firstGuards + secondGuards;
                    float ySpawn = Random.Range(0, MaxRows) * PathwayPositionOffset;
                    for (int j = 0; j < firstGuards; j++) {
                        SpawnGuard(ySpawn);
                        yield return _timeBetweenGuards;
                    }

                    SpawnHostage(ySpawn);

                    for (int j = 0; j < secondGuards; j++) {
                        yield return _timeBetweenGuards;
                        SpawnGuard(ySpawn);
                    }
                } else
                    SpawnWaves();

                yield return new WaitForSeconds(Random.Range(0.5f, MaxSpawnWait));
            }

            yield return _nextWaveWait;
            WaveText.enabled = true;
            WaveText.text = _waveText + (_currentHostage + 1);
            yield return _oneSecondWait;
            WaveText.enabled = false;
        }
    }

    /// <summary>
    /// Coroutine that shows GO point value during certain time
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowScore() {
        ScoreChange.enabled = true;
        yield return _timeToShowScore; // this causes unexpected behaviour if two or more temporary scores are shown in less than 0.3 seconds, only the first one shows
        ScoreChange.enabled = false;
        ShowScoreChange = false;
    }

}
