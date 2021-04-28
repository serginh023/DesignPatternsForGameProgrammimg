using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prototype
{
    public class AsteroidSpawner : MonoBehaviour
    {
        public Material material;
        public GameObject asteroid;


        private void Start()
        {
            CreateAsteroid();
        }

        public void CreateAsteroid()
        {
            asteroid = ProcAsteroid.Clone(this.transform.position);

            asteroid.GetComponent<MeshRenderer>().sharedMaterial = material;


        }

    }
}