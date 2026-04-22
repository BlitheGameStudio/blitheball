using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
	public static ShopManager Instance { get; private set; }
    
	[Header("Shop Settings")]
	public int itemsPerShop = 5;
	public int rerollCost = 25;
    
	[Header("Item Pools")]
	public ShopItemTemplate[] regularCardTemplates;
	public ShopItemTemplate[] playerCardTemplates;
	public ShopItemTemplate[] serviceTemplates;
    
	[Header("Pricing")]
	public int baseRegularCardPrice = 30;
	public int basePlayerCardPrice = 75;
	public int removeCardPrice = 50;
	public int upgradeCardPrice = 75;
	
	[Header("UI References")]
	public CardSelectionUI cardSelectionUI;
    
	private List<ShopItem> currentShopItems = new List<ShopItem>();
    
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}
    
	public List<ShopItem> GenerateShop(int quarterNumber)
	{
		currentShopItems.Clear();
        
		for (int i = 0; i < itemsPerShop; i++)
		{
			ShopItem item = GenerateRandomShopItem(quarterNumber);
			if (item != null)
			{
				currentShopItems.Add(item);
			}
		}
        
		return currentShopItems;
	}
    
	ShopItem GenerateRandomShopItem(int quarterNumber)
	{
		float roll = Random.value;
        
		// 40% regular cards, 30% player cards, 30% services
		if (roll < 0.4f)
		{
			return GenerateRegularCardItem(quarterNumber);
		}
		else if (roll < 0.7f)
		{
			return GeneratePlayerCardItem(quarterNumber);
		}
		else
		{
			return GenerateServiceItem();
		}
	}
    
	ShopItem GenerateRegularCardItem(int quarterNumber)
	{
		if (regularCardTemplates == null || regularCardTemplates.Length == 0)
			return null;
        
		ShopItemTemplate template = regularCardTemplates[Random.Range(0, regularCardTemplates.Length)];       
        
		ShopItem item = new ShopItem
		{
			type = ShopItemType.RegularCard,
			regularCard = template.regularCard,
			itemName = template.regularCard.cardName,
			description = template.regularCard.description,
			price = CalculatePrice(baseRegularCardPrice, template.regularCard.basePoints, quarterNumber),
			icon = template.regularCard.cardArt
		};
        
		return item;
	}
    
	ShopItem GeneratePlayerCardItem(int quarterNumber)
	{
		if (playerCardTemplates == null || playerCardTemplates.Length == 0)
			return null;
        
		ShopItemTemplate template = playerCardTemplates[Random.Range(0, playerCardTemplates.Length)];
        
		int rarityMultiplier = template.playerCard.rarity switch
		{
			CardRarity.Common => 1,
			CardRarity.Uncommon => 2,
			CardRarity.Rare => 3,
			CardRarity.Legendary => 5,
			_ => 1
		};
        
		ShopItem item = new ShopItem
		{
			type = ShopItemType.PlayerCard,
			playerCard = template.playerCard,
			itemName = template.playerCard.playerName,
			description = template.playerCard.description,
			price = basePlayerCardPrice * rarityMultiplier,
			icon = template.playerCard.portrait
		};
        
		return item;
	}
    
	ShopItem GenerateServiceItem()
	{
		ServiceType[] availableServices = new ServiceType[]
		{
			ServiceType.RemoveCard,
			ServiceType.UpgradeCard,
			ServiceType.RerollShop,
			ServiceType.ExtraPlays,
			ServiceType.ExtraHandSize
		};
        
		ServiceType service = availableServices[Random.Range(0, availableServices.Length)];
        
		ShopItem item = new ShopItem
		{
			type = ShopItemType.Service,
			serviceType = service
		};
        
		switch (service)
		{
		case ServiceType.RemoveCard:
			item.itemName = "Card Removal";
			item.description = "Remove a card from your deck";
			item.price = removeCardPrice;
			break;
		case ServiceType.UpgradeCard:
			item.itemName = "Card Upgrade";
			item.description = "Upgrade a card (+1 point)";
			item.price = upgradeCardPrice;
			break;
		case ServiceType.RerollShop:
			item.itemName = "Reroll Shop";
			item.description = "Generate new shop items";
			item.price = rerollCost;
			break;
		case ServiceType.ExtraPlays:
			item.itemName = "+2 Plays";
			item.description = "Permanent +2 plays per quarter";
			item.price = 100;
			break;
		case ServiceType.ExtraHandSize:
			item.itemName = "+1 Hand Size";
			item.description = "Permanent +1 card in hand";
			item.price = 100;
			break;
		}
        
		return item;
	}
    
	int CalculatePrice(int basePrice, int cardValue, int quarterNumber)
	{
		// Prices slightly increase in later quarters
		float multiplier = 1f + (quarterNumber * 0.1f);
		return Mathf.RoundToInt(basePrice * multiplier);
	}
    
	public bool PurchaseItem(ShopItem item)
	{
		if (!CurrencyManager.Instance.CanAfford(item.price))
		{
			Debug.Log("Cannot afford item!");
			return false;
		}
        
		if (!CurrencyManager.Instance.SpendMoney(item.price))
			return false;
        
		ApplyPurchase(item);
		currentShopItems.Remove(item);
		return true;
	}
    
	void ApplyPurchase(ShopItem item)
	{
		switch (item.type)
		{
		case ShopItemType.RegularCard:
			if (item.regularCard != null)
			{
				GameManager.Instance.AddCardToDeck(item.regularCard);
				Debug.Log($"Purchased {item.regularCard.cardName}");
			}
			break;
                
		case ShopItemType.PlayerCard:
			if (item.playerCard != null)
			{
				GameManager.Instance.AddPlayerCard(item.playerCard);
				Debug.Log($"Purchased {item.playerCard.playerName}");
			}
			break;
                
		case ShopItemType.Service:
			ApplyService(item.serviceType);
			break;
			
			
		}
	}
    
    
	public void RerollShop(int quarterNumber)
	{
		GenerateShop(quarterNumber);
	}
	
	void ApplyService(ServiceType service)
	{
		switch (service)
		{
		case ServiceType.RemoveCard:
			if (cardSelectionUI != null)
			{
				cardSelectionUI.ShowCardSelection(
					CardSelectionUI.SelectionMode.Remove, 
					(card) => {
						Debug.Log($"Card removal complete: {card?.cardName}");
					}
				);
			}
			break;
            
		case ServiceType.UpgradeCard:
			if (cardSelectionUI != null)
			{
				cardSelectionUI.ShowCardSelection(
					CardSelectionUI.SelectionMode.Upgrade, 
					(card) => {
						Debug.Log($"Card upgrade complete: {card?.cardName}");
					}
				);
			}
			break;
            
		case ServiceType.RerollShop:
			// Handled by ShopUI
			break;
            
		case ServiceType.ExtraPlays:
			GameManager.Instance.basePlayCount += 2;
			Debug.Log("Permanent +2 plays added!");
			break;
            
		case ServiceType.ExtraHandSize:
			GameManager.Instance.handSize += 1;
			Debug.Log("Permanent +1 hand size added!");
			break;
		}
	}
}

[System.Serializable]
public class ShopItemTemplate
{
	public CardData regularCard;
	public PlayerCardData playerCard;
}