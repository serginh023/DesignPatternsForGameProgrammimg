using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

/// <summary>
/// Sealed: no other classes can Inherit from this
/// </summary>
public sealed class GameEnvironment
{
    private static GameEnvironment instance;
    private List<GameObject> obstacles = new List<GameObject>();
    public List<GameObject> Obstacles
    {
        get { return obstacles; }
    }

    private List<GameObject> goals = new List<GameObject>();
    public List<GameObject> Goals { get { return goals; } }


    public static GameEnvironment Singleton
    {
        get
        {
            if (instance == null)
                instance = new GameEnvironment();
            instance.goals.AddRange(GameObject.FindGameObjectsWithTag("goal"));
            return instance;
        }
    }

    public void AddObstacles(GameObject go)
    {
        obstacles.Add(go);
    }

    public void RemoveObstacles(GameObject go)
    {
        int index = obstacles.IndexOf(go);
        obstacles.RemoveAt(index);
        GameObject.Destroy(go);
    }

    public void AddGoal(GameObject go)
    {
        Goals.Add(go);
    }

    public void RemoveGoal(GameObject go)
    {
        int index = goals.IndexOf(go);
        goals.RemoveAt(index);
        GameObject.Destroy(go);
    }

    public GameObject GetRandomGoal()
    {
        int index = Random.Range(0, goals.Count-1);
        return goals[index];
    }

}
