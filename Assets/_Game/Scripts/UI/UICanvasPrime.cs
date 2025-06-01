using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvasPrime : MonoBehaviour
{
    public new GameObject gameObject;
    [SerializeField] protected bool isDestroyOnClose;
    public Canvas canvas;

    /// FLAG  ///////////////////////////////////////////////////////////////////
    public static bool flagIsOpenTreasureShow;

    public virtual bool IsOpening()
    {
        return gameObject.activeSelf;
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        OnOpenCanvas();
    }

    protected virtual void OnOpenCanvas()
    {
#if UNITY_EDITOR
        Debug.Log("Open Canvas " + base.gameObject.name);
#endif
    }

    public virtual void Close()
    {
        if (gameObject == null) return;
        gameObject.SetActive(false);
        OnCloseCanvas();

        if (isDestroyOnClose)
        {
            Destroy(gameObject);
            UIManager.ins.UICanvasList.Remove(GetType());
        }
    }

    protected virtual void OnCloseCanvas()
    {
#if UNITY_EDITOR
        Debug.Log("Close Canvas " + base.gameObject.name);
#endif
    }

    private void Reset()
    {
        gameObject = base.gameObject;
        canvas = GetComponent<Canvas>();
    }
}