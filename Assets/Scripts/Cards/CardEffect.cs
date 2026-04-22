using UnityEngine;

[System.Serializable]
public class CardEffect
{
	public EffectType type;
	public float value;
	public int duration; // In turns, -1 for permanent
	public EffectTarget target;
}

public enum EffectType
{
	AddPoints,
	MultiplyScore,
	DrawCards,
	ExtraPlays,
	ModifyHandSize,
	RefreshStamina,
	IgnoreDefense,
	DoubleNextShot
}

public enum EffectTarget
{
	Self,
	NextCard,
	AllCardsThisTurn,
	Permanent
}