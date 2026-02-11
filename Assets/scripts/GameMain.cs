using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class GameMain : MonoBehaviour {

    public GameObject bird;
    public GameObject readyPic;
    public GameObject tipPic;
    public GameObject scoreMgr;
    public GameObject pipeSpawner;
    public GameObject background;

    private bool gameStarted = false;
    private bool gameOver = false;
    private int currentScore = 0;
    
    // UI References
    private GameObject gameOverPanel;
    private Text gameOverScoreText;
    private Text gameOverInfoText;

	// Use this for initialization
	void Start () {
        // Auto-find background if not assigned
        if (background == null)
        {
            background = GameObject.Find("bg") ?? GameObject.FindWithTag("bg");
            if (background == null)
            {
                // Try to find any sprite renderer in scene
                SpriteRenderer[] renderers = FindObjectsOfType<SpriteRenderer>();
                foreach (SpriteRenderer renderer in renderers)
                {
                    if (renderer.gameObject.name.Contains("bg") || renderer.gameObject.name.Contains("background"))
                    {
                        background = renderer.gameObject;
                        break;
                    }
                }
            }
        }
        
        // Initialize Game Over UI
        CreateGameOverUI();
    }
	
	// Update is called once per frame
	void Update () {
        if (!gameStarted && Input.GetButtonDown("Fire1") && !gameOver)
        {
            gameStarted = true;
            StartGame();
        }

        // Restart game after Game Over
        if (gameOver && Input.GetButtonDown("Fire1"))
        {
            RestartGame();
        }
    }

    private void StartGame()
    {
        BirdControl control = bird.GetComponent<BirdControl>();
        control.inGame = true;
        control.JumpUp();

        readyPic.GetComponent<SpriteRenderer>().material.DOFade(0f, 0.2f);
        tipPic.GetComponent<SpriteRenderer>().material.DOFade(0f, 0.2f);

        currentScore = 0;
        scoreMgr.GetComponent<ScoreMgr>().SetScore(0);
        pipeSpawner.GetComponent<PipeSpawner>().StartSpawning();
        
        // Update background color for score 0
        UpdateBackgroundColor(0);
        
        gameOver = false;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // Update background color based on score - VISUAL FEEDBACK ENHANCEMENT (Roll ending in 3)
    public void UpdateBackgroundColor(int score)
    {
        currentScore = score;
        
        if (background == null)
            return;

        Color newColor;
        
        // Score ranges with different colors
        if (score < 5)
        {
            newColor = new Color(0.5f, 0.7f, 1f); // Light blue for score 0-4
        }
        else if (score < 10)
        {
            newColor = new Color(0.3f, 0.9f, 0.5f); // Green for score 5-9
        }
        else if (score < 15)
        {
            newColor = new Color(1f, 0.9f, 0.3f); // Yellow for score 10-14
        }
        else if (score < 20)
        {
            newColor = new Color(1f, 0.7f, 0.3f); // Orange for score 15-19
        }
        else
        {
            newColor = new Color(1f, 0.3f, 0.3f); // Red for score 20+
        }

        // Apply color with smooth transition
        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        if (bgRenderer != null)
        {
            bgRenderer.DOColor(newColor, 0.3f);
        }
    }

    // Called by BirdControl when collision happens
    public void OnGameOver()
    {
        gameOver = true;
        gameStarted = false;
        ShowGameOverScreen();
    }

    private void CreateGameOverUI()
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("GameOverCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Create Game Over Panel
        GameObject panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);
        
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Create "GAME OVER" text
        GameObject gameOverTextObj = new GameObject("GameOverText");
        gameOverTextObj.transform.SetParent(panelObj.transform, false);
        
        Text gameOverTextComponent = gameOverTextObj.AddComponent<Text>();
        gameOverTextComponent.text = "GAME OVER";
        gameOverTextComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        gameOverTextComponent.fontSize = 60;
        gameOverTextComponent.fontStyle = FontStyle.Bold;
        gameOverTextComponent.alignment = TextAnchor.MiddleCenter;
        gameOverTextComponent.color = Color.yellow;
        
        RectTransform gameOverTextRect = gameOverTextObj.GetComponent<RectTransform>();
        gameOverTextRect.anchoredPosition = new Vector2(0, 150);
        gameOverTextRect.sizeDelta = new Vector2(600, 100);

        // Create Score Text
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(panelObj.transform, false);
        
        gameOverScoreText = scoreTextObj.AddComponent<Text>();
        gameOverScoreText.text = "Score: 0";
        gameOverScoreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        gameOverScoreText.fontSize = 40;
        gameOverScoreText.alignment = TextAnchor.MiddleCenter;
        gameOverScoreText.color = Color.white;
        
        RectTransform scoreTextRect = scoreTextObj.GetComponent<RectTransform>();
        scoreTextRect.anchoredPosition = new Vector2(0, 50);
        scoreTextRect.sizeDelta = new Vector2(600, 80);

        // Create Mandatory Text (Name and Roll Number)
        GameObject infoTextObj = new GameObject("InfoText");
        infoTextObj.transform.SetParent(panelObj.transform, false);
        
        gameOverInfoText = infoTextObj.AddComponent<Text>();
        gameOverInfoText.text = "Flappy Bird – Modified by Ritesh Hans, Roll No. 220893";
        gameOverInfoText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        gameOverInfoText.fontSize = 20;
        gameOverInfoText.alignment = TextAnchor.MiddleCenter;
        gameOverInfoText.color = new Color(0.2f, 1f, 0.8f); // Cyan color
        
        RectTransform infoTextRect = infoTextObj.GetComponent<RectTransform>();
        infoTextRect.anchoredPosition = new Vector2(0, -50);
        infoTextRect.sizeDelta = new Vector2(800, 100);

        // Create Restart Instructions
        GameObject restartTextObj = new GameObject("RestartText");
        restartTextObj.transform.SetParent(panelObj.transform, false);
        
        Text restartText = restartTextObj.AddComponent<Text>();
        restartText.text = "Tap/Click to Restart";
        restartText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        restartText.fontSize = 25;
        restartText.alignment = TextAnchor.MiddleCenter;
        restartText.color = Color.yellow;
        
        RectTransform restartTextRect = restartTextObj.GetComponent<RectTransform>();
        restartTextRect.anchoredPosition = new Vector2(0, -150);
        restartTextRect.sizeDelta = new Vector2(600, 60);

        gameOverPanel = panelObj;
        gameOverPanel.SetActive(false);
    }

    private void ShowGameOverScreen()
    {
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score: " + currentScore;
        }
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        gameStarted = false;
        gameOver = false;
        currentScore = 0;

        // Reset bird position and state
        BirdControl birdControl = bird.GetComponent<BirdControl>();
        Rigidbody2D birdRigidbody = bird.GetComponent<Rigidbody2D>();
        Animator birdAnimator = bird.GetComponent<Animator>();
        
        bird.transform.position = new Vector3(-5, 0, 0);
        bird.transform.rotation = Quaternion.identity;
        birdRigidbody.velocity = Vector2.zero;
        birdRigidbody.gravityScale = 1;
        birdControl.inGame = false;
        
        // Reset animator state
        if (birdAnimator != null)
        {
            birdAnimator.ResetTrigger("die");
        }

        // Show ready and tip pictures again
        readyPic.GetComponent<SpriteRenderer>().material.DOFade(1f, 0.2f);
        tipPic.GetComponent<SpriteRenderer>().material.DOFade(1f, 0.2f);

        // Destroy all existing pipes
        GameObject[] pipes = GameObject.FindGameObjectsWithTag("movable");
        foreach (GameObject pipe in pipes)
        {
            if (pipe != bird && pipe != pipeSpawner)
                Destroy(pipe);
        }

        // Reset score display
        scoreMgr.GetComponent<ScoreMgr>().SetScore(0);
        
        // Reset background color
        UpdateBackgroundColor(0);

        // Hide Game Over panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
