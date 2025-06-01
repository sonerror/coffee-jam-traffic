using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " UIRef SO", menuName = "SO/IU Ref So")]
public class UIRefSO : ScriptableObject
{
    public static UIRefSO ins;

    public List<UICanvasPrime> uiCanvasRefList;

#if UNITY_EDITOR
    private void OnEnable()
    {
        ins = this;
    }

#else
    private void Awake()
    {
        // Debug.Log("CAR PART CALLED 2");
        ins = this;
    }
#endif
}