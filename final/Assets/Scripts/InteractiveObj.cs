using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // �κ��丮�� ������ ����
    public ItemData data;

    public void Pickup()
    {
        if (data == null)
        {
            Debug.LogWarning($"[{name}]�� ItemData�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        InventoryManager.Instance.AddItem(data);
        gameObject.SetActive(false);
    }
}
