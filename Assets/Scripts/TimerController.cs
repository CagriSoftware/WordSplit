using System.Collections;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private WordChecker wordChecker; 
    [SerializeField] private CityChecker cityChecker; 

    public float initialTime = 15f; 
    private float currentTime;
    private bool timerWorking = false;

    void Start() { StartNewTour(); }

    public void StartNewTour()
    {
        currentTime = initialTime;
        timeText.text = Mathf.Ceil(currentTime).ToString(); 

        if(timerWorking) { StopAllCoroutines(); }
        StartCoroutine(CounterRoutine());
    }

    private IEnumerator CounterRoutine()
    {
        timerWorking = true;
        while(currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeText.text = Mathf.Ceil(currentTime).ToString(); 
            yield return null;
        }
        timerWorking = false;
        timeText.text = "0"; 
        HandleTimeUp();
    }

    private void HandleTimeUp()
    {
        int currentMode = PlayerPrefs.GetInt(GameConstants.GAME_MODE_KEY, GameConstants.MODE_WORD);

        if (currentMode == GameConstants.MODE_WORD && wordChecker != null)
        {
            wordChecker.HandleTimeOut();
        }
        else if (currentMode == GameConstants.MODE_CITY && cityChecker != null)
        {
            cityChecker.HandleTimeOut();
        }
    }
}