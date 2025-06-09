using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;           // ������ �̸�
    public int ItemID; //������ ���� -> 1:tape / 2:newspaper / 3:adopt / 4:photo / 5:flower
    public Sprite itemIcon;           // �κ��丮 ���Կ� ǥ���� ������
    [TextArea(3, 10)]
    public string itemDescription;    // ������ ���� �ؽ�Ʈ
}
