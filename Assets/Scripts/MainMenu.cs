using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text playText;
    [SerializeField] private AndroidNotificationHandler androidNotificationHandler;
    [SerializeField] private int maxEnergy;
    [SerializeField] private int energyRechargeDurationInMinutes;
    [SerializeField] private int energyCost;

    private int energy;

    private const string EnergyKey = "Energy";
    private const string NextEnergyReadyKey = "EnergyReady";

    private void Start() {
        int highScore = PlayerPrefs.GetInt(ScoreSystem.HighScoreKey, 0);
        highScoreText.text = $"High Score: {highScore}";
        playText.text = $"Play (Cost {energyCost})";

        ProcessEnergy();
    }

    private void Update() {
        ProcessEnergy();
    }

    private void ProcessEnergy() {
        energy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);
        DateTime nextEnergyReady = DateTime.MinValue;

        if (energy < maxEnergy) {
            string nextEnergyReadyString = PlayerPrefs.GetString(NextEnergyReadyKey, string.Empty);

            if (string.IsNullOrEmpty(nextEnergyReadyString)) { return; }

            nextEnergyReady = DateTime.Parse(nextEnergyReadyString);

            while(DateTime.Now > nextEnergyReady) {
                energy++;
                if (energy < maxEnergy) {
                    nextEnergyReady = nextEnergyReady.AddMinutes(energyRechargeDurationInMinutes);
                    PlayerPrefs.SetString(NextEnergyReadyKey, nextEnergyReady.ToString());
                } else {
                    PlayerPrefs.SetString(NextEnergyReadyKey, string.Empty);
                    break;
                }
            }
            PlayerPrefs.SetInt(EnergyKey, energy);
        }

        energyText.text = $"Energy: {energy}/{maxEnergy}";

        if (energy < maxEnergy && nextEnergyReady != DateTime.MinValue) {
            int timeInSecondsUntilNextEnergy = (int)(nextEnergyReady - DateTime.Now).TotalSeconds;
            energyText.text += " (" + timeInSecondsUntilNextEnergy + ")";
        }
    }

    public void Play() {
        if (energy < energyCost) { return; }

        energy -= energyCost;
        PlayerPrefs.SetInt(EnergyKey, energy);

        string energyReadyString = PlayerPrefs.GetString(NextEnergyReadyKey, string.Empty);
        if (string.IsNullOrEmpty(energyReadyString)) {
            DateTime nextEnergyReady = DateTime.Now.AddMinutes(energyRechargeDurationInMinutes);
            PlayerPrefs.SetString(NextEnergyReadyKey, nextEnergyReady.ToString());
        }

        SetNotification();

        SceneManager.LoadScene(1);
    }

    private void SetNotification() {
        string nextEnergyReadyString = PlayerPrefs.GetString(NextEnergyReadyKey, string.Empty);
        if (string.IsNullOrEmpty(nextEnergyReadyString)) { return; }
        
        DateTime notificationTime = DateTime.Parse(nextEnergyReadyString);

        for(int i = energy + 1; i < maxEnergy; i++) {
            notificationTime.AddMinutes(energyRechargeDurationInMinutes);
        }

#if UNITY_ANDROID
        androidNotificationHandler.ScheduleNotification(notificationTime);
#endif
    }
}
