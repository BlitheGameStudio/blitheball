using UnityEngine;
using UnityEngine.UI;

public class SelectableCard : MonoBehaviour
{
	private CardData cardData;
	private System.Action<SelectableCard> onSelected;
	private Card cardComponent;
    
	public void Initialize(CardData data, System.Action<SelectableCard> onSelectCallback)
	{
		cardData = data;
		onSelected = onSelectCallback;
        
		// Use the regular Card component to display
		cardComponent = GetComponent<Card>();
		if (cardComponent != null)
		{
			cardComponent.Initialize(data);
		}
        
		// Add click listener
		Button button = GetComponent<Button>();
		if (button == null)
		{
			button = gameObject.AddComponent<Button>();
		}
		button.onClick.AddListener(OnClicked);
	}
    
	void OnClicked()
	{
		onSelected?.Invoke(this);
	}
}