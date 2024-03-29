#region
using System;
using TMPro;
using UnityEngine;
#endregion

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] float timeValue = 90f;
    [SerializeField] float startTime = 90f;
    [SerializeField] float maxTimeValue;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] bool countUp;

    bool countDirectionLastFrame;

    SceneLoader loader;
    Pickup pickup;
    PlayerMovement player;

    public float TimeValue { get; set; }

    void Start()
    {
        countDirectionLastFrame = !countUp;
        timeValue               = startTime;
        player                  = FindObjectOfType<PlayerMovement>();
        pickup                  = FindObjectOfType<Pickup>();
        loader                  = FindObjectOfType<SceneLoader>();
    }

    void Update()
    {
        Count();
        //Debug.Log($"{timeValue}");
        StopCountdown();
    }

    void Count()
    {
        if (countUp != countDirectionLastFrame)
            //timeValue = countUp ? 0 : maxTimeValue;
            countDirectionLastFrame = countUp;

        timeValue += (countUp ? 1 : -1) * Time.deltaTime;

        timeValue = Mathf.Clamp(timeValue, 0, maxTimeValue);

        DisplayTime(timeValue);
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0) timeToDisplay = 0;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void AddTime(float timeToAdd) { timeValue += timeToAdd; }

    void StopCountdown()
    {
        if (timeValue <= 0) CD_PlayerDeath();
    }

    void CD_PlayerDeath()
    {
        Destroy(player);
        Debug.Log("Player Died!!");

        try
        {
            loader.LoadGameOver(loader.DelayInSeconds);
        } catch (Exception error)
        {
            Debug.LogError("Error: " + error.Message + "\n There is no end-scene scene in the build settings.");
            throw;
        }
    }
}