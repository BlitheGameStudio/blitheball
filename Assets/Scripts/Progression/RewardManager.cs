using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RewardManager : MonoBehaviour
{
	public static RewardManager Instance { get; private set; }
    
	[Header("Reward Pools")]
	public PlayerCardData[] commonPlayerCards;
	public PlayerCardData[] uncommonPlayerCards;
	public PlayerCardData[] rarePlayerCards;
	public PlayerCardData[] legendaryPlayerCards;
    
	[Header("Card Pools")]
	public CardData[] commonCards;
	public CardData[] uncommonCards;
	public CardData[] rareCards;
    
	[Header("Reward Settings")]
	public int rewardsPerChoice = 3;
    
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}
    
	public List<Reward> GenerateRewards(int quarterNumber)
	{
		List<Reward> rewards = new List<Reward>();
        
		for (int i = 0; i < rewardsPerChoice; i++)
		{
			Reward reward = GenerateSingleReward(quarterNumber);
			rewards.Add(reward);
		}
        
		return rewards;
	}
    
	Reward GenerateSingleReward(int quarterNumber)
	{
		Reward reward = new Reward();
        
		// Determine reward type with weighted chances
		float roll = Random.value;
        
		if (roll < 0.5f) // 50% chance for player card
		{
			reward.type = RewardType.PlayerCard;
			reward.playerCard = GetRandomPlayerCard(quarterNumber);
			reward.title = reward.playerCard.playerName;
			reward.description = reward.playerCard.description;
		}
		else if (roll < 0.75f) // 25% chance for regular card
		{
			reward.type = RewardType.AddCard;
			reward.regularCard = GetRandomRegularCard(quarterNumber);
			reward.cardCount = Random.Range(1, 3); // 1-2 cards
			reward.title = $"Add {reward.cardCount}x {reward.regularCard.cardName}";
			reward.description = "Add cards to your deck";
		}
		else if (roll < 0.9f) // 15% chance for remove card
		{
			reward.type = RewardType.RemoveCard;
			reward.allowsCardRemoval = true;
			reward.title = "Remove Card";
			reward.description = "Remove a card from your deck";
		}
		else // 10% chance for upgrade
		{
			reward.type = RewardType.UpgradeCard;
			reward.allowsCardUpgrade = true;
			reward.title = "Upgrade Card";
			reward.description = "Increase a card's power";
		}
        
		return reward;
	}
    
	PlayerCardData GetRandomPlayerCard(int quarterNumber)
	{
		// Increase rarity chance as quarters progress
		float legendaryChance = quarterNumber >= 4 ? 0.15f : 0.05f;
		float rareChance = quarterNumber >= 3 ? 0.3f : 0.15f;
		float uncommonChance = 0.35f;
        
		float roll = Random.value;
        
		if (roll < legendaryChance && legendaryPlayerCards.Length > 0)
		{
			return legendaryPlayerCards[Random.Range(0, legendaryPlayerCards.Length)];
		}
		else if (roll < legendaryChance + rareChance && rarePlayerCards.Length > 0)
		{
			return rarePlayerCards[Random.Range(0, rarePlayerCards.Length)];
		}
		else if (roll < legendaryChance + rareChance + uncommonChance && uncommonPlayerCards.Length > 0)
		{
			return uncommonPlayerCards[Random.Range(0, uncommonPlayerCards.Length)];
		}
		else if (commonPlayerCards.Length > 0)
		{
			return commonPlayerCards[Random.Range(0, commonPlayerCards.Length)];
		}
        
		return null;
	}
    
	CardData GetRandomRegularCard(int quarterNumber)
	{
		float rareChance = quarterNumber >= 3 ? 0.2f : 0.1f;
		float uncommonChance = 0.3f;
        
		float roll = Random.value;
        
		if (roll < rareChance && rareCards.Length > 0)
		{
			return rareCards[Random.Range(0, rareCards.Length)];
		}
		else if (roll < rareChance + uncommonChance && uncommonCards.Length > 0)
		{
			return uncommonCards[Random.Range(0, uncommonCards.Length)];
		}
		else if (commonCards.Length > 0)
		{
			return commonCards[Random.Range(0, commonCards.Length)];
		}
        
		return null;
	}
    
	public void ApplyReward(Reward reward)
	{
		switch (reward.type)
		{
		case RewardType.PlayerCard:
			if (reward.playerCard != null)
			{
				GameManager.Instance.AddPlayerCard(reward.playerCard);
			}
			break;
                
		case RewardType.AddCard:
			if (reward.regularCard != null)
			{
				for (int i = 0; i < reward.cardCount; i++)
				{
					GameManager.Instance.AddCardToDeck(reward.regularCard);
				}
			}
			break;
                
		case RewardType.RemoveCard:
			// Will be handled by RewardUI opening card selection
			break;
                
		case RewardType.UpgradeCard:
			// Will be handled by RewardUI opening card selection
			break;
		}
	}
}