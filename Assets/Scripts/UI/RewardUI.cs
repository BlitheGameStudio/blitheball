using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RewardUI : MonoBehaviour
{
	[Header("References")]
	public GameObject rewardScreen;
	public Transform rewardOptionsContainer;
	public GameObject rewardOptionPrefab;
    
	[Header("UI Elements")]
	public TextMeshProUGUI headerText;
	public Button skipButton;
    
	private List<Reward> currentRewards;
	private System.Action onRewardComplete;
    
	void Start()
	{
		if (skipButton != null)
		{
			skipButton.onClick.AddListener(OnSkipClicked);
		}
	}
    
	public void ShowRewardScreen(int quarterNumber, System.Action onComplete)
	{
		currentRewards = RewardManager.Instance.GenerateRewards(quarterNumber);
		onRewardComplete = onComplete;
        
		headerText.text = $"Quarter {quarterNumber} Complete! Choose Your Reward:";
        
		// Clear previous options
		foreach (Transform child in rewardOptionsContainer)
		{
			Destroy(child.gameObject);
		}
        
		// Create reward options
		foreach (Reward reward in currentRewards)
		{
			CreateRewardOption(reward);
		}
        
		rewardScreen.SetActive(true);
	}
    
	void CreateRewardOption(Reward reward)
	{
		GameObject optionObj = Instantiate(rewardOptionPrefab, rewardOptionsContainer);
		RewardOption option = optionObj.GetComponent<RewardOption>();
        
		if (option != null)
		{
			option.Initialize(reward, OnRewardSelected);
		}
	}
    
	void OnRewardSelected(Reward reward)
	{
		Debug.Log($"Reward selected: {reward.title}");
        
		// Handle special reward types that need additional UI
		if (reward.type == RewardType.RemoveCard)
		{
			ShowCardRemovalUI();
		}
		else if (reward.type == RewardType.UpgradeCard)
		{
			ShowCardUpgradeUI();
		}
		else
		{
			// Apply reward directly
			RewardManager.Instance.ApplyReward(reward);
			CloseRewardScreen();
		}
	}
    
	void ShowCardRemovalUI()
	{
		// TODO: Show UI to select which card to remove
		// For now, just close
		Debug.Log("Card removal UI would open here");
		CloseRewardScreen();
	}
    
	void ShowCardUpgradeUI()
	{
		// TODO: Show UI to select which card to upgrade
		// For now, just close
		Debug.Log("Card upgrade UI would open here");
		CloseRewardScreen();
	}
    
	void OnSkipClicked()
	{
		CloseRewardScreen();
	}
    
	void CloseRewardScreen()
	{
		rewardScreen.SetActive(false);
		onRewardComplete?.Invoke();
	}
    
	public void HideRewardScreen()
	{
		rewardScreen.SetActive(false);
	}
}