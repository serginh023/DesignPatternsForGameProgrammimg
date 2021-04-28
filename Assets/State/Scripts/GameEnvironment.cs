using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEnvironment
{
    private List<GameObject> _checkpointList = new List<GameObject>();
    public List<GameObject> CheckPoints { get { return _checkpointList; } }

    private GameObject _safePlace;

    public GameObject SafePlace { get { return _safePlace; } }

    private static GameEnvironment instance;
    public static GameEnvironment Singleton 
    { 
        get 
        { 
            if (instance == null) 
                { 
                    instance = new GameEnvironment();
                    instance.CheckPoints.AddRange(GameObject.FindGameObjectsWithTag("Checkpoint"));
                    instance.CheckPoints.OrderBy(waypoint => waypoint.name).ToList();

                    instance._safePlace = GameObject.FindGameObjectWithTag("Safe");
                    Debug.Log("teste");
                } 
            return instance; 
        } 
    }

}
