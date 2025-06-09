using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;           // 아이템 이름
    public int ItemID; //아이템 종류 -> 1:tape / 2:newspaper / 3:adopt / 4:photo / 5:flower
    public Sprite itemIcon;           // 인벤토리 슬롯에 표시할 아이콘
    [TextArea(3, 10)]
    public string itemDescription;    // 아이템 설명 텍스트
}
