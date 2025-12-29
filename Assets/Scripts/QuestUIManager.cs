using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using System.Collections;

public class QuestUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private TMP_Text questProgress;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text questStatus;
    [SerializeField] private GameObject letterSelectionPanel;

    [Header("Buttons")]
    [SerializeField] private Button button2Letters;
    [SerializeField] private Button button3Letters;

    [Header("Localization Keys")]
    [SerializeField] private LocalizedString titleKey;
    [SerializeField] private LocalizedString completedKey;
    [SerializeField] private LocalizedString progressKey;

    private void OnEnable() 
    { 
        // Panel her açıldığında verileri güncelle
        UpdateQuestUI(); 
    }

    private void Start()
    {
        // Başlangıçta da bir kez kontrol et
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        // Localization anahtarlarının boş olup olmadığını kontrol et
        if (titleKey == null || titleKey.IsEmpty) return;

        // PlayerPrefs'ten güncel verileri çek
        int highScore = PlayerPrefs.GetInt(GameConstants.HIGH_SCORE_KEY, 0);
        bool isDone = PlayerPrefs.GetInt(GameConstants.QUEST_COMPLETED_KEY, 0) == 1;
        
        // Eşik değerini GameConstants'tan al
        int threshold = GameConstants.QUEST_SCORE_THRESHOLD;

        // Başlığı yerelleştirilmiş olarak yaz
        questTitle.text = titleKey.GetLocalizedString();

        // MANTIK KONTROLÜ: Puan eşiği geçildi mi?
        if (isDone || highScore >= threshold)
        {
            // Görev Tamamlandı Durumu
            questStatus.text = $"<color=green>{completedKey.GetLocalizedString()}</color>";
            progressSlider.value = 1f;
            questProgress.text = "MAX / " + threshold; // Veya "Completed"
            
            letterSelectionPanel.SetActive(true);
            
            // Buton Kilitlerini Aç
            if (button2Letters != null) button2Letters.interactable = true;
            if (button3Letters != null) button3Letters.interactable = true;
            
            // Eğer isDone henüz kaydedilmemişse (puanla yeni geçildiyse) kaydet
            if (!isDone) 
            {
                PlayerPrefs.SetInt(GameConstants.QUEST_COMPLETED_KEY, 1);
                PlayerPrefs.Save();
            }
        }
        else
        {
            // Görev Devam Ediyor Durumu
            questStatus.text = $"<color=yellow>{progressKey.GetLocalizedString()}</color>";
            
            // Slider değerini hesapla (0.0 ile 1.0 arası)
            float progress = (float)highScore / threshold;
            progressSlider.value = Mathf.Clamp01(progress);
            
            questProgress.text = $"{highScore} / {threshold}";
            
            // Seçim panelini göster ama butonları kısıtla
            letterSelectionPanel.SetActive(true); 
            
            if (button2Letters != null) button2Letters.interactable = true; 
            if (button3Letters != null) button3Letters.interactable = false; // 3 HARF HALA KİLİTLİ
        }
    }

    // Menüdeki "Görevler" butonuna basıldığında bu metodu çağırabilirsiniz
    public void RefreshUI()
    {
        UpdateQuestUI();
    }
}