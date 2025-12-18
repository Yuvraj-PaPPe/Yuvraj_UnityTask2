using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    // The "Singleton" instance
    public static GameFlowManager Instance;

    private void Awake()
    {
        // If an instance already exists, destroy this new one (duplicate)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Otherwise, this is the main instance
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // You can add global logic here later, like:
    // public int playerHealth;
    // public Deck currentDeck;
    public void LoadScene(string sceneName)
{
    StartCoroutine(TransitionRoutine(sceneName));
}

private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. Fade Out
        if (VRScreenFader.Instance != null)
        {
            VRScreenFader.Instance.FadeOut();
            // CHANGE THIS: WaitForSecondsRealtime ignores the pause state
            yield return new WaitForSecondsRealtime(VRScreenFader.Instance.fadeDuration);
        }

        // 2. Load the Scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // IMPORTANT: Unpause the game! 
        // If you died and timeScale was 0, the next level will start frozen if you don't fix it here.
        Time.timeScale = 1f;

        // 3. Fade Back In
        if (VRScreenFader.Instance != null)
        {
            VRScreenFader.Instance.FadeIn();
        }
    }
}