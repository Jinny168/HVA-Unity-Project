using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Map : ScriptableObject, ISerializationCallbackReceiver
{
    #region
    private static Map instance;
    [HideInInspector] public List<string> rowNames = new List<string>();
    [HideInInspector] public List<string> columnNames = new List<string>();
    [HideInInspector] public int mapWidth=10;
    [HideInInspector] public int mapHeight=10;
    [SerializeField]
    public int[,] mapArray;
    #endregion
    [HideInInspector] public int[] serialized_mapArray;

    public int MapHeight { get => mapHeight; set => mapHeight = value; }
    public int MapWidth { get => mapWidth; set => mapWidth = value; }
    public static Map Instance { get => instance; set => instance = value; }

    public Map(int _mapWidth, int _mapHeight)
    {
        mapWidth = _mapWidth;
        mapHeight = _mapHeight;
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                this.mapArray[i, j] = (int)MapType.walk;
            }
        }
    }
    public void SetWall(int gridX, int gridY)
    {
        mapArray[gridX, gridY] = (int)MapType.wall;
    }

    public void SetWalk(int gridX, int gridY)
    {
        mapArray[gridX, gridY] = (int)MapType.walk;
    }

    public void SetPoint(int gridX, int gridY)
    {
        mapArray[gridX, gridY] = (int)MapType.walk;
    }
    public MapType GetMapType(int gridX, int gridY)
    {
        return (MapType)mapArray[gridX, gridY];
    }

    public void OnBeforeSerialize()
    {

        serialized_mapArray = new int[mapWidth * mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                serialized_mapArray[i * mapHeight + j] = mapArray[i, j];
            }
        }
    }

    public void OnAfterDeserialize()
    {
        mapArray = new int[mapWidth, mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                mapArray[i, j] = serialized_mapArray[i * mapHeight + j];
            }
        }
    }
}
