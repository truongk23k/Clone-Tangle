using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance;

    [Header("UI Loading")]
    public GameObject loadingScreen;
    public Slider progressSlider;
    public Text progressText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Gọi hàm này để load scene với loading screen
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Hiện loading screen nếu có
        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressSlider != null)
                progressSlider.value = progress;

            if (progressText != null)
                progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            // Khi tiến trình load đạt đến 0.9, Unity chờ cho phép chuyển scene
            if (operation.progress >= 0.9f)
            {
                // Đảm bảo thanh đạt 100% trước khi chuyển scene
                if (progressSlider != null)
                    progressSlider.value = 1f;

                if (progressText != null)
                    progressText.text = "100%";

                yield return new WaitForSeconds(0.5f); // delay nhỏ để cảm giác mượt

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
