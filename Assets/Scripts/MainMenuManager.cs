using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject questPanel; 
    
    [SerializeField] private GameObject modeSelectionPanel;

    public void StartGame()
    {
        OpenModeSelectionPanel(); 
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ToggleQuestPanel()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(!questPanel.activeSelf); 
        }
    }

    public void OpenModeSelectionPanel()
    {
        if (modeSelectionPanel != null)
        {
            if (questPanel != null && questPanel.activeSelf)
            {
                questPanel.SetActive(false);
            }
            modeSelectionPanel.SetActive(true);
        }
    }

    public void CloseModeSelectionPanel()
    {
        if (modeSelectionPanel != null)
        {
            modeSelectionPanel.SetActive(false);
        }
    }
    
    public void StartWordGame()
    {
        PlayerPrefs.SetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }

    public void StartCityGame()
    {
        PlayerPrefs.SetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_CITY);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
    
    public void SetLettersToTwo()
    {
        PlayerPrefs.SetInt(GameConstants.MAX_LETTERS_KEY, GameConstants.BASE_LETTERS);
        PlayerPrefs.Save();
    }

    public void SetLettersToThree()
    {
        PlayerPrefs.SetInt(GameConstants.MAX_LETTERS_KEY, GameConstants.QUEST_REWARD_LETTERS);
        PlayerPrefs.Save();
    }
}