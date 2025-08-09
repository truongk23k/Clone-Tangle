using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance;
    [SerializeField] private GameObject _loaderCanvas;
    private float _targetAmount;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private Text _progressText;

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

    void Start()
    {
        LoadScene("Menu");
    }

    public async void LoadScene(string nameScene)
    {
        _progressBar.value = 0;
        _targetAmount = 0;
        _loaderCanvas.SetActive(true);

        var scene = SceneManager.LoadSceneAsync(nameScene);
        scene.allowSceneActivation = false;

        do
        {
            await Task.Delay(Random.Range(200, 500));
            _targetAmount = scene.progress;

        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;

        // Chờ scene được kích hoạt
        while (!scene.isDone)
        {
            await Task.Yield();
        }

        await Task.Delay(500);
        _loaderCanvas.SetActive(false);
    }


    private void Update()
    {
        _progressBar.value = Mathf.MoveTowards(_progressBar.value, _targetAmount, 3 * Time.deltaTime);
        _progressText.text = Mathf.RoundToInt(_progressBar.value * 100).ToString() + " %";
    }
}
