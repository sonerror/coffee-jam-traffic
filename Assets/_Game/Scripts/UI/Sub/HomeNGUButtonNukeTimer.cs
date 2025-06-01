using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeNGUButtonNukeTimer : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            while (BuyingPackDataFragment.cur.gameData.isNeverGiveUpEnable)
            {
                if (!BuyingPackDataFragment.cur.CheckNeverGiveUpAvailable(out _))
                {
                    CanvasHome.CheckNeverGiveUpBundleStatic();
                    yield break;
                }

                yield return Yielders.Get(1);
            }
        }
    }
}