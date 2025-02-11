using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioSource audioSource;  // Reference to the AudioSource component  
    public AudioClip backgroundMusic; // Reference to the AudioClip to play

    void Start()
    {
        // Assign the clip and set it to loop
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        
        // Play the audio
        audioSource.Play();
    }
}
