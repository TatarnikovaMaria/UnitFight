using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitTeams { people, zombie }

public class UnitManager : MonoBehaviour
{
    [System.Serializable]
    public struct UnitPrefabs
    {
        public UnitTeams team;
        public Vector2Int instantiateAreaStartGridPos;
        public Vector2Int instantiateAreaEndGridPos;
        public List<GameObject> prefabs;
    }
    public List<UnitPrefabs> prefabs;

    private List<UnitSettings> unitSettings;

    public static UnitManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            this.enabled = false;

        LoadUnitSettings();
    }

    private void Start()
    {
        LoadUnitSettings();
        CreateStartUnits();
    }

    private void LoadUnitSettings()
    {
        UnitSettingsLoader settingsLoader = new UnitSettingsLoader();
        unitSettings = settingsLoader.LoadUnitSettings("UnitSettings").UnitSettings;
    }

    private void CreateStartUnits()
    {
        CreateUnitType(Random.Range(2, 4), UnitTeams.people, "base");
        CreateUnitType(Random.Range(2, 4), UnitTeams.people, "range");
        CreateUnitType(Random.Range(2, 4), UnitTeams.zombie, "base");
        CreateUnitType(Random.Range(2, 4), UnitTeams.zombie, "range");
    }

    public void CreateUnitType(int count, UnitTeams team, string type)
    {
        GameObject prefab = null;
        GameObject go;
        BaseUnit unitScript;
        UnitSettings settings;

        UnitPrefabs unitPrefabs = prefabs.Find(t => t.team == team);
        if (unitPrefabs.prefabs != null)
        {
            prefab = unitPrefabs.prefabs.Find(t => t.name.ToLower().Contains(type.ToLower()));
        }

        if (prefab != null)
        {
            for (int i = 0; i < count; i++)
            {
                go = Instantiate(prefab, GetRandomUnitPosition(unitPrefabs), Quaternion.identity);
                go.transform.rotation = Quaternion.LookRotation(new Vector3(-go.transform.position.x, 0, 0));
                unitScript = go.GetComponent<BaseUnit>();
                settings = unitSettings.Find(t => t.team.ToLower() == team.ToString().ToLower() && t.unitType.ToLower() == type.ToLower());
                unitScript.Init(settings);
            }
        }
    }

    private Vector3 GetRandomUnitPosition(UnitPrefabs unitPrefabs)
    {
        int x = Random.Range(unitPrefabs.instantiateAreaStartGridPos.x, unitPrefabs.instantiateAreaEndGridPos.x);
        int y = Random.Range(unitPrefabs.instantiateAreaStartGridPos.y, unitPrefabs.instantiateAreaEndGridPos.y);
        return LvlManager.instance.GridPosToWorld(x, y);
    }
}
