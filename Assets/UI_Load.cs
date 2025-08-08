using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Load : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadManager.Instance.LoadScene("Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
