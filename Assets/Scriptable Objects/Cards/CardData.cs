using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Basketball/Card")]
public class CardData : ScriptableObject
{
	[Header("Basic Info")]
	public string cardName;
	public CardType cardType;
	public Sprite cardArt;
	[TextArea(2, 4)]
	public string description;
    
	[Header("Scoring")]
	public int basePoints;
    
	[Header("Costs")]
	public int playCost = 1;
	
	[Header("Possession Mechanics")]
	public PossessionEffect possessionEffect = PossessionEffect.None;
	public int possessionValue = 0;
	
	[Header("Positional Identity")]
	[Tooltip("Primary position tag for synergy detection.")]
	public Position cardPosition = Position.None;
}

public enum Position { None, PG, SG, SF, PF, C }

public enum CardType
{
	Shot,
	Play,
	Defensive
}

public enum PossessionEffect
{
	None,
	ChainBonus,        // +value per card played before
	FinisherBonus,     // Multiply if last card
	SetupBonus,        // Boost next N cards
	DrawCards,         // Draw cards mid-possession
	MultiSlot,         // Uses multiple slots
	RefreshHand        // Discard hand, draw new cards
}