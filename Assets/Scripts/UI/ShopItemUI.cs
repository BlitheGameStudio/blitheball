using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
	[Header("UI References")]
	public Image iconImage;
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI descriptionText;
	public TextMeshProUGUI priceText;
	public Button buyButton;
	public Image affordabilityOverlay;
    
	private ShopItem item;
	private System.Action<ShopItem> onPurchase;
    
	public void Initialize(ShopItem shopItem, System.Action<ShopItem> onPurchaseCallback)
	{
		item = shopItem;
		onPurchase = onPurchaseCallback;
        
		if (nameText != null)
			nameText.text = item.itemName;
        
		if (descriptionText != null)
			descriptionText.text = item.description;
        
		if (priceText != null)
			priceText.text = $"${item.price}";
        
		if (iconImage != null && item.icon != null)
			iconImage.sprite = item.icon;
        
		if (buyButton != null)
		{
			buyButton.onClick.AddListener(OnBuyClicked);
		}
        
		RefreshAffordability();
	}
    
	public void RefreshAffordability()
	{
		bool canAfford = CurrencyManager.Instance.CanAfford(item.price);
        
		if (buyButton != null)
		{
			buyButton.interactable = canAfford;
		}
        
		if (affordabilityOverlay != null)
		{
			affordabilityOverlay.gameObject.SetActive(!canAfford);
		}
	}
    
	void OnBuyClicked()
	{
		onPurchase?.Invoke(item);
	}
    
	public ShopItem GetItem()
	{
		return item;
	}
}