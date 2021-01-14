using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Specialized;
using System;
using System.IO;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    MapGrid IslandGen;
    [SerializeField]
    TerrSection[] terrains; // array of (9) terrains to be generated at a time
    [SerializeField]
    Transform PlayerPosition; // used to determine which terrains need to be generated
    const int posOffset = 100000; // required for Cantor paring to ensure non negative coords
    Vector2 terrOrigin;
    
    [System.Serializable]
    public class TerrSection
    {
        public Terrain Ter;
        public int id;
    };

    // Start is called before the first frame update
    void Start()
    {
        terrOrigin = new Vector2((int)(PlayerPosition.position.x / IslandGen.TerrainSize), (int)(PlayerPosition.position.z / IslandGen.TerrainSize));

        int curX = (int)PlayerPosition.position.x + posOffset;
        int curY = (int)PlayerPosition.position.z + posOffset;
        for (int i = 0; i <3; i++)
        {
            for (int j = 0; j<3; j++)
            {          
                int id = FindID(curX + (i-1)*IslandGen.TerrainSize, curY +(j - 1) * IslandGen.TerrainSize);
                terrains[i*3 + j].id = id;
                LoadMaps(id, terrains[i * 3 + j].Ter);
            }
        }
    }

    void updateTerrains()
    {
        List<int> newIDs = new List<int>();
        for (int i=0; i<3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                newIDs.Add(FindID((int)(terrOrigin.x + posOffset + (i - 1) * IslandGen.TerrainSize), (int)(terrOrigin.y+ posOffset + (i - 1) * IslandGen.TerrainSize))); // ids that should be on terrain
            } 
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (!newIDs.Contains(terrains[i * 3 + j].id)) // if a current terrain id is not in list, regen terrain
                {
                    int id = FindID((int)(terrOrigin.x + posOffset + (i - 1) * IslandGen.TerrainSize), (int)(terrOrigin.y + posOffset + (i - 1) * IslandGen.TerrainSize)); // new id
                    terrains[i * 3 + j].id = id; // set to new id
                    LoadMaps(id, terrains[i * 3 + j].Ter); // regen terrain
                }
                
            }
        }
        placeTerrains();

    }
   

    public void placeTerrains() // places terrains based on id
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Vector2 Tpos = FindCoord(terrains[i * 3 + j].id);
                terrains[i * 3 + j].Ter.transform.position = new Vector3(Tpos.x - posOffset,0,Tpos.y - posOffset);
            }
        }
    }

    int FindID(int xCoord, int yCoord) // find cooresponding terrain ID given x & z Coordinate 
    {
        int x = xCoord / IslandGen.TerrainSize;
        int z = yCoord / IslandGen.TerrainSize;
        int id = (int)(.5*(x + z)*(x + z + 1) + z); //Cantor pairing function
        return id;
    }

    Vector2 FindCoord(int id) // find xy coords given ID
    {
        float w = Mathf.Floor((Mathf.Sqrt((8*id) + 1) - 1)/2);
        float t = ((w * w) + w) / 2;
        int y = (int)(id - t);
        int x = (int)(w - y);
        Debug.Log("FindCoord(id=" + id + "): x=" + x + " y=" + y);
        return new Vector2(x, y);
    }

    void LoadMaps(int id, Terrain curTerrain) //Loads Maps, Then calls MapGrid.cs functions to generate terrain based on maps
    {
        IslandGen.terrain = curTerrain; // set the terrain that is to be generated
        int imgSize = (int)(IslandGen.TerrainSize * IslandGen.scale);
        Texture2D[] Maps = new Texture2D[2];
        Texture2D BiomeMap = new Texture2D(imgSize, imgSize);
        bool BmapFound = false; //Biome Map Found
        bool OmapFound = false; //Obj Map Found
        foreach (string file in System.IO.Directory.GetFiles(IslandGen.savePath + "/BiomeMaps/")) // check for Biome Map
        {
            string pathName = (IslandGen.savePath + "/BiomeMaps/");
            string fileName = file.Remove(0, pathName.Length);
            if (fileName.Substring(0,fileName.Length-4).Equals( ""+id))
            {
                BmapFound = true;
                IslandGen.BiomeMap = new Texture2D(imgSize, imgSize);
                byte[] fileData = File.ReadAllBytes(file);
                IslandGen.BiomeMap.LoadImage(fileData);
                break;
            }
        }

        foreach (string file in System.IO.Directory.GetFiles(IslandGen.savePath + "/ObjMaps/")) // check for Obj Map
        {
            string pathName = (IslandGen.savePath + "/ObjMaps/");
            string fileName = file.Remove(0, pathName.Length);
            if (fileName.Substring(0, fileName.Length - 4).Equals("" + id))
            {
                OmapFound = true;
                IslandGen.ObjectMap = new Texture2D(imgSize, imgSize);
                byte[] fileData = File.ReadAllBytes(file);
                IslandGen.ObjectMap.LoadImage(fileData);
                break;
            }
        }
        if (!BmapFound) // if no biome map found, create it
        {
            IslandGen.GenBiomeMap(id);
        }

        IslandGen.genGrid();
        IslandGen.GenTerrain();
        IslandGen.GenDetailMap();

        if (!OmapFound) // if no obj map found, create it
        {
            IslandGen.ObjectGen(id);
        }

        //IslandGen.SpawnBiomeObjects();

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(terrOrigin, new Vector2(PlayerPosition.position.x, PlayerPosition.position.z)) > 2000)
        {
            terrOrigin = new Vector2((int)(PlayerPosition.position.x / IslandGen.TerrainSize), (int)(PlayerPosition.position.z / IslandGen.TerrainSize));
            updateTerrains();
        }
    }
}
