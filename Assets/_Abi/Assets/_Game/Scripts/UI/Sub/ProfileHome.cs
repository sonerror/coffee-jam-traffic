using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileHome : MonoBehaviour
{
    public static ProfileHome cur;
    public Image img_avatar;
    public Image img_frame;

    private void Awake() {
        cur = this;
    }

    private void Start() {
        UpdateAvatar();
    }

    public void Btn_OpenPopupProfile()
    {
        AudioManager.ins.PlaySound(SoundType.UIClick);
        UIManager.ins.OpenUI<CanvasProfile>();
    }

    public void UpdateAvatar()
    {
        img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[ProfileDataFragment.cur.gameData.idAvatar];
        img_frame.sprite = ProfileDataFragment.cur.spr_frames[ProfileDataFragment.cur.gameData.idFrame];
        img_frame.SetNativeSize();
    }
}
