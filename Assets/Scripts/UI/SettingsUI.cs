using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
	[Header("References")]
	public GameObject settingsScreen;
    
	[Header("Audio Settings")]
	public Slider musicVolumeSlider;
	public Slider sfxVolumeSlider;
	public TextMeshProUGUI musicVolumeText;
	public TextMeshProUGUI sfxVolumeText;
    
	[Header("Buttons")]
	public Button closeButton;
	public Button resetStatsButton;
    
	void Start()
	{
		if (musicVolumeSlider != null)
		{
			musicVolumeSlider.value = AudioManager.Instance != null ? 
				AudioManager.Instance.musicVolume : 0.7f;
			musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
		}
        
		if (sfxVolumeSlider != null)
		{
			sfxVolumeSlider.value = AudioManager.Instance != null ? 
				AudioManager.Instance.sfxVolume : 1f;
			sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
		}
        
		if (closeButton != null)
		{
			closeButton.onClick.AddListener(CloseSettings);
		}
        
		if (resetStatsButton != null)
		{
			resetStatsButton.onClick.AddListener(OnResetStatsClicked);
		}
        
		if (settingsScreen != null)
		{
			settingsScreen.SetActive(false);
		}
	}
    
	public void ShowSettings()
	{
		settingsScreen.SetActive(true);
		UpdateVolumeDisplays();
	}
    
	public void CloseSettings()
	{
		settingsScreen.SetActive(false);
	}
    
	void OnMusicVolumeChanged(float value)
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.SetMusicVolume(value);
		}
		UpdateVolumeDisplays();
	}
    
	void OnSFXVolumeChanged(float value)
	{
		if (AudioManager.Instance != null)
		{
			AudioManager.Instance.SetSFXVolume(value);
		}
		UpdateVolumeDisplays();
	}
    
	void UpdateVolumeDisplays()
	{
		if (musicVolumeText != null && musicVolumeSlider != null)
		{
			musicVolumeText.text = $"{Mathf.RoundToInt(musicVolumeSlider.value * 100)}%";
		}
        
		if (sfxVolumeText != null && sfxVolumeSlider != null)
		{
			sfxVolumeText.text = $"{Mathf.RoundToInt(sfxVolumeSlider.value * 100)}%";
		}
	}
    
	void OnResetStatsClicked()
	{
		PlayerPrefs.DeleteAll();
		Debug.Log("Stats reset!");
	}
}