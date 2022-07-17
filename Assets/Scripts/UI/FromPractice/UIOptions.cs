using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIOptions : MonoBehaviour
{
    [SerializeField]
    UISlider masterSlider, musicSlider, sfxSlider, cameraSensitivityXSlider, cameraSensitivityYSlider;
    [SerializeField]
    TMP_Dropdown fullscreenModeDropdown, resolutionDropdown, vsyncDropdown;
    [SerializeField]
    UIToggle invertCameraX, invertCameraY, skipSplashScreen;
    [SerializeField]
    Scrollbar scrollbar;

    UISlidingPanel panel;
    UIPopUp popUp;
    UIMainMenu mainMenu;

    private void Start() {
        mainMenu = FindObjectOfType<UIMainMenu>();
        panel = GetComponent<UISlidingPanel>();
        panel.OnPanelSlide += OnPanelSlide;
        popUp = FindObjectOfType<UIPopUp>();

        masterSlider.SliderComponent.onValueChanged.AddListener( GameOptions.SetMasterVolume );
        musicSlider.SliderComponent.onValueChanged.AddListener( GameOptions.SetMusicVolume );
        sfxSlider.SliderComponent.onValueChanged.AddListener( GameOptions.SetSFXVolume );

        cameraSensitivityXSlider.SliderComponent.onValueChanged.AddListener(GameOptions.SetCameraSensitivityX);
        cameraSensitivityYSlider.SliderComponent.onValueChanged.AddListener(GameOptions.SetCameraSensitivityY);

        fullscreenModeDropdown?.onValueChanged.AddListener(ChangeFullscreenMode);
        resolutionDropdown?.onValueChanged.AddListener(ChangeResolution);
        vsyncDropdown.onValueChanged.AddListener(SetVsync);

        invertCameraX.OnToggle.AddListener(GameOptions.SetInvertCameraX);
        invertCameraY.OnToggle.AddListener(GameOptions.SetInvertCameraY);
        skipSplashScreen.OnToggle.AddListener(GameOptions.SetSkipSplash);
        UpdateOptionsDisplay();
    }

    FullScreenMode DropDownToFullscreenMode( int mode ) {
        switch(mode) {
            case 0:
                return FullScreenMode.ExclusiveFullScreen;
            case 1:
                return FullScreenMode.FullScreenWindow;
            default: case 2:
                return FullScreenMode.Windowed;
        }
    }

    FullScreenMode lastFullscreenMode;
    FullScreenMode chosenFullscreenMode;
    void ChangeFullscreenMode(int mode) {
        lastFullscreenMode = GameOptions.PlayerOptions.fullscreenMode;
        chosenFullscreenMode = DropDownToFullscreenMode(mode);

        Screen.fullScreenMode = chosenFullscreenMode;

        popUp.CreatePopup(
            fullscreenModeDropdown?.gameObject,
            "Would you like to keep this screen mode?\nReverting back in 5s",
            "Revert",
            "Keep",
            delegate { RevertFullscreenModeAndDismiss(); },
            delegate { KeepFullscreenMode(); },
            5f,
            delegate { RevertFullscreenMode(); }
        );
        PopUpOpened();
    }
    public void RevertFullscreenMode() {
        Screen.fullScreenMode = lastFullscreenMode;
        UpdateOptionsDisplay();
        PopUpClosed();
    }
    public void RevertFullscreenModeAndDismiss() {
        Screen.fullScreenMode = lastFullscreenMode;
        UpdateOptionsDisplay();
        popUp.DismissPopup();
        PopUpClosed();
    }
    public void KeepFullscreenMode() {
        popUp.DismissPopup();
        GameOptions.SetFullscreenMode((int)chosenFullscreenMode);
        PopUpClosed();
    }

    ScreenResolution lastResolution;
    ScreenResolution chosenResolution;
    void ChangeResolution(int res) {
        lastResolution = GameOptions.PlayerOptions.resolution;
        chosenResolution = (ScreenResolution)res;
        Vector2Int resolution = GameOptions.ScreenResolutionToV2Int(chosenResolution);

        Screen.SetResolution(resolution.x, resolution.y, GameOptions.PlayerOptions.fullscreenMode);

        popUp.CreatePopup(
            resolutionDropdown.gameObject,
            "Would you like to keep this resolution?\nReverting back in 5s",
            "Revert",
            "Keep",
            delegate { RevertResolutionAndDismiss(); },
            delegate { KeepResolution(); },
            5f,
            delegate { RevertResolution(); }
        );
        PopUpOpened();
    }
    public void RevertResolution() {
        Vector2Int resolution = GameOptions.ScreenResolutionToV2Int(lastResolution);
        Screen.SetResolution(resolution.x, resolution.y, GameOptions.PlayerOptions.fullscreenMode);
        UpdateOptionsDisplay();
        PopUpClosed();
    }
    public void RevertResolutionAndDismiss() {
        Vector2Int resolution = GameOptions.ScreenResolutionToV2Int(lastResolution);
        Screen.SetResolution(resolution.x, resolution.y, GameOptions.PlayerOptions.fullscreenMode);
        UpdateOptionsDisplay();
        popUp.DismissPopup();
        PopUpClosed();
    }
    public void KeepResolution() {
        popUp.DismissPopup();
        GameOptions.SetResolution(chosenResolution);
        PopUpClosed();
    }

    void PopUpOpened() {
        DisableInteraction();
        mainMenu?.DisableInteraction();
    }

    void PopUpClosed() {
        EnableInteraction();
        mainMenu?.EnableInteraction();
    }

    public void EnableInteraction() {
        masterSlider.SetInteractable            (true);
        musicSlider.SetInteractable             (true);
        sfxSlider.SetInteractable               (true);
        cameraSensitivityXSlider.SetInteractable(true);
        cameraSensitivityYSlider.SetInteractable(true);
        invertCameraX.SetInteractable           (true);
        invertCameraY.SetInteractable           (true);
        skipSplashScreen.SetInteractable        (true);
        if( fullscreenModeDropdown != null )
            fullscreenModeDropdown.interactable    = true;
        if( resolutionDropdown != null )
            resolutionDropdown.interactable        = true;
        vsyncDropdown.interactable             = true;
        scrollbar.interactable                 = true;
    }

    public void DisableInteraction() {
        masterSlider.SetInteractable            (false);
        musicSlider.SetInteractable             (false);
        sfxSlider.SetInteractable               (false);
        cameraSensitivityXSlider.SetInteractable(false);
        cameraSensitivityYSlider.SetInteractable(false);
        invertCameraX.SetInteractable           (false);
        invertCameraY.SetInteractable           (false);
        skipSplashScreen.SetInteractable        (false);
        if( fullscreenModeDropdown != null )
            fullscreenModeDropdown.interactable    = false;
        if( resolutionDropdown != null )
            resolutionDropdown.interactable        = false;
        vsyncDropdown.interactable             = false;
        scrollbar.interactable                 = false;
    }

    void SetVsync( int vsync ) {
        GameOptions.SetVsync(vsync);
        QualitySettings.vSyncCount = vsync;
    }

    private void OnDestroy() {
        panel.OnPanelSlide -= OnPanelSlide;
    }

    void OnPanelSlide(bool slideIn) {
        if( slideIn ) {
            UpdateOptionsDisplay();
            EnableInteraction();
        }
        else {
            GameOptions.SaveOptions();
            DisableInteraction();
        }
    }

    void UpdateOptionsDisplay() {
        Options o = GameOptions.PlayerOptions;
        masterSlider.SetValue( o.masterVolume );
        musicSlider.SetValue( o.musicVolume );
        sfxSlider.SetValue( o.sfxVolume );
        cameraSensitivityXSlider.SetValue( o.cameraSensitivityX );
        cameraSensitivityYSlider.SetValue( o.cameraSensitivityY );

        if( fullscreenModeDropdown != null )
            fullscreenModeDropdown.SetValueWithoutNotify((int)o.fullscreenMode);
        if( resolutionDropdown != null )
            resolutionDropdown.SetValueWithoutNotify((int)o.resolution);
        vsyncDropdown.SetValueWithoutNotify((int)o.vsync);
        invertCameraX.Set(o.invertCameraX);
        invertCameraY.Set(o.invertCameraY);
        skipSplashScreen.Set(o.skipSplash);
    }

}
