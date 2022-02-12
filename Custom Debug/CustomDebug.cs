using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEditor;

[Serializable]
public struct DebugInfo
{
    public string text;
    public string className;
    public int lineNum;
}

public class CustomDebug : MonoBehaviour
{
    #region Singleton

    public static CustomDebug singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            StartCoroutine(DetectInputCoroutine());
        }
        else Destroy(this.gameObject);
    }

    #endregion

    [Header("Display")]
    public bool isOpen;
    public List<DebugInfo> debugLogs;
    public List<DebugDisplay> debugDisplays;
    public int cursorIndex;
    public int debugOffset;

    [Header("Canvas")]
    public GameObject canvasOBJ;

    private IEnumerator DetectInputCoroutine()
    {
        while (true)
        {
            if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                ToggleDebugConsole();
            }
            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                if (isOpen) MoveCursor(-1);
            }
            if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                if (isOpen) MoveCursor(1);
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                if (isOpen) OpenLogInScript(debugLogs[cursorIndex]);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void ToggleDebugConsole()
    {
        if (!isOpen) OpenConsole();
        else CloseConsole();   
    }

    private void OpenConsole()
    {
        isOpen = true;
        canvasOBJ.SetActive(true);
        UpdateDisplays();
    }

    private void CloseConsole()
    {
        isOpen = false;
        canvasOBJ.SetActive(false);
    }

    private void UpdateDisplays()
    {
        for (int i = 0; i < debugLogs.Count; i ++)
        {
            if (i > debugDisplays.Count - 1) break;

            debugDisplays[i].SetText(debugLogs[i + debugOffset]);
        }
        debugDisplays[cursorIndex].OnSelected();
    }

    public void MoveCursor(int amount)
    {
        debugDisplays[cursorIndex].OnDeselected();

        cursorIndex += amount;

        if (cursorIndex > debugDisplays.Count - 1)
        {
            if (debugOffset < GetDifferenceInLogToDisplay()) // If There are more Logs than Displays
            {
                debugOffset++;
                cursorIndex = debugDisplays.Count - 1;
            }
            else
            {
                cursorIndex = 0;
                debugOffset = 0;
            }
        }
        if (cursorIndex < 0)
        {
            if (debugOffset > 0)
            {
                debugOffset--;
                cursorIndex = 0 + debugOffset;
            }
            else
            {
                cursorIndex = debugDisplays.Count - 1; // Sets Index to last in List of Displays
                if (debugLogs.Count > debugDisplays.Count) // If There are more Logs than Displays
                {
                    debugOffset = GetDifferenceInLogToDisplay(); // Sets Offset to appropriate value
                }
            }
        }
        UpdateDisplays();
    }

    private int GetDifferenceInLogToDisplay()
    {
        return debugLogs.Count - debugDisplays.Count;
    }

    public void Log(string _text, string _className, int _lineNum)
    {
        DebugInfo debugInfo;
        debugInfo.text = _text;
        debugInfo.className = _className;
        debugInfo.lineNum = _lineNum;
        debugLogs.Add(debugInfo);

#if UNITY_EDITOR
        Debug.Log(_text);
#endif

        if (!isOpen) return;
        UpdateDisplays();
    }

    public void OpenLogInScript(DebugInfo debugInfo)
    {
        foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
        {
            if (assetPath.EndsWith(debugInfo.className))
            {
                var script = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
                if (script != null)
                {
                    AssetDatabase.OpenAsset(script, debugInfo.lineNum);
                    break;
                }
            }
        }
    }
}