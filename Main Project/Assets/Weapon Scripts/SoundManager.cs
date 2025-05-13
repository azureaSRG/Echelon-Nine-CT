using UnityEngine;

public class SoundManager : MonoBehaviour
{
	
	public static SoundManager instance;
	
	[SerializeField] private AudioSource soundFXObject;
	
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}
	
    public void PlaySoundFX(AudioClip audioClip, Transform spawnTransform, float volume, float pitch)
	{
		AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
		
		audioSource.clip = audioClip;
		audioSource.pitch = pitch;
		audioSource.volume = volume;
		
		audioSource.Play();
		
		float clipLength = audioSource.clip.length;
		
		Destroy(audioSource.gameObject, clipLength);
	}
}
