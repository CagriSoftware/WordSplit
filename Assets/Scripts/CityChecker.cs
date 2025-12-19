using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Globalization;

public class CityChecker : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerInput;
    [SerializeField] private LetterGenerator letterGenerator;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TimerController timerController;
    [SerializeField] private Animator feedbackAnimator;
    [SerializeField] private GameObject letterPanelContainer;

    private HashSet<string> validCities;
    private HashSet<string> usedCities = new HashSet<string>();
    private int score = 0;
    private CultureInfo trCulture = new CultureInfo("tr-TR");

    IEnumerator Start() 
    { 
        // Localization hazır olana kadar bekle
        yield return LocalizationSettings.InitializationOperation;
        RefreshCityList(); 
    }

    public void RefreshCityList()
    {
        validCities = new HashSet<string>();
        if (LocalizationSettings.SelectedLocale == null) return;

        string dil = LocalizationSettings.SelectedLocale.Identifier.Code;
        string dosya = dil.Contains("tr") ? "worldcities" : "worldcities_en";
        
        TextAsset asset = Resources.Load<TextAsset>(dosya);
        if (asset != null)
        {
            CultureInfo culture = dil.Contains("tr") ? trCulture : CultureInfo.InvariantCulture;
            string[] lines = asset.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in lines) validCities.Add(c.Trim().ToUpper(culture));
        }
        else
        {
            Debug.LogError($"Resources klasöründe '{dosya}' dosyası bulunamadı!");
        }
    }

    public void CheckCity()
    {
        // GÜVENLİK KONTROLÜ: NullReferenceException'ı önlemek için
        if (validCities == null || letterGenerator == null || feedbackAnimator == null)
        {
            Debug.LogError("CityChecker: Gerekli referanslar eksik! Inspector'ı kontrol edin.");
            return;
        }

        string dil = LocalizationSettings.SelectedLocale.Identifier.Code;
        CultureInfo culture = dil.Contains("tr") ? trCulture : CultureInfo.InvariantCulture;

        string input = playerInput.text.Trim().ToUpper(culture);
        string generatedLetters = letterGenerator.GetLastGeneratedLetters();

        if (string.IsNullOrEmpty(generatedLetters)) 
        {
            Debug.LogWarning("Henüz harf üretilmedi.");
            return;
        }

        string letters = generatedLetters.ToUpper(culture);

        bool success = validCities.Contains(input) && !usedCities.Contains(input) && CityContainLetters(input, letters);
        
        if (success) { score += 15; usedCities.Add(input); }
        else { score -= 10; }

        if (scoreText != null) scoreText.text = score.ToString();
        StartCoroutine(AnimateResult(success));
    }

    private bool CityContainLetters(string city, string letters)
    {
        foreach (char l in letters) if (!city.Contains(l.ToString())) return false;
        return true;
    }

    private IEnumerator AnimateResult(bool ok)
    {
        if (letterPanelContainer != null) letterPanelContainer.SetActive(false);
        playerInput.text = "";
        
        if (feedbackAnimator != null) feedbackAnimator.SetTrigger(ok ? "Correct" : "Incorrect");
        
        yield return new WaitForSeconds(1.2f);
        
        if (letterPanelContainer != null) letterPanelContainer.SetActive(true);
        if (letterGenerator != null) letterGenerator.GenerateRandomLetters();
        if (timerController != null) timerController.StartNewTour();
    }

    public void HandleTimeOut() 
    { 
        score -= 10; 
        if (scoreText != null) scoreText.text = score.ToString(); 
        StartCoroutine(AnimateResult(false)); 
    }
}