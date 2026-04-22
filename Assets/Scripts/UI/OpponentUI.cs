using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OpponentUI : MonoBehaviour
{
	[Header("Opponent Display")]
	public GameObject opponentPanel;
	public Image teamLogoImage;
	public TextMeshProUGUI teamNameText;
	public TextMeshProUGUI defenseTypeText;
	public Image teamColorBar;
    
	[Header("Abilities")]
	public Transform abilitiesContainer;
	public GameObject abilityIconPrefab;
    
	[Header("Notifications")]
	public GameObject abilityNotificationPanel;
	public TextMeshProUGUI abilityNotificationText;
    
	public void ShowOpponent(OpponentData opponent)
	{
		if (opponent == null)
			return;
        
		if (teamLogoImage != null && opponent.teamLogo != null)
		{
			teamLogoImage.sprite = opponent.teamLogo;
		}
        
		if (teamNameText != null)
		{
			teamNameText.text = opponent.teamName;
		}
        
		if (defenseTypeText != null)
		{
			defenseTypeText.text = GetDefenseDescription(opponent.primaryDefense);
		}
        
		if (teamColorBar != null)
		{
			teamColorBar.color = opponent.teamColor;
		}
        
		// Show abilities
		if (abilitiesContainer != null)
		{
			// Clear existing
			foreach (Transform child in abilitiesContainer)
			{
				Destroy(child.gameObject);
			}
            
			// Add ability icons
			if (opponent.abilities != null)
			{
				foreach (OpponentAbility ability in opponent.abilities)
				{
					CreateAbilityIcon(ability);
				}
			}
		}
        
		if (opponentPanel != null)
		{
			opponentPanel.SetActive(true);
		}
	}
    
	void CreateAbilityIcon(OpponentAbility ability)
	{
		if (abilityIconPrefab == null || abilitiesContainer == null)
			return;
        
		GameObject iconObj = Instantiate(abilityIconPrefab, abilitiesContainer);
        
		// Set tooltip or text
		TextMeshProUGUI text = iconObj.GetComponentInChildren<TextMeshProUGUI>();
		if (text != null)
		{
			text.text = ability.abilityName;
		}
	}
    
	public void ShowAbilityNotification(OpponentAbility ability)
	{
		if (abilityNotificationPanel == null || abilityNotificationText == null)
			return;
        
		abilityNotificationText.text = $"{ability.abilityName}\n{ability.description}";
		abilityNotificationPanel.SetActive(true);
        
		StartCoroutine(HideNotificationAfterDelay());
	}
    
	IEnumerator HideNotificationAfterDelay()
	{
		yield return new WaitForSeconds(3f);
        
		if (abilityNotificationPanel != null)
		{
			abilityNotificationPanel.SetActive(false);
		}
	}
    
	string GetDefenseDescription(DefenseType defense)
	{
		switch (defense)
		{
		case DefenseType.ZoneDefense:
			return "Zone Defense - First card reduced";
		case DefenseType.FullCourtPress:
			return "Full Court Press - Reduced hand size";
		case DefenseType.Physical:
			return "Physical Defense - Inside shots reduced";
		case DefenseType.PerimeterDefense:
			return "Perimeter Defense - 3-pointers reduced";
		case DefenseType.Lockdown:
			return "Lockdown - All shots reduced";
		case DefenseType.Momentum:
			return "Momentum - Stronger over time";
		case DefenseType.FastPaced:
			return "Fast Paced - Higher targets";
		default:
			return "Balanced Defense";
		}
	}
    
	public void HideOpponent()
	{
		if (opponentPanel != null)
		{
			opponentPanel.SetActive(false);
		}
	}
}