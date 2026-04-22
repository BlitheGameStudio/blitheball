using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance { get; private set; }
    
	[Header("Audio Sources")]
	public AudioSource musicSource;
	public AudioSource sfxSource;
    
	[Header("Music")]
	public AudioClip menuMusic;
	public AudioClip gameplayMusic;
	public AudioClip shopMusic;
    
	[Header("SFX")]
	public AudioClip cardPlaySFX;
	public AudioClip cardDrawSFX;
	public AudioClip scoreSFX;
	public AudioClip comboSFX;
	public AudioClip purchaseSFX;
	public AudioClip buttonClickSFX;
	public AudioClip quarterWinSFX;
	public AudioClip gameOverSFX;
    
	[Header("Settings")]
	[Range(0f, 1f)] public float musicVolume = 0.7f;
	[Range(0f, 1f)] public float sfxVolume = 1f;
    
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
    
	void Start()
	{
		if (musicSource != null)
		{
			musicSource.volume = musicVolume;
			musicSource.loop = true;
		}
        
		if (sfxSource != null)
		{
			sfxSource.volume = sfxVolume;
		}
	}
    
	public void PlayMusic(AudioClip clip)
	{
		if (musicSource != null && clip != null)
		{
			if (musicSource.clip != clip)
			{
				musicSource.clip = clip;
				musicSource.Play();
			}
		}
	}
    
	public void PlaySFX(AudioClip clip)
	{
		if (sfxSource != null && clip != null)
		{
			sfxSource.PlayOneShot(clip);
		}
	}
    
	public void PlayCardPlay()
	{
		PlaySFX(cardPlaySFX);
	}
    
	public void PlayCardDraw()
	{
		PlaySFX(cardDrawSFX);
	}
    
	public void PlayScore()
	{
		PlaySFX(scoreSFX);
	}
    
	public void PlayCombo()
	{
		PlaySFX(comboSFX);
	}
    
	public void PlayPurchase()
	{
		PlaySFX(purchaseSFX);
	}
    
	public void PlayButtonClick()
	{
		PlaySFX(buttonClickSFX);
	}
    
	public void PlayQuarterWin()
	{
		PlaySFX(quarterWinSFX);
	}
    
	public void PlayGameOver()
	{
		PlaySFX(gameOverSFX);
	}
    
	public void SetMusicVolume(float volume)
	{
		musicVolume = Mathf.Clamp01(volume);
		if (musicSource != null)
		{
			musicSource.volume = musicVolume;
		}
	}
    
	public void SetSFXVolume(float volume)
	{
		sfxVolume = Mathf.Clamp01(volume);
		if (sfxSource != null)
		{
			sfxSource.volume = sfxVolume;
		}
	}
}