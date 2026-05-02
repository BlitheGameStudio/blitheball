using UnityEngine;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
	[Header("References")]
	public Transform handContainer;
	public GameObject cardPrefab;
	public Transform playAreaContainer;
    
	[Header("Card Size Settings")]
	public Vector2 cardSize = new Vector2(200f, 280f); // NEW: Adjustable in inspector
	public float minCardScale = 0.5f; // Minimum scale when too many cards
	public float maxCardScale = 1.0f; // Maximum scale
    
	[Header("Layout Settings")]
	public float cardSpacing = 10f; // Gap between cards
	public float cardOverlap = 50f; // How much cards overlap when cramped
	public bool enableOverlap = true; // Toggle overlap on/off
	public float hoverOffset = 30f;
    
	[Header("Debug")]
	public bool showDebugInfo = false;
    
	private Card selectedCard = null;
    
	void Update()
	{
		UpdateHandLayout();
		UpdatePlayAreaLayout();
	}
    
	public void CreateCardInHand(CardData cardData)
	{
		GameObject cardObj = Instantiate(cardPrefab, handContainer);
		Card card = cardObj.GetComponent<Card>();
		card.Initialize(cardData);
        
		RectTransform cardRect = cardObj.GetComponent<RectTransform>();
		if (cardRect != null)
		{
			cardRect.anchoredPosition = Vector2.zero;
			cardRect.localScale = Vector3.one;
			cardRect.sizeDelta = cardSize; // Use inspector value
		}
        
		GameManager.Instance.hand.Add(card);
        
		UpdateHandLayout();
	}
    
	void UpdateHandLayout()
	{
		List<Card> hand = GameManager.Instance.hand;
		int cardCount = hand.Count;
        
		if (cardCount == 0) return;
        
		RectTransform handRect = handContainer.GetComponent<RectTransform>();
		float containerWidth = handRect != null ? handRect.rect.width : 800f;
        
		// Calculate how cards should be arranged
		LayoutInfo layout = CalculateLayout(cardCount, containerWidth);
        
		if (showDebugInfo)
		{
			Debug.Log($"Cards: {cardCount}, Scale: {layout.scale:F2}, Spacing: {layout.spacing:F1}, Overlap: {layout.useOverlap}");
		}
        
		// Position each card
		for (int i = 0; i < cardCount; i++)
		{
			Card card = hand[i];
			RectTransform cardRect = card.GetComponent<RectTransform>();
            
			if (cardRect != null)
			{
				// Calculate position
				float xPos = layout.startX + (i * layout.cardStep);
				Vector3 targetPos = new Vector3(xPos, 0, 0);
                
				// Smooth movement
				cardRect.anchoredPosition = Vector2.Lerp(
					cardRect.anchoredPosition,  // ✅ Was: localPosition
					new Vector2(xPos, 0),        // ✅ Use Vector2 for anchoredPosition
					Time.deltaTime * 10f
				);
                
				// Apply scale
				Vector3 targetScale = Vector3.one * layout.scale;
				cardRect.localScale = Vector3.Lerp(
					cardRect.localScale,
					targetScale,
					Time.deltaTime * 10f
				);
                
				// Update card size if needed
				if (cardRect.sizeDelta != cardSize)
				{
					cardRect.sizeDelta = cardSize;
				}
			}
		}
	}
    
	LayoutInfo CalculateLayout(int cardCount, float containerWidth)
	{
		LayoutInfo layout = new LayoutInfo();
        
		// Calculate total width needed at full size
		float fullCardWidth = cardSize.x;
		float totalWidthNeeded = (cardCount * fullCardWidth) + ((cardCount - 1) * cardSpacing);
        
		if (totalWidthNeeded <= containerWidth)
		{
			// Cards fit perfectly - no scaling or overlap needed
			layout.scale = maxCardScale;
			layout.spacing = cardSpacing;
			layout.useOverlap = false;
			layout.cardStep = fullCardWidth + cardSpacing;
			layout.totalWidth = totalWidthNeeded;
		}
		else if (enableOverlap)
		{
			// Try with overlap at full scale
			float overlapStep = fullCardWidth - cardOverlap;
			float totalWidthWithOverlap = fullCardWidth + ((cardCount - 1) * overlapStep);
            
			if (totalWidthWithOverlap <= containerWidth)
			{
				// Fits with overlap, no scaling needed
				layout.scale = maxCardScale;
				layout.spacing = -cardOverlap; // Negative spacing = overlap
				layout.useOverlap = true;
				layout.cardStep = overlapStep;
				layout.totalWidth = totalWidthWithOverlap;
			}
			else
			{
				// Need to scale down even with overlap
				float targetScale = containerWidth / totalWidthWithOverlap;
				layout.scale = Mathf.Clamp(targetScale, minCardScale, maxCardScale);
				layout.spacing = -cardOverlap * layout.scale;
				layout.useOverlap = true;
				layout.cardStep = (fullCardWidth - cardOverlap) * layout.scale;
				layout.totalWidth = totalWidthWithOverlap * layout.scale;
			}
		}
		else
		{
			// Scale down without overlap
			float targetScale = (containerWidth - ((cardCount - 1) * cardSpacing)) / (cardCount * fullCardWidth);
			layout.scale = Mathf.Clamp(targetScale, minCardScale, maxCardScale);
			layout.spacing = cardSpacing;
			layout.useOverlap = false;
			layout.cardStep = (fullCardWidth * layout.scale) + cardSpacing;
			layout.totalWidth = (cardCount * fullCardWidth * layout.scale) + ((cardCount - 1) * cardSpacing);
		}
        
		// Calculate start position (centered)
		layout.startX = -layout.totalWidth / 2f + (fullCardWidth * layout.scale / 2f);
        
		return layout;
	}
    
	public void OnCardClicked(Card card)
	{
		if (GameManager.Instance.currentState != GameState.PlayPhase)
			return;
        
		if (selectedCard == card)
		{
			// ✅ Find the index and remove by index (more reliable than reference)
			int cardIndex = GameManager.Instance.hand.IndexOf(card);
			if (cardIndex != -1)
			{
				GameManager.Instance.hand.RemoveAt(cardIndex);
				Debug.Log($"[HandManager] Removed card at index {cardIndex} | Remaining: {GameManager.Instance.hand.Count}");
			}
			
			
			GameManager.Instance.PlayCard(card);
            
			if (playAreaContainer != null)
			{
				card.transform.SetParent(playAreaContainer);
				RectTransform cardRect = card.GetComponent<RectTransform>();
				if (cardRect != null)
				{
					cardRect.localScale = Vector3.one;
					cardRect.sizeDelta = cardSize; // Maintain size in play area
				}
			}
            
			UpdatePlayAreaLayout();
			selectedCard = null;
		}
		else
		{
			if (selectedCard != null)
			{
				selectedCard.SetSelected(false);
			}
            
			selectedCard = card;
			card.SetSelected(true);
		}
	}
    
	public void UpdatePlayAreaLayout()
	{
		if (playAreaContainer == null) return;
        
		List<Card> playArea = GameManager.Instance.playArea;
		int cardCount = playArea.Count;
        
		if (cardCount == 0) return;
        
		float spacing = 20f;
        
		// Use the current card size for play area too
		float totalWidth = (cardCount * cardSize.x) + ((cardCount - 1) * spacing);
		float startX = -totalWidth / 2f + (cardSize.x / 2f);
        
		for (int i = 0; i < cardCount; i++)
		{
			Card card = playArea[i];
			RectTransform cardRect = card.GetComponent<RectTransform>();
            
			if (cardRect != null)
			{
				float xPos = startX + (i * (cardSize.x + spacing));
				Vector3 targetPos = new Vector3(xPos, 0, 0);
                
				cardRect.anchoredPosition = Vector2.Lerp(
					cardRect.anchoredPosition,
					new Vector2(xPos, 0),
					Time.deltaTime * 15f
				);
                
				// Ensure cards in play area are full size
				cardRect.sizeDelta = cardSize;
				cardRect.localScale = Vector3.one;
			}
		}
	}
}

// Helper class to store layout calculations
public class LayoutInfo
{
	public float scale = 1f;
	public float spacing = 0f;
	public bool useOverlap = false;
	public float cardStep = 0f;
	public float totalWidth = 0f;
	public float startX = 0f;
}