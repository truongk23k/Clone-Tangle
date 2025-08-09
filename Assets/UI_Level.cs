using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Begin,
    Play,
    Pause
}

public class UI_Level : MonoBehaviour
{
    public static UI_Level Instance;

    public GameState gameState;

    public GameObject winObj;
    public GameObject pauseObj;
    public GameObject helpObj;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameState = GameState.Begin;
        winObj.SetActive(false);
        pauseObj.SetActive(false);
        helpObj.SetActive(false);
    }

    public void PlayBtn()
    {
        gameState = GameState.Play;
    }

    public void ResetBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void NextBtn()
    {
        Debug.Log((SceneManager.GetActiveScene().buildIndex + 1) + " | " + SceneManager.sceneCountInBuildSettings);
        if ((int)(SceneManager.GetActiveScene().buildIndex + 1) >= SceneManager.sceneCountInBuildSettings)
            return;

        LoadManager.Instance.LoadScene("Level" + (int)(SceneManager.GetActiveScene().buildIndex + 1));
        AudioManager.Instance.PlayMusic("background");
    }

    public void PauseBtn()
    {
        Time.timeScale = 0;

        AudioManager.Instance.PauseAudio();
    }

    public void HomeBtn()
    {
        Time.timeScale = 1f;
        LoadManager.Instance.LoadScene("Menu");
        AudioManager.Instance.ResumeAudio();
    }

    public void ResumeBtn()
    {
        Time.timeScale = 1f;

        AudioManager.Instance.ResumeAudio();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Win()
    {
        Debug.LogWarning("WIN");
        winObj.SetActive(true);

        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("UnlockedLevel"))
            PlayerPrefs.SetInt("UnlockedLevel", SceneManager.GetActiveScene().buildIndex + 1);
    }
}
