using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public CardData data;
    
	[Header("UI References")]
	public Image backgroundImage;
	public Image cardArtImage;
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI pointsText;
	public TextMeshProUGUI descriptionText;
	public TextMeshProUGUI costText;
	public TextMeshProUGUI positionIndicatorText;
    
	[Header("Visual Settings")]
	public Color normalColor = Color.white;
	public Color hoverColor = new Color(1f, 1f, 0.8f);
	public Color selectedColor = new Color(0.8f, 1f, 0.8f);
    
	private bool isSelected = false;
	private Vector3 originalScale;
    
	void Start()
	{
		originalScale = transform.localScale;
	}
    
	void Update()
	{
		// Update visual state based on whether card can be played
		UpdatePlayableState();
	}
    
	void UpdatePlayableState()
	{
		bool canPlay = GameManager.Instance != null && GameManager.Instance.CanPlayCard(this);
        
		// Dim the card if it cannot be played
		CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
		canvasGroup.alpha = canPlay ? 1f : 0.5f;
	}
    
	public void Initialize(CardData cardData)
	{
		data = cardData;
		UpdateVisuals();
	}
    
	void UpdateVisuals()
	{
		if (data == null) return;
        
		nameText.text = data.cardName;
		descriptionText.text = data.description;
		pointsText.text = data.basePoints > 0 ? data.basePoints.ToString() : "";
		costText.text = $"Cost: {data.playCost}";
        
		if (data.cardArt != null)
			cardArtImage.sprite = data.cardArt;
        
		// Color code by type
		switch (data.cardType)
		{
		case CardType.Shot:
			backgroundImage.color = new Color(0.8f, 0.9f, 1f); // Light blue
			break;
		case CardType.Play:
			backgroundImage.color = new Color(1f, 0.95f, 0.8f); // Light yellow
			break;
		case CardType.Defensive:
			backgroundImage.color = new Color(0.9f, 1f, 0.9f); // Light green
			break;
		}
	}
    
	public void OnPointerClick(PointerEventData eventData)
	{
		// Check if card can be played before allowing interaction
		if (!GameManager.Instance.CanPlayCard(this))
		{
			// Visual feedback that card cannot be played
			StartCoroutine(ShakeCard());
			return;
		}
        
		HandManager handManager = FindObjectOfType<HandManager>();
		if (handManager != null)
		{
			handManager.OnCardClicked(this);
		}
	}
    
	private System.Collections.IEnumerator ShakeCard()
	{
		Vector3 originalPos = transform.localPosition;
		float duration = 0.3f;
		float elapsed = 0f;
        
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float offsetX = Mathf.Sin(elapsed * 50f) * 5f * (1 - elapsed / duration);
			transform.localPosition = originalPos + new Vector3(offsetX, 0, 0);
			yield return null;
		}
        
		transform.localPosition = originalPos;
	}
    
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!isSelected)
		{
			backgroundImage.color = hoverColor;
			transform.localScale = originalScale * 1.1f;
		}
	}
    
	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isSelected)
		{
			backgroundImage.color = normalColor;
			transform.localScale = originalScale;
		}
	}
    
	public void SetSelected(bool selected)
	{
		isSelected = selected;
		backgroundImage.color = selected ? selectedColor : normalColor;
		transform.localScale = selected ? originalScale * 1.05f : originalScale;
	}
	
	public void UpdatePossessionIndicator(int position, int max)
	{
		if (positionIndicatorText != null)
		{
			positionIndicatorText.text = $"{position}/{max}";
        
			// Color code based on position
			if (position == max)
			{
				positionIndicatorText.color = Color.green; // Last card - finisher potential!
			}
			else if (position == 1)
			{
				positionIndicatorText.color = Color.cyan; // First card
			}
			else
			{
				positionIndicatorText.color = Color.yellow; // Middle cards
			}
		}
	}
	
}