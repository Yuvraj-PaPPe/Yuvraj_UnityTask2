using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // Required for Coroutines

public class GameManagerVR : MonoBehaviour
{
    public static GameManagerVR Instance;

    [Header("Scene Management")]
    public string resultSceneName = "ResultScene"; // TYPE EXACT SCENE NAME HERE IN INSPECTOR

    [Header("Timer")]
    public float matchTime = 180f; 
    public TextMeshProUGUI timerText;

    [Header("UI")]
    public GameObject winUI;
    public GameObject loseUI;

    public bool matchEnded = false;
    public bool MatchEnded => matchEnded; 

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (matchEnded) return;

        matchTime -= Time.deltaTime;
        UpdateTimerUI();

        if (matchTime <= 0f)
        {
            matchEnded = true;
            TimeOverCheck();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(matchTime / 60f);
        int seconds = Mathf.FloorToInt(matchTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void TimeOverCheck()
    {
        BaseHealth playerBase = GameObject.FindWithTag("PlayerBase").GetComponent<BaseHealth>();
        BaseHealth enemyBase = GameObject.FindWithTag("EnemyBase").GetComponent<BaseHealth>();

        if (playerBase.currentHealth > enemyBase.currentHealth)
            Win();
        else
            Lose();
    }

    public void OnBaseDestroyed(bool playerLost)
    {
        if (matchEnded) return; // Prevent double calling

        if (playerLost)
            Lose();
        else
            Win();
    }

    public void Win()
    {
        matchEnded = true;
        winUI.SetActive(true);
        Time.timeScale = 0; // Pause the game
        
        // Start the process to change scene
        StartCoroutine(LoadResultSceneDelay());
    }

    public void Lose()
    {
        matchEnded = true;
        loseUI.SetActive(true);
        Time.timeScale = 0; // Pause the game
        
        // Start the process to change scene
        StartCoroutine(LoadResultSceneDelay());
    }

    IEnumerator LoadResultSceneDelay()
    {
        // Wait for 3 seconds using Realtime (because Time.timeScale is 0)
        yield return new WaitForSecondsRealtime(3f);

        // IMPORTANT: Unpause the game before leaving, or the next scene will be frozen!
        Time.timeScale = 1;

        // Load the scene
        SceneManager.LoadScene(resultSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
    #else
        Application.Quit(); 
    #endif
    }
}