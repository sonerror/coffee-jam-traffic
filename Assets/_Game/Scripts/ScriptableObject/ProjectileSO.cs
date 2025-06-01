using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile", menuName = "SO/Projectile")]
public class ProjectileSO : ScriptableObject
{
    public static ProjectileSO ins;
    public ProjectileVariation[] projectiles;

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