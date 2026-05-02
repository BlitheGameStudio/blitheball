using System.Collections.Generic;
using UnityEngine;

public class PositionSynergyDetector : MonoBehaviour
{
	public static PositionSynergyDetector Instance { get; private set; }
	[SerializeField] private List<SynergyData> synergyDatabase;
	private HashSet<string> discoveredIds = new HashSet<string>();

	void Awake() 
	{
		Instance = this;
		LoadDiscoveredSynergies();
	}

	public void RegisterDatabase(List<SynergyData> db) => synergyDatabase = db;

	public SynergyResult EvaluatePlayArea(List<Card> playedCards)
	{
		// ✅ CENTRALIZED FLAG CHECK - Early exit if system is disabled
		if (!FeatureFlagsManager.Instance || !FeatureFlagsManager.Instance.enablePositionalSynergies)
		{
			return new SynergyResult 
			{ 
				activeSynergyNames = new List<string>(),
				multiplierBonus = 1f,   // Neutral multiplier (won't affect score)
				bonusPoints = 0         // No bonus points
			};
		}
		
		var result = new SynergyResult
		{
			activeSynergyNames = new List<string>(),
			multiplierBonus = 1f, // Must start at 1x, otherwise it zeros out scoring!
			bonusPoints = 0
		};

		var playedPositions = new List<Position>();

		foreach (var card in playedCards)
			if (card.data != null && card.data.cardPosition != Position.None)
				playedPositions.Add(card.data.cardPosition);

		if (synergyDatabase == null)
		{
			Debug.LogWarning("SynergyDatabase is empty! Assign SynergyData assets in the Inspector.");
			return result;
		}

		foreach (var syn in synergyDatabase)
		{
			if (MatchesRequirements(playedPositions, syn.requiredPositions))
			{
				if (discoveredIds.Contains(syn.synergyId))
				{
					result.multiplierBonus *= syn.multiplierBonus;
					result.bonusPoints += syn.bonusPoints;
					result.activeSynergyNames.Add(syn.displayName);
				}
				else
				{
					Discover(syn);
					result.newlyDiscoveredName = syn.displayName;
				}
			}
		}
		return result;
	}

	private bool MatchesRequirements(List<Position> played, Position[] required)
	{
		var temp = new List<Position>(played);
		foreach (var req in required)
		{
			int idx = temp.IndexOf(req);
			if (idx == -1) return false;
			temp.RemoveAt(idx);
		}
		return true;
	}

	private void Discover(SynergyData syn)
	{
		discoveredIds.Add(syn.synergyId);
		PlayerPrefs.SetInt($"Synergy_{syn.synergyId}", 1);
		PlayerPrefs.Save();
		Debug.Log($"🔓 DISCOVERED: {syn.displayName}");
	}

	private void LoadDiscoveredSynergies()
	{
		if (synergyDatabase == null) return;
		foreach (var syn in synergyDatabase)
		{
			if (PlayerPrefs.GetInt($"Synergy_{syn.synergyId}", 0) == 1)
				discoveredIds.Add(syn.synergyId);
		}
	}

	public string GetDescription(SynergyData syn) =>
	discoveredIds.Contains(syn.synergyId) ? syn.discoveredDescription : syn.hiddenDescription;
}

public struct SynergyResult
{
	public List<string> activeSynergyNames;
	public float multiplierBonus;
	public int bonusPoints;
	public string newlyDiscoveredName;
}