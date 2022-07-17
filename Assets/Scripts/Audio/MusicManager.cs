using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource ambientMusic;
    [SerializeField] private float ambientVolume = 1f;
    [SerializeField] private AudioSource combatMusic;
    [SerializeField] private float combatVolume = 1f;

    private IEnumerator crossfade;

    private void Awake()
    {
        ambientMusic.volume = ambientVolume;
        combatMusic.volume = combatVolume;
    }

    public void CrossfadeToCombat()
    {
        if (crossfade != null)
            StopCoroutine(crossfade);

        crossfade = Crossfade(combatMusic, combatVolume, ambientMusic, ambientVolume, 0.5f);
        StartCoroutine(crossfade);
    }

    public void CrossfadeToAmbient()
    {
        if (crossfade != null)
            StopCoroutine(crossfade);

        crossfade = Crossfade(ambientMusic, ambientVolume, combatMusic, combatVolume, 0.5f);
        StartCoroutine(crossfade);
    }


    private IEnumerator Crossfade(AudioSource to, float toVolume, AudioSource from, float fromVolume, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            to.volume = t * toVolume;
            from.volume = (1 - t) * fromVolume;
            yield return null;
        }
    }
}
