using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // 클릭 이벤트 등을 위해

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI Slot Buttons")]
    public Button[] slotButtons;        // 슬롯 버튼들 (6개)
    public Image[] slotIcons;           // 슬롯 버튼 내의 Image 컴포넌트 (아이콘 표시용)
    [Header("Description Area")]
    public Text nameText;
    public Text descriptionText;        // box_description 아래에 있는 Text 컴포넌트
    [Header("Button")]
    public Button useButton;            // “Use” 버튼
    public Button backButton;

    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject boxWideView;
    public GameObject existingUI;
    public Text warningText;
    private List<ItemData> currentItems;  // 인벤토리 매니저에서 가져온 리스트
    private int selectedSlotIndex = -1;   // 현재 선택된 슬롯 인덱스
    void Start()
    {
        warningText.text = "";
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;  // 캡처 문제 방지용 로컬 변수
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }

        RefreshUI();
        UpdateInventoryUI();
        // useButton 클릭 시 호출될 메서드 연결
        useButton.onClick.AddListener(OnUseButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        // 처음엔 설명 텍스트와 Use 버튼을 비활성화(또는 빈 상태)로 둔다
        nameText.text = "name";
        descriptionText.text = "description";
        useButton.interactable = false;

        // 인벤토리 UI를 갱신
        RefreshUI();
        inventoryPanel.SetActive(false);
        existingUI.SetActive(true);
    }
    public void UpdateInventoryUI()
    {
        if (slotButtons == null || slotIcons == null)
        {
            Debug.LogError("Slot buttons or slot icons are not assigned!");
            return;
        }

        // currentItems가 null인지 확인
        if (currentItems == null)
        {
            Debug.LogError("currentItems is null! Cannot update inventory UI.");
            return;
        }
        // 슬롯 아이템이 UI에 추가될 수 있도록 반복문으로 처리
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (i < currentItems.Count)
            {
                // 아이템이 있을 때는 슬롯을 활성화하고 아이콘 표시
                slotButtons[i].gameObject.SetActive(true);
                slotIcons[i].sprite = currentItems[i].itemIcon;
                slotIcons[i].color = Color.white;         // 아이콘을 원래 색상으로
                slotButtons[i].onClick.RemoveAllListeners();
                int index = i;  // 슬롯 인덱스를 캡처
                slotButtons[i].onClick.AddListener(() => ShowItemDetails(currentItems[index]));
            }
            else
            {
                // 아이템이 없으면 빈 슬롯 처리
                slotButtons[i].gameObject.SetActive(false);  // 슬롯을 비활성화
            }
        }
    }
    public void ShowItemDetails(ItemData itemData)
    {
        // Ensure itemData is not null
        if (itemData == null)
        {
            Debug.LogError("ItemData is null! Cannot display item details.");
            return;
        }

        // Update the UI elements with the item details
        nameText.text = itemData.itemName;
        descriptionText.text = itemData.itemDescription;

        // Update the icon for the selected item in the inventory
        if (selectedSlotIndex >= 0 && selectedSlotIndex < slotIcons.Length)
        {
            slotIcons[selectedSlotIndex].sprite = itemData.itemIcon;
        }
        else
        {
            Debug.LogError("Invalid slot index!");
        }
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
            selectedSlotIndex = -1;
            descriptionText.text = "";
            useButton.interactable = false;
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
    // In InventoryUIManager
    public void ClearWarning()
    {
        warningText.text = "";  // Clear the warning message
        warningText.gameObject.SetActive(false); // Hide the warning text
    }

    // Use 버튼 클릭 시 호출됨
    public void OnUseButtonClicked()
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= currentItems.Count)
            return;

        ItemData selectedItem = currentItems[selectedSlotIndex];

        if (InventoryManager.Instance.CanStartMiniGame(1))
        {
            // 조건을 만족하면 ConsumeItem을 호출
            ConsumeItemsByID(1);
        }

        if (InventoryManager.Instance.CanStartMiniGame(2))
        {
            // 조건을 만족하면 ConsumeItem을 호출
            ConsumeItemsByID(2);
        }

        //if (InventoryManager.Instance.CanStartMiniGame(3))
        //{
        //    // 조건을 만족하면 ConsumeItem을 호출
        //    ConsumeItemsByID(3);
        //}
        else
        {
            // 조건을 만족하지 않으면 경고 메시지 또는 UI 업데이트
            DisplayWarning("아이템을 다 찾지 않았습니다.");
        }
    }
    private void DisplayWarning(string message)
    {
        warningText.text = message;  // Update the warning message text
        warningText.gameObject.SetActive(true); // Make the warning text visible
        Invoke("ClearWarning", 3f); // Invoke ClearWarning after 3 seconds
    }

    public void OnBackButtonClicked()
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
            inventoryPanel.SetActive(false);
            existingUI.SetActive(true);
        }
    }
    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);  // 인벤토리 UI 활성화
    }


    private void ConsumeItemsByID(int itemID)
    {
        // itemID에 맞는 아이템들을 찾아서 소비
        List<ItemData> itemsToConsume = currentItems.FindAll(item => item.ItemID == itemID);

        // 아이템을 하나씩 소비
        foreach (ItemData item in itemsToConsume)
        {
            // 기존의 ConsumeItem을 호출하여 아이템을 제거하고 미니게임을 체크
            ConsumeItem(item);
        }
    }

    private void ConsumeItem(ItemData item)
    {

        InventoryManager.Instance.GetItemList().Remove(item);
        InventoryManager.Instance.CheckForMiniGames();

        RefreshUI();
    }

private void HighlightSelectedSlot(int index)
    {
        // 모든 슬롯 배경을 기본으로 돌린 뒤
        for (int i = 0; i < slotButtons.Length; i++)
        {
            // 슬롯Color 초기화
            slotButtons[i].GetComponent<Image>().color = Color.white;
        }
        // 선택된 슬롯만 강조
        slotButtons[index].GetComponent<Image>().color = Color.yellow;
    }
}
