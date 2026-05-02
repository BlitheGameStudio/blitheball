// FeatureFlagsManager.cs
using UnityEngine;
using System.Collections.Generic;

public class FeatureFlagsManager : MonoBehaviour
{
	public static FeatureFlagsManager Instance { get; private set; }
    
	[Header("Experimental Features")]
	[Tooltip("Toggle Positional Synergies (PG/SG/SF/PF/C combos)")]
	public bool enablePositionalSynergies = false;
    
	[Tooltip("Toggle Foul Trouble System")]
	public bool enableFoulSystem;
    
	[Tooltip("Toggle Shot Clock Pressure")]
	public bool enableShotClock;
    
	// Add more flags as you implement new systems...
    
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			LoadFlags();
		}
		else
		{
			Destroy(gameObject);
		}
	}
    
	// Optional: Persist flags between sessions for consistent testing
	public void SaveFlags()
	{
		PlayerPrefs.SetInt("Flag_PositionalSynergies", enablePositionalSynergies ? 1 : 0);
		PlayerPrefs.SetInt("Flag_FoulSystem", enableFoulSystem ? 1 : 0);
		PlayerPrefs.Save();
	}
    
	public void LoadFlags()
	{
		enablePositionalSynergies = PlayerPrefs.GetInt("Flag_PositionalSynergies", 0) == 1;
		enableFoulSystem = PlayerPrefs.GetInt("Flag_FoulSystem", 0) == 1;
	}
    
	// Runtime toggle helper (for debug UI)
	public void ToggleFlag(string flagName, bool value)
	{
		switch(flagName)
		{
		case "PositionalSynergies":
			enablePositionalSynergies = value;
			break;
		case "FoulSystem":
			enableFoulSystem = value;
			break;
			// Add cases for new flags...
		}
		SaveFlags();
		Debug.Log($"[FeatureFlags] {flagName} = {value}");
	}
}