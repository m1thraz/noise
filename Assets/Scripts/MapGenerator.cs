using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColourMap, Mesh, FalloffMap};
	public DrawMode drawMode;

	const int mapChunkSize = 241;
	[Range(0,6)]
	public int levelOfDetail;
	public float noiseScale;

	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public float meshHeightMultiplier;
	public AnimationCurve meshHeightCurve;

	public bool autoUpdate;
	public bool useFalloff;

	public float radius = 1;
	public Vector2 regionSize = new Vector2(240,240);
	public int rejectionSamples = 30;

	public List<Vector2> points;

	public TerrainType[] regions;

    public int forestSize = 25;
    public int elementSpacing = 10;

    public Element[] elements;

    float[,] falloffMap;
	float[,] noiseMap;

	void Start()
    {
		GenerateMap();
	//	GenerateObjects();

	}

	void Awake()
    {
		falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
		points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples); // when values change in the inspector

	}

	public void GenerateMap() {
		//Generate the noise map with given parameters
		noiseMap = Noise.GenerateNoiseMap (mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
		

	
		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

		//Color the terrain based on the regions array
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				if (useFalloff) {
					noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
				}
				float currentHeight = noiseMap [x, y];
				//Debug.Log(currentHeight);
				for (int i = 0; i < regions.Length; i++) {
					if (currentHeight <= regions [i].height) {
						colourMap [y * mapChunkSize + x] = regions [i].colour;
						break;
					}
				}
			}
		}

		//GenerateObjects();

		//draw the map with the MapDisplay class
		//dependent on the type of the map
		MapDisplay display = FindObjectOfType<MapDisplay> ();
		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGenerator.TextureFromHeightMap (noiseMap));
		} else if (drawMode == DrawMode.ColourMap) {
			display.DrawTexture (TextureGenerator.TextureFromColourMap (colourMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (MeshGenerator.GenerateTerrainMesh (noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap (colourMap, mapChunkSize, mapChunkSize));
		} else if (drawMode == DrawMode.FalloffMap){
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
		}
	}
    /*
    void GenerateObjects()
    {
        //Place Objects in Map
        if (points != null)
        {
            foreach (Vector2 point in points)
            {
                //z.b baum code nur für grün
                float currentHeight = noiseMap[x, y];
                if (currentHeight > 0.55 && currentHeight < 0.7)
                {
                    for (int x = -10; x < forestSize; x += elementSpacing)
                    {
                        for (int z = -50; z < forestSize; z += elementSpacing)
                        {

                            Element element = elements[0];
                            Vector3 position = new Vector3(x, 0f, z); //0f durch terrain height ersetzen
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
        }

    }
    */
    
    //reset values if not valid
    void OnValidate() {
		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}

		falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
		points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples); // when values change in the inspector
	}
}

[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;

    public GameObject[] prefabs;

    public GameObject GetRandom()
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }
}
