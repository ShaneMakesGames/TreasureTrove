using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.Users;

public enum PlayerNum
    {
        Player1,
        Player2,
        Player3,
        Player4
    }

[System.Serializable]
public class PlayerProfile
{
    public bool isProfileActive;
    public bool isGamepadConnected;
    public bool isReady;

    

    [Header("Player Number")]
    public PlayerNum playerNum;

    [Header("Input User")]
    public InputUser inputUser;
    public Material playerMaterial;

    [Header("Gamepad")]
    public string gamepadID;
    public Gamepad gamepad;

    [Header("Score")]
    public int score;

    public void JoinedAsPlayer(Gamepad _gamepad)
    {
        isProfileActive = true;
        isGamepadConnected = true;
        gamepad = _gamepad;
        gamepadID = _gamepad.name;
    }

    public void ResetProfile()
    {
        if (gamepad != null) inputUser.UnpairDevice(gamepad);
        isProfileActive = false;
        isGamepadConnected = false;
        isReady = false;
        gamepad = null;
        gamepadID = "";
    }

    public void PlayerDisconnected()
    {
        inputUser.UnpairDevice(gamepad);
        isProfileActive = false;
        isGamepadConnected = false;
        isReady = false;
    }

    public void PlayerDisconnectedFromPlayerSelect()
    {
        inputUser.UnpairDevice(gamepad);
        isProfileActive = false;
        isGamepadConnected = false;
        isReady = false;
        gamepad = null;
        gamepadID = "";
    }

}