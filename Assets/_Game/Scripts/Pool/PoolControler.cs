using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class PoolControler : MonoBehaviour
{
    [Header("---- POOL CONTROLER TO INIT POOL ----")]
    [Header("Put object pool to list Pool or Resources/Pool")]
    [Header("Preload: Init Poll")]
    [Header("Spawn: Take object from pool")]
    [Header("Despawn: return object to pool")]
    [Header("Collect: return objects type to pool")]
    [Header("CollectAll: return all objects to pool")]

    [Space]
    [Header("Pool")]
    public List<PoolAmount> Pool;

    [Header("Particle")]
    public ParticleAmount[] Particle;


    public void Awake()
    {
        for (int i = 0; i < Pool.Count; i++)
        {
            SimplePool.Preload(Pool[i].prefab, Pool[i].amount, Pool[i].root, Pool[i].collect);
        }

        for (int i = 0; i < Particle.Length; i++)
        {
            ParticlePool.Preload(Particle[i].prefab, Particle[i].amount, Particle[i].root);
            ParticlePool.Shortcut(Particle[i].particleType, Particle[i].prefab);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(PoolControler))]
public class PoolControlerEditor : Editor
{
    PoolControler pool;

    private void OnEnable()
    {
        pool = (PoolControler)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create Quick Root"))
        {
            for (int i = 0; i < pool.Pool.Count; i++)
            {
                if (pool.Pool[i].root == null)
                {
                    Transform tf = new GameObject(pool.Pool[i].prefab.poolType.ToString()).transform;
                    tf.parent = pool.transform;
                    pool.Pool[i].root = tf;
                }
            }

            for (int i = 0; i < pool.Particle.Length; i++)
            {
                if (pool.Particle[i].root == null)
                {
                    Transform tf = new GameObject(pool.Particle[i].particleType.ToString()).transform;
                    tf.parent = pool.transform;
                    pool.Particle[i].root = tf;
                }
            }
        }

        if (GUILayout.Button("Get Prefab Resource"))
        {
            GameUnit[] resources = Resources.LoadAll<GameUnit>("Pool");

            for (int i = 0; i < resources.Length; i++)
            {
                bool isDuplicate = false;
                for (int j = 0; j < pool.Pool.Count; j++)
                {
                    if (resources[i].poolType == pool.Pool[j].prefab.poolType)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    Transform root = new GameObject(resources[i].name).transform;

                    PoolAmount newPool = new PoolAmount(root, resources[i], SimplePool.DEFAULT_POOL_SIZE, true);

                    pool.Pool.Add(newPool);
                }
            }
        }
    }
}

#endif

[System.Serializable]
public class PoolAmount
{
    [Header("-- Pool Amount --")]
    public Transform root;
    public GameUnit prefab;
    public int amount;
    public bool collect;

    public PoolAmount(Transform root, GameUnit prefab, int amount, bool collect)
    {
        this.root = root;
        this.prefab = prefab;
        this.amount = amount;
        this.collect = collect;
    }
}


[System.Serializable]
public class ParticleAmount
{
    public Transform root;
    public ParticleType particleType;
    public ParticleSystem prefab;
    public int amount;
}


public enum ParticleType
{
    Hit_1,
    Hit_2,
    Hit_3,

    LevelUp_1,
    LevelUp_2,
    LevelUp_3,

    Explosion,
    ColorButton
}

public enum PoolType
{
    None = 0,
    Player = 1,
    Enemy = 2,
    Enemy_00 = 3,
    Enemy_01 = 4,
    Enemy_02 = 5,
    Enemy_03 = 6,
    Enemy_04 = 7,
    Enemy_05 = 8,
    Enemy_06 = 9,
    Enemy_07 = 10,
    Enemy_08 = 11,
    Enemy_09 = 12,
    Enemy_10 = 13,
    Enemy_11 = 14,
    Enemy_12 = 15,
    Enemy_13 = 16,
    Enemy_14 = 17,
    Enemy_15 = 18,
    Enemy_16 = 19,
    Enemy_17 = 20,
    Enemy_18 = 21,
    Enemy_19 = 22,
    Enemy_20 = 23,
    Enemy_21 = 24,
    Enemy_22 = 25,
    ObjMagnet = 100,
    Enemy_Machine = 200,
    Enemy_Rabbit = 201,
    People = 300,
    Boss = 400
}

