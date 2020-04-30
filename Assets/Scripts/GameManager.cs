using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int lives = 0;
    public int difficultyGame;
    public float spawnRate = 1f;
    public bool isClicking = false;
    public bool isPaused = false;

    public List<GameObject> targets;
    public TrailRenderer trailInactive;
    public TrailRenderer trailActive;
    public GameObject titleScreen;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI pauseText;
    public Button restartButton;
    public AudioClip countdownSound;
    public AudioClip endCountdownSound;

    private AudioSource music;
    private int score;
    private int newScore;
    private int countdown = 3;
    private int highScore = 0;
    private bool scoreEffect = false;

    private void Start()
    {
        music = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTrailToCursor(Input.mousePosition);

        isClicking = Input.GetMouseButton(0);
        if (!isPaused && isClicking && !trailActive.gameObject.activeSelf)
        {
            trailActive.gameObject.SetActive(true);
            trailInactive.gameObject.SetActive(false);
        }
        else if (!isPaused && !isClicking && !trailInactive.gameObject.activeSelf)
        {
            trailActive.gameObject.SetActive(false);
            trailInactive.gameObject.SetActive(true);
        }

        if (scoreEffect)
        {
            if (score > newScore)
                score--;
            else
                score++;

            scoreText.text = "Score: " + score.ToString();
            scoreEffect = score != newScore;
        }

        if (Input.GetKeyDown(KeyCode.P) && lives > 0)
            PauseGameToggle();
    }

    void MoveTrailToCursor(Vector3 screenPosition)
    {
        trailInactive.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 5f));
        trailActive.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 5f));
    }

    IEnumerator SpawnTarget()
    {
        yield return new WaitForSeconds(Random.Range(spawnRate-.2f, spawnRate));
        int index = Random.Range(0, targets.Count);
        Instantiate(targets[index]);

        if (lives > 0)
            StartCoroutine(SpawnTarget());
    }

    public void UpdateScore(int scoreToAdd)
    {
        newScore = score + scoreToAdd;
        scoreEffect = true;
    }

    public void UpdateLives()
    {
        if (lives > 0)
        {
            lives--;
            livesText.text = "Lives: " + lives.ToString();
        }

        if(lives <= 0)
        {
            if(newScore > highScore)
                highScore = newScore;

            gameOverText.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            if(highScore > 0)
            {
                highscoreText.text = "HIGHSCORE: " + highScore;
                highscoreText.gameObject.SetActive(true);
            }
        }
    }

    public void StartGame(int difficulty)
    {
        difficultyGame = difficulty;
        spawnRate /= difficulty;

        if (titleScreen.activeSelf)
            titleScreen.SetActive(false);

        GameObject[] remainingFoods = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject remainingFood in remainingFoods)
            Destroy(remainingFood);

        GameObject[] remainingBads = GameObject.FindGameObjectsWithTag("Bad");
        foreach (GameObject remainingBad in remainingBads)
            Destroy(remainingBad);

        score = 0;
        newScore = 0;
        lives = 3;

        scoreText.text = "Score: " + score.ToString();
        livesText.text = "Lives: " + lives.ToString();
        gameOverText.gameObject.SetActive(false);
        highscoreText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        StartCoroutine(CountdownRoutine());
    }

    public void RestartGame()
    {
        spawnRate = 1f;
        gameOverText.gameObject.SetActive(false);
        highscoreText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        if (!titleScreen.activeSelf)
            titleScreen.SetActive(true);
    }

    IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        countdownText.gameObject.SetActive(true);
        for (int i = countdown; i >= 0; i--)
        {
            if (i == 0)
            {
                AudioSource.PlayClipAtPoint(endCountdownSound, Camera.main.transform.position);
                countdownText.text = "GO!";
            }
            else
            {
                AudioSource.PlayClipAtPoint(countdownSound, Camera.main.transform.position);
                countdownText.text = i.ToString();
            }
            yield return new WaitForSeconds(1f);
        }
        countdownText.gameObject.SetActive(false);

        StartCoroutine(SpawnTarget());
    }

    private void PauseGameToggle()
    {
        isPaused = !isPaused;
        if (!isPaused)
        {
            Time.timeScale = 1;
            pauseText.gameObject.SetActive(false);
        }

        else
        {
            Time.timeScale = 0;
            pauseText.gameObject.SetActive(true);
            trailInactive.gameObject.SetActive(false);
            trailActive.gameObject.SetActive(false);
        }
    }

    public void ChangeMusicVolume(Slider slider)
    {
        music.volume = slider.value;
    }
}
