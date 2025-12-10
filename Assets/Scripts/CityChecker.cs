// CityChecker.cs

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CityChecker : MonoBehaviour
{
    [Header("UI & Controllers")]
    [SerializeField] private TMP_InputField playerInput; 
    [SerializeField] private TMP_Text randomLettersText; 
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private LetterGenerator letterGenerator;
    [SerializeField] private TimerController timerController;
    [SerializeField] private TextAsset cityListFile; 

    [Header("Feedback Settings")]
    [SerializeField] private Animator feedbackAnimator;
    [SerializeField] private GameObject letterPanelContainer;
    [SerializeField] private float animationDuration = 1.2f;

    // Şehir modu için eklenenler
    private HashSet<string> validCities;
    private HashSet<string> usedCities;
    private int score = 0;

    private void Awake()
    {
        LoadCityList();
        usedCities = new HashSet<string>();
        UpdateScoreDisplay();
    }

    private void LoadCityList()
    {
        validCities = new HashSet<string>();
        if (cityListFile != null)
        {
            string[] cities = cityListFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var city in cities)
            {
                validCities.Add(city.Trim().ToUpper());
            }
        }
    }

    public void HandleTimeOut()
    {

        if (PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD) != GameConstants.MODE_CITY) return;
        
        score -= 10;
        UpdateScoreAndCheckQuests();
        StartCoroutine(AnimateAndStartNewTour(false));
    }

    public void CheckCity() 
    {

        if (PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD) != GameConstants.MODE_CITY) return;
        
        string city = playerInput.text.ToUpper().Trim();
        string letters = letterGenerator.GetLastGeneratedLetters().ToUpper();
        
        if (city.Length == 0) return;

        bool isCityValid = false;
        
        // 1. Kontrol: Şehir geçerli listede var mı?
        if (!validCities.Contains(city))
        {
             score -= 10;
        }
        // 2. Kontrol: Şehir daha önce kullanıldı mı?
        else if (usedCities.Contains(city))
        {

            StartCoroutine(AnimateAndStartNewTour(false));
            return;
        }
        else if (!CityContainLetters(city, letters))
        {
            score -= 10;
        }
        // Başarılı
        else 
        {
            score += 15;
            usedCities.Add(city);
            isCityValid = true;
        }
        
        UpdateScoreAndCheckQuests();
        StartCoroutine(AnimateAndStartNewTour(isCityValid));
    }

    private void UpdateScoreDisplay()
    {
        scoreText.text = "" + score;
    }
    
    private void UpdateScoreAndCheckQuests()
    {
        UpdateScoreDisplay(); 
        
        int currentHighScore = PlayerPrefs.GetInt(GameConstants.HIGH_SCORE_KEY, 0);
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(GameConstants.HIGH_SCORE_KEY, score);
        }

        if (score >= GameConstants.QUEST_SCORE_THRESHOLD && PlayerPrefs.GetInt(GameConstants.QUEST_COMPLETED_KEY, 0) == 0)
        {
            PlayerPrefs.SetInt(GameConstants.QUEST_COMPLETED_KEY, 1); 
            PlayerPrefs.Save(); 
        }
    }

    private bool CityContainLetters(string city, string letters)
    {
        foreach (char letter in letters)
        {
            if (city.IndexOf(letter) == -1)
            {
                return false;
            }
        }
        return true; 
    }

    private IEnumerator AnimateAndStartNewTour(bool isCorrect)
    {
        letterPanelContainer.SetActive(false);
        playerInput.text = ""; 

        if (isCorrect)
        {
            feedbackAnimator.SetTrigger("Correct");
        }
        else
        {
            feedbackAnimator.SetTrigger("Incorrect");
        }
        yield return new WaitForSeconds(animationDuration); 

        NewTour();
    }

    private void NewTour()
    {
        if (PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD) != GameConstants.MODE_CITY) return;
        
        letterPanelContainer.SetActive(true); 
        letterGenerator.GenerateRandomLetters();
        timerController.StartNewTour();
        playerInput.Select();
    }
}