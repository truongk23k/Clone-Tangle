using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public GameObject panelLevel;
    public Button[] buttons;
    private void Awake()
    {
        AutoChild();

        int unlockedLEvel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < unlockedLEvel)
                buttons[i].interactable = true;
            else
                buttons[i].interactable = false;
        }

        panelLevel.SetActive(false);
    }
    public void OpenLevel(int levelNum)
    {
        string levelName = "Level" + levelNum;
        LoadManager.Instance.LoadScene(levelName);
    }

    private void AutoChild()
    {
        buttons = new Button[transform.childCount];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}
