using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class DisconnectionHandler : PlayerProfileManager
{
    private bool isOpen;
    public List<PlayerSelectVisuals> PlayerVisuals = new List<PlayerSelectVisuals>();
    public Canvas canvas;

    private int readyCount;
    private int playerCount;

    public PlayerProfileManager profileManager;

    #region Singleton

    public static DisconnectionHandler Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CheckForGamepadConnectionsCoroutine());
    }

    protected override void GamepadDetected(Gamepad gamepad, int index)
    {
        base.GamepadDetected(gamepad, index);
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            if (inpt.PlayerProfiles[i].gamepad == gamepad)
            {
                AddPlayerAtIndex(inpt.PlayerProfiles[i], i, true);
                break;
            }
        }
    }


    protected override void GamepadDisconnected(string gamepadID)
    {
        List<string> gamepadNames = GetAllGamepadNames();
        inpt.DetectedGamepadNames.Remove(gamepadID);
        inpt.PlayerGamepadNames.Remove(gamepadID);
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++) // Disconnects the appropriate PlayerProfile
        {
            if (inpt.PlayerProfiles[i].gamepadID == gamepadID)
            {
                inpt.PlayerProfiles[i].isGamepadConnected = false;
                PlayerVisuals[i].pressToJoinText.gameObject.SetActive(true);
                PlayerVisuals[i].playerNumText.gameObject.SetActive(false);
                PlayerVisuals[i].pressToLeaveText.gameObject.SetActive(false);
                PlayerVisuals[i].pressToReadyText.gameObject.SetActive(false);
                if (inpt.PlayerProfiles[i].isReady)
                {
                    inpt.PlayerProfiles[i].isReady = false;
                    readyCount--;
                }
            }
        }
        OnGamepadDisconnected();
    }

    public void OnGamepadDisconnected()
    {
        if (isOpen) return;
        if (profileManager != null) profileManager.DisableMenu();

        SFXSystem.singleton.PauseAllSFX();

        readyCount = 0;
        playerCount = 0;

        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            if (inpt.PlayerProfiles[i].isProfileActive)
            {
                playerCount++;
            }
        }

        isOpen = true;
        Time.timeScale = 0;
        canvas.gameObject.SetActive(true);
        AddPlayersFromInputManager();
    }

    private IEnumerator DelayedExit()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ExitDisconnectionHandler();
    }

    public void ExitDisconnectionHandler()
    {
        isOpen = false;
        Time.timeScale = 1;
        SFXSystem.singleton.UnpauseAllSFX();

        for (int i = 0; i < PlayerVisuals.Count; i++)
        {
            PlayerVisuals[i].pressToJoinText.gameObject.SetActive(false);
            PlayerVisuals[i].playerNumText.gameObject.SetActive(false);
            PlayerVisuals[i].pressToLeaveText.gameObject.SetActive(false);
            PlayerVisuals[i].pressToReadyText.gameObject.SetActive(false);
        }
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            inpt.PlayerProfiles[i].isReady = false;
        }

        canvas.gameObject.SetActive(false);
        if (profileManager != null) profileManager.RestartProfileConnections();
    }

    private void AddPlayersFromInputManager()
    {
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            if (inpt.PlayerProfiles[i].isGamepadConnected)
            {
                AddPlayerAtIndex(inpt.PlayerProfiles[i], i, false);
            }
        }
    }

    public void AddPlayerAtIndex(PlayerProfile profile, int index, bool AddInput)
    {
        OnProfileConnected(profile, index);
        if (!inpt.PlayerGamepadNames.Contains(profile.gamepadID)) inpt.PlayerGamepadNames.Add(profile.gamepadID);
        inpt.PlayerProfiles[index].isGamepadConnected = true;
        inpt.PlayerProfiles[index].isProfileActive = true;
        inpt.PlayerProfiles[index].isReady = false;

        if (AddInput)
        {
            profile.inputUser = InputUser.PerformPairingWithDevice(profile.gamepad);
        }

        PlayerVisuals[index].pressToJoinText.gameObject.SetActive(false);
        PlayerVisuals[index].playerNumText.gameObject.SetActive(true);
        PlayerVisuals[index].pressToReadyText.gameObject.SetActive(true);
    }

    protected override IEnumerator ProfileConnectedCoroutine(Gamepad gamepad, PlayerProfile profile, int index)
    {
        while (menuActive)
        {
            if (!GetAllGamepadNames().Contains(gamepad.name)) yield break;

            if (gamepad.startButton.isPressed && !profile.isReady)
            {
                profile.isReady = true;
                PlayerReadiedAtIndex(index);
                yield break;
            }
            yield return new WaitForSecondsRealtime(.016f);
        }
    }

    protected override void PlayerReadiedAtIndex(int index)
    {
        PlayerVisuals[index].pressToReadyText.gameObject.SetActive(false);
        PlayerVisuals[index].pressToLeaveText.gameObject.SetActive(true);
        readyCount++;

        if (readyCount >= playerCount)
        {
            StartCoroutine(DelayedExit());
        }
    }

    protected override void PlayerUnreadiedAtIndex(int index)
    {
        Debug.Log("Unreadied");
        PlayerVisuals[index].pressToReadyText.gameObject.SetActive(true);
    }
}