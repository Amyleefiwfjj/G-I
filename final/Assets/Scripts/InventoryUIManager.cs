using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;  // Ŭ�� �̺�Ʈ ���� ����

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI Slot Buttons")]
    public Button[] slotButtons;        // ���� ��ư�� (6��)
    public Image[] slotIcons;           // ���� ��ư ���� Image ������Ʈ (������ ǥ�ÿ�)
    [Header("Description Area")]
    public Text descriptionText;        // box_description �Ʒ��� �ִ� Text ������Ʈ
    [Header("Use Button")]
    public Button useButton;            // ��Use�� ��ư

    private List<ItemData> currentItems;  // �κ��丮 �Ŵ������� ������ ����Ʈ
    private int selectedSlotIndex = -1;   // ���� ���õ� ���� �ε���

    void Start()
    {
        // ���� ��ư�� Ŭ�� �̺�Ʈ ����
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;  // ĸó ���� ������ ���� ����
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }

        // useButton Ŭ�� �� ȣ��� �޼��� ����
        useButton.onClick.AddListener(OnUseButtonClicked);

        // ó���� ���� �ؽ�Ʈ�� Use ��ư�� ��Ȱ��ȭ(�Ǵ� �� ����)�� �д�
        descriptionText.text = "";
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
        ItemData sel = currentItems[index];

        // ���� �ؽ�Ʈ�� ������ �̸������� ǥ��
        descriptionText.text = $"[{sel.itemName}]\n{sel.itemDescription}";

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

        ItemData sel = currentItems[selectedSlotIndex];

        // ����: ��� ������ ��� �������� ���Һ��ϰ� ��Ͽ��� �����ϴ� ����
        // ���� ���� ������ ���� �� �κ��� ����
        ConsumeItem(sel);
    }

    // ���� ������ �Һ� ���� ����
    private void ConsumeItem(ItemData item)
    {
        // 1) InventoryManager���� ����
        InventoryManager.Instance.GetItemList().Remove(item);

        // 2) �ʿ��ϴٸ� ������ ȿ�� ����: e.g. ü�� ȸ��, ���� ���� ��
        //    ��) PlayerHealth.Instance.Heal(10);
        //    �Ǵ� �ش� ������ Ÿ�Կ� �´� �ݹ� ȣ��

        // 3) UI ����
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
