using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using TMPro;
using UnityEngine.SceneManagement;

public class MultiInputManager : MonoBehaviour
{
    [Header("Player Profile Data")]
    public List<PlayerProfile> PlayerProfiles = new List<PlayerProfile>();
    public List<string> PlayerGamepadNames = new List<string>();

    [Header("Detected")]
    public List<string> DetectedGamepadNames = new List<string>();
    public int previousGamepadCount = 0;
    public int currentGamepadCount = 0;

    [Header("Minigame Data")]
    public MinigameSO currentMinigame;
    public int currentMinigameIndex;
    public List<MinigameSO> Minigames = new List<MinigameSO>();

    public GameMode gameMode;


    #region Singleton

    public static MultiInputManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    #endregion

    // Temporary
    private void Update()
    {
        //if (Keyboard.current.spaceKey.wasPressedThisFrame) SFXSystem.singleton.PlaySFX("sfx_test");

        //if (Keyboard.current.backquoteKey.isPressed) SceneSwitchManager.Instance.SwitchSceneAfterTime("PlayerSelect", 1f);
    }

    public void IncrementMinigameIndex()
    {
        currentMinigameIndex++;
        if (currentMinigameIndex > Minigames.Count - 1)
        {
            SceneSwitchManager.Instance.SwitchSceneAfterTime("Main Menu", 3);
        }
        else
        {
            currentMinigame = Minigames[currentMinigameIndex];
        }
    }    

}