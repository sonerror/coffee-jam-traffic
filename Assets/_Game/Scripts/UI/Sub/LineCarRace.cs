using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LineCarRace : MonoBehaviour
{
    public RectTransform rectTransform;
    public RectTransform childrenRect;
    public Image img_avatar;
    public Image img_frame;
    public Text txt_name;
    public Transform carTrans;
    public Transform startPos;
    public Transform endPos;
    public CanvasGroup canvasGroup;
    public Transform winPos;
    public GameObject[] obj_tops;
    public GameObject fx_win;
    public GameObject[] obj_buttonUser;
    public GameObject[] obj_buttonBot;
    private bool _isPlayer;
    public void SetUpPos(int curScore, int index)
    {
        if (curScore >= CarRaceDataFragment.MAX_SCORE)
        {
            carTrans.gameObject.SetActive(false);
            for (int i = 0; i < obj_tops.Length; i++)
            {
                obj_tops[i].SetActive(false);
            }

            int indexChest = -1;
            if (CarRaceDataFragment.cur.gameData.top1 == index)
            {
                indexChest = 0;
            }
            else if (CarRaceDataFragment.cur.gameData.top2 == index)
            {
                indexChest = 1;
            }
            else if (CarRaceDataFragment.cur.gameData.top3 == index)
            {
                indexChest = 2;
            }


            if (indexChest > -1)
            {
                if (index == 0)
                {
                    obj_buttonUser[indexChest].gameObject.SetActive(true);
                    obj_buttonUser[indexChest].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -6);
                }
                else
                {
                    obj_buttonBot[indexChest].gameObject.SetActive(true);
                    obj_buttonBot[indexChest].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -6);
                }
            }
        }
        else
        {
            float spacePerScore = (endPos.position.x - startPos.position.x) / CarRaceDataFragment.MAX_SCORE;
            carTrans.position = startPos.position + Vector3.right * spacePerScore * curScore;
        }
    }

    internal void SetUpProfile(PlayerDataRace playerDataRace)
    {
        if (playerDataRace.racePlayerType == RacePlayerType.Bot)
        {
            img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[playerDataRace.idAvatar];
            img_frame.sprite = ProfileDataFragment.cur.spr_frames[playerDataRace.idFrameAvatar];
            img_frame.SetNativeSize();
            txt_name.text = playerDataRace.userName;
            _isPlayer = false;
        }
        else
        {
            img_avatar.sprite = ProfileDataFragment.cur.spr_avatars[ProfileDataFragment.cur.gameData.idAvatar];
            img_frame.sprite = ProfileDataFragment.cur.spr_frames[ProfileDataFragment.cur.gameData.idFrame];
            img_frame.SetNativeSize();
            txt_name.text = ProfileDataFragment.cur.gameData.userName;
            _isPlayer = true;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < obj_tops.Length; i++)
        {
            obj_tops[i].transform.SetParent(childrenRect);
            obj_tops[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-60, 0);
        }

        for (int i = 0; i < 3; i++)
        {
            obj_buttonBot[i].SetActive(false);
            obj_buttonUser[i].SetActive(false);
        }

        carTrans.gameObject.SetActive(true);
    }

    public void Btn_claimUser(int index)
    {
        FrameClaimChest frameClaimChest = UIManager.ins.OpenUI<FrameClaimChest>();
        frameClaimChest.callBack = () =>
        {
            if (true)
            {
                if (CarRaceDataFragment.cur.timeLeft > 0)
                {
                    UIManager.ins.GetUICanvas<CanvasCarGrand>().gameObject.SetActive(false);
                    UIManager.ins.OpenUI<CanvasCarGrand>().Setup();
                }
                else
                {
                    UIManager.ins.GetUICanvas<CanvasCarGrand>().obj_buttonClose.SetActive(true);
                } 
            }
        };
        if (index == 0)
        {
            frameClaimChest.txt_des.text = "You finished first!";
            frameClaimChest.SetReward(new RewardChestBundle[]
            {
                new RewardChestBundle(RewardChestType.swapCar, 1),
                new RewardChestBundle(RewardChestType.vip, 1),
                new RewardChestBundle(RewardChestType.wapMinion, 1)
            }, 9, false);
        }
        else if (index == 1)
        {
            frameClaimChest.txt_des.text = "You finished second!";
            frameClaimChest.SetReward(new RewardChestBundle[]
            {
                new RewardChestBundle(RewardChestType.swapCar, 1),
                new RewardChestBundle(RewardChestType.wapMinion, 1)
            }, 10, false);
        }
        else if (index == 2)
        {
            frameClaimChest.txt_des.text = "You finished third!";
            frameClaimChest.SetReward(new RewardChestBundle[] { new RewardChestBundle(RewardChestType.wapMinion, 1) }, 11, false);
        }
    }

    public static void HandleOnClaim(int index)
    {
        CarRaceDataFragment.cur.gameData.chuaNhanThuong = false;
        CarRaceDataFragment.cur.gameData.raceIndex = Mathf.Min(CarRaceDataFragment.cur.gameData.raceIndex + 1, 20);
        CarRaceDataFragment.cur.gameData.startedRace = false;
        if (index == 0)
        {
            ResourcesDataFragment.cur.AddSwapCar(1, "space_mission");
            ResourcesDataFragment.cur.AddSwapMinion(1, "space_mission");
            ResourcesDataFragment.cur.AddVipBus(1, "space_mission");
        }
        else if (index == 1)
        {
            ResourcesDataFragment.cur.AddSwapCar(1, "space_mission");
            ResourcesDataFragment.cur.AddSwapMinion(1, "space_mission");
        }
        else if (index == 2)
        {
            ResourcesDataFragment.cur.AddSwapMinion(1, "space_mission");
        }
       
    }

    public void DisableButtons()
    {
        for (int i = 0; i < 3; i++)
        {
            obj_buttonBot[i].SetActive(false);
            obj_buttonUser[i].SetActive(false);
        }
    }
}