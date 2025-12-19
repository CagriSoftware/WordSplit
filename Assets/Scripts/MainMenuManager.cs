using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject questPanel; 
    [SerializeField] private GameObject modeSelectionPanel;

    private IEnumerator Start()
{
    // 1. Önce Localization sisteminin uyanmasını bekle
    yield return LocalizationSettings.InitializationOperation;

    // 2. Sistem uyandıktan sonra seçili dili zorla uygula 
    // (Bazen sistem varsayılan dili hemen objelere basamaz)
    var currentLocale = LocalizationSettings.SelectedLocale;
    LocalizationSettings.SelectedLocale = currentLocale;

    // 3. Bir frame daha bekle ki objeler dili kavrasın
    yield return null;

    // 4. Şimdi panelleri kapatabilirsin
    if (modeSelectionPanel != null) modeSelectionPanel.SetActive(false);
    if (questPanel != null) questPanel.SetActive(false);
}

    // --- Dil Değiştirme Sistemi ---
    public void ChangeLanguage(int localeID)
    {
        // Eğer zaten bir dil değişimi süreci varsa durdur ve yenisini başlat
        StopAllCoroutines();
        StartCoroutine(SetLocale(localeID));
    }

    private IEnumerator SetLocale(int _localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        
        if (_localeID < LocalizationSettings.AvailableLocales.Locales.Count)
        {
            // Yeni dili uygula
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];

            // Asset Table'lar bazen asenkron yüklendiği için 
            // değişikliğin tüm objelere yayılmasını beklemek iyi bir pratiktir.
            yield return null; 

            Debug.Log("Dil Değiştirildi: " + LocalizationSettings.SelectedLocale.Identifier.Code);
        }
    }

    // --- Panel Yönetimi ---
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

            // Panel açıldığında Asset'lerin yenilenmeme ihtimaline karşı 
            // küçük bir 'yenileme' tetiklemesi yapıyoruz.
            StartCoroutine(RefreshPanelLocalization());
        }
    }

    private IEnumerator RefreshPanelLocalization()
    {
        // Panelin aktifleşmesi için 1 frame bekle
        yield return null;
        // Localization sistemini mevcut dille tekrar "dürt"
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

    // --- Sahne ve Uygulama Akışı ---
    public void BackToMenu() 
    { 
        SceneManager.LoadScene("MenuScene"); 
    }

    public void QuitGame() 
    { 
        Application.Quit(); 
    }
    
    // --- Oyun Modu Seçimleri ---
    public void StartWordGame() { SaveAndLoad(GameConstants.MODE_WORD); }
    public void StartCityGame() { SaveAndLoad(GameConstants.MODE_CITY); }

    private void SaveAndLoad(int mode)
    {
        PlayerPrefs.SetInt(GameConstants.GAME_MODE_KEY, mode);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
}