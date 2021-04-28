using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject asteroid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame  
    void Update()
    {
        if (Random.Range(1, 100) < 2)
        {
            //Instantiate(asteroid, this.transform.position + new Vector3(Random.Range(-10, 10), 0, 0), Quaternion.identity);
            GameObject go = Pool.singleton.Get("asteroid");
            if (go != null)
            {
                go.SetActive(true);
                go.transform.position = transform.position + new Vector3(Random.Range(-10, 10), 0, 0);
            }
        }
    }
}
