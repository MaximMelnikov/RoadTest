using UnityEngine;
/// <summary>
/// Parent class for all objects on map grid
/// </summary>
public class MapObject : MonoBehaviour
{
    public MapObjectType cellType;
    [HideInInspector]
    public Vector2 pos;

    void Start()
    {
        transform.localPosition = new Vector3(pos.x, pos.y, -0.01f);
    }

    protected readonly Vector2[] fourNeighbours = new[] { new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0) };

    void OnDestroy()
    {
        GameManager.map.mapGrid[(int)pos.x, (int)pos.y] = null;
    }
}

public enum MapObjectType
{
    None,
    Road,
    GasStation
}