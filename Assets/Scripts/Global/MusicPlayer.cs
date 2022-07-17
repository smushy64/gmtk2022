using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }
    AudioSource audioSource;
    private void Awake()
    {
        if( Instance != null && Instance != this ) {
            Destroy(this);
        } else {
            Instance = this;
        }

        audioSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(this);
    }

    public void PlayClip( AudioClip clip, bool loop ) {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.Play();
    }

    public void PlayClip(AudioClip clip) {
        PlayClip(clip, true);
    }

    public void Stop() {
        audioSource.Stop();
    }
}
