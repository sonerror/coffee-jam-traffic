using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteAtlasUser : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private string name;

    private void Awake()
    {
        image.sprite = spriteAtlas.GetSprite(name);
    }

    private void Reset()
    {
        image = gameObject.GetComponent<Image>();
        if (image.sprite != null) name = image.sprite.name;

        if (spriteAtlas.CanBindTo(image.sprite))
        {
        }
    }

    [ContextMenu("UPDATE NAME")]
    public void UpdateName()
    {
        if (image != null && image.sprite != null) name = image.sprite.name;
    }
}