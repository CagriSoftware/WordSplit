// WordChecker.cs

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordChecker : MonoBehaviour
{
    [Header("UI & Controllers")]
    [SerializeField] private TMP_InputField playerInput;
    [SerializeField] private TMP_Text randomLettersText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private LetterGenerator letterGenerator;
    [SerializeField] private TimerController timerController;
    [SerializeField] private TextAsset wordListFile;

    [Header("Feedback Settings")]
    [SerializeField] private Animator feedbackAnimator;
    [SerializeField] private GameObject letterPanelContainer;
    [SerializeField] private float animationDuration = 1.2f;

    private HashSet<string> validWords;
    private int score = 0;

    private void Awake()
    {
        LoadWordList();
        UpdateScoreDisplay();
    }

    private void LoadWordList()
    {
        validWords = new HashSet<string>();
        if (wordListFile != null)
        {
            string[] words = wordListFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                validWords.Add(word.Trim().ToUpper());
            }
        }
    }

    public void HandleTimeOut()
    {
        if (PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD) != GameConstants.MODE_WORD) return;
        
        score -= 10;
        UpdateScoreAndCheckQuests();
        StartCoroutine(AnimateAndStartNewTour(false));
    }

    public void CheckWord()
    {
        if (PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD) != GameConstants.MODE_WORD) return;
        
        string word = playerInput.text.ToUpper().Trim();
        string letters = randomLettersText.text;
        
        if (word.Length == 0) return;

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

    private bool WordContainLetters(string word, string letters)
    {
        if (letters.Length >= GameConstants.BASE_LETTERS)
        {
            return word.Contains(letters[0].ToString()) && word.Contains(letters[1].ToString());
        }
        
        return false;
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
        if (PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD) != GameConstants.MODE_WORD) return;
        
        letterPanelContainer.SetActive(true); 
        letterGenerator.GenerateRandomLetters();
        timerController.StartNewTour();
    }
}