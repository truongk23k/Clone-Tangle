using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpingHand : MonoBehaviour
{
    public GameObject redPlugin;
    public bool checkedFirst;
    void Start()
    {
        checkedFirst = false;

    }
    void Update()
    {
        if (redPlugin.GetComponent<PinController>().canLift == false && !checkedFirst)
        {
            transform.position = new Vector3(-1.5f, transform.position.y, transform.position.z);
            checkedFirst = true;
        }
        if (redPlugin.GetComponent<PinController>().canLift == true && checkedFirst)
        {
            Destroy(gameObject);
        }
    }
}
