using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ChemistrySystem
{
	// Example chemistry sets
	private static readonly List<ChemistrySet> sets = new List<ChemistrySet>
	{
		new ChemistrySet
		{
		name = "Old Guard",
		requiredTags = new Dictionary<ChemistryTag, int> { { ChemistryTag.Veteran, 2 } },
		bonusHandSize = 1,
		bonusScoreModifier = 0,
		bonusMultiplierModifier = 0
		},
		new ChemistrySet
		{
		name = "Fresh Blood",
		requiredTags = new Dictionary<ChemistryTag, int> { { ChemistryTag.Rookie, 3 } },
		bonusHandSize = 0,
		bonusScoreModifier = 5,   // flat +5 to score modifier (adds to the sum)
		bonusMultiplierModifier = 0
		},
		new ChemistrySet
		{
		name = "Mentor & Protégé",
		requiredTags = new Dictionary<ChemistryTag, int> 
		{ 
			{ ChemistryTag.Veteran, 1 }, 
			{ ChemistryTag.Rookie, 1 } 
		},
		bonusHandSize = 0,
		bonusScoreModifier = 3,
		bonusMultiplierModifier = 0.1f
		}
	};

	public struct ChemistryBonus
	{
		public int handSize;
		public float scoreModifier;
		public float multiplierModifier;
	}

	public static ChemistryBonus CalculateBonuses(List<PlayerCard> activePlayers)
	{
		ChemistryBonus totalBonus = new ChemistryBonus();

		// Count active tags
		Dictionary<ChemistryTag, int> tagCounts = new Dictionary<ChemistryTag, int>();
		foreach (var player in activePlayers)
		{
			if (player.chemistryTag == ChemistryTag.None) continue;
			if (!tagCounts.ContainsKey(player.chemistryTag))
				tagCounts[player.chemistryTag] = 0;
			tagCounts[player.chemistryTag]++;
		}

		// Check each chemistry set
		foreach (var set in sets)
		{
			bool requirementsMet = true;
			foreach (var req in set.requiredTags)
			{
				if (!tagCounts.TryGetValue(req.Key, out int count) || count < req.Value)
				{
					requirementsMet = false;
					break;
				}
			}
			if (requirementsMet)
			{
				totalBonus.handSize += set.bonusHandSize;
				totalBonus.scoreModifier += set.bonusScoreModifier;
				totalBonus.multiplierModifier += set.bonusMultiplierModifier;
				// You could also trigger a UI notification here
			}
		}

		return totalBonus;
	}

	private class ChemistrySet
	{
		public string name;
		public Dictionary<ChemistryTag, int> requiredTags;
		public int bonusHandSize;
		public float bonusScoreModifier;
		public float bonusMultiplierModifier;
	}
}