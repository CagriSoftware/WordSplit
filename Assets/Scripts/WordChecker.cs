using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Globalization;

public class WordChecker : MonoBehaviour
{
    [Header("UI & Controllers")]
    [SerializeField] private TMP_InputField playerInput;
    [SerializeField] private TMP_Text randomLettersText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private LetterGenerator letterGenerator;
    [SerializeField] private TimerController timerController;

    [Header("Feedback Settings")]
    [SerializeField] private Animator feedbackAnimator;
    [SerializeField] private GameObject letterPanelContainer;
    [SerializeField] private float animationDuration = 1.2f;

    private HashSet<string> validWords;
    private int score = 0;
    private CultureInfo trCulture = new CultureInfo("tr-TR");

    private IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;
        RefreshWordList();
        UpdateScoreDisplay();
        
        // TEST İÇİN: Oyun başladığında eşik değerini konsola yazdır
        Debug.Log("Aktif Görev Eşiği: " + GameConstants.QUEST_SCORE_THRESHOLD);
    }

    public void RefreshWordList()
    {
        validWords = new HashSet<string>();
        if (LocalizationSettings.SelectedLocale == null) return;

        string dilKodu = LocalizationSettings.SelectedLocale.Identifier.Code;
        string dosyaAdi = dilKodu.Contains("tr") ? "TDK" : "English words";
        TextAsset wordFile = Resources.Load<TextAsset>(dosyaAdi);

        if (wordFile != null)
        {
            string[] lines = wordFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            CultureInfo culture = dilKodu.Contains("tr") ? trCulture : CultureInfo.InvariantCulture;
            foreach (var word in lines) { validWords.Add(word.Trim().ToUpper(culture)); }
        }
    }

    public void CheckWord()
    {
        if (PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD) != GameConstants.MODE_WORD) return;
        
        string dilKodu = LocalizationSettings.SelectedLocale.Identifier.Code;
        CultureInfo culture = dilKodu.Contains("tr") ? trCulture : CultureInfo.InvariantCulture;

        string word = playerInput.text.Trim().ToUpper(culture);
        string letters = randomLettersText.text.ToUpper(culture);
        
        if (string.IsNullOrEmpty(word)) return;

        bool isWordValid = false;

        if(!WordContainLetters(word, letters)) 
        { 
            score -= 5; 
        }
        else if (!validWords.Contains(word)) 
        { 
            score -= 10; 
        }
        else 
        { 
            score += 10; 
            isWordValid = true; 
        }
        
        UpdateScoreAndCheckQuests();
        StartCoroutine(AnimateAndStartNewTour(isWordValid));
    }

    private bool WordContainLetters(string word, string letters)
    {
        foreach (char letter in letters)
        {
            if (!word.Contains(letter.ToString())) return false;
        }
        return true;
    }

    private void UpdateScoreDisplay() 
    { 
        if (scoreText != null) scoreText.text = score.ToString(); 
    }
    
    private void UpdateScoreAndCheckQuests()
    {
        UpdateScoreDisplay();

        // 1. Yüksek Skor Kaydı
        int highScore = PlayerPrefs.GetInt(GameConstants.HIGH_SCORE_KEY, 0);
        if (score > highScore) 
        {
            PlayerPrefs.SetInt(GameConstants.HIGH_SCORE_KEY, score);
        }

        // 2. Görev Kontrolü (10 Puan Testi)
        // Önemli: Skor eksiye düştüyse tetiklenmez, mutlaka pozitif 10 olmalı
        if (score >= GameConstants.QUEST_SCORE_THRESHOLD)
        {
            PlayerPrefs.SetInt(GameConstants.QUEST_COMPLETED_KEY, 1);
            PlayerPrefs.SetInt(GameConstants.MAX_LETTERS_KEY, GameConstants.QUEST_REWARD_LETTERS);
            
            // Log ekleyerek çalıştığını doğrula
            Debug.Log("<color=green>GÖREV TAMAMLANDI: </color> Skor " + score + " ulaştı!");
        }

        PlayerPrefs.Save();
    }

    private IEnumerator AnimateAndStartNewTour(bool isCorrect)
    {
        if (letterPanelContainer != null) letterPanelContainer.SetActive(false);
        playerInput.text = "";
        
        if (feedbackAnimator != null) feedbackAnimator.SetTrigger(isCorrect ? "Correct" : "Incorrect");
        
        yield return new WaitForSeconds(animationDuration);
        NewTour();
    }

    private void NewTour()
    {
        if (letterPanelContainer != null) letterPanelContainer.SetActive(true);
        if (letterGenerator != null) letterGenerator.GenerateRandomLetters();
        if (timerController != null) timerController.StartNewTour();
        playerInput.ActivateInputField();
    }

    public void HandleTimeOut()
    {
        score -= 10;
        UpdateScoreAndCheckQuests();
        StartCoroutine(AnimateAndStartNewTour(false));
    }
}