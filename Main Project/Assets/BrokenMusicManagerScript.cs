using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance; // Singleton instance
    public AudioSource MusicSource;      // AudioSource component reference
    public AudioClip[] musicTracks;      // Array for storing music tracks

    private void Awake()
    {
        // Singleton pattern to ensure only one MusicManager exists
        if (instance is null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure MusicSource is assigned at the time of initialization
        if (MusicSource == null)
        {
            MusicSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // Play the first track by default if it exists
        if (musicTracks.Length > 0)
        {
            PlayMusic(0);
        }
        else
        {
            Debug.LogError("No music tracks assigned!");
        }
    }

    public void PlayMusic(int trackIndex)
    {
        if (MusicSource == null)
        {
            Debug.LogError("MusicSource is not assigned. Please assign it in the Inspector.");
            return;
        }

        if (trackIndex >= 0 && trackIndex < musicTracks.Length)
        {
            MusicSource.Stop();
            MusicSource.clip = musicTracks[trackIndex];
            MusicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Invalid track index: {trackIndex}");
        }
    }
}
