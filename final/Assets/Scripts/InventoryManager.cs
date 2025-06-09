using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // 인벤토리에 담긴 아이템(ItemData)을 저장하는 리스트
    private List<ItemData> items = new List<ItemData>();

    private int tapeFound = 6;
    private int newsFound = 4;
    //private int adoptFound =?;

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
        if (item == null || items.Contains(item)) return;

        items.Add(item);
        Debug.Log($"[Inventory] '{item.itemName}' 아이템이 추가되었습니다.");

        CheckForMiniGames();
    }
    public void RemoveItem(ItemData item)
    {
        if (item == null) return;

        ItemData existingItem = items.Find(i => i.itemName == item.itemName);

        items.Remove(existingItem);
        Debug.Log($"[Inventory] '{item.itemName}' 아이템이 제거되었습니다.");
    }
    // 미니게임 시작 함수 (게임에 맞게 수정)
    private void CheckForMiniGames()
    {
        int countForGame1 = GetItemCountForGame(1);
        int countForGame2 = GetItemCountForGame(2);
        int countForGame3 = GetItemCountForGame(3);

        if (countForGame1 >= tapeFound)
        {
            Debug.Log("[Inventory] 미니게임 1을 시작할 수 있습니다!");
            StartMiniGame(1);
        }

        if (countForGame2 >= newsFound)
        {
            Debug.Log("[Inventory] 미니게임 2를 시작할 수 있습니다!");
            StartMiniGame(2);
        }
        //if (countForGame3 >= adoptFound)
        //{
        //    Debug.Log("[Inventory] 미니게임 3를 시작할 수 있습니다!");
        //    StartMiniGame(3);
        //}
    }

    //특정 미니게임에 필요한 아이템 개수 세는 함수
    private int GetItemCountForGame(int gameNumber)
    {
        int itemCount = 0;

        foreach (var item in items)
        {
            if (gameNumber == 1 && item.ItemID == 1)  //테이프
                itemCount++;
            else if (gameNumber == 2 && item.ItemID ==2)  // 뉴스
                itemCount++;
            else if(gameNumber ==3 && item.ItemID ==3) //입양
                itemCount++;
        }

        return itemCount;
    }

    private void StartMiniGame(int gameNumber)
    {
        // 미니게임을 시작하는 로직 (씬 전환, UI 갱신 등)
        // 예: MiniGameManager.Instance.StartGame(gameNumber);
        Debug.Log($"미니게임 {gameNumber}을(를) 시작합니다.");
    }

    /// <summary>
    /// 현재 인벤토리 목록을 디버그 로그로 출력합니다.
    /// </summary>
    public void PrintInventoryContents() //디버깅용임
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
        return new List<ItemData>(items);  // 새로운 리스트를 반환하여 외부에서 수정되지 않도록
    }

}
