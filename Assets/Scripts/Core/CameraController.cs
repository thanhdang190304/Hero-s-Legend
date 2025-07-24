using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Horizontal Follow")]
    [SerializeField] private float aheadDistance = 2f;
    [SerializeField] private float cameraSpeed = 4f;

    [Header("Vertical Follow")]
    [SerializeField] private float verticalSmoothTime = 0.2f;

    [Header("Zoom Settings")]
    [SerializeField] private float defaultZoom = 5f;
    [SerializeField] private float zoomOutSize = 8f;
    [SerializeField] private float zoomSpeed = 3f;

    [Header("Mouse Edge Movement")]
    [SerializeField] private bool enableMouseEdge = true;
    [SerializeField] private float edgeThreshold = 10f; // pixels
    [SerializeField] private float mouseMoveSpeed = 5f;

    private Transform player;
    private float lookAhead;
    private float verticalVelocity;
    private bool isZoomedOut = false;

    private Camera cam;
    private float currentPosX;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        cam.orthographicSize = defaultZoom;
    }

    void Update()
    {
        // Try to find the player if not yet assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Get current position
        Vector3 currentPos = transform.position;

        // ---------- Vertical follow ----------
        float targetY = player != null
            ? Mathf.SmoothDamp(currentPos.y, player.position.y, ref verticalVelocity, verticalSmoothTime)
            : currentPos.y;

        // ---------- Horizontal follow ----------
        float targetX = currentPos.x;
        if (player != null)
        {
            lookAhead = Mathf.Lerp(lookAhead, aheadDistance * Mathf.Sign(player.localScale.x), Time.deltaTime * cameraSpeed);
            targetX = player.position.x + lookAhead;
        }

        // ---------- Mouse-based movement ----------
        if (enableMouseEdge)
        {
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x >= Screen.width - edgeThreshold)
                targetX += mouseMoveSpeed * Time.deltaTime;
            else if (mousePos.x <= edgeThreshold)
                targetX -= mouseMoveSpeed * Time.deltaTime;

            if (mousePos.y >= Screen.height - edgeThreshold)
                targetY += mouseMoveSpeed * Time.deltaTime;
            else if (mousePos.y <= edgeThreshold)
                targetY -= mouseMoveSpeed * Time.deltaTime;
        }

        transform.position = new Vector3(targetX, targetY, currentPos.z);

        // ---------- Zoom toggle ----------
        float targetZoom = isZoomedOut ? zoomOutSize : defaultZoom;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            isZoomedOut = !isZoomedOut;
    }

    // Optional: exposable method for room‑based jumps
    public void MoveToNewRoom(Transform newRoom)
    {
        currentPosX = newRoom.position.x;
        transform.position = new Vector3(currentPosX, transform.position.y, transform.position.z);
    }
}
