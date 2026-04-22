using UnityEngine;

[CreateAssetMenu(fileName = "NewSynergy", menuName = "BasketballRoguelike/SynergyData")]
public class SynergyData : ScriptableObject
{
	public string synergyId; // Unique key for saving (e.g., "backcourt_vision")
	public string displayName; // "Backcourt Vision"
    
	[Header("Requirements")]
	public Position[] requiredPositions; // e.g., [PG, SG]
    
	[Header("Effects")]
	public float multiplierBonus = 1f;
	public int bonusPoints = 0;
    
	[Header("Discovery")]
	[Tooltip("Shown as '???' until discovered")]
	public string hiddenDescription = "???";
	public string discoveredDescription = "PG + SG played together → Next turn draw +1";
}