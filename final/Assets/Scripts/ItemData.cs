using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;           // ������ �̸�
    public Sprite itemIcon;           // �κ��丮 ���Կ� ǥ���� ������
    [TextArea(3, 10)]
    public string itemDescription;    // ������ ���� �ؽ�Ʈ
    // �ʿ��ϴٸ� �߰� �Ӽ�(����, Ÿ�� ��)�� ����
}
