// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using GooglePlayGames;
// using TMPro;
// using UnityEngine;
// using UnityEngine.SocialPlatforms;
// using UnityEngine.UI;
//
// public class LeaderBoardItem : MonoBehaviour
// {
//     public new GameObject gameObject;
//
//     public TMP_Text rankText;
//     public TMP_Text nameText;
//     public TMP_Text trophyText;
//
//     public GameObject lineObject;
//
//     public Image bgImage; //different on top and middle block
//     [SerializeField] private Sprite[] topSprites;
//
//     public Image cupImage;
//
//     public void SetupOnTop(IScore score, IUserProfile userProfile, bool isPlayer = false)
//     {
//         nameText.text = userProfile != null ? userProfile.userName : score.userID;
//         trophyText.text = score.value.ToString();
//         bgImage.sprite = isPlayer ? topSprites[1] : topSprites[0];
//     }
//
//     public void SetupOnMiddle(IScore score, IUserProfile userProfile, bool isPlayer = false)
//     {
//         Enable(true);
//         rankText.text = Mathf.Min(LeaderBoardDataFragment.MaxRank, score.rank).ToString();
//         nameText.text = userProfile != null ? userProfile.userName : score.userID;
//         trophyText.text = score.value.ToString();
//         bgImage.enabled = isPlayer;
//         cupImage.sprite = CanvasLeaderBoard.cur.CupSprites[LeaderBoardDataFragment.cur.GetCupIndex(score.rank)];
//
//         // Debug.Log("SETUP MIDDLE " + score.rank);
//         // if (isPlayer)
//         // {
//         //     var data = LeaderBoardDataFragment.cur.gameData;
//         //
//         //     // var rank = data.recordedRank;
//         //     // var lastNum = data.recordedNumOfPlayers;
//         //
//         //     // var per = (float)rank / lastNum;
//         //     // LeaderboardPlayerNoff.cur.SetActive(true, per + .0001f < LeaderBoardDataFragment.PromotionPercent);
//         // }
//     }
//
//     public void Enable(bool isOn)
//     {
//         lineObject.SetActive(isOn);
//         gameObject.SetActive(isOn);
//         bgImage.enabled = isOn;
//     }
//
//     public void ClearWhenTop()
//     {
//         nameText.text = "";
//         trophyText.text = "";
//     }
//
//     public void ClearWhenMiddle()
//     {
//         Enable(false);
//     }
// }