using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourEx
{
    public static GameManager Instance;
    public static Map map;
    public static List<MapObject> mapObjects = new List<MapObject>();    
    public static Pathfinding pathfinding;
    public Transform mapParenTransform;
    public Transform ui;

    public float SpawnInterwal
    {
        get {
            if (PlayerPrefs.GetFloat("SpawnInterwal") > 0)
                return PlayerPrefs.GetFloat("SpawnInterwal");
            else
                return 5;
        }
        set { PlayerPrefs.SetFloat("SpawnInterwal", value); }
    }

    public float InGasStationTime
    {
        get {
            if (PlayerPrefs.GetFloat("InGasStationTime") > 0)
                return PlayerPrefs.GetFloat("InGasStationTime");
            else
                return 5;
        }
        set { PlayerPrefs.SetFloat("InGasStationTime", value); }
    }

    void Awake()
    {
        Instance = this;
        pathfinding = new Pathfinding();
        map = new Map(20); //20-map size
        map.GenerateMap();
        pathfinding.Init();
        Instantiate(Resources.Load("Car"), GameManager.Instance.mapParenTransform);
        AddTimer(SpawnInterwal, () => Instantiate(Resources.Load("Car"), GameManager.Instance.mapParenTransform), true);
    }

    public void MenuBtn()
    {
        SceneManager.LoadScene("menu");
    }
}