using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : SingleTons<UIManager>
{

    
    public UIRefSO uiRefSO;
    private readonly Dictionary<Type, UICanvasPrime> uiCanvasReferenceDict = new Dictionary<Type, UICanvasPrime>();
    private readonly Dictionary<Type, UICanvasPrime> uiCanvasList = new Dictionary<Type, UICanvasPrime>();
    public Dictionary<Type, UICanvasPrime> UICanvasList => uiCanvasList;
    public Transform uiCanvasParentTrans;

    public AnimationCurve popCanvasCurve;

    [SerializeField] private float upperOffsetForOffer;
    [SerializeField] private float bottomOfferForOffer;
    public float UpperOffsetForOffer => upperOffsetForOffer;
    public float BottomOffsetForOffer => bottomOfferForOffer;

    protected override void Awake()
    {
        base.Awake();
        InitCanvasData();
    }

    private void InitCanvasData()
    {
        var list = uiRefSO.uiCanvasRefList;
        for (int i = 0; i < list.Count; i++)
        {
            uiCanvasReferenceDict.Add(list[i].GetType(), list[i]);
        }
    }

    //Fucking remember to remove canvas in list after destroy to prevent get a null canvas in list due to " is " operator , more info : https://stackoverflow.com/questions/75013054/is-it-cost-expensive-to-use-if-gameobject-null
    private T LookForCanvas<T>() where T : UICanvasPrime
    {
        var key = typeof(T);
        if (uiCanvasList.ContainsKey(key)) return uiCanvasList[typeof(T)] as T;
        return null;
    }

    private T GetCanvasPrefab<T>() where T : UICanvasPrime
    {
        var key = typeof(T);
        if (uiCanvasReferenceDict.ContainsKey(key)) return uiCanvasReferenceDict[typeof(T)] as T;
        return null; //make sure to have all prefab ready or else fuck you
    }

    public T GetUICanvas<T>() where T : UICanvasPrime
    {
        var tmp = LookForCanvas<T>();
        if (tmp == null)
        {
            tmp = Instantiate(GetCanvasPrefab<T>(), uiCanvasParentTrans);
            tmp.gameObject.SetActive(false);
            uiCanvasList.Add(tmp.GetType(), tmp);
        }

        return tmp;
    }

    public T OpenUI<T>() where T : UICanvasPrime
    {
        var canvas = GetUICanvas<T>();
        canvas.Open();

        return canvas;
    }

    public void CloseUI<T>() where T : UICanvasPrime
    {
        if (IsUICanvasOpened<T>())
        {
            GetUICanvas<T>().Close();
        }
    }

    public bool IsUICanvasOpened<T>() where T : UICanvasPrime
    {
        var tmp = LookForCanvas<T>();
        return (tmp != null) && tmp.IsOpening();
    }

    public void CloseAllUI()
    {
        List<UICanvasPrime> removeList = new List<UICanvasPrime>();
        foreach (var item in UICanvasList)
        {
            removeList.Add(item.Value);
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            var cur = removeList[i];
            if (cur != null && cur.IsOpening()) cur.Close();
        }
    }

    public void CloseAllUI<T>() where T : UICanvasPrime
    {
        List<UICanvasPrime> removeList = new List<UICanvasPrime>();
        foreach (var item in UICanvasList)
        {
            removeList.Add(item.Value);
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            var cur = removeList[i];
            if (cur != null && cur.IsOpening() && cur is not T) cur.Close();
        }
    }
}