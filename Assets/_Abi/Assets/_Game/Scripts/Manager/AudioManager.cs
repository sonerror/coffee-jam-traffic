using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;
using DG.Tweening;

public class AudioManager : SingleTons<AudioManager>
{
    public AudioSource SoundAudioSource;
    public AudioSource MusicAudioSource;

    private AudioClipSO audioClipSO;

    protected override void Awake()
    {
        base.Awake();
        if (!isAwakable) return;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => AssetHolder.ins != null);
        MMVibrationManager.iOSInitializeHaptics();
        audioClipSO = AudioClipSO.ins;
        UpdateAudio();
    }

    public void UpdateAudio()
    {
        SoundAudioSource.enabled = DataManager.ins.gameData.isSoundOn;
        MusicAudioSource.enabled = DataManager.ins.gameData.isMusicOn;
    }

    public void SetSoundState(bool isOn)
    {
        SoundAudioSource.enabled = isOn;
        DataManager.ins.gameData.isSoundOn = isOn;
    }

    public void SetMusicState(bool isOn)
    {
        MusicAudioSource.enabled = isOn;
        DataManager.ins.gameData.isMusicOn = isOn;
    }

    public void SetVibrateState(bool isOn)
    {
        DataManager.ins.gameData.isVibrateOn = isOn;
    }

    public void PlaySound(SoundType type)
    {
        var ad = audioClipSO.soundDatas[(int)type];
        SoundAudioSource.PlayOneShot(ad.clip, ad.volume);
    }

    public void PlayMusic(MusicType type)
    {
        var ad = audioClipSO.musicDatas[(int)type];
        MusicAudioSource.clip = ad.clip;
        // CurMusicVolumeScale = ad.volume;
        // MusicAudioSource.volume = CurMusicVolumeScale * DataManager.ins.gameData.musicVolume;
        MusicAudioSource.Play();
    }

    public void MakeVibrate(HapticTypes type = HapticTypes.MediumImpact)
    {
        if (DataManager.ins.gameData.isVibrateOn)
            MMVibrationManager.Haptic(type);
    }
}

public enum SoundType
{
    Lose,
    Win,
    UIClick,
    SpinTick,
    SpinEnd,
    ClaimStuff,
    BusCollide,
    BusClicked,
    HardLevel,
    SuperHardLevel,
    MinionOnBoard,
    NotEnoughMoney,
    GainObjective,
    PopMoney,
    PopChestGift,
    PopItem,
    TreasureStart,
    BoosterRefreshStart,
    TextUp,
    TextDown,
}

public enum MusicType
{
    BGM
}