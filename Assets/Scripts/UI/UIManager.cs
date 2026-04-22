using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
	[Header("Score Display")]
	public TextMeshProUGUI currentScoreText;
	public TextMeshProUGUI targetScoreText;
	public TextMeshProUGUI quarterText;
	public TextMeshProUGUI remainingPlaysText;
	public TextMeshProUGUI deckCountText;
    
	[Header("Currency Display")]
	public TextMeshProUGUI currencyText;
    
	[Header("Buttons")]
	public Button endTurnButton;
	public Button addPlayerCardButton; // For testing on mobile
    
	[Header("Screens")]
	public GameObject quarterEndScreen;
	public GameObject gameOverScreen;
	public GameObject victoryScreen;
    
	[Header("Score Popup")]
	public GameObject scorePopupPrefab;
	public Transform popupParent;
	
	[Header("Combo Display")]
	public TextMeshProUGUI comboText;
	public TextMeshProUGUI multiplierText;
	public GameObject comboPanel;
	public CanvasGroup comboPanelCanvasGroup;
	
	[Header("Possession Display")]
	public TextMeshProUGUI possessionCountText;
	public TextMeshProUGUI cardsPlayedText;
	public GameObject possessionBonusPanel;
	public TextMeshProUGUI possessionBonusText;
	public Slider possessionProgressBar;
	
	[Header("Discovery Popup")]
	public GameObject discoveryPopup;
	public TextMeshProUGUI discoveryPopupText;
	


    
	void Start()
	{
		endTurnButton.onClick.AddListener(OnEndTurnClicked);
    
		if (addPlayerCardButton != null)
		{
			addPlayerCardButton.onClick.AddListener(OnAddPlayerCardClicked);
		}
    
		HideAllScreens();
    
		// Setup combo panel canvas group for fading
		if (comboPanel != null && comboPanelCanvasGroup == null)
		{
			comboPanelCanvasGroup = comboPanel.GetComponent<CanvasGroup>();
			if (comboPanelCanvasGroup == null)
			{
				comboPanelCanvasGroup = comboPanel.AddComponent<CanvasGroup>();
			}
		}
	}
    
	void OnAddPlayerCardClicked()
	{
		GameManager.Instance.TestAddPlayerCard();
	}
    
	public void UpdateAllUI()
	{
		GameManager gm = GameManager.Instance;
    
		currentScoreText.text = $"Score: {gm.currentScore}";
		targetScoreText.text = $"Target: {gm.targetScore}";
		quarterText.text = $"Q{gm.currentQuarter}";
    
		// Update possession display
		if (possessionCountText != null)
		{
			possessionCountText.text = $"Possessions: {gm.remainingPossessions}";
		}
    
		if (cardsPlayedText != null)
		{
			cardsPlayedText.text = $"Cards: {gm.currentPossessionCards}/{gm.maxCardsPerPossession}";
        
			// Change color based on progress
			if (gm.currentPossessionCards == gm.maxCardsPerPossession)
			{
				cardsPlayedText.color = Color.green; // Full possession
			}
			else if (gm.currentPossessionCards >= 3)
			{
				cardsPlayedText.color = Color.yellow; // Good progress
			}
			else
			{
				cardsPlayedText.color = Color.white; // Just started
			}
		}
    
		// Update progress bar
		if (possessionProgressBar != null)
		{
			possessionProgressBar.value = (float)gm.currentPossessionCards / gm.maxCardsPerPossession;
		}
    
		int deckCount = gm.GetDeckCount();
		int discardCount = gm.GetDiscardCount();
		deckCountText.text = $"Deck: {deckCount} | Discard: {discardCount}";
    
		if (currencyText != null && CurrencyManager.Instance != null)
		{
			currencyText.text = $"${CurrencyManager.Instance.currentMoney}";
		}
    
		endTurnButton.interactable = gm.playArea.Count > 0;
	}
    
	void OnEndTurnClicked()
	{
		GameManager.Instance.EndTurn();
	}
    
	public void ShowScorePopup(int points)
	{
		if (scorePopupPrefab == null || popupParent == null) return;
        
		GameObject popup = Instantiate(scorePopupPrefab, popupParent);
		TextMeshProUGUI popupText = popup.GetComponent<TextMeshProUGUI>();
        
		if (popupText != null)
		{
			popupText.text = $"+{points}";
			StartCoroutine(AnimatePopup(popup));
		}
	}
    
	IEnumerator AnimatePopup(GameObject popup)
	{
		float duration = 1.5f;
		float elapsed = 0f;
		Vector3 startPos = popup.transform.position;
		CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        
		if (canvasGroup == null)
			canvasGroup = popup.AddComponent<CanvasGroup>();
        
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float progress = elapsed / duration;
            
			popup.transform.position = startPos + Vector3.up * (progress * 100f);
			canvasGroup.alpha = 1f - progress;
            
			yield return null;
		}
        
		Destroy(popup);
	}
    
	public void HideAllScreens()
	{
		if (quarterEndScreen) quarterEndScreen.SetActive(false);
		if (gameOverScreen) gameOverScreen.SetActive(false);
		if (victoryScreen) victoryScreen.SetActive(false);
	}
    
	public void ShowQuarterEndScreen(bool won)
	{
		HideAllScreens();
		if (quarterEndScreen)
		{
			quarterEndScreen.SetActive(true);
            
			Button continueButton = quarterEndScreen.GetComponentInChildren<Button>();
			if (continueButton)
			{
				continueButton.onClick.RemoveAllListeners();
				continueButton.onClick.AddListener(() => {
					quarterEndScreen.SetActive(false);
					GameManager.Instance.ContinueToNextQuarter();
				});
			}
		}
	}
    
	public void ShowGameOverScreen()
	{
		HideAllScreens();
        
		Debug.Log("Showing Game Over Screen");
        
		if (gameOverScreen)
		{
			gameOverScreen.SetActive(true);
            
			TextMeshProUGUI gameOverText = gameOverScreen.GetComponentInChildren<TextMeshProUGUI>();
			if (gameOverText != null)
			{
				gameOverText.text = $"Game Over!\n\nFinal Score: {GameManager.Instance.currentScore} / {GameManager.Instance.targetScore}\n\nQuarter {GameManager.Instance.currentQuarter}";
			}
            
			Button restartButton = gameOverScreen.GetComponentInChildren<Button>();
			if (restartButton)
			{
				restartButton.onClick.RemoveAllListeners();
				restartButton.onClick.AddListener(OnRestartClicked);
			}
		}
		else
		{
			Debug.LogWarning("Game Over Screen is not assigned!");
		}
	}
    
	public void ShowVictoryScreen()
	{
		HideAllScreens();
        
		if (victoryScreen)
		{
			victoryScreen.SetActive(true);
            
			TextMeshProUGUI victoryText = victoryScreen.GetComponentInChildren<TextMeshProUGUI>();
			if (victoryText != null)
			{
				victoryText.text = $"Victory!\n\nYou completed all 4 quarters!\n\nFinal Score: {GameManager.Instance.currentScore}";
			}
            
			Button restartButton = victoryScreen.GetComponentInChildren<Button>();
			if (restartButton)
			{
				restartButton.onClick.RemoveAllListeners();
				restartButton.onClick.AddListener(OnRestartClicked);
			}
		}
	}
    
	void OnRestartClicked()
	{
		Debug.Log("Restart button clicked");
		GameManager.Instance.RestartGame();
	}
	
	public void ShowComboText(string combo)
	{
		ShowComboText(combo, 1f);
	}

	public void ShowComboText(string combo, float multiplier)
	{
		if (comboText != null && comboPanel != null)
		{
			comboText.text = combo;
        
			if (multiplierText != null && multiplier > 1f)
			{
				multiplierText.text = $"{multiplier:F1}x!";
			}
        
			comboPanel.SetActive(true);
        
			// Play combo animation
			StartCoroutine(AnimateCombo());
		}
	}
	
	IEnumerator AnimateCombo()
	{
		// Fade in + scale up
		float duration = 0.3f;
		float elapsed = 0f;
    
		Vector3 startScale = Vector3.one * 0.5f;
		Vector3 targetScale = Vector3.one * 1.2f;
    
		if (comboPanelCanvasGroup != null)
		{
			comboPanelCanvasGroup.alpha = 0f;
		}
    
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / duration;
        
			if (comboPanelCanvasGroup != null)
			{
				comboPanelCanvasGroup.alpha = t;
			}
        
			comboPanel.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        
			yield return null;
		}
    
		comboPanel.transform.localScale = targetScale;
    
		// Hold for a moment
		yield return new WaitForSeconds(1.5f);
    
		// Fade out
		elapsed = 0f;
		duration = 0.5f;
    
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / duration;
        
			if (comboPanelCanvasGroup != null)
			{
				comboPanelCanvasGroup.alpha = 1f - t;
			}
        
			yield return null;
		}
    
		comboPanel.SetActive(false);
		comboPanel.transform.localScale = Vector3.one;
	}

	IEnumerator HideComboAfterDelay()
	{
		yield return new WaitForSeconds(2f);
		if (comboPanel != null)
		{
			comboPanel.SetActive(false);
		}
	}
	
	public void ShowPossessionBonus(string bonusText)
	{
		if (possessionBonusPanel != null && possessionBonusText != null)
		{
			possessionBonusText.text = bonusText;
			possessionBonusPanel.SetActive(true);
			StartCoroutine(HidePossessionBonusAfterDelay());
		}
	}
	
	IEnumerator HidePossessionBonusAfterDelay()
	{
		yield return new WaitForSeconds(2f);
		if (possessionBonusPanel != null)
		{
			possessionBonusPanel.SetActive(false);
		}
	}
	
	public void AnimatePossessionProgress(int current, int max)
	{
		if (possessionProgressBar != null)
		{
			float targetValue = (float)current / max;
			StartCoroutine(AnimateSlider(possessionProgressBar, targetValue));
		}
	}

	IEnumerator AnimateSlider(Slider slider, float targetValue)
	{
		float duration = 0.3f;
		float elapsed = 0f;
		float startValue = slider.value;
    
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			slider.value = Mathf.Lerp(startValue, targetValue, elapsed / duration);
			yield return null;
		}
    
		slider.value = targetValue;
	}
	
	public void ShowDiscoveryPopup(string synergyName)
	{
		// Assumes you have a TextMeshProUGUI field named discoveryPopupText
		// and an inactive GameObject named discoveryPopup
		if (discoveryPopup != null && discoveryPopupText != null)
		{
			discoveryPopupText.text = $"🔓 NEW SYNERGY DISCOVERED!\n{synergyName}";
			discoveryPopup.SetActive(true);
			StartCoroutine(DisablePopupAfter(2.5f));
		}
	}

	private System.Collections.IEnumerator DisablePopupAfter(float delay)
	{
		yield return new WaitForSeconds(delay);
		discoveryPopup?.SetActive(false);
	}
}