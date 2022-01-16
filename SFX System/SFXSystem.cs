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

    public List<SFXPlayer> sfxPlayerPool;
    public List<SFXPlayer> activeSFXPlayers;

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
    public void PlaySFX(string sfxID)
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

        sfxPlayerPool[0].PlaySFX(sfx);
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