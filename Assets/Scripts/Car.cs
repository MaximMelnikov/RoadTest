using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Car : MonoBehaviourEx
{
    [SerializeField]
    private float speed; //car movement speed
    private Queue<Vector2> waypoints = new Queue<Vector2>(); //car path
    
    private MapObject target;
    private bool driveToGasStation = true;

    void Start ()
	{
	    int spawnNum = Random.Range(0, GameManager.map.exits.Count);
	    transform.localPosition = GameManager.map.exits[spawnNum].transform.localPosition;
        
        target = GameManager.map.gasStations[spawnNum];
        Debug.Log("gasTargrtPos " + target.pos);
        double distance = GameManager.pathfinding.GetDistance(GameManager.map.exits[spawnNum], target);

        if (distance != Double.PositiveInfinity)
        {
            waypoints = GameManager.pathfinding.FindShorthestPath(GameManager.map.exits[spawnNum], GameManager.map.gasStations[spawnNum]);
        }
        else
        {
            Debug.Log(distance);
        }
    }

    void ShowProgressBar()
    {        
        GameObject gameObject = Instantiate(Resources.Load("ProgressBar"), GameManager.Instance.ui) as GameObject;
        ProgressBar progressBar = gameObject.GetComponent<ProgressBar>();
        progressBar.Init(transform.position, ()=> {
            waypoints = GameManager.pathfinding.FindRandomPath(target, GameManager.map.exits[Random.Range(0, GameManager.map.exits.Count)]);
            foreach (var item in waypoints)
            {
                Debug.Log(item);
            }
        } );
    }

    void Update () {
        //move car trough waypoints
	    if (waypoints.Count > 0)
	    {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, waypoints.Peek(), speed * Time.deltaTime);

            //find the vector pointing from our position to the target
            Vector2 _direction = (waypoints.Peek() - (Vector2)transform.localPosition);
            transform.localRotation = Quaternion.LookRotation(_direction, Vector3.back);

            if (Vector3.Distance(waypoints.Peek(), transform.localPosition) < .05f)
            {
                if (waypoints.Count == 1) //destination reached
                {
                    if (driveToGasStation)
                    {
                        driveToGasStation = false;
                        ShowProgressBar();
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
                waypoints.Dequeue();
            }
        }
	}
}