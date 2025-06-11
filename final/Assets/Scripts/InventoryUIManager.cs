using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // Ŭ�� �̺�Ʈ ���� ����

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI Slot Buttons")]
    public Button[] slotButtons;        // ���� ��ư�� (6��)
    public Image[] slotIcons;           // ���� ��ư ���� Image ������Ʈ (������ ǥ�ÿ�)
    [Header("Description Area")]
    public Text nameText;
    public Text descriptionText;        // box_description �Ʒ��� �ִ� Text ������Ʈ
    [Header("Button")]
    public Button useButton;            // ��Use�� ��ư
    public Button backButton;

    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject boxWideView;
    public GameObject existingUI;
    public Text warningText;
    private List<ItemData> currentItems;  // �κ��丮 �Ŵ������� ������ ����Ʈ
    private int selectedSlotIndex = -1;   // ���� ���õ� ���� �ε���
    void Start()
    {
        warningText.text = "";
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;  // ĸó ���� ������ ���� ����
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }

        RefreshUI();
        UpdateInventoryUI();
        // useButton Ŭ�� �� ȣ��� �޼��� ����
        useButton.onClick.AddListener(OnUseButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        // ó���� ���� �ؽ�Ʈ�� Use ��ư�� ��Ȱ��ȭ(�Ǵ� �� ����)�� �д�
        nameText.text = "name";
        descriptionText.text = "description";
        useButton.interactable = false;

        // �κ��丮 UI�� ����
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

        // currentItems�� null���� Ȯ��
        if (currentItems == null)
        {
            Debug.LogError("currentItems is null! Cannot update inventory UI.");
            return;
        }
        // ���� �������� UI�� �߰��� �� �ֵ��� �ݺ������� ó��
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (i < currentItems.Count)
            {
                // �������� ���� ���� ������ Ȱ��ȭ�ϰ� ������ ǥ��
                slotButtons[i].gameObject.SetActive(true);
                slotIcons[i].sprite = currentItems[i].itemIcon;
                slotIcons[i].color = Color.white;         // �������� ���� ��������
                slotButtons[i].onClick.RemoveAllListeners();
                int index = i;  // ���� �ε����� ĸó
                slotButtons[i].onClick.AddListener(() => ShowItemDetails(currentItems[index]));
            }
            else
            {
                // �������� ������ �� ���� ó��
                slotButtons[i].gameObject.SetActive(false);  // ������ ��Ȱ��ȭ
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

    // �κ��丮 UI ��ü�� ���� �׷� �ִ� �޼���
    public void RefreshUI()
    {
        // 1) InventoryManager�κ��� �ֽ� ������ ����Ʈ�� �޾� �´�
        currentItems = InventoryManager.Instance.GetItemList();

        // 2) ���Ը��� �����ܰ� Ȱ��/��Ȱ�� ����
        for (int i = 0; i < slotButtons.Length; i++)
        {
            if (i < currentItems.Count)
            {
                // ���Կ� �������� �����ϸ�
                slotButtons[i].gameObject.SetActive(true); // ���� ��Ȱ��ȭ �ɼ��� ���� ������ ����
                slotIcons[i].sprite = currentItems[i].itemIcon;
                slotIcons[i].color = Color.white;         // �������� ����� �ƴ� ���� �÷���
                slotButtons[i].interactable = true;
            }
            else
            {
                // ���Կ� �������� ������ �� ���� ó��
                slotIcons[i].sprite = null;
                slotIcons[i].color = new Color(0, 0, 0, 0);  // ���� ���� Ȥ�� ȸ�� ó��
                slotButtons[i].interactable = false;
            }
            selectedSlotIndex = -1;
            descriptionText.text = "";
            useButton.interactable = false;
        }

        int tapeCount = currentItems.Count(item => item.ItemID == 1);

        if (tapeCount >= 6)  // ������ �������� 6�� �̻� ������
        {
            descriptionText.text = "�������� ��� ��ҽ��ϴ�. ���� �������� ã���� ���ô�!";
        }

        int newsCount = currentItems.Count(item => item.ItemID == 2);

        if (tapeCount >= 4)  // ������ �������� 6�� �̻� ������
        {
            descriptionText.text = "�Ź� ������ ��� ��ҽ��ϴ�. �Ź� ��簡 � �����̿������?";
        }

        int adoptCount = currentItems.Count(item => item.ItemID == 3);

        //if (tapeCount >= ?)  // �������� ������
        //{
        //    descriptionText.text = "";
        //}


        // ���� ���� �ʱ�ȭ
        selectedSlotIndex = -1;
        descriptionText.text = "";
        useButton.interactable = false;
    }

    // ���� ��ư Ŭ�� �� ȣ���
    private void OnSlotClicked(int index)
    {
        // ���� �ε��� ���� ��ȿ�� �˻�
        if (index < 0 || index >= currentItems.Count)
            return;

        // �ش� ������ ����
        selectedSlotIndex = index;
        ItemData selectedItem = currentItems[index];

        // ���� �ؽ�Ʈ�� ������ �̸������� ǥ��
        nameText.text = $"[{selectedItem.itemName}]";
        descriptionText.text = $"[{selectedItem.itemDescription}";
        boxWideView.GetComponent<Image>().sprite = selectedItem.itemIcon;

        // Use ��ư Ȱ��ȭ
        useButton.interactable = true;

        // (���� ǥ�ø� ���� ���� ��� ������ �ٲٰų� ���̶���Ʈ ó���ϰ� ������ �̰��� ���� �߰�)
        HighlightSelectedSlot(index);
    }
    // In InventoryUIManager
    public void ClearWarning()
    {
        warningText.text = "";  // Clear the warning message
        warningText.gameObject.SetActive(false); // Hide the warning text
    }

    // Use ��ư Ŭ�� �� ȣ���
    public void OnUseButtonClicked()
    {
        if (selectedSlotIndex < 0 || selectedSlotIndex >= currentItems.Count)
            return;

        ItemData selectedItem = currentItems[selectedSlotIndex];

        if (InventoryManager.Instance.CanStartMiniGame(1))
        {
            // ������ �����ϸ� ConsumeItem�� ȣ��
            ConsumeItemsByID(1);
        }

        if (InventoryManager.Instance.CanStartMiniGame(2))
        {
            // ������ �����ϸ� ConsumeItem�� ȣ��
            ConsumeItemsByID(2);
        }

        //if (InventoryManager.Instance.CanStartMiniGame(3))
        //{
        //    // ������ �����ϸ� ConsumeItem�� ȣ��
        //    ConsumeItemsByID(3);
        //}
        else
        {
            // ������ �������� ������ ��� �޽��� �Ǵ� UI ������Ʈ
            DisplayWarning("�������� �� ã�� �ʾҽ��ϴ�.");
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
        inventoryPanel.SetActive(true);  // �κ��丮 UI Ȱ��ȭ
    }


    private void ConsumeItemsByID(int itemID)
    {
        // itemID�� �´� �����۵��� ã�Ƽ� �Һ�
        List<ItemData> itemsToConsume = currentItems.FindAll(item => item.ItemID == itemID);

        // �������� �ϳ��� �Һ�
        foreach (ItemData item in itemsToConsume)
        {
            // ������ ConsumeItem�� ȣ���Ͽ� �������� �����ϰ� �̴ϰ����� üũ
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
        // ��� ���� ����� �⺻���� ���� ��
        for (int i = 0; i < slotButtons.Length; i++)
        {
            // ����Color �ʱ�ȭ
            slotButtons[i].GetComponent<Image>().color = Color.white;
        }
        // ���õ� ���Ը� ����
        slotButtons[index].GetComponent<Image>().color = Color.yellow;
    }
}
