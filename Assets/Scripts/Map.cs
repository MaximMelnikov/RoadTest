using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Map
{
    private const int EXITSCOUNT = 3;
    private const int GASSTATIONSCOUNT = 3;
    public MapObject[,] mapGrid;
    public List<Road> exits = new List<Road>();
    public List<GasStation> gasStations = new List<GasStation>();

    public Map(int mapSize)
    {
        mapGrid = new MapObject[mapSize, mapSize];
        GameManager.Instance.mapParenTransform.GetChild(0).transform.localScale = new Vector3(mapSize, mapSize);
        GameManager.Instance.mapParenTransform.GetChild(0).transform.localPosition = new Vector3(mapSize/2-0.5f, mapSize/2-0.5f);

        //Camera.main.transform.position = 
    }

    public void GenerateMap()
    {
        //roads generating (BSP alghoritm)
        List<Rectangle> rectangles = new List<Rectangle>();
        Rectangle root = new Rectangle(1, 1, mapGrid.GetLength(0) + 2, mapGrid.GetLength(1) + 2);
        rectangles.Add(root);

        for (int i = 0; i < 5;)
        {
            int splitIdx = UnityEngine.Random.Range(0, rectangles.Count);

            Rectangle toSplit = rectangles[splitIdx];
            if (toSplit.split())
            {
                rectangles.Add(toSplit.leftChild);
                rectangles.Add(toSplit.rightChild);
                i++;
            }
        }

        //fill roads temporary array (top and left rect sides only for better view IMHO)
        bool[,] roads = new bool[mapGrid.GetLength(0) + 2, mapGrid.GetLength(1) + 2];

        foreach (var rect in rectangles)
        {
            for (int i = 0; i < rect.height; i++)
            {
                roads[rect.left - 1, i] = true;
            }
            for (int i = 0; i < rect.width; i++)
            {
                roads[i, rect.top - 1] = true;
            }
        }

        //place roads on map
        for (int i = 0; i < mapGrid.GetLength(1) - 3; i++)
        {
            for (int j = 0; j < mapGrid.GetLength(0) - 3; j++)
            {
                if (roads[i, j])
                    ConstructMapObject(MapObjectType.Road, new Vector2(j + 1, i + 1));
            }
        }
        
        //place exits
        for (int i = 0; i < EXITSCOUNT;)
        {
            bool horisontal = UnityEngine.Random.value > 0.5f;
            Vector2 randPos = new Vector2(Convert.ToInt32(horisontal)*UnityEngine.Random.Range(1, mapGrid.GetLength(0) - 3), Convert.ToInt32(!horisontal)*UnityEngine.Random.Range(1, mapGrid.GetLength(1) - 3));
            if (mapGrid[(int)randPos.x, (int)randPos.y] == null)
            {
                i++;
                Road road = ConstructMapObject(MapObjectType.Road, randPos) as Road;
                exits.Add(road);
                GameManager.pathfinding.graph.AddNode(road);
            }
        }

        //place gas stations on map
        for (int i = 0; i < GASSTATIONSCOUNT;)
        {
            Vector2 randPos = new Vector2(UnityEngine.Random.Range(1, mapGrid.GetLength(0) - 2), UnityEngine.Random.Range(1, mapGrid.GetLength(1) - 2));
            if (mapGrid[(int)randPos.x, (int)randPos.y] == null && GetNeighboursOfType(new Vector2(randPos.x, randPos.y), MapObjectType.Road).Contains("1"))
            {
                i++;
                GasStation gasStation = ConstructMapObject(MapObjectType.GasStation, randPos) as GasStation;
                gasStations.Add(gasStation);
                GameManager.pathfinding.graph.AddNode(gasStation);
            }
        }

        //setting roads types for creating intersections, turns etc...
        for (int i = 0; i < mapGrid.GetLength(1) - 2; i++)
        {
            for (int j = 0; j < mapGrid.GetLength(0) - 2; j++)
            {
                if (mapGrid[i, j] != null && mapGrid[i, j].cellType == MapObjectType.Road)
                {
                    Road road = mapGrid[i, j] as Road;
                    road.SetRoadType(GetNeighbours(road.pos));
                }
            }
        }
        foreach (var i in exits)
        {
            i.SetRoadType(RoadType.ROAD_straight, i.pos.x == 0 ? 90 : 0);
            i.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }
    /// <summary>
    /// Get cell Neighbours by coordinates and returns string "0110" = "up, right, down, left"
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    public string GetNeighbours(Vector2 coordinates)
    {
        Vector2[] fourNeighbours = new[] { new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0) };
        StringBuilder neighbours = new StringBuilder("0000");
        for (int i = 0; i < fourNeighbours.Length; i++)
        {
            try
            {
                if (GameManager.map.mapGrid[(int)coordinates.x + (int)fourNeighbours[i].x, (int)coordinates.y + (int)fourNeighbours[i].y] != null)
                {
                    neighbours[i] = '1';
                }
            }
            catch (Exception)
            {
            }
        }

        return neighbours.ToString();
    }

    public string GetNeighboursOfType(Vector2 coordinates, MapObjectType type)
    {
        Vector2[] fourNeighbours = new[] {new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0)};
        StringBuilder neighbours = new StringBuilder("0000");
        for (int i = 0; i < fourNeighbours.Length; i++)
        {
            try
            {
                    if (GameManager.map.mapGrid[(int)coordinates.x + (int)fourNeighbours[i].x, (int)coordinates.y + (int)fourNeighbours[i].y] != null && GameManager.map.mapGrid[(int)coordinates.x + (int)fourNeighbours[i].x, (int)coordinates.y + (int)fourNeighbours[i].y].cellType == type)
                {
                    neighbours[i] = '1';
                }
            }
            catch (Exception)
            {
            }
        }

        return neighbours.ToString();
    }

    public MapObject ConstructMapObject(MapObjectType type, Vector2 pos)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(type.ToString()), GameManager.Instance.mapParenTransform) as GameObject;

        if (gameObject != null)
        {
            MapObject obj = gameObject.GetComponent<MapObject>();
            ConstructMapObject(obj, pos);
            return obj;
        }
        else
            throw new Exception("Missing prefab name: " + type);
    }

    public MapObject ConstructMapObject(MapObject mapObject, Vector2 pos)
    {
        mapObject.pos = pos;
        mapGrid[(int)pos.x, (int)pos.y] = mapObject;
        return mapObject;
    }
}

/// <summary>
/// BSP tree rectangle
/// </summary>
internal class Rectangle
{
    private const int MIN_SIZE = 3;

    public int top, left, width, height;
    public Rectangle leftChild;
    public Rectangle rightChild;

    public Rectangle(int top, int left, int height, int width)
    {
        this.top = top;
        this.left = left;
        this.width = width;
        this.height = height;
    }

    public bool split()
    {
        if (leftChild != null) //if already splited
            return false;
        bool horizontal = height >= width; //direction of split
        int max = (horizontal ? width : height) - MIN_SIZE; //maximum height/width we can split
        if (max < MIN_SIZE) // area too small to split
            return false;

        if (horizontal)
        {
            int split = UnityEngine.Random.Range(2, height-1);
            leftChild = new Rectangle(top, left, split, width);
            rightChild = new Rectangle(top + split, left, height - split, width);
        }
        else
        {
            int split = UnityEngine.Random.Range(2, width-1);
            leftChild = new Rectangle(top, left, height, split);
            rightChild = new Rectangle(top, left + split, height, width - split);
        }
        return true;
    }
}