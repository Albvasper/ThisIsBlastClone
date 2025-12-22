using UnityEngine;

/// <summary>
/// Handles playing sound effects
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Play a sound effect.
    /// </summary>
    /// <param name="clip">Sound effect.</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) 
            return;

        sfxSource.PlayOneShot(clip);
    }
}
