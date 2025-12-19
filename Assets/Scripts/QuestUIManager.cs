using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private TMP_Text questProgress;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text questStatus;
    [SerializeField] private GameObject letterSelectionPanel;

    [SerializeField] private LocalizedString titleKey;
    [SerializeField] private LocalizedString completedKey;
    [SerializeField] private LocalizedString progressKey;

    void OnEnable() { UpdateQuestUI(); }

    public void UpdateQuestUI()
    {
        // GÜVENLİK: Inspector'da boş bırakılan key varsa hata vermez.
        if (titleKey == null || titleKey.IsEmpty || completedKey.IsEmpty || progressKey.IsEmpty) return;

        questTitle.text = titleKey.GetLocalizedString();
        bool isDone = PlayerPrefs.GetInt(GameConstants.QUEST_COMPLETED_KEY, 0) == 1;
        int score = PlayerPrefs.GetInt(GameConstants.HIGH_SCORE_KEY, 0);

        if (isDone)
        {
            questStatus.text = $"<color=green>{completedKey.GetLocalizedString()}</color>";
            progressSlider.value = 1f;
            letterSelectionPanel.SetActive(true);
            questProgress.text = "3 Harf Modu Aktif"; 
        }
        else
        {
            questStatus.text = $"<color=yellow>{progressKey.GetLocalizedString()}</color>";
            progressSlider.value = (float)score / GameConstants.QUEST_SCORE_THRESHOLD;
            questProgress.text = $"{score}/{GameConstants.QUEST_SCORE_THRESHOLD}";
            letterSelectionPanel.SetActive(false);
        }
    }
}