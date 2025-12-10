// LetterGenerator.cs

using TMPro;
using UnityEngine;

public class LetterGenerator : MonoBehaviour
{
    [SerializeField] private TMP_Text randomLettersText;
    
    private char[] words = { 'A','B','C','Ç','D','E','F','G','Ğ','H','I','İ','J','K','L','M','N',
    'O','Ö','P','R','S','Ş','T','U','Ü','V','Y','Z', };
    
    private int maxLettersAllowed = GameConstants.BASE_LETTERS; 
    
    private string lastGeneratedLetters = ""; 

    void Start()
    {
        maxLettersAllowed = PlayerPrefs.GetInt(GameConstants.MAX_LETTERS_KEY, GameConstants.BASE_LETTERS); 
        
        GenerateRandomLetters();
    }

    public void GenerateRandomLetters()
    {
        string generatedLetters = "";

        for (int i = 0; i < maxLettersAllowed; i++)
        {
            char newLetter = words[Random.Range(0, words.Length)];
            generatedLetters += newLetter.ToString();
        }

        lastGeneratedLetters = generatedLetters; 
        
        randomLettersText.text = generatedLetters;
    }
    
    public string GetLastGeneratedLetters()
    {
        return lastGeneratedLetters;
    }
}