using UnityEngine;

[System.Serializable]
public class ShopItem
{
	public ShopItemType type;
	public string itemName;
	public string description;
	public int price;
    
	// For card items
	public CardData regularCard;
	public PlayerCardData playerCard;
    
	// For services
	public ServiceType serviceType;
    
	// Visual
	public Sprite icon;
}

public enum ShopItemType
{
	RegularCard,
	PlayerCard,
	Service,
	Consumable
}

public enum ServiceType
{
	RemoveCard,      // Pay to remove a card
	UpgradeCard,     // Pay to upgrade a card
	RerollShop,      // Reroll shop items
	HealDamage,      // If health system exists
	ExtraPlays,      // Add permanent plays
	ExtraHandSize    // Add permanent hand size
}