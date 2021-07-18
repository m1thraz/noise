using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    public int forestSize = 25;
    public int elementSpacing = 10;

    public Element[] elements;


    const int mapChunkSize = 241;
    public float noiseScale = 50;
    public int seed = 5;
    public int octaves = 12;
    public float persistance = 0.5f;
    public float lacunarity = 2;
    public Vector2 offset = new Vector2(50, 0);
    public float meshHeightMultiplier = 100;
    public AnimationCurve meshHeightCurve ;


    float[,] noiseMap;

    void Awake()
    {
        noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
    }

    private void Start()
    {

        for (int x = 0; x < forestSize; x += elementSpacing)
        {
            for (int z = 0; z < forestSize; z += elementSpacing)
            {

                for(int i = 0; i < elements.Length; i++)
                {
                    Element element = elements[i];


                    if (element.CanPlace())
                    {
                        Debug.Log(-noiseMap[x,z]*25);
                        Vector3 position = new Vector3(x, meshHeightCurve.Evaluate(noiseMap[x, z]) * meshHeightMultiplier, z); //0f durch terrain height ersetzen
                        Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 65f, Random.Range(-0.75f, 0.75f));
                        Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));
                        Vector3 scale = Vector3.one * Random.Range(0.75f, 1.2f);


                        GameObject newElement = Instantiate(element.GetRandom());
                        newElement.transform.SetParent(transform);
                        newElement.transform.position = position + offset;
                        newElement.transform.eulerAngles = rotation;
                        newElement.transform.localScale = scale;
                        break;
                    }
                }

                
            }
        }
    }
}


[System.Serializable]
public class Element
{
    public string name;
    [Range(1,10)]
    public int density;
    
    public GameObject[] prefabs;

    public bool CanPlace (){
        if(Random.Range(0, 10) < density)
        
            return true;
            else
            return false;
        
    }


    public GameObject GetRandom()
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }
}