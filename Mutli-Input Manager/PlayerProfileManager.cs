using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerProfileManager : MonoBehaviour
{
    protected MultiInputManager inpt;
    protected bool menuActive;

    protected virtual void Start()
    {
        inpt = MultiInputManager.Instance;
        menuActive = true;
    }

    protected IEnumerator CheckForGamepadConnectionsCoroutine()
    {
        while (menuActive)
        {
            //Debug.Log("Check Connections");
            inpt.currentGamepadCount = Gamepad.all.Count;

            if (inpt.currentGamepadCount <= 0 && inpt.PlayerGamepadNames.Count > 0) // Clears ConnectedGamepads if all Gamepads are disconnected
            {
                for (int i = 0; i < inpt.PlayerGamepadNames.Count; i++)
                {
                    GamepadDisconnected(inpt.PlayerGamepadNames[i]);
                    continue;
                }
            }

            if (inpt.currentGamepadCount > inpt.previousGamepadCount) // A new Gamepad has connected
            {
                for (int i = 0; i < Gamepad.all.Count; i++)
                {
                    if (!inpt.DetectedGamepadNames.Contains(Gamepad.all[i].name)) // If not already added, adds Gamepad to list
                    {
                        GamepadDetected(Gamepad.all[i], i);
                    }
                }
            }

            if (inpt.currentGamepadCount < inpt.previousGamepadCount) // A Gamepad has disconnected
            {
                if (inpt.PlayerGamepadNames.Count > 0)
                {
                    List<string> currentDetectedGamepadNames = GetAllGamepadNames();

                    for (int i = 0; i < inpt.PlayerGamepadNames.Count; i++)
                    {
                        if (!currentDetectedGamepadNames.Contains(inpt.PlayerGamepadNames[i]))
                        {
                            GamepadDisconnected(inpt.PlayerGamepadNames[i]);
                        }
                    }
                }
            }
            inpt.previousGamepadCount = inpt.currentGamepadCount;
            yield return new WaitForSecondsRealtime(.016f);
        }
    }

    public void RestartProfileConnections()
    {
        menuActive = true;
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            if (inpt.PlayerProfiles[i].isProfileActive)
            {
                OnProfileConnected(inpt.PlayerProfiles[i], i);
            }
        }
    }

    public void DisableMenu()
    {
        menuActive = false;
    }

    /// <summary>
    /// Calls a coroutine to check for "Ready Up" Input
    /// </summary>
    /// <param name="profile"></param>
    /// <param name="index"></param>
    protected virtual void OnProfileConnected(PlayerProfile profile, int index)
    {
        StartCoroutine(ProfileConnectedCoroutine(profile.gamepad, profile, index));
    }

    protected virtual IEnumerator ProfileConnectedCoroutine(Gamepad gamepad, PlayerProfile profile, int index)
    {
        while (menuActive)
        {
            if (SceneSwitchManager.Instance.sceneEnum == SceneSwitchManager.SceneEnum.Playing) yield break;
            if (!GetAllGamepadNames().Contains(gamepad.name)) yield break;

            if (gamepad.buttonEast.isPressed)
            {
                profile.isReady = false;
                PlayerUnreadiedAtIndex(index);
            }

            if (gamepad.startButton.isPressed && !profile.isReady)
            {
                profile.isReady = true;
                PlayerReadiedAtIndex(index);
            }
            yield return new WaitForSecondsRealtime(.016f);
        }
    }

    protected virtual void PlayerReadiedAtIndex(int index)
    {
    }

    protected virtual void PlayerUnreadiedAtIndex(int index)
    {
    }

    protected virtual void GamepadDisconnected(string gamepadID)
    {
        List<string> gamepadNames = GetAllGamepadNames();
        if (!gamepadNames.Contains(gamepadID)) inpt.DetectedGamepadNames.Remove(gamepadID);
        else
        {
            int index = gamepadNames.IndexOf(gamepadID);
            StartCoroutine(CheckForInputCoroutine(Gamepad.all[index], index));
        }
        inpt.PlayerGamepadNames.Remove(gamepadID);
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++) // Disconnects the appropriate PlayerProfile
        {
            if (inpt.PlayerProfiles[i].gamepadID == gamepadID)
            {
                inpt.PlayerProfiles[i].PlayerDisconnected();
            }
        }
    }

    protected virtual void GamepadDetected(Gamepad gamepad, int index)
    {
        inpt.DetectedGamepadNames.Add(gamepad.name);
    }

    protected IEnumerator CheckForInputCoroutine(Gamepad gamepad, int index)
    {
        while (menuActive)
        {
            if (ShouldRemoveDetectedGamepad(gamepad, index))
            {
                GamepadDisconnected(gamepad.name);
                yield break;
            }

            if (gamepad.buttonSouth.isPressed)
            {
                AddProfileFromGamepad(gamepad);
                yield break;
            }
            yield return null;
        }
    }

    protected bool ShouldRemoveDetectedGamepad(Gamepad gamepad, int index)
    {
        if (Gamepad.all.Count <= 0) return true; // No Gamepads detected

        else if (Gamepad.all.Count > index) // Specific Gamepad has been disconnected
        {
            if (Gamepad.all[index] != gamepad) return true;
            else return false;
        }

        else return false; // Gamepad is still connected
    }

    protected List<string> GetAllGamepadNames()
    {
        List<string> _currentDetectedGamepadNames = new List<string>();
        for (int i = 0; i < Gamepad.all.Count; i++) _currentDetectedGamepadNames.Add(Gamepad.all[i].name);
        return _currentDetectedGamepadNames;
    }

    protected List<string> GetAllProfileGamepadNames()
    {
        List<string> _currentDetectedGamepadNames = new List<string>();
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++) _currentDetectedGamepadNames.Add(inpt.PlayerProfiles[i].gamepadID);
        return _currentDetectedGamepadNames;
    }

    protected virtual void AddProfileFromGamepad(Gamepad gamepad)
    {
        if (GetAllProfileGamepadNames().Contains(gamepad.name)) return;

        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            if (inpt.PlayerProfiles[i].gamepadID == "")
            {
                PlayerProfile profile = inpt.PlayerProfiles[i];
                profile.JoinedAsPlayer(gamepad);
                inpt.PlayerGamepadNames.Add(gamepad.name);
                ScoreManager.Instance.PlayerScores.Add(0);
                break;
            }
        }
    }
}