using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{

    //variables for the forest
    public int trees = 120;
    public int spacing = 1;

    public Element[] elements;


    //variables for the mesh (needed for the right positioning)
    const int mapChunkSize = 241;
    public float noiseScale = 50;
    public int seed = 5;
    public int octaves = 12;
    public float persistance = 0.5f;
    public float lacunarity = 2;
    public Vector2 offset = new Vector2(50, 0);
    public float meshHeightMultiplier = 25;
    public AnimationCurve meshHeightCurve;


    float[,] noiseMap;
    float[,] falloffMap;

    //awaken the NoiseMap and FalloffGenerator in order to let this class know where it can places the Elements

    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
        noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
    }
    //generating the plants when play got pressed
    private void Start()
    {

        for (int x = 0; x < mapChunkSize; x++)
        {
            for (int y = 0; y < mapChunkSize; y++)
            {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);    //generating the noisemap
                for (int i = 0; i < elements.Length; i++)
                {
                    Element element = elements[i];

                    if (noiseMap[x, y] >= 0.45 && noiseMap[x, y] <= 0.6)
                    {          // if noisemap height is between 0.45 and 0.6 (green areas) the elements can be placed
                        if (Random.Range(0f, 1f) <= 0.9)
                        {
                            if (element.CanPlace())
                            {
                                Debug.Log(noiseMap[x, y]);
                                Vector3 position = new Vector3((x * 10), (meshHeightCurve.Evaluate(noiseMap[x, y]) * meshHeightMultiplier) * 10, 2400 - (y * 10)); // places elements in the right spots
                                Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));                                        // variation to make the forest look more natural
                                Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));                                   //rotates the Elements randomly
                                Vector3 scale = Vector3.one * Random.Range(0.75f, 1.2f);                                                                           //gives the Elements a random scale


                                GameObject newElement = Instantiate(element.GetRandom());           //instantiate a random elemt from the prefab array
                                newElement.transform.SetParent(transform);                          // setting the parrent so´the scene tab looks more clean
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

// Classs for the Elemets
[System.Serializable]
public class Element
{
    public string name;     //name of the elemet
    [Range(1, 10)]
    public int density;     //how dense should the elements placed (1-10)

    public GameObject[] prefabs;

    // function for checking if there is another object
    public bool CanPlace()
    {
        if (Random.Range(0, 10) < density)

            return true;
        else
            return false;

    }

    //get a random object from the prefabs array
    public GameObject GetRandom()
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }
}