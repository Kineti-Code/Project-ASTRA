using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.ScrollRect;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource playerWalkingSource;
    [SerializeField] private AudioSource extraPlayerSFXSource;

    [Header("Player Movement - Stone")]
    public AudioClip stoneWalking;
    public AudioClip stoneJumping;
    public AudioClip stoneSliding;

    [Header("Player Movement - Metal")]
    public AudioClip metalWalking;
    public AudioClip metalJumping;
    public AudioClip metalSliding;

    [Header("Player Movement - Soil")]
    public AudioClip soilWalking;
    public AudioClip soilJumping;
    public AudioClip soilSliding;

    [Header("Player Movement - Grass")]
    public AudioClip grassWalking;
    public AudioClip grassJumping;
    public AudioClip grassSliding;

    [Header("Player SFX")]
    public AudioClip[] collectArtifact;

    [Header("Level Select")]
    public AudioClip buttonClick;
    public AudioClip lockedButtonClick;
    public AudioClip levelSelectAmbience;

    [Header("Main Menu")]
    public AudioClip mainMenuMusic;
    public AudioClip mainMenuAmbience;
    public AudioClip trapdoorHydraulics;
    public AudioClip tubeLightOn;

    [Header("Terus1")]
    public AudioClip terusBgMusic;
    public AudioClip terusAmbience;
    public AudioClip flashlightToggle;

    [Header("Scopulosus53")]
    public AudioClip scopulosusBgMusic;
    public AudioClip scopulosusAmbience;

    [Header("Magnus25")]
    public AudioClip magnusBgMusic;
    public AudioClip magnusAmbience;
    public AudioClip impendingStorm;

    public enum SurfaceType
    {
        Normal,
        Stone,
        Metal,
        Soil,
        Ladder,
        Hook,
        Grass
    }
    public enum MovementType
    {
        Walking,
        Jumping,
        Sliding,
        Climbing
    }

    private SurfaceType currentSurface = SurfaceType.Normal;
    Dictionary<(SurfaceType, MovementType), AudioClip> movementSounds = new Dictionary<(SurfaceType, MovementType), AudioClip>();
    private float startVolume;

    void Start()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        if (buildIndex == 0 || buildIndex == 3)
        {
            ambienceSource.clip = mainMenuAmbience;
            ambienceSource.Play();
        }

        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            ambienceSource.clip = levelSelectAmbience;
            ambienceSource.Play();
        }

        startVolume = playerWalkingSource.volume; // change later to use playerprefs
        initializePlayerSounds();
        LoadVolumes();
    }

    private void initializePlayerSounds()
    {
        movementSounds.Add((SurfaceType.Stone, MovementType.Walking), stoneWalking);
        movementSounds.Add((SurfaceType.Stone, MovementType.Jumping), stoneJumping);
        movementSounds.Add((SurfaceType.Stone, MovementType.Sliding), stoneSliding);

        movementSounds.Add((SurfaceType.Normal, MovementType.Walking), stoneWalking);
        movementSounds.Add((SurfaceType.Normal, MovementType.Jumping), stoneJumping);
        movementSounds.Add((SurfaceType.Normal, MovementType.Sliding), stoneSliding);

        movementSounds.Add((SurfaceType.Metal, MovementType.Walking), metalWalking);
        movementSounds.Add((SurfaceType.Metal, MovementType.Jumping), metalJumping);
        movementSounds.Add((SurfaceType.Metal, MovementType.Sliding), metalSliding);

        movementSounds.Add((SurfaceType.Soil, MovementType.Walking), soilWalking);
        movementSounds.Add((SurfaceType.Soil, MovementType.Jumping), soilJumping);
        movementSounds.Add((SurfaceType.Soil, MovementType.Sliding), soilSliding);

        movementSounds.Add((SurfaceType.Grass, MovementType.Walking), grassWalking);
        movementSounds.Add((SurfaceType.Grass, MovementType.Jumping), grassJumping);
        movementSounds.Add((SurfaceType.Grass, MovementType.Sliding), grassSliding);

        movementSounds.Add((SurfaceType.Ladder, MovementType.Climbing), stoneWalking);
        movementSounds.Add((SurfaceType.Hook, MovementType.Climbing), stoneWalking);
    }

    public void PlayMovementSound(string groundType, MovementType movement)
    {
        if (Enum.TryParse(groundType, true, out SurfaceType surface))
        {
            if (movementSounds.TryGetValue((surface, movement), out AudioClip clip))
            {
                if (clip == null)
                {
                    Debug.LogError($"Missing audio clip for {surface} - {movement}!");
                    return;
                }

                else
                {
                    currentSurface = surface;
                    if (movement == MovementType.Walking) { PlayPlayerMovement(clip); }
                    if (movement == MovementType.Jumping || movement == MovementType.Sliding) { PlayPlayerSFX(clip); }
                }
            }
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayPlayerMovement(AudioClip clip)
    {
        if (playerWalkingSource.clip != clip || !playerWalkingSource.isPlaying)
        {
            playerWalkingSource.clip = clip;
            playerWalkingSource.loop = true;

            if (!playerWalkingSource.isPlaying)
            {
                playerWalkingSource.volume = 0f;
                playerWalkingSource.mute = false;
                playerWalkingSource.Play(); // **Ensure playback starts**
                StartCoroutine(FadeIn(playerWalkingSource, 0.3f));
            }
        }
    }


    public void CrossfadeFootstepSound(string newGroundType, MovementType movement, bool isAirborne)
    {
        if (isAirborne) // If player is in the air, stop sound completely
        {
            if (playerWalkingSource.isPlaying) // Prevent unnecessary stops
            {
                StartCoroutine(FadeOut(playerWalkingSource, 0.3f)); // Fade out quickly
            }
            return; // Exit early
        }

        if (Enum.TryParse(newGroundType, true, out SurfaceType newSurface))
        {
            if (newSurface != currentSurface) // Only transition if the surface changes
            {
                StartCoroutine(CrossfadeAudio(newSurface, movement, 0.3f)); // Smooth transition
                currentSurface = newSurface; // Update surface type
            }
        }
    }
    private IEnumerator CrossfadeAudio(SurfaceType newSurface, MovementType movement, float duration)
    {
        float currentVolume = playerWalkingSource.volume; // Capture current volume before fading out

        // Fade out old sound only if it’s different from the new one
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            playerWalkingSource.volume = Mathf.Lerp(currentVolume, 0, t / duration); // Use `currentVolume`, not `startVolume`
            yield return null;
        }

        // Change to new surface sound and fade in only if it’s different
        if (movementSounds.TryGetValue((newSurface, movement), out AudioClip newClip) && playerWalkingSource.clip != newClip)
        {
            playerWalkingSource.clip = newClip;
            playerWalkingSource.mute = false;
            playerWalkingSource.Play();

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                playerWalkingSource.volume = Mathf.Lerp(0, startVolume, t / duration); // Restore back to `startVolume`
                yield return null;
            }
        }
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        float currentVolume = source.volume; // Capture current volume before fading out

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(currentVolume, 0, t / duration); // Use `currentVolume`
            yield return null;
        }

        source.Stop();
        source.volume = startVolume; // Ensure volume resets for next playback
    }

    private IEnumerator FadeIn(AudioSource source, float duration)
    {
        float targetVolume = startVolume;

        if (!source.isPlaying) // If the audio stopped, restart it
        {
            source.Play();
        }

        source.volume = 0f; // Ensure volume starts from zero

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0, targetVolume, t / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void PlayPlayerSFX(AudioClip clip)
    {
        playerWalkingSource.PlayOneShot(clip);
    }

    public void AdjustMusicVolume(float masterVolume, float[] volumes)
    {
        musicSource.volume = volumes[0] * masterVolume;
        ambienceSource.volume = volumes[1] * masterVolume;
        sfxSource.volume = volumes[2] * masterVolume;

        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", volumes[0]);
        PlayerPrefs.SetFloat("AmbienceVolume", volumes[1]);
        PlayerPrefs.SetFloat("SFXVolume", volumes[2]);
    }

    private void LoadVolumes()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSource.volume = musicVolume * masterVolume;
        ambienceSource.volume = ambienceVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
    }
}
