using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IAPButtonPro : MonoBehaviour
{
    public TMP_Text priceText;
    [IAPSelector] public string id;

    private const string EMPTY = "-----";
    private bool isInitialized;

    [SerializeField] private bool isSeperateLine;

    private void OnEnable()
    {
        if (isInitialized) return;
        StartCoroutine(ie_Check());
        return;

        IEnumerator ie_Check()
        {
            if (priceText != null) priceText.text = EMPTY;
            yield return new WaitUntil(() => IAPPrime.ins.initialized);

            var (currencyCode, price) = IAPPrime.ins.GetLocalPriceSupreme(id);

            if (priceText != null) priceText.text = currencyCode + (isSeperateLine ? "\n" : " ") + price;
            // Debug.Log("IAP BUTTON " + currencyCode + " " + price);

            isInitialized = true;
        }
    }

    public void OnClickDefault(string placement = "")
    {
        // Debug.Log("IAP " + id, gameObject);
        AudioManager.ins.PlaySound(SoundType.UIClick);
        IAPPrime.ins.ClickPurchase(id, placement);
    }
}