using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerSelectManager : PlayerProfileManager
{
    public bool playerInReadyArea;
    public List<Vector3> SpawnPositions = new List<Vector3>();
    public GameObject playerPrefab;
    public List<PlayerController> PlayerControllers = new List<PlayerController>();


    [Header("Player Visuals")]
    public List<PlayerSelectVisuals> PlayerVisuals = new List<PlayerSelectVisuals>();

    [Header("Dev Tool")]
    public bool overridePlayerNumLimit; // Allows you to start the game with only a single player (Should only be enabled for testing purposes)

    #region Singleton

    public static PlayerSelectManager Instance;
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
        inpt = MultiInputManager.Instance;
        base.Start();
        ClearDetectedGamepads();
        StartCoroutine(CheckForGamepadConnectionsCoroutine());
    }

    private void ClearDetectedGamepads()
    {
        inpt.currentGamepadCount = 0;
        inpt.previousGamepadCount = 0;
        inpt.DetectedGamepadNames.Clear();
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            inpt.PlayerProfiles[i].ResetProfile();
        }
    }

    protected override void GamepadDetected(Gamepad gamepad, int index)
    {
        base.GamepadDetected(gamepad, index);
        StartCoroutine(CheckForInputCoroutine(gamepad, index));
    }

    protected override void GamepadDisconnected(string gamepadID)
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
                inpt.PlayerProfiles[i].PlayerDisconnectedFromPlayerSelect();
                RemovePlayerAtIndex(i);
            }
        }
    }

    protected override void AddProfileFromGamepad(Gamepad gamepad)
    {
        if (GetAllProfileGamepadNames().Contains(gamepad.name))
        {
            for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
            {
                if (inpt.PlayerProfiles[i].gamepadID == gamepad.name)
                {
                    PlayerProfileConnected(gamepad, i);

                    CreatePlayerController(i);
                    CustomDebug.singleton.Log("Player Has Pressed A", "PlayerSelectManager", 91);
                    //Debug.Log("Player Has Pressed A");
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
            {
                if (inpt.PlayerProfiles[i].gamepadID == "")
                {
                    PlayerProfileConnected(gamepad, i);
                    CreatePlayerController(i);
                    break;
                }
            }
        }


    }

    protected virtual void CreatePlayerController(int index)
    {
       
           if (inpt.PlayerProfiles[index].isProfileActive)
            {
                inpt.PlayerProfiles[index].isReady = false;
                GameObject obj = Instantiate(playerPrefab, SpawnPositions[index], Quaternion.identity);
                obj.transform.localScale = Vector3.one;
                PlayerController playerController = obj.GetComponent<PlayerController>();
                playerController.playerProfile = inpt.PlayerProfiles[index];
                playerController.AssignPlayerProfileData();
                obj.name = playerController.playerProfile.playerNum.ToString();
                PlayerControllers.Add(playerController);
           }
       
        
    }
    private void PlayerProfileConnected(Gamepad gamepad, int i)
    {
        PlayerProfile profile = inpt.PlayerProfiles[i];
        profile.JoinedAsPlayer(gamepad);
        inpt.PlayerGamepadNames.Add(gamepad.name);
        profile.inputUser = InputUser.PerformPairingWithDevice(gamepad);

        AddPlayerAtIndex(i);
        OnProfileConnected(profile, i);
    }



    protected override IEnumerator ProfileConnectedCoroutine(Gamepad gamepad, PlayerProfile profile, int index)
    {
        while (menuActive)
        {
            if (SceneSwitchManager.Instance.sceneEnum == SceneSwitchManager.SceneEnum.Playing) yield break;
            if (!GetAllGamepadNames().Contains(gamepad.name)) yield break;

            if(playerInReadyArea == true)
            {
                profile.isReady = true;
                PlayerReadiedAtIndex(index);
                Debug.Log("player In Ready Area Status: " + playerInReadyArea + " PlayerProfile: " + inpt.PlayerProfiles[index].gamepad.name);
            }
           
            if (gamepad.buttonEast.isPressed)
            {
                profile.isReady = false;

                GamepadDisconnected(inpt.PlayerProfiles[index].gamepad.name);
                yield break;
            }

            if (gamepad.startButton.isPressed && !profile.isReady)
            {
                profile.isReady = true;
                PlayerReadiedAtIndex(index);
            }
            yield return new WaitForSecondsRealtime(.016f);
        }
    }

    public void AddPlayerAtIndex(int index)
    {
        PlayerVisuals[index].pressToJoinText.gameObject.SetActive(false);
        PlayerVisuals[index].playerPreview.SetActive(true);
        PlayerVisuals[index].playerNumText.gameObject.SetActive(true);
        PlayerVisuals[index].pressToLeaveText.gameObject.SetActive(true);
        PlayerVisuals[index].pressToReadyText.gameObject.SetActive(true);
    }

    public void RemovePlayerAtIndex(int index)
    {
        PlayerVisuals[index]?.pressToJoinText.gameObject.SetActive(true);
        PlayerVisuals[index]?.playerPreview.SetActive(false);
        PlayerVisuals[index]?.playerNumText.gameObject.SetActive(false);
        PlayerVisuals[index]?.pressToLeaveText.gameObject.SetActive(false);
        PlayerVisuals[index]?.pressToReadyText.gameObject.SetActive(false);
        
        for (int i = 0; i < PlayerControllers.Count; i++)
        {
            if (PlayerControllers[i].playerProfile == inpt.PlayerProfiles[index])
            {
                Destroy(PlayerControllers[i].gameObject);
                PlayerControllers.RemoveAt(i);
                break;
            }
        }
    }

    protected override void PlayerReadiedAtIndex(int index)
    {
        PlayerVisuals[index].pressToReadyText.gameObject.SetActive(false);
        if (AllProfilesAreReady()) {
            if (inpt.gameMode == GameMode.MiniGames) {
                SetupMinigame();
            } else if (inpt.gameMode == GameMode.BoardGame) {
                SetupBoardgame();
            }
        }
    }



    public bool AllProfilesAreReady()
    {
        if (overridePlayerNumLimit) return true;
        int numProfilesActive = 0;
        for (int i = 0; i < inpt.PlayerProfiles.Count; i++)
        {
            PlayerProfile profile = inpt.PlayerProfiles[i];
            if (profile.isProfileActive)
            {
                numProfilesActive++;
                if (!profile.isReady) return false;
            }
        }
        if (numProfilesActive > 1) return true;
        else return false;
    }

    public void SetupMinigame()
    {
        menuActive = false;
        inpt.currentMinigame = inpt.Minigames[inpt.currentMinigameIndex];
        inpt.currentMinigameIndex++;
        SceneSwitchManager.Instance.SwitchSceneAfterTime("RulesScene", 1f);
    }

    public void SetupBoardgame()
    {
        menuActive = false;
        SceneSwitchManager.Instance.SwitchSceneAfterTime("BoardGame", 1f);
    }
}