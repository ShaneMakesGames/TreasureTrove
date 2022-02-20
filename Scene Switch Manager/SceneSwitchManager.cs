using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitchManager : MonoBehaviour
{
    #region Singleton

    public static SceneSwitchManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public enum SceneEnum
    {
        Playing,
        Fading,
        Finished
    }

    public SceneEnum sceneEnum;
    public Image fadeImage;
    public string targetScene;

    public void SwitchSceneAfterTime(string _sceneString, float timeToWait)
    {
        if (sceneEnum == SceneEnum.Finished)
        {
            StartCoroutine(DelayedSwitchSceneCoroutine(_sceneString, timeToWait));
        }
        else if (_sceneString != targetScene)
        {
            QueueSceneSwitch(_sceneString);
        }
    }

    public void SwitchSceneFade(string _sceneString)
    {
        StartCoroutine(SwitchSceneFadeCoroutine(_sceneString));
    }

    private IEnumerator DelayedSwitchSceneCoroutine(string _sceneString, float timeToWait)
    {
        targetScene = _sceneString;
        sceneEnum = SceneEnum.Playing;
        yield return new WaitForSeconds(timeToWait);
        SwitchSceneFade(_sceneString);
    }

    private void OnEnable()
    {
        sceneEnum = SceneEnum.Finished;
    }

    private IEnumerator SwitchSceneFadeCoroutine(string _sceneString)
    {
        SFXSystem.singleton.CleanUp();
        sceneEnum = SceneEnum.Playing;

        while (fadeImage.color.a < 1) // Fade to black
        {
            Color newColor = fadeImage.color;
            newColor.a += Time.deltaTime;
            fadeImage.color = newColor;
            yield return null;
        }

        SceneManager.LoadScene(_sceneString);
        sceneEnum = SceneEnum.Fading;

        while (SceneManager.GetActiveScene().name != _sceneString) // Loops until scene has switched
        {
            yield return new WaitForSeconds(0.25f);
        }

        while (fadeImage.color.a > 0) // Fade from black
        {
            Color newColor = fadeImage.color;
            newColor.a -= Time.deltaTime;
            fadeImage.color = newColor;
            yield return null;
        }
        sceneEnum = SceneEnum.Finished;
        targetScene = "";
    }

    public void QueueSceneSwitch(string _sceneString)
    {
        StartCoroutine(QueueSceneSwitchCoroutine(_sceneString));
    }

    private IEnumerator QueueSceneSwitchCoroutine(string _sceneString)
    {
        while (sceneEnum != SceneEnum.Finished)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        SwitchSceneFade(_sceneString);
    }
}