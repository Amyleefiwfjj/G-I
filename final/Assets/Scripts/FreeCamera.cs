using UnityEngine;
using UnityEngine.UI;

public class FreeCameraWithPickup : MonoBehaviour
{
    [Header("Camera Movement")]
    public float movementSpeed = 5.0f;

    [Header("Interaction Settings")]
    public LayerMask interactableLayer;  // 상호작용 가능한 오브젝트 레이어
    public float pickupDistance = 2.0f;  // 이 거리 이내로 오면 인벤토리 메시지 표시
    public Text promptText;              // “인벤토리에 넣으시겠습니까?” UI 텍스트

    private bool isInteractionMode = false;
    private Camera cam;
    private Transform selectedObject;
    private Plane dragPlane;
    private Vector3 offset;

    // 드래그 중 오브젝트와 카메라 사이의 거리(매 프레임 갱신)
    private float currentDist = Mathf.Infinity;

    void Start()
    {
        cam = Camera.main;

        // 시작할 때 커서를 잠그고 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 처음에는 인벤토리 메시지를 숨김
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1) G 키로 상호작용 모드 토글
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
                HidePrompt();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // 2) 상호작용 모드인 동안 드래그 처리
        if (isInteractionMode)
        {
            HandleDragging();

            // 인벤토리 메시지가 보이고, 오브젝트가 선택되어 있으며, E 키를 눌렀을 때
            if (promptText != null
                && promptText.gameObject.activeSelf
                && selectedObject != null
                && Input.GetKeyDown(KeyCode.E))
            {
                // 선택된 오브젝트가 InteractableObject 컴포넌트를 갖고 있는지 체크
                var interactable = selectedObject.GetComponent<InteractableObject>();
                if (interactable != null)
                {
                    // 실제 인벤토리에 추가 후 오브젝트 비활성화
                    interactable.Pickup();

                    // 선택 해제하고 메시지 숨기기
                    ReleaseSelected();
                    HidePrompt();
                }
            }
        }
    }

    void FixedUpdate()
    {
        // 3) 카메라 이동: WASD (항상 동작)
        float h = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * movementSpeed;
        float v = Input.GetAxis("Vertical") * Time.fixedDeltaTime * movementSpeed;
        transform.Translate(h, 0, v);

        // 4) 마우스 룩: 상호작용 모드가 아닐 때만
        if (!isInteractionMode)
        {
            float lookX = Input.GetAxis("Mouse X");
            float lookY = Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(-lookY, lookX, 0);
        }
    }

    /// <summary>
    /// 드래그 상태라면 매 프레임 오브젝트를 움직이고, 카메라와의 거리를 계산하여
    /// 인벤토리 메시지를 표시하거나 숨깁니다.
    /// </summary>
    private void HandleDragging()
    {

        // (1) 마우스 왼쪽 클릭해서 오브젝트 선택
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
            {
                selectedObject = hit.transform;

                // 선택된 오브젝트가 움직일 평면: 오브젝트의 현재 Y 높이에서 수평 평면 생성
                float y = selectedObject.position.y;
                dragPlane = new Plane(Vector3.up, new Vector3(0, y, 0));

                if (dragPlane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    offset = selectedObject.position - hitPoint;
                }
            }
        }

        // (2) 마우스 버튼을 누르고 있을 때: 오브젝트 이동 및 거리 체크
        if (selectedObject != null && Input.GetMouseButton(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                selectedObject.position = hitPoint + offset;

                // 카메라와 오브젝트 사이의 현재 거리 계산
                currentDist = Vector3.Distance(cam.transform.position, selectedObject.position);

                // 거리가 pickupDistance 이내인지 검사하여 메시지 토글
                if (promptText != null)
                {
                    if (currentDist <= pickupDistance)
                    {
                        promptText.text = "인벤토리에 넣으시겠습니까? (E 키)";
                        promptText.gameObject.SetActive(true);
                    }
                    else
                    {
                        HidePrompt();
                    }
                }
            }
        }

        // (3) 마우스 왼쪽 버튼을 떼면 선택 해제 및 메시지 숨김
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelected();
            HidePrompt();
        }
    }

    /// <summary>
    /// 선택된 오브젝트를 해제합니다.
    /// </summary>
    private void ReleaseSelected()
    {
        selectedObject = null;
        currentDist = Mathf.Infinity;
    }

    /// <summary>
    /// promptText가 존재하면 꺼 줍니다.
    /// </summary>
    private void HidePrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
}
