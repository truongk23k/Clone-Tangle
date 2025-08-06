using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : MonoBehaviour
{
    public GameObject lefted;
    public bool leftingOn, leftingOff;
    RaycastHit hit;

    void Start()
    {
        leftingOn = true;
        leftingOff = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(transform.position, Vector3.forward * 10, Color.yellow);
            if (Physics.Raycast(ray, out hit, 20))
            {
                //top
                if (lefted == null && hit.collider.tag == "pin")
                {
                    lefted = hit.collider.gameObject;
                    lefted.GetComponent<PinController>().StartLifting();
                    //return;
                }

                //down
                if (lefted != null && hit.collider.tag == "drop")
                {
                    Vector3 next = hit.collider.gameObject.transform.position;
                    lefted.GetComponent<PinController>().MoveToDrop(next);
                    Resetting();
                }
            }

        }
    }

    public void Resetting()
    {
        leftingOn = true;
        leftingOff = false;
        lefted = null;
    }
}
