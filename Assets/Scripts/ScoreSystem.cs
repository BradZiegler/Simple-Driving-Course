using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour {
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreMultiplier = 1f;

    private float score;

    void Update() {
        score += Time.deltaTime * scoreMultiplier;

        scoreText.text = Mathf.FloorToInt(score).ToString();
    }
}
