using UnityEngine;
using UnityEngine.Audio;

public static class GameOptions {
    static Options options;
    public static Options PlayerOptions { get{
        if( !optionsLoaded ) {
            LoadOptions();
        }
        return options;
    } }
    static bool optionsLoaded = false;

    public static void ApplyVolume() {
        mixer.SetFloat("Master", 20f * Mathf.Log10(Mathf.Max(PlayerOptions.masterVolume, 0.0001f)));
        mixer.SetFloat("Music", 20f * Mathf.Log10( Mathf.Max(PlayerOptions.musicVolume, 0.0001f)));
        mixer.SetFloat("SFX", 20f * Mathf.Log10(   Mathf.Max(PlayerOptions.sfxVolume, 0.0001f)));
    }
    public static void SetMasterVolume(float v) {
        options.masterVolume = v;
        ApplyVolume();
    }
    public static void SetMusicVolume(float v) {
        options.musicVolume = v;
        ApplyVolume();
    }
    public static void SetSFXVolume(float v) {
        options.sfxVolume = v;
        ApplyVolume();
    }
    public static void SetFullscreenMode(int fullscreenMode) =>
        options.fullscreenMode = (FullScreenMode)fullscreenMode;
    public static void SetResolution(ScreenResolution resolution) =>
        options.resolution = resolution;
    public static void SetVsync(int v) => options.vsync = v;
    public static void SetInvertCameraX(bool invert) => options.invertCameraX = invert;
    public static void SetInvertCameraY(bool invert) => options.invertCameraY = invert;
    public static void SetCameraSensitivityX(float sensitivity) =>
        options.cameraSensitivityX = sensitivity;
    public static void SetCameraSensitivityY(float sensitivity) =>
        options.cameraSensitivityY = sensitivity;
    public static void SetSkipSplash(bool skip) => options.skipSplash = skip;

    public static void ApplyScreenOptions() {
        QualitySettings.vSyncCount = PlayerOptions.vsync;
        Vector2Int resolution = ScreenResolutionToV2Int( PlayerOptions.resolution );
        Screen.SetResolution(resolution.x, resolution.y, PlayerOptions.fullscreenMode);
    }

    static void LoadOptions() {
        options.masterVolume       = PlayerPrefs.GetFloat("masterVolume", 1.0f);
        options.musicVolume        = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        options.sfxVolume          = PlayerPrefs.GetFloat("sfxVolume", 0.6f);
        options.cameraSensitivityX = PlayerPrefs.GetFloat("cameraSensitivityX", 0.5f);
        options.cameraSensitivityY = PlayerPrefs.GetFloat("cameraSensitivityY", 0.5f);
        options.fullscreenMode     = (FullScreenMode)PlayerPrefs.GetInt("fullscreenMode", 2);
        options.resolution         = (ScreenResolution)PlayerPrefs.GetInt("resolution", 2);
        options.vsync              = PlayerPrefs.GetInt("vsync", 0);
        options.invertCameraX      = PlayerPrefs.GetInt("invertCameraX", 0) != 0;
        options.invertCameraY      = PlayerPrefs.GetInt("invertCameraY", 0) != 0;
        options.skipSplash         = PlayerPrefs.GetInt("skipSplash", 0) != 0;

        optionsLoaded = true;
    }

    public static void SaveOptions() {
        PlayerPrefs.SetFloat("masterVolume",       options.masterVolume );
        PlayerPrefs.SetFloat("musicVolume",        options.musicVolume );
        PlayerPrefs.SetFloat("sfxVolume",          options.sfxVolume );
        PlayerPrefs.SetFloat("cameraSensitivityX", options.cameraSensitivityX );
        PlayerPrefs.SetFloat("cameraSensitivityY", options.cameraSensitivityY );
        PlayerPrefs.SetInt  ("fullscreenMode",     (int)options.fullscreenMode );
        PlayerPrefs.SetInt  ("resolution",         (int)options.resolution );
        PlayerPrefs.SetInt  ("vsync",              options.vsync );
        PlayerPrefs.SetInt  ("invertCameraX",      options.invertCameraX ? 1 : 0);
        PlayerPrefs.SetInt  ("invertCameraY",      options.invertCameraY ? 1 : 0);
        PlayerPrefs.SetInt  ("skipSplash",         options.skipSplash    ? 1 : 0);
        PlayerPrefs.Save();
    }

    static AudioMixer mixer {
        get {
            if( _mixer == null ) {
                _mixer = Resources.Load<AudioMixer>("Mixer");
            }
            return _mixer;
        }
    }
    static AudioMixer _mixer;

    public static Vector2Int ScreenResolutionToV2Int( ScreenResolution resolution ) {
        switch(resolution) {
            case ScreenResolution.r1920x1080:
                return new Vector2Int(1920, 1080);
            case ScreenResolution.r1600x900:
                return new Vector2Int(1600, 900);
            default: case ScreenResolution.r1280x720:
                return new Vector2Int(1280, 720);
        }
    }

}
public struct Options {
    public float masterVolume, musicVolume, sfxVolume;
    public float cameraSensitivityX, cameraSensitivityY;
    public FullScreenMode fullscreenMode;
    public int vsync;
    public bool invertCameraX, invertCameraY, skipSplash;
    public ScreenResolution resolution;
}
public enum ScreenResolution {
    r1920x1080,
    r1600x900,
    r1280x720,
}
