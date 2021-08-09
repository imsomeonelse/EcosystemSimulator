using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateText : MonoBehaviour
{
    TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string newText)
    {
        text.SetText(newText);
    }
}
