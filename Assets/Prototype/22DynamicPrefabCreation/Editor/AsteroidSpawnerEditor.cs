using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace prototype
{
    [CustomEditor(typeof(AsteroidSpawner))]
    public class AsteroidSpawnerEditor : Editor
    {

        string path;
        string assetPath;
        string fileName;

        private void OnEnable()
        {
            path = Application.dataPath + "/22DynamicPrefabCreation/Asteroid";
            assetPath = "Assets/22DynamicPrefabCreation/Asteroid/";
            fileName = "asteroid_" + System.DateTime.Now.Ticks.ToString();
        }

        public override void OnInspectorGUI()
        {
            AsteroidSpawner asteroidSpawner = (AsteroidSpawner)target;
            DrawDefaultInspector();

            if (GUILayout.Button("Create Asteroid"))
            {
                asteroidSpawner.CreateAsteroid();
            }
            if(GUILayout.Button("Save Asteroid"))
            {
                System.IO.Directory.CreateDirectory(path);
                Mesh mesh = asteroidSpawner.asteroid.GetComponent<MeshFilter>().sharedMesh;
                AssetDatabase.CreateAsset(mesh, assetPath + mesh.name + ".asset");
                AssetDatabase.SaveAssets();

                PrefabUtility.SaveAsPrefabAsset(asteroidSpawner.asteroid, assetPath + fileName + ".prefab");
            }
        }
    }
}