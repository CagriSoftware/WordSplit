using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LetterGenerator : MonoBehaviour
{
    [SerializeField] private TMP_Text randomLettersText;
    
    private char[] trAlphabet = { 'A','B','C','Ç','D','E','F','G','Ğ','H','I','İ','J','K','L','M','N','O','Ö','P','R','S','Ş','T','U','Ü','V','Y','Z' };
    private char[] enAlphabet = { 'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };
    
    private char[] currentAlphabet;
    private int maxLettersAllowed;
    private string lastGeneratedLetters = ""; 

    void Start()
    {
        UpdateAlphabet();
        maxLettersAllowed = PlayerPrefs.GetInt(GameConstants.MAX_LETTERS_KEY, GameConstants.BASE_LETTERS);
        GenerateRandomLetters();
    }

    public void UpdateAlphabet()
    {
        string dil = LocalizationSettings.SelectedLocale.Identifier.Code;
        currentAlphabet = dil.Contains("tr") ? trAlphabet : enAlphabet;
    }

    public void GenerateRandomLetters()
    {
        if (currentAlphabet == null) UpdateAlphabet();
        string generated = "";
        for (int i = 0; i < maxLettersAllowed; i++)
        {
            generated += currentAlphabet[Random.Range(0, currentAlphabet.Length)];
        }
        lastGeneratedLetters = generated;
        randomLettersText.text = generated;
    }

    public string GetLastGeneratedLetters() => lastGeneratedLetters;
}