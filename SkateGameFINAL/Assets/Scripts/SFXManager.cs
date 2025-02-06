using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;  // Reference to the AudioSource component
    [SerializeField] private AudioClip[] audioClips;   // Serializable array of audio clips

    // Play a sound effect by index
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("SFX index out of range!");
        }
    }
}
