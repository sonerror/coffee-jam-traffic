using AppsFlyerSDK;
using System.Collections.Generic;
using UnityEngine;

public class AppsflyerEventRegister : MonoBehaviour
{
    /// <summary>
    /// Bắn khi hoàn thành / skip tutorial
    /// </summary>
    /// <param name="tutId": Tên tutorial (hướng dẫn build, hướng dẫn shop ...)></param>
    /// <param name="isComplete": Có hoàn thành tutorial không hay ấn skip></param>
    public static void af_Tutorial_Completion(string tutId, bool isComplete)
    {
        AppsFlyer.sendEvent("af_tutorial_completion",
            new Dictionary<string, string>()
            {
                { "af_success", isComplete.ToString() },
                { "af_tutorial_id", tutId }
            });
    }

    /// <summary>
    /// Level the user achieved
    /// Cấp độ của người chơi đạt được (mỗi khi lên level)
    /// </summary>
    /// <param name="level"></param>
    /// <param name="score" -> Nếu game không tính điểm thì để là 0></param>
    public static void af_Level_Achived(int level, int score)
    {
        AppsFlyer.sendEvent("af_level_achieved",
            new Dictionary<string, string>()
            {
                { "af_level", level.ToString() },
                { "af_score", score.ToString() }
            });
    }

    /// <summary>
    /// Bắn khi mở khóa 1 achievement
    /// </summary>
    /// <param name="content_id": Tên achievement></param>
    /// <param name="af_level": Level hiện tại của player / game play hiện tại nếu là hypergame></param>
    public static void af_achievement_unlocked(string content_id, int af_level)
    {
        AppsFlyer.sendEvent("af_level_achieved",
            new Dictionary<string, string>()
            {
                { "content_id", content_id },
                { "af_level", af_level.ToString() }
            });
    }

    /// <summary>
    /// Bắn khi có mua IAP
    /// </summary>
    /// <param name="af_revenue": số tiền bán></param>
    /// <param name="af_currency": loại tiền tệ (USD, VND ...)></param>
    /// <param name="af_quantity": số lượng bán (vd: 500 gem)></param>
    /// <param name="af_content_id": loại mặt hàng bán (gem, outfit ...)></param>
    public static void af_Purchase(decimal af_revenue, string af_currency, int af_quantity, string af_content_id)
    {
        float fCost = (float)af_revenue;
        fCost *= 0.63f;
        AppsFlyer.sendEvent("af_Purchase",
            new Dictionary<string, string>()
            {
                { "af_revenue", fCost.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) },
                { "af_currency", af_currency },
                { "af_quantity", af_quantity.ToString() },
                { "af_content_id", af_content_id }
            });
    }

    public static void pass_level()
    {
        var passLevel = LevelDataFragment.cur.GetFireBaseLevel();

        if (passLevel == 10 || passLevel == 20 || passLevel == 30 || (passLevel > 30 && passLevel % 5 == 0))
        {
            AppsFlyer.sendEvent("pass_level_" + passLevel, new Dictionary<string, string>());
        }

        AppsFlyer.sendEvent("af_level_achieved", new Dictionary<string, string>()
        {
            { "level", passLevel.ToString() }
        });
    }


    #region Inter Ads

    /// <summary>
    /// Bắn lên khi ấn nút bất kỳ theo logic show inter của game 
    /// (bắn lên khi ấn nút show ads theo logic của game)
    /// Khi đủ capping time + đã load được ads
    /// </summary>
    public static void af_inters_ad_eligible()
    {
        AppsFlyer.sendEvent("af_inters_ad_eligible", new Dictionary<string, string>());
    }

    /// <summary>
    /// Bắn lên khi ấn nút bất kỳ theo logic show inter của game 
    /// (bắn lên khi ấn nút show ads theo logic của game)
    /// Khi đủ capping time + đã load được ads
    /// </summary>
    public static void af_inters_ad_missing()
    {
        AppsFlyer.sendEvent("af_inters_ad_missing", new Dictionary<string, string>());
    }

    /// <summary>
    /// Bắn lên khi check đã có sẵn ads lưu về máy thành công (bắn lên khi ads available)
    /// Trong sự kiện của mediation manager
    /// </summary>
    public static void af_inters_api_called()
    {
        AppsFlyer.sendEvent("af_inters_api_called", new Dictionary<string, string>());
    }

    /// <summary>
    /// Bắn lên khi ad hiện lên màn hình cho user xem (open inter)
    /// Trong sự kiện của mediation manager
    /// </summary>
    public static void af_inters_displayed()
    {
        AppsFlyer.sendEvent("af_inters_displayed", new Dictionary<string, string>());
    }

    #endregion

    #region Reward Ads

    /// <summary>
    /// Bắn lên khi ấn nút bất kỳ theo logic show Reward của game 
    /// (bắn lên khi ấn nút show ads theo logic của game)
    /// Khi đủ capping time + đã load được ads
    /// </summary>
    public static void af_rewarded_ad_eligible()
    {
        AppsFlyer.sendEvent("af_rewarded_ad_eligible", new Dictionary<string, string>());
    }

    /// <summary>
    /// Bắn lên khi check đã có sẵn ads lưu về máy thành công (bắn lên khi ads available)
    /// Trong sự kiện của mediation manager
    /// </summary>
    public static void af_rewarded_api_called()
    {
        AppsFlyer.sendEvent("af_rewarded_api_called", new Dictionary<string, string>());
    }

    /// <summary>
    /// Bắn lên khi ad hiện lên màn hình cho user xem (open inter)
    /// Trong sự kiện của mediation manager
    /// </summary>
    public static void af_rewarded_displayed()
    {
        AppsFlyer.sendEvent("af_rewarded_displayed", new Dictionary<string, string>());
    }

    /// <summary>
    /// Bắn lên khi User tắt ads và nhận được reward
    /// Trong sự kiện của mediation manager
    /// </summary>
    public static void af_rewarded_ad_completed()
    {
        AppsFlyer.sendEvent("af_rewarded_ad_completed", new Dictionary<string, string>());
    }

    #endregion
}