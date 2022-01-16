using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource source;
    public bool coroutinePlaying;

    public void PlaySFX(AudioClip sfx)
    {
        sfxCoroutine = StartCoroutine(PlaySFXCoroutine(sfx));
    }

    public Coroutine sfxCoroutine;
    private IEnumerator PlaySFXCoroutine(AudioClip sfx)
    {
        SFXSystem.singleton.sfxPlayerPool.Remove(this);
        SFXSystem.singleton.activeSFXPlayers.Add(this);

        source.clip = sfx;
        source.Play();

        float timePassed = 0;
        while (timePassed < sfx.length)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        SFXSystem.singleton.sfxPlayerPool.Add(this);
        SFXSystem.singleton.activeSFXPlayers.Remove(this);
    }

    public void CleanUp()
    {
        source.Stop();
        StopCoroutine(sfxCoroutine);

        if (!SFXSystem.singleton.sfxPlayerPool.Contains(this)) SFXSystem.singleton.sfxPlayerPool.Add(this);
        if (SFXSystem.singleton.activeSFXPlayers.Contains(this)) SFXSystem.singleton.activeSFXPlayers.Remove(this);
    }
}