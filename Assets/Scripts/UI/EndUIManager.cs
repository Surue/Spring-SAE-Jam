using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreTextMesh;

    private const float INVALID_SCORE = -1;
    private float score = INVALID_SCORE;

    public void DisplayEndMessage()
    {
        if (score == INVALID_SCORE)
        {
            score = Time.timeSinceLevelLoad;
        }
        scoreTextMesh.text = "Your score : " + score;
    }
}
