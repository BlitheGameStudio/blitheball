using UnityEngine;

[CreateAssetMenu(fileName = "New Player Card", menuName = "Basketball/Player Card")]
public class PlayerCardData : ScriptableObject
{
	[Header("Basic Info")]
	public string playerName;
	public Sprite portrait;
	[TextArea(3, 5)]
	public string description;
    
	[Header("Classification")]
	public PlayerCardType cardType;
	public CardRarity rarity;
    
	[Header("Effects")]
	public PlayerEffect[] effects;
}

public enum PlayerCardType
{
	StarPlayer,
	RolePlayer,
	Coach
}

public enum CardRarity
{
	Common,
	Uncommon,
	Rare,
	Legendary
}

[System.Serializable]
public class PlayerEffect
{
	public EffectTrigger trigger;
	public EffectAction action;
	public CardType targetCardType; // Which cards this affects
	public int value; // Amount to add/multiply
	public float multiplier; // Multiplier value
}

public enum EffectTrigger
{
	Always,              // Passive, always active
	OnCardPlayed,        // When any card is played
	OnShotPlayed,        // When a shot card is played
	OnThreePointer,      // When a 3-pointer is played
	OnLayup,             // When a layup is played
	OnDunk,              // When a dunk is played
	OnAssist,            // When an assist is played
	OnTurnEnd,           // At end of turn
	OnQuarterStart,      // At start of quarter
	BelowHalfTarget      // When score is below 50% of target
}

public enum EffectAction
{
	AddPoints,           // Add flat points
	MultiplyPoints,      // Multiply points by value
	IncreaseHandSize,    // Permanent hand size increase
	IncreasePlayCount,   // Permanent play count increase
	DrawCard,            // Draw extra cards
	RefreshPlays,        // Add plays back
	DoubleNextShot       // Next shot worth 2x
}