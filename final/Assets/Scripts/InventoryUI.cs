
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text itemNameText;
    public Text itemDescriptionText;
    public Image itemIconImage;

    public void ShowItemDetails(ItemData itemData)
    {
        // Update UI with item details
        itemNameText.text = itemData.itemName;
        itemDescriptionText.text = itemData.itemDescription;
        itemIconImage.sprite = itemData.itemIcon;
    }
}
