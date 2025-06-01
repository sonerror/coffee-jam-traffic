using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChooseAvatar : MonoBehaviour
{
    public int id;
    public Image img_avatar;
    public Image img_frame;
    public Image img_selecting;
    
    public int idAvatar;
    public int idFrame;

    public void SetUp()
    {
        img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[idAvatar];
        img_frame.sprite = ProfileDataFragment.cur.spr_frames[idFrame];
        img_frame.SetNativeSize();
    }

    public void Btn_clickChooseThis()
    {
        ProfileDataFragment.cur.gameData.idChoose = id;
        ProfileDataFragment.cur.gameData.idAvatar = idAvatar;
        ProfileDataFragment.cur.gameData.idFrame = idFrame;

        ProfileHome.cur.UpdateAvatar();
        CanvasProfile.cur.UpdateAvatar();
    }
}
