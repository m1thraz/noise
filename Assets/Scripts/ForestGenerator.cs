using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : MonoBehaviour
{
    public int forestSize = 25;
    public int elementSpacing = 10;

    public Element[] elements;

    private void Start()
    {

        for (int x = -10; x < forestSize; x += elementSpacing)
        {
            for (int z = -50; z < forestSize; z += elementSpacing)
            {

                Element element = elements[0];
                Vector3 position = new Vector3(x, 61.7f, z); //0f durch terrain height ersetzen
                Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
                Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));
                Vector3 scale = Vector3.one * Random.Range(0.75f, 1.2f);


                GameObject newElement = Instantiate(element.GetRandom());
                newElement.transform.SetParent(transform);
                newElement.transform.position = position + offset;
                newElement.transform.eulerAngles = rotation;
                newElement.transform.localScale = scale;
            }
        }
    }
}


[System.Serializable]
public class Element
{
    public string name;

    public GameObject[] prefabs;

    public GameObject GetRandom()
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }
}