using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField]
    Image dng, gmtk;

    [SerializeField, Range(0f, 1f)]
    float startSize, endSize;

    [SerializeField, Min(0.01f)]
    float dngAnimationLength = 1f, gmtkAnimationLength = 1f;

    void Start() {
        Initialize();
        if( animate != null ) {
            this.StopCoroutine(animate);
        }
        animate = Animate();
        this.StartCoroutine(animate);
    }

    void Initialize() {
        GameOptions.ApplyScreenOptions();
        if( GameOptions.PlayerOptions.skipSplash ) {
            LoadMainMenu();
        }
    }

    void LoadMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator animate;
    IEnumerator Animate() {
        float fadeThreshold = 0.8f;
        float timer = 0f;
        float fadeTimer = 0f;
        float fadeLength = dngAnimationLength - Mathf.Lerp(0f, dngAnimationLength, fadeThreshold);
        dng.rectTransform.localScale = Vector3.one * startSize;
        gmtk.rectTransform.localScale = Vector3.one * startSize;
        while( timer < dngAnimationLength ) {
            float t = timer / dngAnimationLength;
            dng.rectTransform.localScale = Vector3.Lerp(
                Vector3.one * startSize,
                Vector3.one * endSize,
                t
            );

            if( t >= fadeThreshold ) {
                dng.color = Color.Lerp(
                    Color.white,
                    Color.clear,
                    fadeTimer / fadeLength
                );
                gmtk.color = Color.Lerp(
                    Color.clear,
                    Color.white,
                    fadeTimer / fadeLength
                );
                fadeTimer += Time.unscaledDeltaTime;
            }
            timer += Time.unscaledDeltaTime;

            yield return null;
        }
        dng.rectTransform.localScale = Vector3.one * endSize;
        dng.color = Color.clear;
        gmtk.color = Color.white;

        timer = 0f;
        fadeTimer = 0f;
        fadeLength = gmtkAnimationLength - Mathf.Lerp(0f, gmtkAnimationLength, fadeThreshold);
        while( timer < gmtkAnimationLength ) {
            float t = timer / gmtkAnimationLength;
            gmtk.rectTransform.localScale = Vector3.Lerp(
                Vector3.one * startSize,
                Vector3.one * endSize,
                t
            );
            if( t >= fadeThreshold ) {
                gmtk.color = Color.Lerp(
                    Color.white,
                    Color.clear,
                    fadeTimer / fadeLength
                );
                fadeTimer += Time.unscaledDeltaTime;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        gmtk.rectTransform.localScale = Vector3.one * endSize;
        gmtk.color = Color.clear;
        LoadMainMenu();
    }
    
}
