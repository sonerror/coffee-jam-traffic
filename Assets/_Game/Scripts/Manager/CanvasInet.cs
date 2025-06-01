using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasInet : UICanvasPrime
{
    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        GrandManager.ins.ResetFlag();
    }
}