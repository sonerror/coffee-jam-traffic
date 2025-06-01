using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySettingsController : MonoBehaviour
{
    void Awake()
    {
        SetQualityBasedOnRAM();
        DontDestroyOnLoad(gameObject);
    }

    void SetQualityBasedOnRAM()
    {
        #if !UNITY_EDITOR
        int ram = SystemInfo.systemMemorySize; // Get the device's RAM in MB
        // Example thresholds, adjust as needed
        if (ram >= 8000) // 8 GB or more
        {
            QualitySettings.SetQualityLevel(3); // High
        }
        else if (ram >= 6000) // 4 GB to less than 8 GB
        {
            QualitySettings.SetQualityLevel(2); // Medium
        }
        else if (ram >= 3000) // 2 GB to less than 4 GB
        {
            QualitySettings.SetQualityLevel(1); // Low
        }
        else // Less than 2 GB
        {
            QualitySettings.SetQualityLevel(0); // Very Low
        }
        Debug.Log("Quality Level set based on RAM: " + QualitySettings.GetQualityLevel());
#endif
    }
}


