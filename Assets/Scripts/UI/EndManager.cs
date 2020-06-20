using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endTextMesh;
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
        if (GameManager.Instance.Win)
        {
            endTextMesh.text = "Win";
        } else
        {
            endTextMesh.text = "Loose";
        }
    }
}
