using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager ins;
    public Stack<ProjectileVariation>[] grandPool = new Stack<ProjectileVariation>[System.Enum.GetNames(typeof(ProjectileType)).Length];

    private void Awake()
    {
        ins = this;
        for (var i = 0; i < grandPool.Length; i++)
        {
            grandPool[i] = new Stack<ProjectileVariation>();
        }
    }

    public ProjectileVariation Pop(ProjectileType projectileType, Vector3 pos, Quaternion rot)
    {
        var variation = grandPool[(int)projectileType].Count > 0 ? grandPool[(int)projectileType].Pop() : Instantiate(ProjectileSO.ins.projectiles[(int)projectileType]);

        variation.transform.SetPositionAndRotation(pos, rot);
        variation.gameObject.SetActive(true);

        return variation;
    }

    public void Push(ProjectileVariation projectileVariation)
    {
        if (!grandPool[(int)projectileVariation.projectileType].Contains(projectileVariation))
            grandPool[(int)projectileVariation.projectileType].Push(projectileVariation);
        projectileVariation.gameObject.SetActive(false);
        if (projectileVariation.IsScaleChanged)
        {
            projectileVariation.transform.localScale = projectileVariation.RootScale;
            projectileVariation.IsScaleChanged = false;
        }
    }
}