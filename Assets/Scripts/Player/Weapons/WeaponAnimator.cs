using System;
using System.Collections;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private float reloadAngle;
    [SerializeField] private float reloadTransitionInDuration;
    [SerializeField] private float reloadTransitionOutDuration;
    [SerializeField] private float swapAngle;
    [SerializeField] private float equipTransitionDuration;
    [SerializeField] private float unequipTransitionDuration;

    private IEnumerator currentAnimation;

    #region RELOAD
    public void StartReload(float duration, Action onComplete)
    {
        PlayAnimation(Reload(duration, onComplete));
    }

    IEnumerator reload;
    private IEnumerator Reload(float duration, Action onReloadFinished)
    {
        yield return StartCoroutine(TransitionTo(
            reloadTransitionInDuration, 
            transform.localEulerAngles.x, 
            reloadAngle,
            EaseInQuad));

        yield return new WaitForSeconds(duration);
        onReloadFinished?.Invoke();

        yield return StartCoroutine(TransitionTo(
            reloadTransitionOutDuration,
            transform.localEulerAngles.x, 
            0f,
            EaseOutQuint));
    }
    #endregion
    #region EQUIP
    public void StartEquip(Action onComplete)
    {
        PlayAnimation(Equip(onComplete));
    }

    IEnumerator equip;
    private IEnumerator Equip(Action onComplete)
    {
        yield return StartCoroutine(TransitionTo(
            equipTransitionDuration,
            swapAngle,
            0f,
            EaseOutQuint));

        onComplete?.Invoke();
    }
    #endregion
    #region UNEQUIP
    public void StartUnequip(Action onComplete)
    {
        PlayAnimation(Unequip(onComplete));
    }

    IEnumerator unequip;
    private IEnumerator Unequip(Action onComplete)
    {
        yield return StartCoroutine(TransitionTo(
               unequipTransitionDuration,
               transform.localEulerAngles.x,
               swapAngle,
               EaseInQuad));

        onComplete?.Invoke();
    }
    #endregion

    private IEnumerator TransitionTo(float duration, float from, float to, Func<float, float> ease)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            UpdateRotation(Mathf.Lerp(from, to, ease(t)));
            yield return null;
        }
    }

    private void PlayAnimation(IEnumerator animation)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = animation;
        StartCoroutine(currentAnimation);
    }

    #region EASINGS
    private float EaseOutQuint(float t) => 1 - Mathf.Pow(1 - t, 5f);
    private float EaseInQuad(float t) => t * t;
    #endregion

    private void UpdateRotation(float newX) => transform.localEulerAngles = new Vector3(newX, 0f, 0f);
}
