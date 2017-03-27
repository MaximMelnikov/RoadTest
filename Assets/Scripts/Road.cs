using System;
using System.Collections.Generic;
using UnityEngine;

public class Road : MapObject
{
    [SerializeField]
    private List<RoadElement> elements;
    private RoadType _roadType;
    public RoadType roadType
    {
        get { return _roadType; }
        internal set { _roadType = value; }
    }
    /// <summary>
    /// Set road type by stringNeighbours "0110" = "up, right, down, left"
    /// </summary>
    /// <param name="stringNeighbours"></param>
    public void SetRoadType(string stringNeighbours)
    {
        foreach (var neighbour in elements)
        {
            if (neighbour.neighbours == stringNeighbours)
            {
                SetRoadType(neighbour.type, neighbour.rotation);
            }
        }
    }
    /// <summary>
    /// Manual set of road type
    /// </summary>
    /// <param name="type"></param>
    /// <param name="rotation"></param>
    public void SetRoadType(RoadType type, float rotation)
    {
        foreach (var neighbour in elements)
        {
            if (neighbour.type == type)
            {
                GetComponent<MeshFilter>().mesh = neighbour.mesh;
                _roadType = type;
                transform.eulerAngles = new Vector3(0, rotation, 0);
                if (type != RoadType.ROAD_straight)
                {
                    GameManager.pathfinding.graph.AddNode(this);
                }
            }
        }
    }
}

public enum RoadType
{
    ROAD_straight,
    ROAD_deadlock,
    ROAD_turn,
    ROAD_intersection_T,
    ROAD_intersection
}
/// <summary>
/// Settings for road elements.
/// </summary>
[Serializable]
public struct RoadElement
{
    public RoadType type;
    public Mesh mesh;
    public float rotation;
    public string neighbours;
}