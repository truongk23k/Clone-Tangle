using UnityEngine;

public class PinController : MonoBehaviour
{
    public bool canLift = true; // Pin có thể được nâng
    public bool isLifting = false; // Pin đang nâng lên

    public bool isMovingX = false; // Pin đang di chuyển theo trục X

    public bool isDropping = false; // Pin đang thả xuống

    private RaycastHit hitDown; // Kết quả raycast bắn xuống

    public Vector3 liftTarget; // Vị trí mục tiêu khi nâng (Y + 0.9f)
    public Vector3 dropTarget; // Vị trí mục tiêu khi thả (Y - 0.9f)
    public Vector3 moveXTarget; // Vị trí mục tiêu trên trục X
    public Vector3 initialPos; // Vị trí ban đầu của pin

    public Selecter selecter; // Tham chiếu Selecter
    public Decider ropeDecider; // Tham chiếu Decider
    public float speed = 5f; // Tốc độ di chuyển
    public AudioSource audioSource; // Âm thanh khi nhấp

    void Start()
    {
        Reset();

        selecter = GameObject.Find("Main Camera")?.GetComponent<Selecter>();
        ropeDecider = GameObject.Find("ROPE_DECIDER")?.GetComponent<Decider>();

        if (selecter == null) Debug.LogError("Selecter not found on Main Camera!");
        if (ropeDecider == null) Debug.LogError("Decider not found on ROPE_DECIDER!");
        if (audioSource == null) Debug.LogError("AudioSource not assigned!");
    }

    void Update()
    {
        // Di chuyển theo trục X
        ProcessMoveX();

        // Di chuyển xuống
        if (isDropping)
        {
            transform.position = Vector3.MoveTowards(transform.position, dropTarget, speed * Time.deltaTime);
            if (transform.position == dropTarget)
            {
                isDropping = false;
                ropeDecider.Dropped();

                Reset();
            }
        }

        // Di chuyển lên
        ProcessMoveUp();
    }

    private void ProcessMoveX()
    {
        if (isMovingX)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveXTarget, speed * Time.deltaTime);
            if (transform.position == moveXTarget)
            {
                Physics.Raycast(transform.position, Vector3.down, out hitDown, 10f);
                if (hitDown.collider.CompareTag("pin")) //|| hitDown.collider.CompareTag("otherrope")
                {
                    MoveToDrop(initialPos); // Trở về vị trí ban đầu nếu va chạm pin/rope khác
                }
                else
                {
                    dropTarget = transform.position - new Vector3(0, 0.9f, 0);
                    isMovingX = false; //lock move X
                    isDropping = true; //start drop

                }
            }
        }
    }

    private void ProcessMoveUp()
    {
        if (isLifting)
        {
            transform.position = Vector3.MoveTowards(transform.position, liftTarget, speed * Time.deltaTime);
            if (transform.position == liftTarget)
                isLifting = false;
        }
    }

    public void Reset()
    {
        liftTarget = transform.position + new Vector3(0, 0.9f, 0);

        canLift = true;
        isMovingX = false;
        isLifting = false;
        isDropping = false;
    }

    public void StartLifting()
    {
        if (canLift)
        {
            canLift = false;
            initialPos = transform.position;
            isLifting = true;

            ropeDecider.Lefted();
        }
    }

    public void MoveToDrop(Vector3 target)
    {
        moveXTarget = new Vector3(target.x, transform.position.y, transform.position.z);
        isMovingX = true;
    }
}