using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardOption : MonoBehaviour
{
	[Header("UI References")]
	public Image iconImage;
	public TextMeshProUGUI titleText;
	public TextMeshProUGUI descriptionText;
	public Button selectButton;
	public Image rarityBorder;
    
	private Reward reward;
	private System.Action<Reward> onSelected;
    
	public void Initialize(Reward rewardData, System.Action<Reward> onSelectCallback)
	{
		reward = rewardData;
		onSelected = onSelectCallback;
        
		titleText.text = reward.title;
		descriptionText.text = reward.description;
        
		// Set icon based on reward type
		SetIconForRewardType(reward.type);
        
		// Set rarity color for player cards
		if (reward.type == RewardType.PlayerCard && reward.playerCard != null)
		{
			rarityBorder.color = GetRarityColor(reward.playerCard.rarity);
		}
        
		selectButton.onClick.AddListener(OnSelectClicked);
	}
    
	void SetIconForRewardType(RewardType type)
	{
		// Set appropriate sprite based on reward type
		// For now, just use a solid color
		switch (type)
		{
		case RewardType.PlayerCard:
			iconImage.color = new Color(1f, 0.84f, 0f); // Gold
			break;
		case RewardType.AddCard:
			iconImage.color = Color.green;
			break;
		case RewardType.RemoveCard:
			iconImage.color = Color.red;
			break;
		case RewardType.UpgradeCard:
			iconImage.color = Color.cyan;
			break;
		}
	}
    
	Color GetRarityColor(CardRarity rarity)
	{
		switch (rarity)
		{
		case CardRarity.Common: return Color.gray;
		case CardRarity.Uncommon: return Color.green;
		case CardRarity.Rare: return Color.blue;
		case CardRarity.Legendary: return new Color(1f, 0.5f, 0f);
		default: return Color.white;
		}
	}
    
	void OnSelectClicked()
	{
		onSelected?.Invoke(reward);
	}
}