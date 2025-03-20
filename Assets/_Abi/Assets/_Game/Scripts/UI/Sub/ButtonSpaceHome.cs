using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonSpaceHome : MonoBehaviour
{
    public static ButtonSpaceHome cur;
    public int oldScore = -1;
    public GameObject obj_fxAdd;
    public TMP_Text txt_rank;
    public TMP_Text txt_diem;
    public GameObject obj_noti;

    private void Awake()
    {
        cur = this;
    }

    private void OnEnable()
    {
        if (oldScore == -1)
        {
            oldScore = SpaceMissionDataFragment.cur.gameData.data_of_players[0].score;
        }
        else
        {
            if (SpaceMissionDataFragment.cur.gameData.startedSpace)
            {
                if (oldScore != SpaceMissionDataFragment.cur.gameData.data_of_players[0].score)
                {
                    txt_diem.text = (SpaceMissionDataFragment.cur.gameData.data_of_players[0].score - oldScore).ToString();
                    oldScore = SpaceMissionDataFragment.cur.gameData.data_of_players[0].score;
                    obj_fxAdd.SetActive(true);
                    Timer.Schedule(GrandManager.ins, 1.5f, () => { obj_fxAdd.SetActive(false); });
                }
            }
        }
    }

    public void TinhRank()
    {
        int soLuong = 0;
        if (SpaceMissionDataFragment.cur.gameData.missionIndex == 0)
        {
            soLuong = 3;
        }
        else if (SpaceMissionDataFragment.cur.gameData.missionIndex == 1)
        {
            soLuong = 4;
        }
        else if (SpaceMissionDataFragment.cur.gameData.missionIndex == 2)
        {
            soLuong = 5;
        }

        int rank = 1;
        if (SpaceMissionDataFragment.cur.gameData.startedSpace)
        {
            for (int i = 1; i < soLuong; i++)
            {
                if (SpaceMissionDataFragment.cur.gameData.data_of_players[i].score > SpaceMissionDataFragment.cur.gameData.data_of_players[0].score)
                {
                    rank++;
                }
            }
        }

        txt_rank.text = rank.ToString();
    }

    private void Update()
    {
        if (SpaceMissionDataFragment.cur.gameData.startedSpace == false)
        {
            txt_rank.text = "1";
        }

        obj_noti.SetActive(SpaceMissionDataFragment.cur.gameData.startedSpace == false && SpaceMissionDataFragment.cur.gameData.missionIndex < 3);
    }
}