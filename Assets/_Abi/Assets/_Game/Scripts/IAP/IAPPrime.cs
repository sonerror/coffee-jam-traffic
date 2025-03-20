using UnityEngine;
using System;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing.Security;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Globalization;
using _Game.Scripts.Bus;

[Serializable]
public class NonConsumableItem
{
    public string id;
}

[Serializable]
public class ConsumableItem
{
    public string id;
}

public class IAPPrime : MonoBehaviour, IDetailedStoreListener
{
    public static IAPPrime ins;
    private IStoreController _storeController;
#if UNITY_IOS
    private IExtensionProvider _extension;
#endif
    private bool autoRestore = true;
    public bool initialized { get; private set; }

    public bool isFireAFEvent;
    private bool isNeedToBuild = true;

    public static string[] packIds
    {
        get
        {
            return new string[]
            {
                REMOVE_ADS,
                REVIVE_PACK,
                BOOSTER_PACK,
                STARTER_PACK,
                MINI_TICKETS, SMALL_TICKETS, MEDIUM_TICKETS, BIG_TICKETS, SUPER_TICKETS, MEGA_TICKETS,
                REMOVE_ADS_BUNDLE, NEVER_GIVE_UP,
                PROFESSIONAL_BUNDLE, MASTER_BUNDLE, MEGA_BUNDLE,
                HALLOWEEN_BUNDLE,
                VIP_PASS_3, VIP_PASS_7, VIP_PASS_15,
            };
        }
    }

    // các gói đặc biệt
    public const string REMOVE_ADS = "com.busjam.removeads";
    public const string REVIVE_PACK = "com.busjam.revivepack";
    public const string BOOSTER_PACK = "com.busjam.boosterpack";
    public const string STARTER_PACK = "com.busjam.starterpack";

    public const string MINI_TICKETS = "com.busjam.minitickets";
    public const string SMALL_TICKETS = "com.busjam.smalltickets";
    public const string MEDIUM_TICKETS = "com.busjam.mediumtickets";
    public const string BIG_TICKETS = "com.busjam.bigtickets";
    public const string SUPER_TICKETS = "com.busjam.supertickets";
    public const string MEGA_TICKETS = "com.busjam.megatickets";

    public const string REMOVE_ADS_BUNDLE = "com.busjam.removeadsbundle";
    public const string NEVER_GIVE_UP = "com.busjam.nevergiveup";

    public const string PROFESSIONAL_BUNDLE = "com.busjam.profressionalbundle";
    public const string MASTER_BUNDLE = "com.busjam.masterbundle";
    public const string MEGA_BUNDLE = "com.busjam.megabundle";

    public const string HALLOWEEN_BUNDLE = "com.busjam.halloween";

    public const string VIP_PASS_3 = "com.busjam.vip3";
    public const string VIP_PASS_7 = "com.busjam.vip7";
    public const string VIP_PASS_15 = "com.busjam.vip15";

    private Action successAction = null;

    private string curPlacement = "AUTO_RESTORE";
    private const string IAP_FIRST = "IAP_first_Purchase";

    private void Awake()
    {
        ins = this;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        ins = this;
    }
#endif

    private async void Start()
    {
        try
        {
            var options = new InitializationOptions()
#if UNITY_EDITOR
                .SetEnvironmentName("test");
#else
                .SetEnvironmentName("production");
#endif
            await UnityServices.InitializeAsync(options);
        }
        catch (System.Exception)
        {
            // ignored
        }

        Timer.ScheduleCondition(() => DataManager.ins.IsDoneLoadData, SetupBuilder);
    }

    public void ClickPurchase(string id, string placement = "", Action completeActionOnSuccess = null)
    {
        id = GetNewIdFromOldId(id);
        var fakeIap = false;
        if (fakeIap)
        {
            curPlacement = placement;
            successAction = completeActionOnSuccess;
            autoRestore = false;

            FakeHandleIAP(id);
            return;
        }
        if (!initialized) return;
        curPlacement = placement;
        successAction = completeActionOnSuccess;
        autoRestore = false;
        /*
        ProcessPurchase(new PurchaseEventArgs())
        */
        _storeController.InitiatePurchase(id);
    }

    public string GetNewIdFromOldId(string oldId)
    {
        return oldId.Replace("busjam", "packingrush");
    }

    public string GetOldIdFromNewId(string newId)
    {
        return newId.Replace("packingrush", "busjam");
    }
    
    private void SetupBuilder()
    {
        if (!isNeedToBuild) return;
        isNeedToBuild = false;
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));

        builder.AddProduct(GetNewIdFromOldId(REMOVE_ADS), ProductType.NonConsumable);
        builder.AddProduct(GetNewIdFromOldId(REVIVE_PACK), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(BOOSTER_PACK), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(STARTER_PACK), ProductType.Consumable);

        builder.AddProduct(GetNewIdFromOldId(MINI_TICKETS), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(SMALL_TICKETS), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(MEDIUM_TICKETS), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(BIG_TICKETS), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(SUPER_TICKETS), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(MEGA_TICKETS), ProductType.Consumable);

        builder.AddProduct(GetNewIdFromOldId(REMOVE_ADS_BUNDLE), ProductType.NonConsumable);
        builder.AddProduct(GetNewIdFromOldId(NEVER_GIVE_UP), ProductType.Consumable);

        builder.AddProduct(GetNewIdFromOldId(PROFESSIONAL_BUNDLE), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(MASTER_BUNDLE), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(MEGA_BUNDLE), ProductType.Consumable);

        /*
        builder.AddProduct(GetNewIdFromOldId(HALLOWEEN_BUNDLE), ProductType.Consumable);
        */

        builder.AddProduct(GetNewIdFromOldId(VIP_PASS_3), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(VIP_PASS_7), ProductType.Consumable);
        builder.AddProduct(GetNewIdFromOldId(VIP_PASS_15), ProductType.Consumable);

        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        initialized = true;
        _storeController = controller;
#if UNITY_IOS
        _extension = extensions;
    }

    public void OnClickRestore()
    {
        _extension.GetExtension<IAppleExtensions>().RestoreTransactions(((b, s) => { }));
#endif
    }

    // processing purchase
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Product product = purchaseEvent.purchasedProduct;
        Debug.Log("ProcessPurchase complete " + product.definition.id);
        var productId = product.definition.id;
        Debug.Log($"isAutoRestore : {autoRestore}");
        Debug.Log($"isFireAfEvent : {isFireAFEvent}");

        if (autoRestore == false) // real purchase not auto restore
        {
            if (isFireAFEvent)
            {
                FirebaseManager.Ins.iapPurchased(productId, GetPackageName(productId), curPlacement);
                VerifyIAPM(product);
                FirebaseManager.Ins.CheckPurchasedValue((float)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);
            }
        }

        switch (GetOldIdFromNewId(productId))
        {
            case REMOVE_ADS:
                OnRemoveAds();
                break;
            case REVIVE_PACK:
                OnRevivePack();
                break;
            case BOOSTER_PACK:
                OnBoosterPack();
                break;
            case STARTER_PACK:
                OnStarterPack();
                break;

            case MINI_TICKETS:
                OnTicketBundle(250);
                break;
            case SMALL_TICKETS:
                OnTicketBundle(1100);
                break;
            case MEDIUM_TICKETS:
                OnTicketBundle(2250);
                break;
            case BIG_TICKETS:
                OnTicketBundle(5000);
                break;
            case SUPER_TICKETS:
                OnTicketBundle(10500);
                break;
            case MEGA_TICKETS:
                OnTicketBundle(22000);
                break;

            case REMOVE_ADS_BUNDLE:
                OnRemoveAdsBundle();
                break;
            case NEVER_GIVE_UP:
                OnNeverGiveUp();
                break;

            case PROFESSIONAL_BUNDLE:
                OnProfessionalBundle();
                break;
            case MASTER_BUNDLE:
                OnMasterBundle();
                break;
            case MEGA_BUNDLE:
                OnMegaBundle();
                break;

            case HALLOWEEN_BUNDLE:
                OnHalloweenBundle();
                break;

            case VIP_PASS_3:
                OnVipPass_3();
                break;
            case VIP_PASS_7:
                OnVipPass_7();
                break;
            case VIP_PASS_15:
                OnVipPass_15();
                break;
        }

        successAction?.Invoke();

        return PurchaseProcessingResult.Complete;

        void VerifyIAPM(Product pr)
        {
#if UNITY_EDITOR
            return;
#endif
            // Debug.LogWarning("verify" + pr.metadata.localizedDescription);
            bool _isvalid = true;
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);
            try
            {
                var result = validator.Validate(pr.receipt);
                foreach (IPurchaseReceipt productReceipt in result)
                {
                     Debug.Log("af_Purchase_abi" + productReceipt.productID);
                     Debug.Log("af_Purchase_abi" + productReceipt.purchaseDate);
                     Debug.Log("af_Purchase_abi" + productReceipt.transactionID);
                }
            }
            catch (IAPSecurityException ex)
            {
                _isvalid = false;
                Debug.LogException(ex);

            }

            if (_isvalid)
            {
                Debug.Log("af_Purchase_abi verifyed");
                FirebaseManager.Ins.iapPurchased_confirmed(productId, GetPackageName(productId), curPlacement);
                AppsflyerEventRegister.af_Purchase(product.metadata.localizedPrice, product.metadata.isoCurrencyCode, 1, productId);
            }
        }
    }


    private PurchaseProcessingResult FakeHandleIAP(string id)
    {
        var productId = id;
        productId = GetOldIdFromNewId(id);
        #if UNITY_EDITOR
        Debug.LogError(productId);
#endif
        if (autoRestore == false) // real purchase not auto restore
        {
            if (isFireAFEvent)
            {
                /*
                FirebaseManager.Ins.iapPurchased(productId, GetPackageName(productId), curPlacement);
                */
                /*
                VerifyIAPM(product);
                */
                /*
                FirebaseManager.Ins.CheckPurchasedValue((float)product.metadata.localizedPrice, product.metadata.isoCurrencyCode);
            */
            }
        }
 
        switch (productId)
        {
            case REMOVE_ADS:
                OnRemoveAds();
                break;
            case REVIVE_PACK:
                OnRevivePack();
                break;
            case BOOSTER_PACK:
                OnBoosterPack();
                break;
            case STARTER_PACK:
                OnStarterPack();
                break;
            case MINI_TICKETS:
                OnTicketBundle(250);
                break;
            case SMALL_TICKETS:
                OnTicketBundle(1100);
                break;
            case MEDIUM_TICKETS:
                OnTicketBundle(2250);
                break;
            case BIG_TICKETS:
                OnTicketBundle(5000);
                break;
            case SUPER_TICKETS:
                OnTicketBundle(10500);
                break;
            case MEGA_TICKETS:
                OnTicketBundle(22000);
                break;

            case REMOVE_ADS_BUNDLE:
                OnRemoveAdsBundle();
                break;
            case NEVER_GIVE_UP:
                OnNeverGiveUp();
                break;

            case PROFESSIONAL_BUNDLE:
                OnProfessionalBundle();
                break;
            case MASTER_BUNDLE:
                OnMasterBundle();
                break;
            case MEGA_BUNDLE:
                OnMegaBundle();
                break;

            case HALLOWEEN_BUNDLE:
                OnHalloweenBundle();
                break;

            case VIP_PASS_3:
                OnVipPass_3();
                break;
            case VIP_PASS_7:
                OnVipPass_7();
                break;
            case VIP_PASS_15:
                OnVipPass_15();
                break;
        }

        successAction?.Invoke();

        return PurchaseProcessingResult.Complete;

        void VerifyIAPM(Product pr)
        {
#if UNITY_EDITOR
            return;
#endif
            // Debug.LogWarning("verify" + pr.metadata.localizedDescription);
            bool _isvalid = true;
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);
            try
            {
                var result = validator.Validate(pr.receipt);
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    // Debug.Log("af_Purchase_abi" + productReceipt.productID);
                    // Debug.Log("af_Purchase_abi" + productReceipt.purchaseDate);
                    // Debug.Log("af_Purchase_abi" + productReceipt.transactionID);
                }
            }
            catch (IAPSecurityException)
            {
                _isvalid = false;
            }

            if (_isvalid)
            {
                Debug.Log("af_Purchase_abi verifyed");
                /*
                FirebaseManager.Ins.iapPurchased_confirmed(productId, GetPackageName(productId), curPlacement);
                */
                /*
                AppsflyerEventRegister.af_Purchase(product.metadata.localizedPrice, product.metadata.isoCurrencyCode, 1, productId);
            */
            }
        }
    }
    
    public string GetPackageName(string productId)
    {
        return productId switch
        {
            REMOVE_ADS => "REMOVE_ADS",
            REVIVE_PACK => "REVIVE_PACK",
            BOOSTER_PACK => "BOOSTER_PACK",
            STARTER_PACK => "STARTER_PACK",

            MINI_TICKETS => "MINI_TICKETS",
            SMALL_TICKETS => "SMALL_TICKETS",
            MEDIUM_TICKETS => "MEDIUM_TICKETS",
            BIG_TICKETS => "BIG_TICKETS",
            SUPER_TICKETS => "SUPER_TICKETS",
            MEGA_TICKETS => "MEGA_TICKETS",

            REMOVE_ADS_BUNDLE => "REMOVE_ADS_BUNDLE",
            NEVER_GIVE_UP => "NEVER_GIVE_UP",

            PROFESSIONAL_BUNDLE => "PROFESSIONAL_BUNDLE",
            MASTER_BUNDLE => "MASTER_BUNDLE",
            MEGA_BUNDLE => "MEGA_BUNDLE",

            HALLOWEEN_BUNDLE => "HALLOWEEN_BUNDLE",

            VIP_PASS_3 => "VIP_PASS_3",
            VIP_PASS_7 => "VIP_PASS_7",
            VIP_PASS_15 => "VIP_PASS_15",

            _ => "",
        };
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        // GameManager.instance.popupPurchaseFail.gameObject.SetActive(true);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        isNeedToBuild = true;
        Timer.ScheduleSupreme(60, SetupBuilder);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        isNeedToBuild = true;
        Timer.ScheduleSupreme(64, SetupBuilder);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
    }

    // support
    public string GetLocalPrice(string id)
    {
        Product product = _storeController.products.WithID(id);
        return product.metadata.localizedPriceString;
    }

    public (string, string) GetLocalPriceSupreme(string id)
    {
        if (id == "") return ("", "");
        id = GetNewIdFromOldId(id);
        Product product = _storeController.products.WithID(id);

        // var ri = new RegionInfo(System.Threading.Thread.CurrentThread.CurrentUICulture.LCID);
        var isSymbolAtStart = IsSymbolAtStart(product.metadata.localizedPriceString);
        var currencyCode = product.metadata.isoCurrencyCode;
        // var price = product.metadata.localizedPrice.ToString(CultureInfo.CurrentCulture);
        var price = TrimSymbol(product.metadata.localizedPriceString, isSymbolAtStart);

        if (price.StartsWith(" ")) price.Remove(0, 1);
        if (price.EndsWith(" ")) price.Remove(price.Length - 1, 1);

        return isSymbolAtStart ? (currencyCode, price) : (price, currencyCode);
    }

    private string[] checkNum = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    private bool IsSymbolAtStart(string s)
    {
        for (int i = 0; i < checkNum.Length; i++)
        {
            if (s.StartsWith(checkNum[i])) return false;
        }

        return true;
    }

    private string TrimSymbol(string s, bool isSymbolAtStart)
    {
        if (isSymbolAtStart) return s.Remove(0, 1);
        return s.Remove(s.Length - 1, 1);
    }

    private void OnRemoveAds()
    {
        AdsManager.isNoAds = true;
        PlayerPrefs.SetInt("NoAds", 1);

        AdsManager.Ins.HideBanner();

        CanvasHome.CheckNoAdsOfferStatic();
        CanvasHome.CheckRemoveAdsBundleStatic();
        CanvasIapShop.CheckContent();

        CanvasBannerOff.CheckActive();
    }

    private void OnRevivePack()
    {
        ResourcesDataFragment.cur.AddGold(1000, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(5, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(5, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    private void OnBoosterPack()
    {
        ResourcesDataFragment.cur.AddSwapCar(2, "IAP");
        ResourcesDataFragment.cur.AddVipBus(2, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(2, "IAP");
    }

    private void OnStarterPack()
    {
        if (BuyingPackDataFragment.cur.gameData.isStarterPackBought)
        {
            CanvasHome.CheckStarterPackStatic();
            return;
        }

        BuyingPackDataFragment.cur.gameData.isStarterPackBought = true;

        ResourcesDataFragment.cur.AddGold(200, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(1, "IAP");
        ResourcesDataFragment.cur.AddVipBus(1, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(1, "IAP");

        if (!autoRestore) CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);

        CanvasHome.CheckStarterPackStatic();
        CanvasIapShop.CheckContent();
    }

    private void OnTicketBundle(int amount)
    {
        ResourcesDataFragment.cur.AddGold(amount, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    private void OnRemoveAdsBundle()
    {
        AdsManager.isNoAds = true;
        PlayerPrefs.SetInt("NoAds", 1);

        AdsManager.Ins.HideBanner();

        ResourcesDataFragment.cur.AddGold(500, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(1, "IAP");
        ResourcesDataFragment.cur.AddVipBus(1, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(1, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);

        CanvasHome.CheckNoAdsOfferStatic();
        CanvasHome.CheckRemoveAdsBundleStatic();
        CanvasIapShop.CheckContent();

        CanvasBannerOff.CheckActive();
    }

    private void OnNeverGiveUp()
    {
        ResourcesDataFragment.cur.AddGold(400, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(2, "IAP");
        ResourcesDataFragment.cur.AddVipBus(2, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(2, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);

        BuyingPackDataFragment.cur.OnBuyNGUPackTimeHandle();
        CanvasHome.CheckNeverGiveUpBundleStatic();
    }

    private void OnProfessionalBundle()
    {
        ResourcesDataFragment.cur.AddGold(1100, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(2, "IAP");
        ResourcesDataFragment.cur.AddVipBus(2, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(2, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    private void OnMasterBundle()
    {
        ResourcesDataFragment.cur.AddGold(5000, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(8, "IAP");
        ResourcesDataFragment.cur.AddVipBus(8, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(8, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    private void OnMegaBundle()
    {
        ResourcesDataFragment.cur.AddGold(17500, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(15, "IAP");
        ResourcesDataFragment.cur.AddVipBus(15, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(15, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);
    }

    private void OnHalloweenBundle()
    {
        if (BuyingPackDataFragment.cur.gameData.isHalloweenPackBought)
        {
            CanvasHome.CheckHalloweenPackStatic();
            return;
        }

        BuyingPackDataFragment.cur.gameData.isHalloweenPackBought = true;

        ResourcesDataFragment.cur.AddGold(350, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(1, "IAP");
        ResourcesDataFragment.cur.AddVipBus(1, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(1, "IAP");

        CanvasFloatingStuff.cur.PopItemStuff(FloatingEffectType.GoldSplash, Vector2.zero);

        CanvasHome.CheckHalloweenPackStatic();
    }

    private void OnVipPass_3()
    {
        ResourcesDataFragment.cur.AddGold(250, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(1, "IAP");
        ResourcesDataFragment.cur.AddVipBus(1, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(1, "IAP");

        VipPassDataFragment.cur.SetVip_3();

        IapShopVipBox.CheckStatic(IapShopVipBox.VipType.Vip_3);
        UIManager.ins.OpenUI<CanvasVipNoffGift>().Setup(IapShopVipBox.VipType.Vip_3);

        AdsManager.Ins.CheckBanner();

        if (GrandManager.ins.IsGame) ParkingLot.cur.UnlockSlotAfterBuyingVipPack();
    }

    private void OnVipPass_7()
    {
        ResourcesDataFragment.cur.AddGold(1000, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(2, "IAP");
        ResourcesDataFragment.cur.AddVipBus(2, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(2, "IAP");

        VipPassDataFragment.cur.SetVip_7();

        IapShopVipBox.CheckStatic(IapShopVipBox.VipType.Vip_7);
        UIManager.ins.OpenUI<CanvasVipNoffGift>().Setup(IapShopVipBox.VipType.Vip_7);

        AdsManager.Ins.CheckBanner();

        if (GrandManager.ins.IsGame) ParkingLot.cur.UnlockSlotAfterBuyingVipPack();
    }

    private void OnVipPass_15()
    {
        ResourcesDataFragment.cur.AddGold(2250, "IAP");
        ResourcesDataFragment.cur.AddSwapCar(5, "IAP");
        ResourcesDataFragment.cur.AddVipBus(5, "IAP");
        ResourcesDataFragment.cur.AddSwapMinion(5, "IAP");

        VipPassDataFragment.cur.SetVip_15();
        VipPassDataFragment.cur.gameData.isEverBuyLargestPack = true;

        IapShopVipBox.CheckStatic(IapShopVipBox.VipType.Vip_15);
        UIManager.ins.OpenUI<CanvasVipNoffGift>().Setup(IapShopVipBox.VipType.Vip_15);

        AdsManager.Ins.CheckBanner();

        if (GrandManager.ins.IsGame) ParkingLot.cur.UnlockSlotAfterBuyingVipPack();
    }
}