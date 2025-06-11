using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // 인벤토리에 저장할 정보
    public ItemData itemData;

    public void Pickup()
    {
        if (itemData == null)
        {
            Debug.LogWarning($"[{name}]에 ItemData가 할당되지 않았습니다.");
            return;
        }

        InventoryManager.Instance.AddItem(itemData);
        gameObject.SetActive(false);
    }
}
