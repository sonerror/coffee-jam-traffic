using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileVariation : MonoBehaviour
{
    public new Transform transform;
    public new GameObject gameObject;

    public ProjectileType projectileType;
    [SerializeField] private bool isAutoPush;
    [SerializeField] private float lifeTime;
    public bool IsScaleChanged { get; set; }
    public Vector3 RootScale { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        RootScale = transform.localScale;
    }

    private void OnEnable()
    {
        if (isAutoPush)
            Timer.ScheduleSupreme(lifeTime, () => ProjectileManager.ins.Push(this));
    }

    public ProjectileVariation SetScale(Vector3 scale)
    {
        transform.localScale = scale;
        IsScaleChanged = true;
        return this;
    }
}

public enum ProjectileType
{
    HitSpark,
    DoneExplode,
    LittleSpark,
}