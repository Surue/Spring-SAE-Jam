using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endTextMesh;
    [SerializeField] private TextMeshProUGUI scoreTextMesh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayEndMessage()
    {
        //if (GameManager.Instance.Win)
        //{
        //    endTextMesh.text = "Win";
        //} else
        //{
        //    endTextMesh.text = "Loose";
        //}
        scoreTextMesh.text = "Your score : " + Time.timeSinceLevelLoad;
    }
}
