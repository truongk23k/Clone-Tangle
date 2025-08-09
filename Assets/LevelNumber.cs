using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelNumber : MonoBehaviour
{
    private Text level;
    private int levelNum;
    void Start()
    {
        level = GetComponent<Text>();
        levelNum = SceneManager.GetActiveScene().buildIndex;
        level.text = "LEVEL " + levelNum.ToString();
    }
    void Update()
    {

    }
}
