using UnityEngine;

public class Decider : MonoBehaviour
{
    public Selecter selecter;
    public Animator deviceAnimation;
    private bool lefted;
    public int inTouch;
    private float timerLeftWin;
    public float timeToWin;
    public GameObject winPanel;
    private GameObject paperEffect;

    private bool showedWin = false;
    void Start()
    {
        paperEffect = GameObject.Find("papereffect");
        paperEffect.SetActive(false);
        timeToWin = 0.75f;
        lefted = false;
        inTouch = 0;
        timerLeftWin = timeToWin;
        //winPanel.SetActive(false);
        //deviceAnimation.enabled = false;
    }
    void Update()
    {
        if (inTouch == 0 && lefted)
        {
            Debug.Log("Have a cap on the air!");
        }
        else if (inTouch == 0 && !lefted)
        {
            timerLeftWin -= Time.deltaTime;
            if (timerLeftWin <= 0 && !showedWin)
            {
                showedWin = true;
                Debug.LogWarning("WIN");
                //winPanel.SetActive(true);
                //PlayerPrefs.SetInt("level", SceneManager.GetActiveScene().buildIndex + 1);
                Destroy(selecter); //prevent from click when win
                paperEffect.SetActive(true);
                //deviceAnimation.enabled = true;
                //  Audiomanager.Instance.BgAudio.enabled = false;

            }
        }
        else
        {
            timerLeftWin = timeToWin;
        }
    }
    public void stillInContact()
    {
        inTouch++;
    }
    public void outOfContact()
    {
        inTouch--;
    }
    public void Lefted()
    {
        lefted = true;
    }
    public void Dropped()
    {
        lefted = false;
    }

}
