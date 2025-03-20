using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBlockage : UICanvasPrime
{
    private int runningCor;

    protected override void OnCloseCanvas()
    {
        base.OnCloseCanvas();

        runningCor = 0;
    }

    public void FuckingBlockTilTime(float time, Action task = null)
    {
        runningCor++;
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            yield return Yielders.Get(time);

            task?.Invoke();

            runningCor--;
            if (runningCor == 0) Close();
        }
    }
}