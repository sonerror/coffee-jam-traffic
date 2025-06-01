using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardChest : MonoBehaviour
{
    public Transform _transform;

    public GameObject obj_tickets;
    public TMP_Text txt_ticket;
    public GameObject obj_sort;
    public TMP_Text txt_sort;
    public GameObject obj_vip;
    public TMP_Text txt_vip;
    public GameObject obj_refresh;
    public TMP_Text txt_refresh;
    public Transform currentReward;

    public void Setup(RewardChestType rewardChestType, int quantity)
    {
        obj_tickets.SetActive(rewardChestType == RewardChestType.ticket);
        obj_sort.SetActive(rewardChestType == RewardChestType.swapCar);
        obj_vip.SetActive(rewardChestType == RewardChestType.vip);
        obj_refresh.SetActive(rewardChestType == RewardChestType.wapMinion);
        
        if(rewardChestType == RewardChestType.ticket)
        {
            currentReward = obj_tickets.transform;
            txt_ticket.text = "x" + quantity.ToString();
        }
        else if(rewardChestType == RewardChestType.swapCar)
        {
            currentReward = obj_sort.transform;
            txt_sort.text = "x" + quantity.ToString();
        }
        else if(rewardChestType == RewardChestType.vip)
        {
            currentReward = obj_vip.transform;
            txt_vip.text = "x" + quantity.ToString();
        }
        else if(rewardChestType == RewardChestType.wapMinion)
        {
            currentReward = obj_refresh.transform;
            txt_refresh.text = "x" + quantity.ToString();
        }
    }
}

public enum RewardChestType
{
    ticket,
    swapCar,
    vip,
    wapMinion
}

[Serializable]
public class RewardChestBundle
{
    public RewardChestType rewardChestType;
    public int quantity;

    public RewardChestBundle(RewardChestType _rewardChestType, int _quantity)
    {
        rewardChestType = _rewardChestType;
        quantity = _quantity;
    }
}
