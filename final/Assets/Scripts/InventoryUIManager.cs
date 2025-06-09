using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // Ŭ�� �̺�Ʈ ���� ����

public class InventoryUIManager : MonoBehaviour
{

    private string previousSceneName = ""; //�κ��丮�� �� ���� �̸� �����
    [Header("UI Slot Buttons")]
    public Button[] slotButtons;        // ���� ��ư�� (6��)
    public Image[] slotIcons;           // ���� ��ư ���� Image ������Ʈ (������ ǥ�ÿ�)
    [Header("Description Area")]
    public Text nameText;
    public Text descriptionText;        // box_description �Ʒ��� �ִ� Text ������Ʈ
    [Header("Use Button")]
    public Button useButton;            // ��Use�� ��ư
    public Button backButton;

    [Header("UI Panels")]
    public GameObject boxWideView;   

    private List<ItemData> currentItems;  // �κ��丮 �Ŵ������� ������ ����Ʈ
    private int selectedSlotIndex = -1;   // ���� ���õ� ���� �ε���

    void Start()
    {
        previousSceneName = SceneManager.GetActiveScene().name; //UI���� ���� �� �� �̸����� ����
        // ���� ��ư�� Ŭ�� �̺�Ʈ ����
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;  // ĸó ���� ������ ���� ����
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }

        // useButton Ŭ�� �� ȣ��� �޼��� ����
        useButton.onClick.AddListener(OnUseButtonClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        // ó���� ���� �ؽ�Ʈ�� Use ��ư�� ��Ȱ��ȭ(�Ǵ� �� ����)�� �д�
        nameText.text = "name";
        descriptionText.text = "description";
        useButton.interactable = false;

        // �κ��丮 UI�� ����
        RefreshUI();
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

    // Use ��ư Ŭ�� �� ȣ���
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

        // ���� ������ ���ư���
        SceneManager.LoadScene(previousSceneName);
    }
    // ���� ������ �Һ� ���� ����
    private void ConsumeItem(ItemData item)
    {
        InventoryManager.Instance.GetItemList().Remove(item);
        RefreshUI();
    }

    // (������ ���Կ� ���̶���Ʈ�� �ְ� ���� ��)
    private void HighlightSelectedSlot(int index)
    {
        // ��: ��� ���� ����� �⺻���� ���� ��
        for (int i = 0; i < slotButtons.Length; i++)
        {
            // ���� ���(Image)�� Color �� �ʱ�ȭ (Ŀ���� ���)
            slotButtons[i].GetComponent<Image>().color = Color.white;
        }
        // ���õ� ���Ը� ���� (����� �׵θ� ��)
        slotButtons[index].GetComponent<Image>().color = Color.yellow;
    }
}
