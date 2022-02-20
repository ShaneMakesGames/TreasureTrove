using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugDisplay : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI indexText;
    public Image debugImage;
    public Color defaultColor;
    public Color selectedColor;

    public void Awake()
    {
        defaultColor = debugImage.color;
    }

    public void SetText(DebugInfo debugInfo, int index)
    {
        debugText.text = debugInfo.text;
        infoText.text = "From " + debugInfo.className + " on Line " + debugInfo.lineNum;
        indexText.text = "" + index;
    }

    public void OnSelected()
    {
        debugImage.color = selectedColor;
    }

    public void OnDeselected()
    {
        debugImage.color = defaultColor;
    }

    public void ClearDisplay()
    {
        debugText.text = "";
        infoText.text = "";
        indexText.text = "";
    }
}