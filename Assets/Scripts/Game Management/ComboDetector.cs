using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ComboDetector : MonoBehaviour
{
	public static ComboDetector Instance { get; private set; }
    
	[Header("Combo Definitions")]
	public ComboDefinition[] comboDefs;
    
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}
    
	public ComboResult DetectCombos(List<Card> playedCards)
	{
		ComboResult result = new ComboResult();
		result.multiplier = 1f;
		result.comboNames = new List<string>();
        
		foreach (ComboDefinition combo in comboDefs)
		{
			if (CheckCombo(combo, playedCards))
			{
				result.multiplier *= combo.multiplier;
				result.bonusPoints += combo.bonusPoints;
				result.comboNames.Add(combo.comboName);
                
				Debug.Log($"COMBO: {combo.comboName} - {combo.multiplier}x multiplier");
			}
		}
        
		return result;
	}
    
	bool CheckCombo(ComboDefinition combo, List<Card> cards)
	{
		switch (combo.type)
		{
		case ComboType.SameCardType:
			return CheckSameType(combo, cards);
		case ComboType.SpecificCards:
			return CheckSpecificCards(combo, cards);
		case ComboType.MinimumCards:
			return cards.Count >= combo.requiredCount;
		case ComboType.AllThreePointers:
			return cards.All(c => c.data.cardName.Contains("Three"));
		case ComboType.MixedShots:
			return CheckMixedShots(cards);
		default:
			return false;
		}
	}
    
	bool CheckSameType(ComboDefinition combo, List<Card> cards)
	{
		if (cards.Count < combo.requiredCount) return false;
        
		var firstCard = cards[0].data.cardName;
		return cards.Take(combo.requiredCount).All(c => c.data.cardName == firstCard);
	}
    
	bool CheckSpecificCards(ComboDefinition combo, List<Card> cards)
	{
		foreach (string requiredCard in combo.requiredCardNames)
		{
			if (!cards.Any(c => c.data.cardName.Contains(requiredCard)))
			{
				return false;
			}
		}
		return true;
	}
    
	bool CheckMixedShots(List<Card> cards)
	{
		var shots = cards.Where(c => c.data.cardType == CardType.Shot).ToList();
		if (shots.Count < 3) return false;
        
		var uniqueShots = shots.Select(c => c.data.cardName).Distinct().Count();
		return uniqueShots >= 3;
	}
}

[System.Serializable]
public class ComboDefinition
{
	public string comboName;
	public ComboType type;
	public int requiredCount;
	public string[] requiredCardNames;
	public float multiplier = 1.5f;
	public int bonusPoints = 0;
}

public enum ComboType
{
	SameCardType,
	SpecificCards,
	MinimumCards,
	AllThreePointers,
	MixedShots
}

public class ComboResult
{
	public float multiplier = 1f;
	public int bonusPoints = 0;
	public List<string> comboNames = new List<string>();
}