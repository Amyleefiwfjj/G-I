using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // 클릭 이벤트 등을 위해

public class InventoryUIManager : MonoBehaviour
{

    private string previousSceneName = ""; //인벤토리를 연 씬의 이름 저장용
    [Header("UI Slot Buttons")]
    public Button[] slotButtons;        // 슬롯 버튼들 (6개)
    public Image[] slotIcons;           // 슬롯 버튼 내의 Image 컴포넌트 (아이콘 표시용)
    [Header("Description Area")]
    public Text nameText;
    public Text descriptionText;        // box_description 아래에 있는 Text 컴포넌트
    [Header("Use Button")]
    public Button useButton;            // “Use” 버튼
    public Button backButton;

    [Header("UI Panels")]
    public GameObject boxWideView;   

    private List<ItemData> currentItems;  // 인벤토리 매니저에서 가져온 리스트
    private int selectedSlotIndex = -1;   // 현재 선택된 슬롯 인덱스

    void Start()
    {
        previousSceneName = SceneManager.GetActiveScene().name; //UI열기 전에 전 씬 이름부터 저장
        // 슬롯 버튼에 클릭 이벤트 연결
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;  // 캡처 문제 방지용 로컬 변수
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }

        // useButton 클릭 시 호출될 메서드 연결
        useButton.onClick.AddListener(OnUseButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        // 처음엔 설명 텍스트와 Use 버튼을 비활성화(또는 빈 상태)로 둔다
        nameText.text = "name";
        descriptionText.text = "description";
        useButton.interactable = false;

        // 인벤토리 UI를 갱신
        RefreshUI();
    }

    // 인벤토리 UI 전체를 새로 그려 주는 메서드
    public void RefreshUI()
    {
        // 1) InventoryManager로부터 최신 아이템 리스트를 받아 온다
        currentItems = InventoryManager.Instance.GetItemList();

        // 2) 슬롯마다 아이콘과 활성/비활성 설정
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (i < currentItems.Count)
            {
                // 슬롯에 아이템이 존재하면
                slotButtons[i].gameObject.SetActive(true); // 슬롯 비활성화 옵션을 쓰고 싶으면 조정
                slotIcons[i].sprite = currentItems[i].itemIcon;
                slotIcons[i].color = Color.white;         // 아이콘을 흑백이 아닌 원래 컬러로
                slotButtons[i].interactable = true;
            }
            else
            {
                // 슬롯에 아이템이 없으면 빈 슬롯 처리
                slotIcons[i].sprite = null;
                slotIcons[i].color = new Color(0, 0, 0, 0);  // 완전 투명 혹은 회색 처리
                slotButtons[i].interactable = false;
            }
        }

        int tapeCount = currentItems.Count(item => item.ItemID == 1);

        if (tapeCount >= 6)  // 테이프 아이템이 6개 이상 모였으면
        {
            descriptionText.text = "테이프를 모두 모았습니다. 원본 테이프를 찾으러 갑시다!";
        }

        int newsCount = currentItems.Count(item => item.ItemID == 2);

        if (tapeCount >= 4)  // 테이프 아이템이 6개 이상 모였으면
        {
            descriptionText.text = "신문 조각을 모두 모았습니다. 신문 기사가 어떤 내용이였을까요?";
        }

        int adoptCount = currentItems.Count(item => item.ItemID == 3);

        //if (tapeCount >= ?)  // 아이템이 모였으면
        //{
        //    descriptionText.text = "";
        //}


        // 선택 상태 초기화
        selectedSlotIndex = -1;
        descriptionText.text = "";
        useButton.interactable = false;
    }

    // 슬롯 버튼 클릭 시 호출됨
    private void OnSlotClicked(int index)
    {
        // 슬롯 인덱스 범위 유효성 검사
        if (index < 0 || index >= currentItems.Count)
            return;

        // 해당 슬롯을 선택
        selectedSlotIndex = index;
        ItemData selectedItem = currentItems[index];

        // 설명 텍스트에 아이템 이름·설명 표시
        nameText.text = $"[{selectedItem.itemName}]";
        descriptionText.text = $"[{selectedItem.itemDescription}";
        boxWideView.GetComponent<Image>().sprite = selectedItem.itemIcon;

        // Use 버튼 활성화
        useButton.interactable = true;

        // (선택 표시를 위해 슬롯 경계 색상을 바꾸거나 하이라이트 처리하고 싶으면 이곳에 로직 추가)
        HighlightSelectedSlot(index);
    }

    // Use 버튼 클릭 시 호출됨
    private void OnUseButtonClicked()
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= currentItems.Count)
            return;

        ItemData selectedItem = currentItems[selectedSlotIndex];

        ConsumeItem(selectedItem);
    }
    private void OnBackButtonClicked()
    {
        if(selectedSlotIndex>=0 &&  selectedSlotIndex < currentItems.Count)
        {
            selectedSlotIndex = -1;
            nameText.text = "";
            descriptionText.text="";
            useButton.interactable = false;

            RefreshUI();
        }
        else
        {
            CloseInventory();
        }
    }

    private void CloseInventory()
    {
        if (string.IsNullOrEmpty(previousSceneName))
        {
            Debug.LogWarning("Previous scene name is not set.");
            return;
        }

        // 이전 씬으로 돌아가기
        SceneManager.LoadScene(previousSceneName);
    }
    // 실제 아이템 소비 로직 예시
    private void ConsumeItem(ItemData item)
    {
        InventoryManager.Instance.GetItemList().Remove(item);
        RefreshUI();
    }

    // (선택한 슬롯에 하이라이트를 주고 싶을 때)
    private void HighlightSelectedSlot(int index)
    {
        // 예: 모든 슬롯 배경을 기본으로 돌린 뒤
        for (int i = 0; i < slotButtons.Length; i++)
        {
            // 슬롯 배경(Image)나 Color 등 초기화 (커스텀 방식)
            slotButtons[i].GetComponent<Image>().color = Color.white;
        }
        // 선택된 슬롯만 강조 (노란색 테두리 등)
        slotButtons[index].GetComponent<Image>().color = Color.yellow;
    }
}
