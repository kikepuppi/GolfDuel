using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("General Sounds")]
    [SerializeField] private AudioClip shotSound;      // Som da tacada
    [SerializeField] private AudioClip holeSound;      // Som do buraco
    [SerializeField] private AudioClip collisionSound; // Som de colisão genérica

    [Header("Object Selection Sounds")]
    [SerializeField] private AudioClip cowSelectSound;
    [SerializeField] private AudioClip foxSelectSound;
    [SerializeField] private AudioClip treeSelectSound;
    [SerializeField] private AudioClip rockSelectSound;
    [SerializeField] private AudioClip windmillSelectSound;

    [Header("Object Collision Sounds")]
    [SerializeField] private AudioClip cowCollisionSound;
    [SerializeField] private AudioClip foxCollisionSound;
    [SerializeField] private AudioClip treeCollisionSound;
    [SerializeField] private AudioClip rockCollisionSound;
    [SerializeField] private AudioClip windmillCollisionSound;

    [Header("Audio Settings")]
    [SerializeField] private float shotVolume = 0.7f;
    [SerializeField] private float holeVolume = 1f;
    [SerializeField] private float collisionVolume = 0.5f;
    [SerializeField] private float selectVolume = 0.6f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // General Sounds
    public void PlayShotSound()
    {
        if (shotSound != null)
            PlaySound(shotSound, shotVolume);
    }

    public void PlayHoleSound()
    {
        if (holeSound != null)
            PlaySound(holeSound, holeVolume);
    }

    public void PlayCollisionSound()
    {
        if (collisionSound != null)
            PlaySound(collisionSound, collisionVolume);
    }

    // Selection Sounds
    public void PlayCowSelectSound()
    {
        if (cowSelectSound != null)
            PlaySound(cowSelectSound, selectVolume);
    }

    public void PlayFoxSelectSound()
    {
        if (foxSelectSound != null)
            PlaySound(foxSelectSound, selectVolume);
    }

    public void PlayTreeSelectSound()
    {
        if (treeSelectSound != null)
            PlaySound(treeSelectSound, selectVolume);
    }

    public void PlayRockSelectSound()
    {
        if (rockSelectSound != null)
            PlaySound(rockSelectSound, selectVolume);
    }

    public void PlayWindmillSelectSound()
    {
        if (windmillSelectSound != null)
            PlaySound(windmillSelectSound, selectVolume);
    }

    // Collision Sounds by Object Type
    public void PlayCowCollisionSound()
    {
        if (cowCollisionSound != null)
            PlaySound(cowCollisionSound, collisionVolume);
    }

    public void PlayFoxCollisionSound()
    {
        if (foxCollisionSound != null)
            PlaySound(foxCollisionSound, collisionVolume);
    }

    public void PlayTreeCollisionSound()
    {
        if (treeCollisionSound != null)
            PlaySound(treeCollisionSound, collisionVolume);
    }

    public void PlayRockCollisionSound()
    {
        if (rockCollisionSound != null)
            PlaySound(rockCollisionSound, collisionVolume);
    }

    public void PlayWindmillCollisionSound()
    {
        if (windmillCollisionSound != null)
            PlaySound(windmillCollisionSound, collisionVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        audioSource.PlayOneShot(clip, volume);
    }
}
