using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : Singleton<SceneController>
{
    public Animator sceneTransition;
    public string currentSceneName = "Loading";
    public bool isLoadingNewScene = false;
    public void LoadCurrentScene(Action OnComplete = null)
    {
        ChangeScene(SceneManager.GetActiveScene().name, OnComplete);
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void ChangeScene(string sceneName, Action OnComplete)
    {
        if (isLoadingNewScene) return;
        StartCoroutine(I_ChangeScene(sceneName, OnComplete));
    }
    IEnumerator I_ChangeScene(string sceneName, Action OnComplete)
    {
        yield return StartCoroutine(CloseAnimation());
        isLoadingNewScene = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Scene scene = SceneManager.GetSceneByName(sceneName);
        yield return new WaitUntil(() => scene.IsValid() && scene.isLoaded);
        OpenAnimation();
        currentSceneName = sceneName;
        OnComplete?.Invoke();
        isLoadingNewScene = false;

    }

    public void ChangeScene(string sceneName, bool isOpenAnimation, Action<float> onLoading = null, Action onComplete = null)
    {
        StartCoroutine(ChangeSceneRoutine(sceneName, isOpenAnimation, onLoading, onComplete));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName, bool isOpenAnimation, Action<float> onLoading, Action onComplete)
    {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            onLoading?.Invoke(asyncOperation.progress);
            if (asyncOperation.progress >= 0.9f)
            {
                if (!asyncOperation.allowSceneActivation)
                {
                    CloseAnimation();
                    yield return new WaitForSeconds(1);
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
        currentSceneName = sceneName;
        onComplete?.Invoke();

        if (isOpenAnimation)
        {
            OpenAnimation();
        }
    }
    private IEnumerator CloseAnimation()
    {
        sceneTransition.SetTrigger("Close");
        yield return new WaitForSeconds(1f);
    }
    private void OpenAnimation()
    {
        sceneTransition.SetTrigger("Open");
    }

}
