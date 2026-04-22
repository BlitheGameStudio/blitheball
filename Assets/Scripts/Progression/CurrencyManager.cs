using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
	public static CurrencyManager Instance { get; private set; }
    
	[Header("Currency")]
	public int currentMoney = 100; // Starting money
	public int baseMoneyPerQuarter = 50;
    
	[Header("Earnings")]
	public int moneyPerPoint = 1; // Earn 1 money per point scored
	public int bonusForPerfectQuarter = 50; // Bonus if target exactly met
    
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}
    
	public void AddMoney(int amount)
	{
		currentMoney += amount;
		Debug.Log($"Added {amount} money. Total: {currentMoney}");
	}
    
	public bool SpendMoney(int amount)
	{
		if (currentMoney >= amount)
		{
			currentMoney -= amount;
			Debug.Log($"Spent {amount} money. Remaining: {currentMoney}");
			return true;
		}
        
		Debug.Log("Not enough money!");
		return false;
	}
    
	public bool CanAfford(int amount)
	{
		return currentMoney >= amount;
	}
    
	public void RewardQuarterCompletion(int scoreEarned, bool isPerfect)
	{
		int reward = baseMoneyPerQuarter;
		reward += scoreEarned * moneyPerPoint;
        
		if (isPerfect)
		{
			reward += bonusForPerfectQuarter;
			Debug.Log("Perfect quarter bonus!");
		}
        
		AddMoney(reward);
	}
    
	public void ResetCurrency()
	{
		currentMoney = 100;
	}
}