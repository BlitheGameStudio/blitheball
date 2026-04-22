using UnityEngine;

[System.Serializable]
public class Reward
{
	public RewardType type;
	public string title;
	public string description;
    
	// For player card rewards
	public PlayerCardData playerCard;
    
	// For regular card rewards
	public CardData regularCard;
	public int cardCount = 1;
    
	// For card removal
	public bool allowsCardRemoval;
    
	// For upgrades
	public bool allowsCardUpgrade;
}

public enum RewardType
{
	PlayerCard,
	AddCard,
	RemoveCard,
	UpgradeCard,
	IncreaseMaxHealth, // Future: if you add health system
	Heal               // Future: if you add health system
}