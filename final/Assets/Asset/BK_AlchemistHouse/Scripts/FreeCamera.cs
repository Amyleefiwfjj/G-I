using UnityEngine;
using UnityEngine.UI;

public class FreeCamera : MonoBehaviour
{
    [Header("Camera Movement")]
    public float movementSpeed = 5.0f;

    [Header("Interaction Settings")]
    public LayerMask interactableLayer;      // 상호작용 가능한 물체 레이어
    public float pickupDistance = 2.0f;      // 이 거리 이하로 오면 인벤토리 메시지 표시
    public Text promptText;                  // “인벤토리에 넣으시겠습니까?” UI 텍스트

    private bool isInteractionMode = false;
    private Camera cam;
    private Transform selectedObject;
    private Plane dragPlane;
    private Vector3 offset;

    void Start()
    {
        cam = Camera.main;

        // 마우스 커서 잠그기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 처음에는 메시지를 숨김
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        // G 키로 상호작용 모드 토글
        if (Input.GetKeyDown(KeyCode.G))
        {
            isInteractionMode = !isInteractionMode;

            if (isInteractionMode)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                ReleaseSelected();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                // 모드 빠져나오면 메시지 숨김
                if (promptText != null)
                    promptText.gameObject.SetActive(false);
            }
        }

        if (isInteractionMode)
        {
            HandleDragging();
        }
    }

    void FixedUpdate()
    {
        // 카메라 WASD 이동 (항상 적용)
        float h = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * movementSpeed;
        float v = Input.GetAxis("Vertical") * Time.fixedDeltaTime * movementSpeed;
        transform.Translate(h, 0, v);

        // 마우스 룩은 상호작용 모드가 아닐 때만
        if (!isInteractionMode)
        {
            float lookX = Input.GetAxis("Mouse X");
            float lookY = Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(-lookY, lookX, 0);
        }
    }

    private void HandleDragging()
    {
        // 1) 물체 선택
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
            {
                selectedObject = hit.transform;

                float y = selectedObject.position.y;
                dragPlane = new Plane(Vector3.up, new Vector3(0, y, 0));

                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    offset = selectedObject.position - hitPoint;
                }
            }
        }

        // 2) 드래그 중 물체 이동 및 거리 체크
        if (selectedObject != null && Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                selectedObject.position = hitPoint + offset;

                // 카메라와 물체 사이 거리 계산
                float dist = Vector3.Distance(cam.transform.position, selectedObject.position);
                if (promptText != null)
                {
                    if (dist <= pickupDistance)
                    {
                        promptText.text = "인벤토리에 넣으시겠습니까?";
                        promptText.gameObject.SetActive(true);
                    }
                    else
                    {
                        promptText.gameObject.SetActive(false);
                    }
                }
            }
        }

        // 3) 마우스 버튼을 떼면 선택 해제 및 메시지 숨김
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelected();
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }

    private void ReleaseSelected()
    {
        selectedObject = null;
    }
}
