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
    public float timeToWait;

    private IEnumerator DetectInputCoroutine()
    {
        while (true)
        {
            if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                ToggleDebugConsole();
            }
            if (Keyboard.current.backspaceKey.wasPressedThisFrame || Keyboard.current.deleteKey.isPressed)
            {
                ClearLogs();
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                OpenCurrentLogInScript();
            }

            if (isOpen)
            {
                if (Keyboard.current.spaceKey.isPressed)
                {
#if UNITY_EDITOR
                    TestLog();
                    yield return new WaitForSeconds(timeToWait);
#endif
                }
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                {
                    MoveCursor(-1);
                    yield return new WaitForSeconds(timeToWait);
                }
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                {
                    MoveCursor(1);
                    yield return new WaitForSeconds(timeToWait);
                }
            }
            yield return null;
        }
    }

    private void TestLog()
    {
        Log("Example Debug Log", "CustomDebug.cs", 87);
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

            debugDisplays[i].SetText(debugLogs[i + debugOffset], i + debugOffset);
        }
        debugDisplays[cursorIndex].OnSelected();
    }

    public void MoveCursor(int amount)
    {
        if (!isOpen) return;
        if (debugLogs.Count == 0) return;

        debugDisplays[cursorIndex].OnDeselected();
        cursorIndex += amount;

        if (cursorIndex > debugLogs.Count - 1 || cursorIndex > debugDisplays.Count - 1)
        {
            MoveCursorToTop();
        }
        if (cursorIndex < 0)
        {
            MoveCursorToBottom();
        }

        UpdateDisplays();
    }

    private void MoveCursorToTop()
    {
        if (HasLessLogsThanDisplays())
        {
            cursorIndex = 0;
            debugOffset = 0;
        }
        else
        {
            debugOffset++;
            if (debugOffset + debugDisplays.Count - 1 > debugLogs.Count - 1)
            {
                debugOffset = 0;
                cursorIndex = 0;
            }
            else
            {
                cursorIndex = debugDisplays.Count - 1;
            }
        }
    }

    private void MoveCursorToBottom()
    {
        if (HasLessLogsThanDisplays())
        {
            cursorIndex = debugLogs.Count - 1;
        }
        else
        {
            if (debugOffset > 0)
            {
                debugOffset--;
                cursorIndex = 0;
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
    }

    private bool HasLessLogsThanDisplays()
    {
        return debugLogs.Count < debugDisplays.Count;
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

    public void OpenCurrentLogInScript()
    {
#if UNITY_EDITOR
        if (!isOpen) return;
        if (debugLogs.Count == 0) return;

        DebugInfo debugInfo = debugLogs[cursorIndex + debugOffset];
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
#endif
    }

    private void ClearLogs()
    {
        if (!isOpen) return;

        debugDisplays[cursorIndex].OnDeselected();

        for (int i = 0; i < debugDisplays.Count; i++)
        {
            debugDisplays[i].ClearDisplay();
        }

        debugLogs.Clear();
        cursorIndex = 0;
        debugOffset = 0;
        debugDisplays[0].OnSelected();
    }
}