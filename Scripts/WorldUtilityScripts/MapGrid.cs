using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapGrid : MonoBehaviour
{
    public string savePath = "L:/AISurvivialGame/SaveData"; // Path to save folder... Must contain correct sub-folders

    [Header("Terrain Settings")]
    [SerializeField]
    public Terrain terrain; // set this to current terrain to be generated in World Generator script
    public int TerrainSize = 1000;// Terrain size
    public int TerrainWidth = 513; // Terrain Height map Width
    public int TerrainLength = 513;// Terrain Height map Length
    public int TerrainHeight = 500; // Terrain max Height
    public int grassDensity = 2048;
    public int patchDetail = 64;
    int gridSize;
    [SerializeField]
    public float scale = .5f; // resolution of maps - probably shouldn't change
    [SerializeField]
    float islandScale = 1f; // island size 

    [Header("Island Seeds")]
    public bool RandomizeSeed = true;
    public float seedx;
    public float seedy;
    public float seedScale;
    public float[] seedBiome;
    public float noIsland = 0.5f; //chance that no island is generated // all ocean on terrain
    bool isOcean;

    [Header("Map Files")]
    [SerializeField]
    public Texture2D BiomeMap;
    [SerializeField]
    public Texture2D ObjectMap;

    //ObjectMap Colors
    Color32 TreeC = new Color32(0, 0, 0, 255);
    Color32 BushC = new Color32(255, 0, 0, 255);
    Color32 RockC = new Color32(0, 255, 0, 255);
    Color32 OtherC = new Color32(0, 0, 255, 255);

    //Biome Map Colors
    Color32 OceanC = new Color32(255, 0, 0, 255);
    Color32 BeachC = new Color32(255, 255, 0, 255);
    Color32 RiverC = new Color32(0, 255, 0, 255);
    Color32 PlanesC = new Color32(0, 255, 255, 255);
    Color32 ForestC = new Color32(255, 0, 255, 255);
    Color32 MountainsC = new Color32(0, 0, 255, 255);



    [System.Serializable]
    public class BiomeGenSettings
    {
        public float baseHeight; // base biome height
        public float jaggedness; // roughness of terrain
        
    };


    [System.Serializable]
    public class BiomeSettings //0 = 0% per pixel, 1 = 100% per pixel
    {
        public float treeDensity;
        public float bushDensity;
        public float rockDensity;
        public float otherDensity;
        public float grassDensity;
    };

    [System.Serializable]
    public class BiomeObjects{
        public GameObject[] Trees;
        public GameObject[] Bushes;
        public GameObject[] Rocks;
        public GameObject[] Other;
        public BiomeSettings Settings;
        public BiomeGenSettings terrSettings;
    };

    [System.Serializable]
    public enum Biome
    {
        Ocean,
        River,
        Beach,
        Planes,
        Forest,
        Mountains,
        Jungle,
        Desert
    }
    [Header("Biome Objects & Settings")]
    public BiomeObjects Ocean;
    public BiomeObjects River;
    public BiomeObjects Beach;
    public BiomeObjects Planes;
    public BiomeObjects Forest;
    public BiomeObjects Mountains;
   

    [SerializeField]
    public Biome[,] grid;

    Dictionary <Biome,BiomeObjects> objDict = new Dictionary<Biome, BiomeObjects>();
    Dictionary<float, Biome> BioDict = new Dictionary<float, Biome>();
    Dictionary<Biome, int> SplatDict = new Dictionary<Biome, int>();

    public AnimationCurve IslandCurve;


    // Start is called before the first frame update
    void Start()
    {
        setup();
        //GenBiomeMap();
        //genGrid();
        //GenTerrain();
        //GenDetailMap();
        //Debug.Log(checkBiome(308, 24)); // TEST CODE- prints biome for  given coordinate
        //ObjectGen();
        //SpawnBiomeObjects();
    }

    public void genSeed()// generates random seeds
    {
        if (!RandomizeSeed)
        {
            return;
        }
        seedx = Random.Range(0, 99999);
        seedy = Random.Range(0, 99999);
        seedScale = Random.Range(2, 15);
        seedBiome = new float[5];

        float OceanRNG = Random.value;
        Debug.Log("OceanRNG:" + OceanRNG);
        if (noIsland < OceanRNG)
        {
            isOcean = true;
        }

        //calc Biome seed
        float sum = 0;
        for (int i = 0; i < 5; i++)
        {
            float r = Random.value;
            sum += r;
            seedBiome[i] = r;
        }
        for (int i = 0; i < 5; i++)
        {
            
            seedBiome[i] /= sum;
        }

    }

    public void setup() //Defines Dictionaries
    {
        objDict.Add(Biome.Ocean, Ocean);
        objDict.Add(Biome.River, River);
        objDict.Add(Biome.Beach, Beach);
        objDict.Add(Biome.Planes, Planes);
        objDict.Add(Biome.Forest, Forest);
        objDict.Add(Biome.Mountains, Mountains);

        SplatDict.Add(Biome.Ocean, 0);
        SplatDict.Add(Biome.River, 1);
        SplatDict.Add(Biome.Beach, 2);
        SplatDict.Add(Biome.Planes, 3);
        SplatDict.Add(Biome.Forest, 4);
        SplatDict.Add(Biome.Mountains, 5);
    }

    public Biome checkBiome(int x, int y) // returns the biome for a given coordinate
    {
        //Debug.Log(grid[(int)(x * scale), (int)(y * scale)]);
        return grid[(int)(x*scale), (int)(y*scale)];
    }

    public void GenBiomeMap(int ID=0)
    {
        genSeed();

        float Ot = seedBiome[0];
        float Bt = Ot + seedBiome[1];
        float Pt = Bt + seedBiome[2];
        float Ft = Pt + seedBiome[3];
        float Mt = Ft + seedBiome[4];



        int mapSize = (int)(TerrainSize * scale);
        BiomeMap = new Texture2D(mapSize, mapSize);
        int midpoint = mapSize / 2;
        for(int x = 0; x<mapSize; x++)
        {
            for(int z = 0; z < mapSize; z++)
            {
                float xcoord = ((float)x / TerrainLength * seedScale + seedx);
                float zcoord = ((float)z / TerrainWidth * seedScale + seedy);
                float rng = Mathf.PerlinNoise(xcoord, zcoord); //perlin noise
                //Debug.Log(rng);
                Vector2 coord = new Vector2(x, z);
                float dist = Vector2.Distance(coord, new Vector2(mapSize / 2, mapSize / 2));
                rng -= IslandCurve.Evaluate(dist/islandScale);
                if (rng < Ot || isOcean)
                {
                    BiomeMap.SetPixel(x, z, OceanC);
                }
                
                else if (rng < Bt)
                {
                    BiomeMap.SetPixel(x, z, BeachC);
                }
                else if (rng < Pt)
                {
                    BiomeMap.SetPixel(x, z, PlanesC);
                }
                else if (rng < Ft)
                {
                    BiomeMap.SetPixel(x, z, ForestC);
                }
                else
                {
                    BiomeMap.SetPixel(x, z, MountainsC);
                }
            }
        }
        BiomeMap.Apply();
        //genRiver();
        SaveTextureAsPNG(BiomeMap, "/BiomeMaps/" + ID + ".png");
    }

    public void genRiver() //WIP
    {
        int Rwidth = 5;
        int startX = (int)(seedx % TerrainSize / seedScale) + (TerrainSize/2);
        int startZ = (int)(seedy % TerrainSize / seedScale)+ (TerrainSize / 2);
        Debug.Log(startX);
        int x=startX, z=startZ;
        while (!BiomeMap.GetPixel(x, z).Equals(OceanC))
        {
            for(int i = x-Rwidth; i<x+Rwidth; i++)
            {
                for (int j = z-Rwidth; j < z + Rwidth; j++)
                {
                    BiomeMap.SetPixel(i, j, RiverC);
                }
            }
            //x +=(int) Mathf.Abs(Rwidth * Mathf.Cos(x));
            //z +=(int) Mathf.Abs(Rwidth * Mathf.Sin(z));
            x++;
        }
        BiomeMap.Apply();
    }

    public void genGrid() // creates grid of Biomes
    {
        gridSize =(int)(TerrainSize * scale);
        grid = new Biome[gridSize, gridSize];
        
        for (int i=0; i<gridSize; i++)
        {
            for(int k = 0; k < gridSize; k++)
            {
                Color32 pixel_colour = BiomeMap.GetPixel(i, k);
                //red
                if (pixel_colour.Equals(OceanC))
                {
                    grid[i, k] = Biome.Ocean;
                }
                //yellow
                else if (pixel_colour.Equals(BeachC))
                {
                    grid[i, k] = Biome.Beach;
                }
                //green
                else if (pixel_colour.Equals(RiverC))
                {
                    grid[i, k] = Biome.River;
                }
                //lightblue
                else if (pixel_colour.Equals(PlanesC))
                {
                    grid[i, k] = Biome.Planes;
                }
                //darkblue
                else if (pixel_colour.Equals(MountainsC))
                {
                    grid[i, k] = Biome.Mountains;
                }
                //pink
                else if (pixel_colour.Equals(ForestC))
                {
                    grid[i, k] = Biome.Forest;
                }
            }
        }
    }


    public void GenTerrain()
    {
        int mapSize = (int)(TerrainSize * scale);
        float[,] heights = new float[TerrainLength, TerrainWidth];
        for (int x=0; x< TerrainLength; x++)
        {
            for (int z=0; z< TerrainWidth; z++)
            {

                Vector2 coord = new Vector2(x, z);
                float dist = Vector2.Distance(coord, new Vector2(mapSize / 2, mapSize / 2));
                float xcoord = (float)x / (float)TerrainLength * (float)TerrainSize;
                float zcoord = (float)z / (float)TerrainWidth * (float)TerrainSize;
                //Debug.Log(xcoord + ", " + zcoord);
                //Debug.Log(checkBiome(xcoord, zcoord));
                Biome bio = checkBiome((int)xcoord, (int)zcoord);
                BiomeGenSettings BGS = objDict[bio].terrSettings;
                if (bio != Biome.River)
                {  
                    heights[z, x] = (((BGS.baseHeight * 1 + Mathf.Clamp(Mathf.PerlinNoise(xcoord, zcoord), 0f, .2f)) / TerrainHeight) - Mathf.PerlinNoise((float)xcoord / TerrainLength * BGS.jaggedness, (float)zcoord / TerrainWidth * BGS.jaggedness) * BGS.baseHeight / TerrainHeight) * (1.5f - dist / mapSize);
                }
                else
                {
                    heights[z, x] = heights[z - 1, x] - BGS.baseHeight;
                }
                
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
        smoothTerrain(5);
        GenSplatMap();
    }

    public void smoothTerrain(int radius = 10) 
    {
        float[,] heights = terrain.terrainData.GetHeights(0, 0, TerrainLength, TerrainWidth);
        for (int x = 0; x < TerrainLength; x++)
        {
            for (int z =0; z < TerrainWidth; z++)
            {
                float sum = 0;
                int count = 0;
                for (int rx = -radius; rx < radius; rx++)
                {
                    for(int rz = -radius; rz< radius; rz++)
                    {
                        
                        int tmpX = x + rx;
                        int tmpZ = z + rz;
                        if (tmpX >= 0 && tmpZ >= 0 && tmpX < TerrainLength && tmpZ < TerrainWidth)
                        {
                            count++;
                            sum += heights[tmpX, tmpZ];
                        }
                    }
                }
                
                float avg = sum / count;
                heights[x, z] = avg;
            }
        }
        terrain.terrainData.SetHeights(0, 0, heights);
    }

    public void GenDetailMap() // generate terrain Detail Map (grass,etc)
    {
        TerrainData terrainData = terrain.terrainData; //get terrain data
        terrainData.SetDetailResolution(grassDensity, patchDetail); //set detail resolution
        int[,] detailMap = new int[grassDensity, grassDensity];//create new detail map
        for (int i = 0; i < grassDensity; i++)
        {
            for (int j = 0; j < grassDensity; j++)
            {
                Biome bio = checkBiome((int)(((float)j / (float)grassDensity) * (float)TerrainSize), (int)(((float)i / (float)grassDensity) * (float)TerrainSize));
                if(Random.value < objDict[bio].Settings.grassDensity)
                {
                    detailMap[i, j] = 1;
                }
            }
        }
        terrainData.SetDetailLayer(0, 0, 0, detailMap);
    }

    public void GenSplatMap() // generate terrain splat map (ground textures) 
    {
        TerrainData terrainData = terrain.terrainData; //get terrain data
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers]; //create new splat map
        //Debug.Log(terrainData.alphamapHeight);
        int height = terrainData.alphamapHeight;
        int width = terrainData.alphamapWidth;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float Ynorm = (float)y / (float)height;
                float Xnorm = (float)x / (float)width;
                float elev = terrainData.GetHeight(Mathf.RoundToInt(Ynorm * terrainData.heightmapResolution), Mathf.RoundToInt(Xnorm * terrainData.heightmapResolution));
                Vector3 normal = terrainData.GetInterpolatedNormal(Ynorm, Xnorm);
                float steepness = terrainData.GetSteepness(Ynorm, Xnorm);
                float[] splatWeights = new float[terrainData.alphamapLayers];
                //weights:
                //splatWeights[0] = 0.1f; // const weight
                //splatWeights[1] = (elev/terrainData.heightmapResolution); // Low Altitudes
                //splatWeights[2] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapResolution / 5.0f)); //flatter Terrain
                //Debug.Log((int)((float)x/(float)width)*1000f);
                Biome bio= checkBiome((int)(((float)y / (float)width) * (float)TerrainSize), (int)(((float)x / (float)height) * (float)TerrainSize));
                splatWeights[SplatDict[bio]] = 1f;
                if (bio == Biome.Mountains)
                {
                    splatWeights[6] = 1.0f - Mathf.Clamp01(steepness * steepness / (terrainData.heightmapResolution / 0.5f)); //snow on mountain tops
                }

                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];


                }
            }
        }
            terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    public void ObjectGen(int ID = 0) // generates Biome object Map (trees, rocks, etc)
    {
        
        int mapSize = (int)(TerrainSize * scale);
        ObjectMap = new Texture2D(mapSize, mapSize);
        for (int x=0; x<mapSize; x++)
        {
            for (int y=0; y < mapSize; y++)
            {
                
                Biome bio = checkBiome((int)(x/scale), (int)(y/scale));
                if (Random.value < objDict[bio].Settings.treeDensity)
                {
                    ObjectMap.SetPixel(x, y, TreeC);
                } else if (Random.value < objDict[bio].Settings.bushDensity)
                {
                    ObjectMap.SetPixel(x, y, BushC);
                }
                else if (Random.value < objDict[bio].Settings.rockDensity)
                {
                    ObjectMap.SetPixel(x, y, RockC);
                }
                else if (Random.value < objDict[bio].Settings.otherDensity)
                {
                    ObjectMap.SetPixel(x, y, OtherC);
                }




            }
        }
        ObjectMap.Apply();
        SaveTextureAsPNG(ObjectMap, "/ObjMaps/" + ID + ".png");
    }


    public void SpawnBiomeObjects() // spawns biome objects based on biome object map
    {
        int mapSize = (int)(TerrainSize * scale);
        
        for (int x=0; x < mapSize; x++)
        {
            for(int y=0; y < mapSize; y++)
            {

                Biome bio = checkBiome((int)(x/scale), (int)(y/scale));
                float h = terrain.SampleHeight(new Vector3((x/scale),0,(y /scale)));
                if (ObjectMap.GetPixel(x, y) == TreeC)
                {
                    Instantiate(objDict[bio].Trees[Random.Range(0, objDict[bio].Trees.Length-1)], new Vector3((x / scale), h,(y / scale)), Quaternion.identity);
                }
                else if(ObjectMap.GetPixel(x, y) == BushC)
                {
                    Instantiate(objDict[bio].Bushes[Random.Range(0, objDict[bio].Bushes.Length-1)], new Vector3((x / scale), h,(y / scale)), Quaternion.identity);
                }
                else if (ObjectMap.GetPixel(x, y) == RockC)
                {
                    Instantiate(objDict[bio].Rocks[Random.Range(0, objDict[bio].Rocks.Length-1)], new Vector3((x / scale), h,(y / scale)), Quaternion.identity);
                }
                else if (ObjectMap.GetPixel(x, y) == OtherC)
                {
                    Instantiate(objDict[bio].Other[Random.Range(0, objDict[bio].Other.Length-1)], new Vector3((x / scale), h,(y / scale)), Quaternion.identity);
                }

            }
        }
    }

    public void SaveTextureAsPNG(Texture2D _texture, string _fullPath)
    {
        Debug.Log(savePath + _fullPath);
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(savePath + _fullPath, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
