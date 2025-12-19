// GameModeManager.cs

using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    [Header("Mode Controllers")]
    [SerializeField] private WordChecker wordChecker;
    [SerializeField] private CityChecker cityChecker;

    void Awake()
    {
        int selectedMode = PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD);

        wordChecker.enabled = false;
        cityChecker.enabled = false;

        if (selectedMode == GameConstants.MODE_WORD)
        {
            wordChecker.enabled = true;
        }
        else if (selectedMode == GameConstants.MODE_CITY)
        {
            cityChecker.enabled = true;
        }
        else
        {
            wordChecker.enabled = true;
        }
    }

    public void SubmitInput()
{
    int selectedMode = PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD);

    if (selectedMode == GameConstants.MODE_WORD && wordChecker != null)
    {
        wordChecker.CheckWord();
    }
    else if (selectedMode == GameConstants.MODE_CITY && cityChecker != null)
    {
        cityChecker.CheckCity();
    }
}
}