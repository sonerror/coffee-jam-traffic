
///
/// Create by linh soi - Abi Game studio
/// mentor Minh tito - CTO Abi Game studio
/// 
/// Manage list UI canvas for easy to use
/// Member nen inherit UI canvas
/// 
/// Update: 09-10-2020 
///             manage UI with Generic
///         09-10-2021 
///             Open, Close UI with Typeof(T)
///         28/11/2022
///             Close All UI
///             Close delay time
///

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    //dict for quick query UI prefab
    //dict dung de lu thong tin prefab canvas truy cap cho nhanh
    private Dictionary<System.Type, UICanvas> uiCanvasPrefab = new Dictionary<System.Type, UICanvas>();
    public Queue<System.Action> uiQueue = new Queue<System.Action>();
    //list from resource
    //list load ui resource
    private UICanvas[] uiResources;
    private bool isProcessingQueue = false;
    //dict for UI active
    //dict luu cac ui dang dung
    private Dictionary<System.Type, UICanvas> uiCanvas = new Dictionary<System.Type, UICanvas>();

    //canvas container, it should be a canvas - root
    //canvas chua dung cac canvas con, nen la mot canvas - root de chua cac canvas nay
    public Transform parentTF;
    public RectTransform canvasTF;

    public float screenWidth;
    public float screenHeight;
    #region Canvas

    //open UI
    //mo UI canvas
    public void Awake()
    {
    }
    public void Oninit()
    {
        screenWidth = Screen.width / canvasTF.localScale.x;
        screenHeight = Screen.height / canvasTF.localScale.y;
    }
    public T OpenUI<T>() where T : UICanvas
    {
        UICanvas canvas = GetUI<T>();
        canvas.Setup();
        canvas.Open();

        return canvas as T;
    }
    public void QueueOpenUI<T>() where T : UICanvas
    {
        // Thêm lệnh vào hàng đợi
        uiQueue.Enqueue(() => {
           var canvas =  OpenUI<T>();
            canvas.OnClose += ProcessQueue;
        });

        // Bắt đầu xử lý hàng đợi nếu chưa bắt đầu
        if (!isProcessingQueue)
        {
            isProcessingQueue = true;
            ProcessQueue();
        }
    }
    public void QueueActionOpenUI(Action action)
    {
        uiQueue.Enqueue(action);
        if (!isProcessingQueue)
        {
            isProcessingQueue = true;
            ProcessQueue();
        }
    }
    public void ResetQueue()
    {
        StopCoroutine(IE_ProcessQueue());
        uiQueue.Clear();
    }
    public void ProcessQueue()
    {
        StartCoroutine(IE_ProcessQueue());
    }
    private IEnumerator IE_ProcessQueue()
    {
        if (uiQueue.Count > 0)
        {
            // Thực hiện lệnh từ hàng đợi
            var action = uiQueue.Dequeue();
            yield return new WaitForSeconds(0.2f);
            action.Invoke();

            // Có thể thêm delay nếu cần
            yield return null; // Chờ một frame
        }
        if(uiQueue.Count == 0)
        {
            yield return new WaitForSeconds(1f);
            isProcessingQueue = false;
        }

    }

    //close UI directly
    //dong UI canvas ngay lap tuc
    public void CloseUI<T>() where T : UICanvas
    {
        if (IsOpened<T>())
        {
            GetUI<T>().CloseDirectly();
        }
    }

    //close UI with delay time
    //dong ui canvas sau delay time
    public void CloseUI<T>(float delayTime) where T : UICanvas
    {
        if (IsOpened<T>())
        {
            GetUI<T>().Close(delayTime);
        }
    }

    //check UI is Opened
    //kiem tra UI dang duoc mo len hay khong
    public bool IsOpened<T>() where T : UICanvas
    {
        return IsLoaded<T>() && uiCanvas[typeof(T)].gameObject.activeInHierarchy;
    }

    //check UI is loaded
    //kiem tra UI da duoc khoi tao hay chua
    public bool IsLoaded<T>() where T : UICanvas
    {
        System.Type type = typeof(T);
        return uiCanvas.ContainsKey(type) && uiCanvas[type] != null;
    }

    //Get component UI 
    //lay component cua UI hien tai
    public T GetUI<T>() where T : UICanvas
    {
        if (!IsLoaded<T>())
        {
            UICanvas canvas = Instantiate(GetUIPrefab<T>(), parentTF);
            uiCanvas[typeof(T)] = canvas;
        }

        return uiCanvas[typeof(T)] as T;
    }

    //Close all UI
    //dong tat ca UI ngay lap tuc -> tranh truong hop dang mo UI nao dong ma bi chen 2 UI cung mot luc
    public void CloseAll()
    {
        foreach (var item in uiCanvas)
        {
            if (item.Value != null && item.Value.gameObject.activeInHierarchy)
            {
                item.Value.CloseDirectly();
            }
        }
    }

    //Get prefab from resource
    //lay prefab tu Resources/UI 
    private T GetUIPrefab<T>() where T : UICanvas
    {
        if (!uiCanvasPrefab.ContainsKey(typeof(T)))
        {
            if (uiResources == null)
            {
                uiResources = Resources.LoadAll<UICanvas>("UI/");
            }

            for (int i = 0; i < uiResources.Length; i++)
            {
                if (uiResources[i] is T)
                {
                    uiCanvasPrefab[typeof(T)] = uiResources[i];
                    break;
                }
            }
        }

        return uiCanvasPrefab[typeof(T)] as T;
    }


    #endregion

    #region Back Button

    private Dictionary<UICanvas, UnityAction> BackActionEvents = new Dictionary<UICanvas, UnityAction>();
    private List<UICanvas> backCanvas = new List<UICanvas>();
    UICanvas BackTopUI
    {
        get
        {
            UICanvas canvas = null;
            if (backCanvas.Count > 0)
            {
                canvas = backCanvas[backCanvas.Count - 1];
            }

            return canvas;
        }
    }


    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.Escape) && BackTopUI != null)
        {
            BackActionEvents[BackTopUI]?.Invoke();
        }
    }

    public void PushBackAction(UICanvas canvas, UnityAction action)
    {
        if (!BackActionEvents.ContainsKey(canvas))
        {
            BackActionEvents.Add(canvas, action);
        }
    }

    public void AddBackUI(UICanvas canvas)
    {
        if (!backCanvas.Contains(canvas))
        {
            backCanvas.Add(canvas);
        }
    }

    public void RemoveBackUI(UICanvas canvas)
    {
        backCanvas.Remove(canvas);
    }

    /// <summary>
    /// CLear backey when comeback index UI canvas
    /// </summary>
    public void ClearBackKey()
    {
        backCanvas.Clear();
    }

    #endregion
}