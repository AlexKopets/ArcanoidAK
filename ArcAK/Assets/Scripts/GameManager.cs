using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public LevelSpawner spawner;
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public GameObject paddlePrefab;
    public Transform paddleParent;
    public TextMeshProUGUI bricksText;
    public TextMeshProUGUI stateText;
    public bool pauseOnGameOver = true;

    GameObject currentBall;
    GameObject currentPaddle;
    int totalBricks;

    void Awake()
    {
        // Safety: hide state text at start
        if (stateText != null) stateText.gameObject.SetActive(false);
    }

    void Start()
    {
        // try auto-assign if inspector left empty
        if (spawner == null) spawner = FindObjectOfType<LevelSpawner>();
        if (ballSpawnPoint == null)
        {
            var t = GameObject.Find("BallSpawnPoint");
            if (t != null) ballSpawnPoint = t.transform;
        }
        if (paddleParent == null)
        {
            var p = GameObject.Find("PaddleParent");
            if (p != null) paddleParent = p.transform;
        }
        if (bricksText == null)
            bricksText = GameObject.Find("BricksText")?.GetComponent<TextMeshProUGUI>();
        if (stateText == null)
            stateText = GameObject.Find("StateText")?.GetComponent<TextMeshProUGUI>();

        StartLevel();
    }

    public void StartLevel()
    {
        // cleanup from previous run
        Time.timeScale = 1f;
        if (stateText != null) stateText.gameObject.SetActive(false);

        // spawn bricks
        GameObject[] spawned = new GameObject[0];
        if (spawner != null)
            spawned = spawner.SpawnAll();

        totalBricks = spawned.Length;
        UpdateBricksUI();

        // spawn paddle and ball
        SpawnPaddle();
        SpawnBall();
    }

    void SpawnPaddle()
    {
        if (paddlePrefab == null || paddleParent == null) return;

        // remove old paddle
        if (currentPaddle != null) Destroy(currentPaddle);

        currentPaddle = Instantiate(paddlePrefab, paddleParent);
        // optionally force local position reset
        currentPaddle.transform.localPosition = Vector3.zero;
    }

    void SpawnBall()
    {
        if (ballPrefab == null || ballSpawnPoint == null) return;

        if (currentBall != null) Destroy(currentBall);

        currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
    }

    public void OnBrickDestroyed()
    {
        totalBricks = Mathf.Max(0, totalBricks - 1);
        UpdateBricksUI();

        if (totalBricks <= 0)
            Win();
    }

    void UpdateBricksUI()
    {
        if (bricksText != null)
            bricksText.text = $"Bricks: {totalBricks}";
    }

    public void GameOver(string reason = "Game Over")
    {
        if (stateText != null)
        {
            stateText.text = reason + "\nClick to Restart";
            stateText.gameObject.SetActive(true);
        }

        if (pauseOnGameOver) Time.timeScale = 0f;
    }

    void Win()
    {
        if (stateText != null)
        {
            stateText.text = "You Win!\nClick to Restart";
            stateText.gameObject.SetActive(true);
        }

        if (pauseOnGameOver) Time.timeScale = 0f;
    }

    void Update()
    {
        // Restart on mouse click when state text active
        if (stateText != null && stateText.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
                Restart();
        }
    }

    public void Restart()
    {
        // cleanup
        Time.timeScale = 1f;

        // destroy bricks, paddle, ball
        if (spawner != null && spawner.parentForBricks != null)
        {
            var parent = spawner.parentForBricks;
            for (int i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }

        if (currentBall != null) Destroy(currentBall);
        if (currentPaddle != null) Destroy(currentPaddle);

        StartLevel();
    }
}