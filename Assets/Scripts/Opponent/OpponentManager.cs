using UnityEngine;
using System.Collections.Generic;

public class OpponentManager : MonoBehaviour
{
	public static OpponentManager Instance { get; private set; }
    
	[Header("Opponent Pool")]
	public OpponentData[] easyOpponents;
	public OpponentData[] mediumOpponents;
	public OpponentData[] hardOpponents;
	public OpponentData[] bossOpponents;
    
	[Header("Current Match")]
	public OpponentData currentOpponent;
	private List<OpponentAbility> activeAbilities = new List<OpponentAbility>();
    
	[Header("UI")]
	public OpponentUI opponentUI;
    
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}
    
	public void SelectOpponent(int quarterNumber)
	{
		// Select opponent based on quarter
		if (quarterNumber == 4)
		{
			// Boss opponent for quarter 4
			currentOpponent = GetRandomOpponent(bossOpponents);
		}
		else if (quarterNumber == 3)
		{
			currentOpponent = GetRandomOpponent(hardOpponents);
		}
		else if (quarterNumber == 2)
		{
			currentOpponent = GetRandomOpponent(mediumOpponents);
		}
		else
		{
			currentOpponent = GetRandomOpponent(easyOpponents);
		}
        
		if (currentOpponent != null)
		{
			Debug.Log($"Opponent selected: {currentOpponent.teamName}");
            
			// Update UI
			if (opponentUI != null)
			{
				opponentUI.ShowOpponent(currentOpponent);
			}
            
			// Apply quarter start abilities
			TriggerAbilities(AbilityTrigger.OnQuarterStart);
		}
	}
    
	OpponentData GetRandomOpponent(OpponentData[] pool)
	{
		Debug.Log($"=== GetRandomOpponent Called ===");
		Debug.Log($"Pool is null: {pool == null}");
    
		if (pool == null)
		{
			Debug.LogError("Pool is NULL!");
			return null;
		}
    
		Debug.Log($"Pool length: {pool.Length}");
    
		if (pool.Length == 0)
		{
			Debug.LogError("Pool is EMPTY!");
			return null;
		}
    
		// Check if pool contains actual data
		for (int i = 0; i < pool.Length; i++)
		{
			Debug.Log($"Pool[{i}]: {(pool[i] != null ? pool[i].teamName : "NULL")}");
		}
    
		int randomIndex = Random.Range(0, pool.Length);
		Debug.Log($"Random index: {randomIndex}");
    
		OpponentData selected = pool[randomIndex];
		Debug.Log($"Selected opponent: {(selected != null ? selected.teamName : "NULL")}");
    
		return selected;
	}
    
	public int GetQuarterTarget(int quarter, int baseTarget)
	{
		if (currentOpponent == null)
			return baseTarget;
        
		int modifier = 0;
		switch (quarter)
		{
		case 1: modifier = currentOpponent.q1TargetModifier; break;
		case 2: modifier = currentOpponent.q2TargetModifier; break;
		case 3: modifier = currentOpponent.q3TargetModifier; break;
		case 4: modifier = currentOpponent.q4TargetModifier; break;
		}
        
		return baseTarget + modifier;
	}
    
	public void ApplyDefenseModifiers(ref int score, ref float multiplier, Card card)
	{
		if (currentOpponent == null)
			return;
        
		switch (currentOpponent.primaryDefense)
		{
		case DefenseType.ZoneDefense:
			ApplyZoneDefense(ref score, ref multiplier);
			break;
                
		case DefenseType.Physical:
			ApplyPhysicalDefense(ref score, card);
			break;
                
		case DefenseType.PerimeterDefense:
			ApplyPerimeterDefense(ref score, card);
			break;
                
		case DefenseType.Lockdown:
			ApplyLockdownDefense(ref multiplier);
			break;
                
		case DefenseType.Momentum:
			ApplyMomentumDefense(ref multiplier);
			break;
		}
	}
    
	void ApplyZoneDefense(ref int score, ref float multiplier)
	{
		// First card each turn scores less
		if (GameManager.Instance.playArea.Count == 1)
		{
			multiplier *= 0.5f;
			Debug.Log("Zone Defense: First card reduced!");
		}
	}
    
	void ApplyPhysicalDefense(ref int score, Card card)
	{
		// Reduce inside scoring
		if (card.data.cardName.Contains("Layup") || card.data.cardName.Contains("Dunk"))
		{
			score = Mathf.RoundToInt(score * 0.7f);
			Debug.Log("Physical Defense: Inside shot reduced!");
		}
	}
    
	void ApplyPerimeterDefense(ref int score, Card card)
	{
		// Reduce three-pointers
		if (card.data.cardName.Contains("Three"))
		{
			score = Mathf.RoundToInt(score * 0.6f);
			Debug.Log("Perimeter Defense: Three-pointer reduced!");
		}
	}
    
	void ApplyLockdownDefense(ref float multiplier)
	{
		// Reduce all scoring
		multiplier *= 0.8f;
	}
    
	void ApplyMomentumDefense(ref float multiplier)
	{
		// Gets stronger as quarter progresses
		float progress = 1f - (GameManager.Instance.remainingPlays / (float)GameManager.Instance.basePlayCount);
		float reduction = 1f - (progress * 0.3f); // Up to 30% reduction
		multiplier *= reduction;
	}
    
	public int GetHandSizePenalty()
	{
		if (currentOpponent == null)
			return 0;
            
		if (currentOpponent.primaryDefense == DefenseType.FullCourtPress)
		{
			return 2; // -2 hand size
		}
        
		return 0;
	}
    
	public void TriggerAbilities(AbilityTrigger trigger)
	{
		if (currentOpponent == null || currentOpponent.abilities == null)
			return;
        
		foreach (OpponentAbility ability in currentOpponent.abilities)
		{
			if (ability.trigger == trigger)
			{
				ApplyAbility(ability);
			}
		}
	}
    
	void ApplyAbility(OpponentAbility ability)
	{
		Debug.Log($"Opponent Ability: {ability.abilityName}");
        
		switch (ability.effect)
		{
		case AbilityEffect.ReduceScore:
			// Applied in defense modifiers
			break;
                
		case AbilityEffect.IncreaseTarget:
			GameManager.Instance.targetScore += ability.value;
			Debug.Log($"Target increased by {ability.value}!");
			break;
                
		case AbilityEffect.ReduceHandSize:
			// Applied at quarter start
			break;
                
		case AbilityEffect.ReducePlays:
			GameManager.Instance.remainingPlays -= ability.value;
			Debug.Log($"Plays reduced by {ability.value}!");
			break;
                
		case AbilityEffect.StealMoney:
			if (CurrencyManager.Instance != null)
			{
				CurrencyManager.Instance.currentMoney = 
					Mathf.Max(0, CurrencyManager.Instance.currentMoney - ability.value);
				Debug.Log($"Lost ${ability.value} to opponent ability!");
			}
			break;
		}
        
		// Show ability notification
		if (opponentUI != null)
		{
			opponentUI.ShowAbilityNotification(ability);
		}
	}
    
	public void OnCardPlayed(Card card)
	{
		TriggerAbilities(AbilityTrigger.OnCardPlayed);
        
		if (card.data.cardName.Contains("Three"))
		{
			TriggerAbilities(AbilityTrigger.OnThreePointer);
		}
	}
    
	public void OnPlayerScore(int score)
	{
		TriggerAbilities(AbilityTrigger.OnPlayerScore);
	}
    
	public void OnCombo()
	{
		TriggerAbilities(AbilityTrigger.OnCombo);
	}
    
	public float GetMoneyMultiplier()
	{
		return currentOpponent != null ? currentOpponent.moneyMultiplier : 1f;
	}
}