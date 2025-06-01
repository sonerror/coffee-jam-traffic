using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonRaceHome : MonoBehaviour
{
    public static ButtonRaceHome cur;
    public int oldScore = -1;
    public GameObject obj_fxAdd;
    public TMP_Text txt_rank;
    public GameObject obj_noti;

    private void Awake()
    {
        cur = this;
    }

    private void OnEnable()
    {
        if (oldScore == -1)
        {
            oldScore = CarRaceDataFragment.cur.gameData.data_of_players[0].score;
        }
        else
        {
            if (CarRaceDataFragment.cur.gameData.startedRace)
            {
                if (oldScore < CarRaceDataFragment.cur.gameData.data_of_players[0].score)
                {
                    oldScore = CarRaceDataFragment.cur.gameData.data_of_players[0].score;
                    obj_fxAdd.SetActive(true);
                    Timer.Schedule(GrandManager.ins, 1.5f, () => { obj_fxAdd.SetActive(false); });
                }
            }
        }

        
    }

    public void TinhRank()
    {
        int rank = 1;
        if (CarRaceDataFragment.cur.gameData.startedRace)
        {
            for (int i = 1; i < 5; i++)
            {
                if (CarRaceDataFragment.cur.gameData.data_of_players[i].score > CarRaceDataFragment.cur.gameData.data_of_players[0].score)
                {
                    rank++;
                }
            }
        }

        txt_rank.text = rank.ToString();
    }

    private void Update()
    {
        if(CarRaceDataFragment.cur.gameData.startedRace == false)
        {
            txt_rank.text = "1";
        }
        obj_noti.SetActive(CarRaceDataFragment.cur.gameData.startedRace == false && CarRaceDataFragment.cur.gameData.raceIndex < 3);
    }
}