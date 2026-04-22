using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CardSelectionUI : MonoBehaviour
{
	[Header("References")]
	public GameObject selectionScreen;
	public Transform cardSelectionContainer;
	public GameObject selectableCardPrefab;
    
	[Header("UI Elements")]
	public TextMeshProUGUI headerText;
	public Button confirmButton;
	public Button cancelButton;
    
	private SelectionMode currentMode;
	private Card selectedCard;
	private System.Action<CardData> onSelectionComplete;
    
	public enum SelectionMode
	{
		Remove,
		Upgrade
	}
    
	void Start()
	{
		if (confirmButton != null)
		{
			confirmButton.onClick.AddListener(OnConfirmClicked);
		}
        
		if (cancelButton != null)
		{
			cancelButton.onClick.AddListener(OnCancelClicked);
		}
        
		if (selectionScreen != null)
		{
			selectionScreen.SetActive(false);
		}
	}
    
	public void ShowCardSelection(SelectionMode mode, System.Action<CardData> onComplete)
	{
		currentMode = mode;
		onSelectionComplete = onComplete;
		selectedCard = null;
        
		if (headerText != null)
		{
			headerText.text = mode == SelectionMode.Remove ? 
				"Select a Card to Remove" : 
				"Select a Card to Upgrade";
		}
        
		PopulateCards();
        
		if (confirmButton != null)
		{
			confirmButton.interactable = false;
		}
        
		selectionScreen.SetActive(true);
	}
    
	void PopulateCards()
	{
		// Clear existing
		foreach (Transform child in cardSelectionContainer)
		{
			Destroy(child.gameObject);
		}
        
		// Show all cards in deck
		List<CardData> deckCards = GameManager.Instance.startingDeck;
        
		foreach (CardData cardData in deckCards)
		{
			CreateSelectableCard(cardData);
		}
	}
    
	void CreateSelectableCard(CardData cardData)
	{
		GameObject cardObj = Instantiate(selectableCardPrefab, cardSelectionContainer);
		SelectableCard selectable = cardObj.GetComponent<SelectableCard>();
        
		if (selectable != null)
		{
			selectable.Initialize(cardData, OnCardSelected);
		}
	}
    
	void OnCardSelected(SelectableCard card)
	{
		// Deselect previous
		if (selectedCard != null)
		{
			selectedCard.SetSelected(false);
		}
        
		// Select new
		selectedCard = card.GetComponent<Card>();
		if (selectedCard != null)
		{
			selectedCard.SetSelected(true);
		}
        
		if (confirmButton != null)
		{
			confirmButton.interactable = true;
		}
	}
    
	void OnConfirmClicked()
	{
		if (selectedCard == null) return;
        
		CardData cardData = selectedCard.data;
        
		if (currentMode == SelectionMode.Remove)
		{
			GameManager.Instance.RemoveCardFromDeck(cardData);
			Debug.Log($"Removed {cardData.cardName} from deck");
		}
		else if (currentMode == SelectionMode.Upgrade)
		{
			UpgradeCard(cardData);
			Debug.Log($"Upgraded {cardData.cardName}");
		}
        
		CloseSelection();
		onSelectionComplete?.Invoke(cardData);
	}
    
	void UpgradeCard(CardData card)
	{
		// Simple upgrade: +1 point
		card.basePoints += 1;
		Debug.Log($"{card.cardName} upgraded to {card.basePoints} points");
	}
    
	void OnCancelClicked()
	{
		CloseSelection();
		onSelectionComplete?.Invoke(null);
	}
    
	void CloseSelection()
	{
		selectionScreen.SetActive(false);
	}
    
	public void HideSelection()
	{
		if (selectionScreen != null)
		{
			selectionScreen.SetActive(false);
		}
	}
}