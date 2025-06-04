using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // �κ��丮�� ��� ������(ItemData)�� �����ϴ� ����Ʈ
    private List<ItemData> items = new List<ItemData>();

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
        if (item == null)
            return;

        if (items.Contains(item))
            return;

        items.Add(item);
        Debug.Log($"[Inventory] '{item.itemName}' �������� �߰��Ǿ����ϴ�. (�� {items.Count}��)");
    }

    /// <summary>
    /// ���� �κ��丮 ����� ����� �α׷� ����մϴ�.
    /// </summary>
    public void PrintInventoryContents()
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
        return items;
    }
}
