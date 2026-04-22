using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUI : MonoBehaviour
{
	[Header("References")]
	public GameObject shopScreen;
	public Transform shopItemsContainer;
	public GameObject shopItemPrefab;
    
	[Header("UI Elements")]
	public TextMeshProUGUI headerText;
	public TextMeshProUGUI moneyText;
	public Button closeButton;
	public Button rerollButton;
	public TextMeshProUGUI rerollCostText;
    
	private int currentQuarter;
	private System.Action onShopComplete;
    
	void Start()
	{
		if (closeButton != null)
		{
			closeButton.onClick.AddListener(OnCloseClicked);
		}
        
		if (rerollButton != null)
		{
			rerollButton.onClick.AddListener(OnRerollClicked);
		}
        
		if (shopScreen != null)
		{
			shopScreen.SetActive(false);
		}
	}
    
	public void ShowShop(int quarterNumber, System.Action onComplete)
	{
		currentQuarter = quarterNumber;
		onShopComplete = onComplete;
        
		if (headerText != null)
		{
			headerText.text = $"Shop - Quarter {quarterNumber}";
		}
        
		UpdateMoneyDisplay();
		PopulateShop();
        
		shopScreen.SetActive(true);
	}
    
	void PopulateShop()
	{
		// Clear existing items
		foreach (Transform child in shopItemsContainer)
		{
			Destroy(child.gameObject);
		}
        
		// Generate and display shop items
		List<ShopItem> items = ShopManager.Instance.GenerateShop(currentQuarter);
        
		foreach (ShopItem item in items)
		{
			CreateShopItemUI(item);
		}
        
		UpdateRerollButton();
	}
    
	void CreateShopItemUI(ShopItem item)
	{
		GameObject itemObj = Instantiate(shopItemPrefab, shopItemsContainer);
		ShopItemUI itemUI = itemObj.GetComponent<ShopItemUI>();
        
		if (itemUI != null)
		{
			itemUI.Initialize(item, OnItemPurchased);
		}
	}
    
	void OnItemPurchased(ShopItem item)
	{
		bool success = ShopManager.Instance.PurchaseItem(item);
        
		if (success)
		{
			UpdateMoneyDisplay();
            
			// Remove item from display
			foreach (Transform child in shopItemsContainer)
			{
				ShopItemUI itemUI = child.GetComponent<ShopItemUI>();
				if (itemUI != null && itemUI.GetItem() == item)
				{
					Destroy(child.gameObject);
					break;
				}
			}
            
			// Refresh remaining items' affordability
			RefreshItemStates();
		}
	}
    
	void OnRerollClicked()
	{
		int rerollCost = ShopManager.Instance.rerollCost;
        
		if (CurrencyManager.Instance.SpendMoney(rerollCost))
		{
			PopulateShop();
			UpdateMoneyDisplay();
		}
		else
		{
			Debug.Log("Cannot afford reroll!");
		}
	}
    
	void UpdateRerollButton()
	{
		if (rerollButton != null && rerollCostText != null)
		{
			int cost = ShopManager.Instance.rerollCost;
			rerollCostText.text = $"Reroll (${cost})";
			rerollButton.interactable = CurrencyManager.Instance.CanAfford(cost);
		}
	}
    
	void UpdateMoneyDisplay()
	{
		if (moneyText != null)
		{
			moneyText.text = $"Money: ${CurrencyManager.Instance.currentMoney}";
		}
	}
    
	void RefreshItemStates()
	{
		foreach (Transform child in shopItemsContainer)
		{
			ShopItemUI itemUI = child.GetComponent<ShopItemUI>();
			if (itemUI != null)
			{
				itemUI.RefreshAffordability();
			}
		}
        
		UpdateRerollButton();
	}
    
	void OnCloseClicked()
	{
		CloseShop();
	}
    
	void CloseShop()
	{
		shopScreen.SetActive(false);
		onShopComplete?.Invoke();
	}
    
	public void HideShop()
	{
		if (shopScreen != null)
		{
			shopScreen.SetActive(false);
		}
	}
}