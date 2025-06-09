using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // �κ��丮�� ��� ������(ItemData)�� �����ϴ� ����Ʈ
    private List<ItemData> items = new List<ItemData>();

    private int tapeFound = 6;
    private int newsFound = 4;
    //private int adoptFound =?;

    void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// ItemData ������ �������� �κ��丮�� �߰��մϴ�.
    /// </summary>
    public void AddItem(ItemData item)
    {
        if (item == null || items.Contains(item)) return;

        items.Add(item);
        Debug.Log($"[Inventory] '{item.itemName}' �������� �߰��Ǿ����ϴ�.");

        CheckForMiniGames();
    }
    public void RemoveItem(ItemData item)
    {
        if (item == null) return;

        ItemData existingItem = items.Find(i => i.itemName == item.itemName);

        items.Remove(existingItem);
        Debug.Log($"[Inventory] '{item.itemName}' �������� ���ŵǾ����ϴ�.");
    }
    // �̴ϰ��� ���� �Լ� (���ӿ� �°� ����)
    private void CheckForMiniGames()
    {
        int countForGame1 = GetItemCountForGame(1);
        int countForGame2 = GetItemCountForGame(2);
        int countForGame3 = GetItemCountForGame(3);

        if (countForGame1 >= tapeFound)
        {
            Debug.Log("[Inventory] �̴ϰ��� 1�� ������ �� �ֽ��ϴ�!");
            StartMiniGame(1);
        }

        if (countForGame2 >= newsFound)
        {
            Debug.Log("[Inventory] �̴ϰ��� 2�� ������ �� �ֽ��ϴ�!");
            StartMiniGame(2);
        }
        //if (countForGame3 >= adoptFound)
        //{
        //    Debug.Log("[Inventory] �̴ϰ��� 3�� ������ �� �ֽ��ϴ�!");
        //    StartMiniGame(3);
        //}
    }

    //Ư�� �̴ϰ��ӿ� �ʿ��� ������ ���� ���� �Լ�
    private int GetItemCountForGame(int gameNumber)
    {
        int itemCount = 0;

        foreach (var item in items)
        {
            if (gameNumber == 1 && item.ItemID == 1)  //������
                itemCount++;
            else if (gameNumber == 2 && item.ItemID ==2)  // ����
                itemCount++;
            else if(gameNumber ==3 && item.ItemID ==3) //�Ծ�
                itemCount++;
        }

        return itemCount;
    }

    private void StartMiniGame(int gameNumber)
    {
        // �̴ϰ����� �����ϴ� ���� (�� ��ȯ, UI ���� ��)
        // ��: MiniGameManager.Instance.StartGame(gameNumber);
        Debug.Log($"�̴ϰ��� {gameNumber}��(��) �����մϴ�.");
    }

    /// <summary>
    /// ���� �κ��丮 ����� ����� �α׷� ����մϴ�.
    /// </summary>
    public void PrintInventoryContents() //��������
    {
        Debug.Log("=== �κ��丮 ��� ===");
        foreach (var it in items)
        {
            Debug.Log(it.itemName);
        }
        Debug.Log("=====================");
    }

    /// <summary>
    /// UI �Ǵ� �ܺο��� �κ��丮 ������ ������ �� ����մϴ�.
    /// </summary>
    public List<ItemData> GetItemList()
    {
        return new List<ItemData>(items);  // ���ο� ����Ʈ�� ��ȯ�Ͽ� �ܺο��� �������� �ʵ���
    }

}
