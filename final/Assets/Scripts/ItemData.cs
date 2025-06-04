using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;           // 아이템 이름
    public Sprite itemIcon;           // 인벤토리 슬롯에 표시할 아이콘
    [TextArea(3, 10)]
    public string itemDescription;    // 아이템 설명 텍스트
    // 필요하다면 추가 속성(무게, 타입 등)을 정의
}
