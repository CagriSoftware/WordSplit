// QuestUIManager.cs

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    [Header("Quest UI Elements")]
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private TMP_Text questProgress;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text questStatus;
    
    [Header("Selection UI Element")]
    [SerializeField] private GameObject letterSelectionPanel; 

    void OnEnable()
    {
        UpdateQuestUI();
    }
    
    public void UpdateQuestUI()
    {
        questTitle.text = $"Görev: {GameConstants.QUEST_SCORE_THRESHOLD} Puana Ulaş ve 3 Harf Modunu Aç";

        bool isCompleted = PlayerPrefs.GetInt(GameConstants.QUEST_COMPLETED_KEY, 0) == 1;
        
        int currentMaxLetters = PlayerPrefs.GetInt(GameConstants.MAX_LETTERS_KEY, GameConstants.BASE_LETTERS); 

        if (isCompleted)
        {
            questStatus.text = "<color=#00FF00>TAMAMLANDI</color>";
            progressSlider.value = 1f;
            
            if(letterSelectionPanel != null) letterSelectionPanel.SetActive(true); 

            questProgress.text = $"Aktif Harf Sayısı: {currentMaxLetters} | Kaç Harfli Oynayacağını Seç";

        }
        else
        {
            questStatus.text = "<color=#FFFF00>Devam Ediyor...</color>";
            if(letterSelectionPanel != null) letterSelectionPanel.SetActive(false);
            
            int currentScore = PlayerPrefs.GetInt(GameConstants.HIGH_SCORE_KEY, 0); 
            
            float progress = (float)currentScore / GameConstants.QUEST_SCORE_THRESHOLD;
            progressSlider.value = Mathf.Clamp01(progress);
            
            questProgress.text = $"{currentScore}/{GameConstants.QUEST_SCORE_THRESHOLD}";
        }
    }
}