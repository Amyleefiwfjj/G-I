using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // 인벤토리에 담긴 아이템(ItemData)을 저장하는 리스트
    private List<ItemData> items = new List<ItemData>();

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        // 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// ItemData 형태의 아이템을 인벤토리에 추가합니다.
    /// </summary>
    public void AddItem(ItemData item)
    {
        if (item == null)
            return;

        if (items.Contains(item))
            return;

        items.Add(item);
        Debug.Log($"[Inventory] '{item.itemName}' 아이템이 추가되었습니다. (총 {items.Count}개)");
    }

    /// <summary>
    /// 현재 인벤토리 목록을 디버그 로그로 출력합니다.
    /// </summary>
    public void PrintInventoryContents()
    {
        Debug.Log("=== 인벤토리 목록 ===");
        foreach (var it in items)
        {
            Debug.Log(it.itemName);
        }
        Debug.Log("=====================");
    }

    /// <summary>
    /// UI 또는 외부에서 인벤토리 내역을 가져갈 때 사용합니다.
    /// </summary>
    public List<ItemData> GetItemList()
    {
        return items;
    }
}
