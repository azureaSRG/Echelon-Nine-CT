using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance; // Singleton for global access

    private AudioSource audioSource;

    // Array to store multiple audio clips
    public AudioClip[] musicTracks;

    private int currentTrackIndex = 0;

    private void Awake()
    {
        // Singleton pattern: Ensure only one MusicManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    // Play a specific track by index
    public void PlayTrack(int trackIndex)
    {    
        //Checks to see if the inputted Track is in range of the current amounts of tracking
        if (trackIndex >= 0 && trackIndex < musicTracks.Length)
        {
            //Checks to see if audio is playing and stops it if it is playing
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            // Swaps & plays new track
            audioSource.clip = musicTracks[trackIndex];
            audioSource.Play();
            currentTrackIndex = trackIndex;
        }
        else
        {
            Debug.LogWarning("Track index out of range!");
        }
    }

    //Start() is a function that gets called by unity when this script is ran for the first time
    private void Start()
    {
        if(musicTracks.Length > 0)
        {
            PlayTrack(0);
        }
    }


    // Play the next track in the array
    public void PlayNextTrack()
    {
        int nextTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayTrack(nextTrackIndex);
    }

    // Stop playing music
    public void StopMusic()
    {
        audioSource.Stop();
    }

    // Set volume of the music
    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume); // Clamp volume between 0 and 1
    }
}
