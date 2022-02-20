using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSystem : MonoBehaviour
{
    #region Singleton

    public static SFXSystem singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(this.gameObject);
            GetSFXFromResources();
        }
        else Destroy(this.gameObject);
    }

    #endregion

    public const string path = "SFX";
    public Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();

    [Header("SFX Players")]
    public List<SFXPlayer> sfxPlayerPool;
    public List<SFXPlayer> activeSFXPlayers;
    public List<SFXPlayer> pausedSFXPlayers;

    [Header("SFX ID Lists")]
    public List<string> UIOnHoverSFX;
    public List<string> UIOnSubmitSFX;
    public List<string> UIOnCancelSFX;
    public List<string> FallingWhistleSFX;


    /// <summary>
    /// Adds all SFX in "Resources/SFX" to sfxDict
    /// </summary>
    private void GetSFXFromResources()
    {
        AudioClip[] sfxArray = Resources.LoadAll<AudioClip>(path);
        if (sfxArray.Length == 0) return;

        for (int i = 0; i < sfxArray.Length; i++)
        {
            sfxDict.Add(sfxArray[i].name, sfxArray[i]);
        }
    }    

    /// <summary>
    /// Gets SFXPlayer from pool and plays a SFX
    /// </summary>
    /// <param name="sfxID"></param>
    public void PlaySFX(string sfxID, bool isLoop)
    {
        AudioClip sfx;
        sfxDict.TryGetValue(sfxID, out sfx);

        if (sfx == null)
        {
            Debug.LogError("SFX " + sfxID + " Not Found");
            return;
        }
        if (sfxPlayerPool.Count == 0)
        {
            Debug.LogError("No Available SFX Players");
            return;
        }

        if (isLoop) sfxPlayerPool[0].PlaySFXOnLoop(sfx);
        else sfxPlayerPool[0].PlaySFX(sfx);
    }

    public void PlayRandomSFX(List<string> sfxIDs)
    {
        AudioClip sfx;
        int randIndex = Random.Range(0, sfxIDs.Count);
        sfxDict.TryGetValue(sfxIDs[randIndex], out sfx);

        if (sfx == null)
        {
            Debug.LogError("SFX " + sfxIDs[randIndex] + " Not Found");
            return;
        }
        if (sfxPlayerPool.Count == 0)
        {
            Debug.LogError("No Available SFX Players");
            return;
        }

        sfxPlayerPool[0].PlaySFX(sfx);
    }


    public void PauseAllSFX()
    {
        for (int i = 0; i < activeSFXPlayers.Count; i++)
        {
            activeSFXPlayers[i].PauseSFX();
            pausedSFXPlayers.Add(activeSFXPlayers[i]);
            activeSFXPlayers.Remove(activeSFXPlayers[i]);
        }
    }

    public void UnpauseAllSFX()
    {
        for (int i = 0; i < pausedSFXPlayers.Count; i++)
        {
            pausedSFXPlayers[i].UnpauseSFX();
            activeSFXPlayers.Add(pausedSFXPlayers[i]);
            pausedSFXPlayers.Remove(pausedSFXPlayers[i]);
        }
    }

    /// <summary>
    /// Stops all active SFXPlayers
    /// </summary>
    public void CleanUp()
    {
        List<SFXPlayer> sfxPlayers = activeSFXPlayers;
        if (activeSFXPlayers.Count == 0) return;

        for (int i = 0; i < sfxPlayers.Count; i++)
        {
            activeSFXPlayers[i].CleanUp();
        }
    }
}