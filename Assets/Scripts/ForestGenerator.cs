using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    public int forestSize = 120;
    public int elementSpacing = 1;

    public Element[] elements;


    const int mapChunkSize = 241;
    public float noiseScale = 50;
    public int seed = 5;
    public int octaves = 12;
    public float persistance = 0.5f;
    public float lacunarity = 2;
    public Vector2 offset = new Vector2(50, 0);
    public float meshHeightMultiplier = 25;
    public AnimationCurve meshHeightCurve ;


    float[,] noiseMap;
    float[,] falloffMap;

    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
    }

    private void Start()
    {
 /*       for (int j = 0; j < mapChunkSize; j++)
        {
            for (int k = 0; k < mapChunkSize; k++)
            {
                noiseMap[j, k] = Mathf.Clamp01(noiseMap[j, k] - falloffMap[j, k]);
            }
        }*/


        for (int x = 0; x < mapChunkSize; x ++)
        {
            for (int y = 0; y < mapChunkSize; y ++)
            {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x,y] - falloffMap[x, y]);
                for (int i = 0; i < elements.Length; i++)
                {
                    Element element = elements[i];

                    if (noiseMap[x,y] >= 0.45 && noiseMap[x, y] <= 0.6) {
                        if (Random.Range(0f, 1f) <= 0.9)
                        {
                            if (element.CanPlace())
                            {
                                Debug.Log(noiseMap[x, y]);
                                Vector3 position = new Vector3((x * 10), (meshHeightCurve.Evaluate(noiseMap[x, y]) * meshHeightMultiplier) * 10, 2400 - (y * 10)); //0f durch terrain height ersetzen
                                Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
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