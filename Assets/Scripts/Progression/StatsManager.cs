using UnityEngine;

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance { get; private set; }
    
	[Header("Current Run Stats")]
	public int totalPointsScored = 0;
	public int totalCardsPlayed = 0;
	public int totalMoneyEarned = 0;
	public int totalMoneySpent = 0;
	public int highestComboMultiplier = 1;
	public int perfectQuarters = 0;
    
	[Header("All-Time Stats")]
	public int totalRuns = 0;
	public int successfulRuns = 0;
	public int totalQuartersWon = 0;
	public int highestScoreEver = 0;
    
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			LoadStats();
		}
		else
		{
			Destroy(gameObject);
		}
	}
    
	public void RecordCardPlayed(int points)
	{
		totalCardsPlayed++;
		totalPointsScored += points;
	}
    
	public void RecordCombo(int multiplier)
	{
		if (multiplier > highestComboMultiplier)
		{
			highestComboMultiplier = multiplier;
		}
	}
    
	public void RecordPerfectQuarter()
	{
		perfectQuarters++;
	}
    
	public void RecordQuarterWin()
	{
		totalQuartersWon++;
	}
    
	public void RecordRunComplete(bool success)
	{
		totalRuns++;
		if (success)
		{
			successfulRuns++;
		}
        
		if (totalPointsScored > highestScoreEver)
		{
			highestScoreEver = totalPointsScored;
		}
        
		SaveStats();
		ResetRunStats();
	}
    
	void ResetRunStats()
	{
		totalPointsScored = 0;
		totalCardsPlayed = 0;
		totalMoneyEarned = 0;
		totalMoneySpent = 0;
		highestComboMultiplier = 1;
		perfectQuarters = 0;
	}
    
	void SaveStats()
	{
		PlayerPrefs.SetInt("TotalRuns", totalRuns);
		PlayerPrefs.SetInt("SuccessfulRuns", successfulRuns);
		PlayerPrefs.SetInt("TotalQuartersWon", totalQuartersWon);
		PlayerPrefs.SetInt("HighestScoreEver", highestScoreEver);
		PlayerPrefs.Save();
	}
    
	void LoadStats()
	{
		totalRuns = PlayerPrefs.GetInt("TotalRuns", 0);
		successfulRuns = PlayerPrefs.GetInt("SuccessfulRuns", 0);
		totalQuartersWon = PlayerPrefs.GetInt("TotalQuartersWon", 0);
		highestScoreEver = PlayerPrefs.GetInt("HighestScoreEver", 0);
	}
    
	public string GetStatsDisplay()
	{
		return $"Total Runs: {totalRuns}\n" +
			$"Wins: {successfulRuns}\n" +
			$"Quarters Won: {totalQuartersWon}\n" +
			$"High Score: {highestScoreEver}\n" +
			$"Current Run: {totalPointsScored} points, {totalCardsPlayed} cards";
	}
}