using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LineSpaceMission : MonoBehaviour
{
    public RectTransform rectTransform;
    public RectTransform childrenRect;
    public Image img_avatar;
    public Image img_frame;
    public Text txt_name;
    public Transform rocket;
    public Transform startPos;
    public Transform endPos;
    public TMP_Text txt_score;
    public SkeletonGraphic skeletonRocket;
    public CanvasGroup canvasGroup;
    public GameObject obj_frameScore;
    public Transform winPos;
    public GameObject obj_top1;
    public GameObject fx_win;

    // private void Awake()
    // {
    //     obj_frameScore.SetActive(false);
    // }

    public void ShowRocket()
    {
        skeletonRocket.transform.localPosition = new Vector3(0, 150, 0);
        skeletonRocket.transform.DOLocalMoveY(-15, 0.35f);
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.35f);


        // Play animation và lấy ra TrackEntry
        Spine.TrackEntry trackEntry = skeletonRocket.AnimationState.SetAnimation(0, "animation", true);

        // Lấy ra thời gian ngẫu nhiên trong khoảng thời gian của animation
        float randomStartTime = Random.Range(0f, trackEntry.Animation.Duration);

        // Set animation bắt đầu từ thời gian ngẫu nhiên
        trackEntry.TrackTime = randomStartTime;
    }

    public void SetUpPos(int numGame, int curScore)
    {
        float spacePerScore = (endPos.position.y - startPos.position.y) / numGame;
        rocket.position = startPos.position + Vector3.up * spacePerScore * curScore;
    }

    internal void SetUpProfile(PlayerDataSpace playerDataSpace)
    {
        if (playerDataSpace.spacePlayerType == SpacePlayerType.Bot)
        {
            img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[playerDataSpace.idAvatar];
            img_frame.sprite = ProfileDataFragment.cur.spr_frames[playerDataSpace.idFrameAvatar];
            img_frame.SetNativeSize();
            txt_name.text = playerDataSpace.userName;
        }
        else
        {
            img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[ProfileDataFragment.cur.gameData.idAvatar];
            img_frame.sprite = ProfileDataFragment.cur.spr_frames[ProfileDataFragment.cur.gameData.idFrame];
            img_frame.SetNativeSize();
            txt_name.text = ProfileDataFragment.cur.gameData.userName;
        }
    }
}