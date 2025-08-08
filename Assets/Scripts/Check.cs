using UnityEngine;

public class Check : MonoBehaviour
{
    private Decider ropeDecider;

    void Start()
    {
        ropeDecider = GameObject.Find("ROPE_DECIDER").GetComponent<Decider>();
    }

    void Update()
    {
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "otherrope")
        {
            if (collision.gameObject.transform.parent == transform.parent)
            {

            }
            else
            {
                ropeDecider.stillInContact();
            }

        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "otherrope")
        {
            if (collision.gameObject.transform.parent == transform.parent)
            {
                //  Debug.Log(collision.gameObject.transform.parent+"<--->"+transform.parent);
                // Debug.Log("same parent 2.0");
            }
            else
            {
                ropeDecider.outOfContact();
            }
        }
    }
}
