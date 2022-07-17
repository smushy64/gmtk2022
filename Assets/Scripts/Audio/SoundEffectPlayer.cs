using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    AudioSource audioSource;

    public bool destroyOnFinish = true;
    public List<AudioClip> clips;
    public bool looping = false;
    float lengthOfCurrentClip = 0f;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play() {
        audioSource.clip = clips[UnityEngine.Random.Range(0, clips.Count)];
        lengthOfCurrentClip = audioSource.clip.length;
        audioSource.loop = looping;
        audioSource.Play();
        if( destroyOnFinish ) {
            this.StartCoroutine(LifetimeCounter());
        }
    }

    public void Stop() {
        audioSource.Stop();
    }

    IEnumerator LifetimeCounter() {
        yield return new WaitForSeconds(lengthOfCurrentClip);
        Destroy(gameObject);
    }

}
