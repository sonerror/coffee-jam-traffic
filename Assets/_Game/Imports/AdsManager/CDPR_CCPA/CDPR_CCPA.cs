using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CDPR_CCPA : MonoBehaviour
{
    public static CDPR_CCPA Ins;

    public GameObject GDPR;
    public GameObject CCPA;

    public Toggle toggleAgeGDPR;
    public Toggle toggleAgeCCPA;

    private Toggle toggleAge;

    public RectTransform transContentGDPR;
    public RectTransform transContentCCPA;

    private void Awake()
    {
        Ins = this;
    }

    [HideInInspector] public bool isComplete;
    [HideInInspector] public bool isAgree;
    [HideInInspector] public bool isCalifornia;

    private bool _isDetectCountryDone;

    public void Setup()
    {
        StartCoroutine(ie_Setup());
    }

    IEnumerator ie_Setup()
    {
        isComplete = false;
        _isDetectCountryDone = false;
        transContentCCPA.anchoredPosition = Vector3.up * -1000;
        transContentGDPR.anchoredPosition = Vector3.up * -1000;
        StartCoroutine(DetectCountry());
        var waitDetech = false;

        //yield return new WaitUntil(() => InAppUpdate.Ins != null && InAppUpdate.Ins.CheckDone);

        Timer.Schedule(this, 3, () =>
        {
            Debug.LogWarning("Detech location lau qua");
            waitDetech = true;
        });

        yield return new WaitUntil(() => _isDetectCountryDone || waitDetech);

        PlayerPrefs.SetInt("IsCalifornia", isCalifornia ? 1 : 0);
        toggleAge = isCalifornia ? toggleAgeCCPA : toggleAgeGDPR;

        isComplete = false;
        if (!PlayerPrefs.HasKey("CDPR/CCPA"))
        {
            GDPR.SetActive(!isCalifornia);
            CCPA.SetActive(isCalifornia);
        }
        else
        {
            isAgree = PlayerPrefs.GetInt("CDPR/CCPA") == 1;
            GDPR.SetActive(false);
            CCPA.SetActive(false);

            isComplete = true;
        }
    }

    IEnumerator DetectCountry()
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post("http://ip-api.com/json", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ProtocolError
                || www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
                isCalifornia = false;
                _isDetectCountryDone = true;
            }
            else
            {
                Country res = JsonUtility.FromJson<Country>(www.downloadHandler.text);
                isCalifornia = res.regionName.Contains("California");
                _isDetectCountryDone = true;
            }
        }
    }

    public void Btn_Agree()
    {
        isAgree = true;
        PlayerPrefs.SetInt("CDPR/CCPA", 1);
        isComplete = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void BtnNoThanks()
    {
        isAgree = false;
        PlayerPrefs.SetInt("CDPR/CCPA", 0);
        isComplete = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Btn_Ruler2()
    {
        Application.OpenURL("https://abigames.com.vn/ct-policy.html");
    }
}

public class Country
{
    public string status;
    public string country;
    public string countryCode;
    public string region;
    public string regionName;
    public string city;
    public string zip;
    public string lat;
    public string lon;
    public string timezone;
    public string isp;
    public string org;
}