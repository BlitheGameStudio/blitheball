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
		var result = new SynergyResult();
		var playedPositions = new List<Position>();

		foreach (var card in playedCards)
			if (card.data.cardPosition != Position.None)
				playedPositions.Add(card.data.cardPosition);

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