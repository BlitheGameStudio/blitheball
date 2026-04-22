using UnityEngine;

[CreateAssetMenu(fileName = "New Opponent", menuName = "Basketball/Opponent")]
public class OpponentData : ScriptableObject
{
	[Header("Team Info")]
	public string teamName;
	public Sprite teamLogo;
	public Color teamColor = Color.white;
	[TextArea(2, 4)]
	public string teamDescription;
    
	[Header("Difficulty")]
	public OpponentDifficulty difficulty;
    
	[Header("Quarter Modifiers")]
	public int q1TargetModifier = 0;  // Added to base target
	public int q2TargetModifier = 0;
	public int q3TargetModifier = 0;
	public int q4TargetModifier = 0;
    
	[Header("Defense Type")]
	public DefenseType primaryDefense;
	public float defenseStrength = 1f;  // 1 = normal, >1 = harder
    
	[Header("Special Abilities")]
	public OpponentAbility[] abilities;
    
	[Header("Rewards Modifier")]
	public float moneyMultiplier = 1f;  // Harder opponents = more money
}

public enum OpponentDifficulty
{
	Easy,
	Medium,
	Hard,
	Boss
}

public enum DefenseType
{
	Balanced,           // No special modifiers
	ZoneDefense,        // First card each turn reduced
	FullCourtPress,     // Reduces hand size
	Physical,           // Reduces layup/dunk damage
	PerimeterDefense,   // Reduces three-pointer damage
	Lockdown,           // All shots reduced
	FastPaced,          // Increases target scores
	Momentum            // Gets stronger as quarter progresses
}

[System.Serializable]
public class OpponentAbility
{
	public string abilityName;
	[TextArea(2, 3)]
	public string description;
	public AbilityTrigger trigger;
	public AbilityEffect effect;
	public int value;
}

public enum AbilityTrigger
{
	OnQuarterStart,
	OnTurnStart,
	OnCardPlayed,
	OnPlayerScore,
	OnThreePointer,
	OnCombo,
	OnQuarterEnd,
	WhenBehind,       // When player is behind target
	WhenAhead         // When player is ahead of pace
}

public enum AbilityEffect
{
	ReduceScore,
	ReduceMultiplier,
	IncreaseTarget,
	ReduceHandSize,
	ReducePlays,
	StealMoney,
	DisableRandomPlayerCard,
	ReduceSpecificCardType
}