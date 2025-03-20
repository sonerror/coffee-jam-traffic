using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Clip", menuName = "SO/Audio Clip")]
public class AudioClipSO : ScriptableObject
{
    public static AudioClipSO ins;

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

    public List<AudioData> soundDatas;
    public List<AudioData> musicDatas;

    [System.Serializable]
    public class AudioData
    {
        public AudioClip clip;
        [Range(0, 1)] public float volume = 1;
    }
}