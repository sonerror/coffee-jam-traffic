using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarPro : MonoBehaviour
{
    public Image image;
    [HideInInspector] public Material mat;
    private static int FILL = Shader.PropertyToID("_Float");
    [SerializeField, Range(0, 1)] private float fillAmount;

    public float FillAmount
    {
        get => fillAmount;

        set
        {
            fillAmount = Mathf.Clamp01(value);
            mat.SetFloat(FILL, fillAmount);
        }
    }

    private void Awake()
    {
        mat = Instantiate(image.material);
        image.material = mat;
        mat.SetFloat(FILL, fillAmount);
    }
}