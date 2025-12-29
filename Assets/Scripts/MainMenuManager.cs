using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject questPanel; 
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private GameObject mainCanvasRoot;

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        var currentLocale = LocalizationSettings.SelectedLocale;
        LocalizationSettings.SelectedLocale = currentLocale;

        yield return null;

        if (modeSelectionPanel != null) modeSelectionPanel.SetActive(false);
        if (questPanel != null) questPanel.SetActive(false);
    }

    public void ChangeLanguage(int localeID)
    {
        StopAllCoroutines();
        StartCoroutine(SetLocale(localeID));
    }

   private IEnumerator SetLocale(int _localeID)
{
    yield return LocalizationSettings.InitializationOperation;
    
    var targetLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
    LocalizationSettings.SelectedLocale = targetLocale;

    while (!LocalizationSettings.SelectedLocale.Identifier.Equals(targetLocale.Identifier))
    {
        yield return null;
    }

    yield return new WaitForSecondsRealtime(0.2f);

    if (mainCanvasRoot != null)
    {
        mainCanvasRoot.SetActive(false);
        yield return new WaitForEndOfFrame();
        mainCanvasRoot.SetActive(true);
    }
    else if (modeSelectionPanel != null && modeSelectionPanel.transform.parent != null)
    {
        GameObject root = modeSelectionPanel.transform.parent.gameObject;
        root.SetActive(false);
        yield return new WaitForEndOfFrame();
        root.SetActive(true);
    }

}

    public void SetActiveLetters(int count)
    {
        PlayerPrefs.SetInt("ActiveLetters", count);
        PlayerPrefs.Save();

    }

    public void StartGame() 
    { 
        OpenModeSelectionPanel(); 
    }

    public void OpenModeSelectionPanel()
    {
        if (modeSelectionPanel != null)
        {
            if (questPanel != null) questPanel.SetActive(false);
            modeSelectionPanel.SetActive(true);
            StartCoroutine(RefreshPanelLocalization());
        }
    }

    private IEnumerator RefreshPanelLocalization()
    {
        yield return null;
        LocalizationSettings.SelectedLocale = LocalizationSettings.SelectedLocale;
    }

    public void CloseModeSelectionPanel()
    {
        if (modeSelectionPanel != null)
            modeSelectionPanel.SetActive(false);
    }

    public void ToggleQuestPanel()
    {
        if (questPanel != null) 
            questPanel.SetActive(!questPanel.activeSelf); 
    }

    public void BackToMenu() 
    { 
        SceneManager.LoadScene("MenuScene"); 
    }

    public void QuitGame() 
    { 
        Application.Quit(); 
    }
    
    public void StartWordGame() { SaveAndLoad(GameConstants.MODE_WORD); }
    public void StartCityGame() { SaveAndLoad(GameConstants.MODE_CITY); }

    private void SaveAndLoad(int mode)
    {
        PlayerPrefs.SetInt(GameConstants.GAME_MODE_KEY, mode);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
}