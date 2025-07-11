using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource sfxObject;

    private List<AudioSource> activeAudioSources = new List<AudioSource>();

    private void Awake()
    {
        instance = this;
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, float minDistanceToSameClip = 0.1f)
    {
        // Check for duplicate nearby clips
        foreach (var source in activeAudioSources)
        {
            if (source != null && source.clip == audioClip && source.isPlaying)
            {
                float distance = Vector3.Distance(source.transform.position, spawnTransform.position);
                if (distance < minDistanceToSameClip)
                {
                    return; // Skip playing the same clip too close
                }
            }
        }

        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        //Destroy(audioSource, clipLength);

        activeAudioSources.Add(audioSource);

        StartCoroutine(RemoveSourceAfterDuration(audioSource, audioClip.length));
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(sfxObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;
        Destroy(audioSource, clipLength);
    }

    private IEnumerator RemoveSourceAfterDuration(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeAudioSources.Remove(source);
        if (source != null)
        {
            Destroy(source.gameObject);
        }
    }
}
