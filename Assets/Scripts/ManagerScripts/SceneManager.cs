using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SceneManagerEx : MonoBehaviour
{
    public static SceneManagerEx Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // === Ýsimle yükle (async + callback) ===
    public void Load(string sceneName, Action onLoaded = null)
    {
        StartCoroutine(LoadSceneAsync(sceneName, onLoaded));
    }

    // === Ýndeks ile yükle (async + callback) ===
    public void Load(int buildIndex, Action onLoaded = null)
    {
        StartCoroutine(LoadSceneAsync(buildIndex, onLoaded));
    }

    // === Bir sonraki sahneyi yükle (async + callback) ===
    public void LoadNext(Action onLoaded = null)
    {
        int idx = SceneManager.GetActiveScene().buildIndex;
        int next = (idx + 1) % SceneManager.sceneCountInBuildSettings;
        StartCoroutine(LoadSceneAsync(next, onLoaded));
    }

    // === Ortak async loader ===
    private IEnumerator LoadSceneAsync(int buildIndex, Action onLoaded)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;

        onLoaded?.Invoke();
    }

    private IEnumerator LoadSceneAsync(string sceneName, Action onLoaded)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;

        onLoaded?.Invoke();
    }
}
