using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{   
    public Player myPlayer;
    public Text raceTimeDisplay;
    public Text countdownDisplay;
    public Text lapDisplay;

    float boostMeterFillTarget = 1f;
    public Image boostMeter;

    private void Awake()
    {
        myPlayer = GetComponentInParent<Player>();

        Race race = FindObjectOfType<Race>();

        race.RaceTimeUpdate.AddListener(SetRaceTime);
        race.CountdownTextUpdate.AddListener(SetCountdownText);
        race.PlayerLapUpdate.AddListener(PlayerCompletedLap);
        myPlayer.BoostMeterUpdate.AddListener(SetBoostMeterFillTarget);
    }

    private void Update()
    {
        boostMeter.fillAmount = Mathf.MoveTowards(boostMeter.fillAmount, boostMeterFillTarget,Time.deltaTime);
    }

    public void SetRaceTime(float newRaceTime){
        raceTimeDisplay.text = newRaceTime.ToString("0.0");
    }

    public void SetCountdownText(string countdownText){
        countdownDisplay.text = countdownText;
    }

    public void PlayerCompletedLap(Player player, int newLap){
        if (player != myPlayer) return;
        lapDisplay.text = $"Lap {newLap}";
    }

    public void SetBoostMeterFillTarget(Player player, float amount){
        if (player != myPlayer) return;
        boostMeterFillTarget = amount;
    }

}
