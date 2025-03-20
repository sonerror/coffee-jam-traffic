using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CanvasLoading : UICanvasPrime
{
    public Image fill;
    private float i;

    private Material mat;

    // public RectTransform carRect;
    // public Vector2 start;
    // public Vector2 end;

    private static int TIME = Shader.PropertyToID("_UnScaleTime");

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        if (!PlayerPrefs.HasKey(Variables.PREF_KEY_LOAD_TIME_START))
        {
            PlayerPrefs.SetFloat(Variables.PREF_KEY_LOAD_TIME_START, UnbiasedTime.TrueDateTime.Second);
            // FirebaseManager.Ins.start_loading();
        }

        mat = fill.material;
        mat.SetFloat(Variables.FLOAT, 0);

        yield return new WaitUntil(() => DataManager.ins != null && DataManager.ins.gameData != null);
        yield return null;
        yield return null;
        yield return null;
        i = 0;
        var operation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        float cur = 0;
        while (cur < 0.99f || !AdsManager.Ins.IsAdsSetupDone)
        {
            cur = Mathf.Clamp01(operation.progress / 0.9f);
            i += Time.deltaTime * .42f;
            mat.SetFloat(Variables.FLOAT, Mathf.Clamp01(i + .0152f));
            yield return null;
        }

        DOVirtual.Float(i, 1, .62f, (fuk) =>
        {
            mat.SetFloat(Variables.FLOAT, Mathf.Clamp01(fuk + .0152f));
            i = fuk;
        });
        operation = SceneManager.LoadSceneAsync(2);
        operation.allowSceneActivation = false;

        yield return new WaitUntil(() => i > .99f);

        operation.allowSceneActivation = true;
        yield return new WaitUntil(() => GrandManager.ins != null
                                         && TransportCenter.cur != null && LevelDataFragment.cur != null);
        Application.targetFrameRate = 120;
        yield return null;
        AdsManager.Ins.DropAOA();
        yield return null;

        AudioManager.ins.PlayMusic(MusicType.BGM);

        if (LevelDataFragment.cur.IsBabySitLevel())
        {
            GrandManager.ins.InitLevel();
        }
        else
        {
            GrandManager.ins.InitHome();
        }

        AdsManager.Ins.ShowBanner();
        FirebaseManager.Ins.OnSetUserProperty();

        Destroy(gameObject);
    }

    private void Update()
    {
        Shader.SetGlobalFloat(TIME, Time.unscaledTime);
    }
}