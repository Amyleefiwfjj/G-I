using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // �κ��丮�� ������ ����
    public ItemData itemData;

    public void Pickup()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"[{name}]�� ItemData�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        InventoryManager.Instance.AddItem(itemData);
        gameObject.SetActive(false);
    }
}
