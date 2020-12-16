using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSettingsLoader
{
    public Settings LoadUnitSettings(string fileName, bool needPrint = false)
    {
        TextAsset json = Resources.Load<TextAsset>(fileName);
        Settings settingsSet = JsonUtility.FromJson<Settings>(json.text);

        if (needPrint && settingsSet != null)
        {
            for (int i = 0; i < settingsSet.UnitSettings.Count; i++)
            {
                Debug.Log(settingsSet.UnitSettings[i].unitName
                        + ", type: " + settingsSet.UnitSettings[i].unitType
                        + ", lifes: " + settingsSet.UnitSettings[i].lifes
                        + ", moveSpeed: " + settingsSet.UnitSettings[i].moveSpeed
                        + ", rotationSpeed: " + settingsSet.UnitSettings[i].rotationSpeed
                        + ", attackDistance: " + settingsSet.UnitSettings[i].attackDistance
                        + ", attackDamage: " + settingsSet.UnitSettings[i].attackDamage
                        + ", attackDelay: " + settingsSet.UnitSettings[i].attackDelay
                        + ", bulletSpeed: " + settingsSet.UnitSettings[i].bulletSpeed
                        + ", team: " + settingsSet.UnitSettings[i].team);
            }
        }

        return settingsSet;
    }    
}

[System.Serializable]
public class UnitSettings
{
    public string unitName;
    public string unitType;
    public int lifes;
    public float moveSpeed;
    public float rotationSpeed;
    public float attackDistance;
    public int attackDamage;
    public float attackDelay;
    public float bulletSpeed;
    public string team;
}

[System.Serializable]
public class Settings
{
    public List<UnitSettings> UnitSettings;
}
