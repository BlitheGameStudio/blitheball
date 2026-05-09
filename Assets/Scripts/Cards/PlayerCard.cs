using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCard : MonoBehaviour
{
	public PlayerCardData data;
    
	[Header("UI References")]
	public Image portraitImage;
	public Image backgroundImage;
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI descriptionText;
	public Image rarityGlow;
    
	private bool isActive = true;


	public void Initialize(PlayerCardData cardData)
	{
		data = cardData;
		UpdateVisuals();
	}
	
	public ChemistryTag chemistryTag
	{
		get
		{
			if (data != null)
				return data.chemistryTag;
			else
			{
				Debug.LogWarning("CardData is null, returning None chemistry tag.");
				return ChemistryTag.None;
			}
		}
	}
    
	void UpdateVisuals()
	{
		if (data == null) return;
        
		nameText.text = data.playerName;
		descriptionText.text = data.description;
        
		if (data.portrait != null)
			portraitImage.sprite = data.portrait;
        
		Color typeColor = Color.white;
		switch (data.cardType)
		{
		case PlayerCardType.StarPlayer:
			typeColor = new Color(1f, 0.84f, 0f);
			break;
		case PlayerCardType.RolePlayer:
			typeColor = new Color(0.75f, 0.75f, 0.75f);
			break;
		case PlayerCardType.Coach:
			typeColor = new Color(0.6f, 0.4f, 0.8f);
			break;
		}
        
		if (backgroundImage != null)
			backgroundImage.color = typeColor;
        
		if (rarityGlow != null)
		{
			rarityGlow.color = GetRarityColor(data.rarity);
		}
	}
    
	Color GetRarityColor(CardRarity rarity)
	{
		switch (rarity)
		{
		case CardRarity.Common: return Color.gray;
		case CardRarity.Uncommon: return Color.green;
		case CardRarity.Rare: return Color.blue;
		case CardRarity.Legendary: return new Color(1f, 0.5f, 0f);
		default: return Color.white;
		}
	}
    
	public void ApplyEffects(EffectTrigger trigger, Card card, ref int points, ref float multiplier)
	{
		if (!isActive || data == null) return;
        
		foreach (PlayerEffect effect in data.effects)
		{
			if (effect.trigger != trigger && effect.trigger != EffectTrigger.Always)
				continue;
            
			if (effect.targetCardType != CardType.Shot && card != null)
			{
				if (card.data.cardType != effect.targetCardType)
					continue;
			}
            
			ApplyEffect(effect, ref points, ref multiplier);
		}
	}
    
	void ApplyEffect(PlayerEffect effect, ref int points, ref float multiplier)
	{
		switch (effect.action)
		{
		case EffectAction.AddPoints:
			points += effect.value;
			Debug.Log($"{data.playerName}: Added {effect.value} points");
			break;
                
		case EffectAction.MultiplyPoints:
			multiplier *= effect.multiplier;
			Debug.Log($"{data.playerName}: Multiplied by {effect.multiplier}x");
			break;
		}
	}
    
	public void ApplyGlobalEffect(EffectTrigger trigger)
	{
		if (!isActive || data == null) return;
        
		foreach (PlayerEffect effect in data.effects)
		{
			if (effect.trigger != trigger)
				continue;
            
			switch (effect.action)
			{
			case EffectAction.DrawCard:
				for (int i = 0; i < effect.value; i++)
					GameManager.Instance.DrawCard();
				break;
                    
			case EffectAction.RefreshPlays:
				GameManager.Instance.remainingPlays += effect.value;
				break;
			}
		}
	}
    
	public int GetHandSizeModifier()
	{
		int modifier = 0;
		if (!isActive || data == null) return modifier;
        
		foreach (PlayerEffect effect in data.effects)
		{
			if (effect.action == EffectAction.IncreaseHandSize)
				modifier += effect.value;
		}
        
		return modifier;
	}
    
	public int GetPlayCountModifier()
	{
		int modifier = 0;
		if (!isActive || data == null) return modifier;
        
		foreach (PlayerEffect effect in data.effects)
		{
			if (effect.action == EffectAction.IncreasePlayCount)
				modifier += effect.value;
		}
        
		return modifier;
	}
	

}