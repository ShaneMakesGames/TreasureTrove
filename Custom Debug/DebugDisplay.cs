using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugDisplay : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public TextMeshProUGUI infoText;
    public Image debugImage;
    public Color defaultColor;
    public Color selectedColor;

    public void Awake()
    {
        defaultColor = debugImage.color;
    }

    public void SetText(DebugInfo debugInfo)
    {
        debugText.text = debugInfo.text;
        infoText.text = "From " + debugInfo.className + " .cs on Line " + debugInfo.lineNum;
    }

    public void OnSelected()
    {
        debugImage.color = selectedColor;
    }

    public void OnDeselected()
    {
        debugImage.color = defaultColor;
    }
}