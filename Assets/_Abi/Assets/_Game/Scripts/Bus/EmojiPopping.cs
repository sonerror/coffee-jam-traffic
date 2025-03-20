using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Bus;
using UnityEngine;
using Random = UnityEngine.Random;

public class EmojiPopping : MonoBehaviour
{
    public static EmojiPopping cur;

    public new GameObject gameObject;

    public GameObject[] emojiObjects;
    public Transform[] emojiTrans;
    private bool[] emojiFlags;

    [SerializeField] private Vector3 offsetFromMinion;
    [SerializeField] private Vector3 offsetFromMinionOnBoard;
    [SerializeField] private float[] durationOptions;
    [SerializeField] private float[] timeSpaceOptions;

    private float timePoint;

    private void Awake()
    {
        cur = this;
        emojiFlags = new bool[emojiTrans.Length];

        Activate(false);
    }

    private void Update()
    {
        return;
        if (timePoint < Time.time)
        {
            PopEmoji();
            timePoint = Time.time + timeSpaceOptions.GetRandom();
        }
    }

    public static void Activate(bool isOn)
    {
        if (cur == null) return;
        cur.gameObject.SetActive(isOn);

        if (!isOn)
        {
            var list = cur.emojiFlags;
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = false;
                cur.OffEmoji(i);
            }
        }
    }

    private void OffEmoji(int index)
    {
        emojiObjects[index].SetActive(false);
        // emojiTrans[index].SetParent(null, true);
    }

    private void PopEmoji()
    {
        var index = -1;
        var ran = Random.Range(0, emojiFlags.Length);
        for (int i = ran; i < emojiFlags.Length + ran; i++)
        {
            if (emojiFlags[i % emojiFlags.Length]) continue;
            index = i % emojiFlags.Length;
            break;
        }

        if (index == -1) return;

        var obj = emojiObjects[index];
        var trans = emojiTrans[index];

        var minion = GetMinion();

        if (minion == null) return;

        emojiFlags[index] = true;
        obj.SetActive(true);
        trans.position = minion.transform.position + (minion.IsOnBoard ? offsetFromMinionOnBoard : offsetFromMinion);
        StartCoroutine(ie_Wait());
        StartCoroutine(ie_Follow());

        return;

        IEnumerator ie_Follow()
        {
            while (emojiFlags[index] && !minion.IsPushed)
            {
                trans.position = minion.transform.position + (minion.IsOnBoard ? offsetFromMinionOnBoard : offsetFromMinion);
                yield return null;
            }
        }

        IEnumerator ie_Wait()
        {
            yield return Yielders.Get(durationOptions.GetRandom());
            obj.SetActive(false);
            emojiFlags[index] = false;
        }

        Minion GetMinion()
        {
            var candidate = new List<Minion>();
            var list = JunkPile.ins.ActiveMinions;

            for (int i = 0; i < list.Count; i++)
            {
                var m = list[i];
                if (!m.IsPushed) candidate.Add(m);
            }

            return candidate.Count == 0 ? null : candidate.GetRandom();
        }
    }
}