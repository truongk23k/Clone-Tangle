using UnityEngine;

public class PinController : MonoBehaviour
{
    public bool canLift = true; // Pin có thể được nâng lên
    public bool isMoving = false; // Pin đang di chuyển đến vị trí mục tiêu
    public bool isMovingX = true; // Cho phép di chuyển theo trục X
    public bool isDragging = false; // Pin đang được kéo bằng chuột
    public bool hasMoved = false; // Pin đã được di chuyển từ vị trí ban đầu
    public bool isLifting = false; // Pin đang di chuyển lên
    public bool isDropping = false; // Pin đang di chuyển xuống
    private RaycastHit hitDown; // Kết quả raycast bắn xuống
    public Vector3 liftTarget; // Vị trí mục tiêu khi nâng (Y + 0.9f)
    public Vector3 dropTarget; // Vị trí mục tiêu khi thả xuống (Y - 0.9f)
    public Vector3 moveXTarget; // Vị trí mục tiêu trên trục X
    public Vector3 initialPos; // Vị trí ban đầu của pin
    public Vector3 dropPos; // Vị trí điểm thả (drop)
    public Selecter selecter;
    public float speed = 5f;
    public AudioSource audioSource; // Âm thanh khi nhấp pin

    void Start()
    {
        liftTarget = new Vector3(transform.position.x, transform.position.y + 0.9f, transform.position.z);
        selecter = GameObject.Find("Main Camera")?.GetComponent<Selecter>();

        if (selecter == null)
            Debug.LogError("Selecter not found on Main Camera!");
        if (audioSource == null)
            Debug.LogError("AudioSource not assigned!");
    }

    void Update()
    {
        // Kéo pin theo chuột
        if (Input.GetMouseButton(0) && !isLifting)
        {
            if (isDragging)
            {
                Physics.Raycast(transform.position, Vector3.down, out hitDown, 10f);
                Debug.DrawRay(transform.position, Vector3.down * 10, Color.yellow);
                float xInput = Input.GetAxis("Mouse X") * 5 * 0.1f;
                xInput = Mathf.Clamp(xInput, -1f, 1f);
                transform.position += new Vector3(xInput * Time.deltaTime, 0, 0);
                //limit X
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, -2.5f, 2.5f),
                    transform.position.y,
                    transform.position.z
                );

                if (transform.position.x != initialPos.x)
                {
                    hasMoved = true;
                    dropPos = (hitDown.collider != null && hitDown.collider.CompareTag("drop"))
                        ? hitDown.collider.transform.position
                        : initialPos;
                }
            }
        }

        // Thả pin
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if (hasMoved)
            {
                MoveToDrop(dropPos);
            }
        }

        // Di chuyển theo trục X
        if (isMoving && isMovingX)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveXTarget, speed * Time.deltaTime);
            if (transform.position == moveXTarget)
            {
                Physics.Raycast(transform.position, Vector3.down, out hitDown, 10f);
                if (hitDown.collider != null && hitDown.collider.CompareTag("pin"))
                {
                    MoveToDrop(initialPos);
                }
                else
                {
                    dropTarget = new Vector3(transform.position.x, transform.position.y - 0.9f, transform.position.z);
                    isDropping = true;
                    Reset();
                }
            }
        }

        // Di chuyển xuống
        if (isDropping)
        {
            transform.position = Vector3.MoveTowards(transform.position, dropTarget, speed * Time.deltaTime);
            if (transform.position == dropTarget)
            {
                isDropping = false;
                dropTarget = Vector3.zero;
                moveXTarget = Vector3.zero;
                liftTarget = new Vector3(transform.position.x, transform.position.y + 0.9f, transform.position.z);
                // Gọi sự kiện thả pin nếu cần
            }
        }

        // Di chuyển lên
        if (isLifting)
        {
            transform.position = Vector3.MoveTowards(transform.position, liftTarget, speed * Time.deltaTime);
            if (transform.position == liftTarget)
            {
                canLift = false;
                isLifting = false;
            }
        }
    }

    private void OnMouseDown()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    public void Reset()
    {
        isMoving = false;
        isDragging = false;
        isMovingX = true;
        canLift = true;
        hasMoved = false;
        initialPos = Vector3.zero;
        dropPos = Vector3.zero;
    }

    public void StartLifting()
    {
        if (canLift)
        {
            initialPos = transform.position;
            isDragging = true;
            isLifting = true;
        }
    }

    public void MoveToDrop(Vector3 target)
    {
        moveXTarget = new Vector3(target.x, transform.position.y, transform.position.z);
        isMoving = true;
        isMovingX = true;
        isDragging = false;
    }
}