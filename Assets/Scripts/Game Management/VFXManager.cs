using UnityEngine;
using System.Collections;

public class VFXManager : MonoBehaviour
{
	public static VFXManager Instance { get; private set; }
    
	[Header("Particle Effects")]
	public ParticleSystem scoreParticles;
	public ParticleSystem comboParticles;
	public ParticleSystem cardPlayParticles;
    
	[Header("Screen Effects")]
	public Camera mainCamera;
	public float screenShakeIntensity = 0.1f;
	public float screenShakeDuration = 0.2f;
    
	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
            
		if (mainCamera == null)
			mainCamera = Camera.main;
	}
    
	public void PlayScoreEffect(Vector3 position)
	{
		if (scoreParticles != null)
		{
			ParticleSystem particles = Instantiate(scoreParticles, position, Quaternion.identity);
			Destroy(particles.gameObject, 2f);
		}
	}
    
	public void PlayComboEffect(Vector3 position)
	{
		if (comboParticles != null)
		{
			ParticleSystem particles = Instantiate(comboParticles, position, Quaternion.identity);
			Destroy(particles.gameObject, 3f);
		}
        
		StartCoroutine(ScreenShake(screenShakeIntensity, screenShakeDuration));
	}
    
	public void PlayCardEffect(Vector3 position)
	{
		if (cardPlayParticles != null)
		{
			ParticleSystem particles = Instantiate(cardPlayParticles, position, Quaternion.identity);
			Destroy(particles.gameObject, 1f);
		}
	}
    
	IEnumerator ScreenShake(float intensity, float duration)
	{
		if (mainCamera == null) yield break;
        
		Vector3 originalPos = mainCamera.transform.position;
		float elapsed = 0f;
        
		while (elapsed < duration)
		{
			float x = Random.Range(-1f, 1f) * intensity;
			float y = Random.Range(-1f, 1f) * intensity;
            
			mainCamera.transform.position = new Vector3(
				originalPos.x + x,
				originalPos.y + y,
				originalPos.z
			);
            
			elapsed += Time.deltaTime;
			yield return null;
		}
        
		mainCamera.transform.position = originalPos;
	}
}