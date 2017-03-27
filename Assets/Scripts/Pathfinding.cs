using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using GraphNamespace;
using Random = UnityEngine.Random;

public class Pathfinding
{
    public Graph graph;
    public Pathfinding()
    {
        graph = new Graph();
    }
    /// <summary>
    /// Returns diastance from startingNode to targetNode
    /// </summary>
    /// <param name="startingNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public double GetDistance(MapObject startingNode, MapObject targetNode)
    {
        var calculator = new DistanceCalculator();
        var distances = calculator.CalculateDistances(graph, startingNode, targetNode);
        double distance = Double.PositiveInfinity;

        foreach (var d in distances)
        {
            if (d.Key == targetNode)
            {
                distance = d.Value;
                break;
            }
        }

        return distance;
    }

    /// <summary>
    /// Returns shorthest path waypoints queue from startingNode to targetNode.
    /// (By finding neighbour node of targetNode with smallest distanceFromStart and repeating while startingNode is reached)
    /// </summary>
    /// <param name="startingNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public Queue<Vector2> FindShorthestPath(MapObject startingNode, MapObject targetNode)
    {
        Queue<Vector2> waypoints = new Queue<Vector2>();
        waypoints.Enqueue(targetNode.pos);
        var calculator = new DistanceCalculator();
        calculator.CalculateDistances(graph, startingNode, targetNode);

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(graph.GetNode(targetNode));

        while (queue.Count > 0 || queue.Peek() != graph.GetNode(startingNode))
        {
            NodeConnection connection = queue.Peek().Connections.OrderBy(n=>n.Target.DistanceFromStart).FirstOrDefault(n => !double.IsPositiveInfinity(n.Target.DistanceFromStart));
            if (connection != null)
            {
                if (startingNode == connection.Target.obj as MapObject)
                {
                    return new Queue<Vector2>(waypoints.Reverse());
                }
                queue.Dequeue();
                queue.Enqueue(connection.Target);
                waypoints.Enqueue((connection.Target.obj as MapObject).pos);
            }
            else
            {
                return new Queue<Vector2>(waypoints.Reverse());
            }
        }

        return new Queue<Vector2>(waypoints.Reverse());
    }
    /// <summary>
    /// Returns random path waypoints queue from startingNode to targetNode.
    /// Absolutely stupid. But absolutely random. As you wish i hope.
    /// Dont use that in real game because of while.
    /// </summary>
    /// <param name="startingNode"></param>
    /// <param name="targetNode"></param>
    /// <returns></returns>
    public Queue<Vector2> FindRandomPath(MapObject startingNode, MapObject targetNode)
    {
        Queue<Vector2> waypoints = new Queue<Vector2>();
        //var calculator = new DistanceCalculator();        
        Queue<Node> queue = new Queue<Node>();
        while (true)
        {
            waypoints.Clear();
            queue.Clear();
            queue.Enqueue(graph.GetNode(startingNode));
            HashSet<Node> checkedNodes = new HashSet<Node>();
            checkedNodes.Add(graph.GetNode(startingNode));

            int x = 0;
            while (x<30)
            {
                x++;
                Node curNode;

                List<NodeConnection> currentNodeUnvisitedConnections = new List<NodeConnection>();
                foreach (var i in queue.Peek().Connections)
                {
                    if (!checkedNodes.Contains(i.Target))
                    {
                        currentNodeUnvisitedConnections.Add(i);
                    }
                }
                if (currentNodeUnvisitedConnections.Count == 0)
                {
                    break;
                }else
                {
                    curNode = currentNodeUnvisitedConnections[Random.Range(0, currentNodeUnvisitedConnections.Count())].Target;
                    checkedNodes.Add(curNode);
                    queue.Dequeue();
                    queue.Enqueue(curNode);
                    waypoints.Enqueue((curNode.obj as MapObject).pos);
                }
                
                if (curNode.obj as MapObject == targetNode)
                    return waypoints;
            }
        }

        return waypoints;
    }

    //build graph from map array
    public void Init()
    {
        int lenght = 0;
        Node startNode;

        //fetch vertical graph connections
        for (int y = 0; y < GameManager.map.mapGrid.GetLength(1); y++)
        {
            lenght = 0;
            startNode = null;
            for (int x = 0; x < GameManager.map.mapGrid.GetLength(0); x++)
            {
                if (GameManager.map.mapGrid[x, y] != null)
                {
                    // if lenght == 1 or nodeGraph - previous cell was road, so we know startNode
                    if (lenght == 1 && GameManager.map.mapGrid[x, y] != null)
                    {
                        startNode = graph.GetNode(GameManager.map.mapGrid[x - 1, y]);
                        Node endNode = graph.GetNode(GameManager.map.mapGrid[x, y]);
                        if (endNode != null && startNode != null)
                        {
                            startNode.AddConnection(endNode, lenght);
                            endNode.AddConnection(startNode, lenght);
                            lenght = 0;
                        }
                    }
                    lenght++;

                    //check for last road in row
                    if (graph.GetNode(GameManager.map.mapGrid[x, y]) != null)
                    {
                        //if (lenght > 1 && x == GameManager.Instance.map.mapGrid.GetLength(0) - 1)
                        if (lenght > 2 && GameManager.map.mapGrid[x, y] != null)
                        {
                            Node endNode = graph.GetNode(GameManager.map.mapGrid[x, y]);
                            if (endNode != null && startNode != null)
                            {
                                startNode.AddConnection(endNode, lenght-1);
                                endNode.AddConnection(startNode, lenght-1);
                                lenght = 1;
                            }
                        }
                    }
                }
                else
                {
                    //if (lenght > 2 && GameManager.Instance.map.mapGrid[x, y] != null)
                    if (lenght >= 2 && GameManager.map.mapGrid[x, y] == null)
                    {
                        Node endNode = graph.GetNode(GameManager.map.mapGrid[x - 1, y]);
                        if (endNode != null && startNode != null)
                        {
                            startNode.AddConnection(endNode, lenght-1);
                            endNode.AddConnection(startNode, lenght-1);
                        }
                    }
                    lenght = 0;
                }
            }
        }

        //fetch horisontal graph connections
        for (int x = 0; x < GameManager.map.mapGrid.GetLength(0); x++)
        {
            lenght = 0;
            startNode = null;
            for (int y = 0; y < GameManager.map.mapGrid.GetLength(1); y++)
            {
                if (GameManager.map.mapGrid[x, y] != null)
                {
                    // if lenght == 1 or nodeGraph - previous cell was road, so we know startNode
                    if (lenght == 1 && GameManager.map.mapGrid[x, y] != null)
                    {
                        startNode = graph.GetNode(GameManager.map.mapGrid[x, y-1]);
                        Node endNode = graph.GetNode(GameManager.map.mapGrid[x, y]);
                        if (endNode != null && startNode != null)
                        {
                            startNode.AddConnection(endNode, lenght);
                            endNode.AddConnection(startNode, lenght);
                            lenght = 0;
                        }
                    }
                    lenght++;

                    //check for last road in column
                    if (graph.GetNode(GameManager.map.mapGrid[x, y]) != null)
                    {
                        //if (lenght > 1 && x == GameManager.Instance.map.mapGrid.GetLength(1) - 1)
                        if (lenght > 2 && GameManager.map.mapGrid[x, y] != null)
                        {
                        Node endNode = graph.GetNode(GameManager.map.mapGrid[x, y]);
                            if (endNode != null && startNode != null)
                            {
                                startNode.AddConnection(endNode, lenght-1);
                                endNode.AddConnection(startNode, lenght-1);
                                lenght = 1;
                            }
                        }
                    }
                }
                else
                {
                    if (lenght >= 2 && GameManager.map.mapGrid[x, y] == null)
                    {
                        Node endNode = graph.GetNode(GameManager.map.mapGrid[x, y-1]);
                        if (endNode != null && startNode != null)
                        {
                            startNode.AddConnection(endNode, lenght-1);
                            endNode.AddConnection(startNode, lenght-1);
                        }
                    }
                    lenght = 0;
                }
            }
        }
        foreach (var node in graph.objNodes)
        {
            foreach (var connection in node.Value.Connections)
            {
                MapObject sNode = node.Value.obj as MapObject;
                MapObject fNode = connection.Target.obj as MapObject;
                Debug.Log(sNode.pos + " to " + fNode.pos + " distance = " + connection.Distance);
            }
        }
    }
}

#region Graph and pathfinding
namespace GraphNamespace
{
    public class Graph
    {
        internal IDictionary<object, Node> objNodes { get; private set; }

        public Graph()
        {
            objNodes = new Dictionary<object, Node>();
        }

        public void AddNode(object obj)
        {
            var node = new Node(obj);
            if (!objNodes.ContainsKey(obj))
                objNodes.Add(obj, node);
        }

        public void AddConnection(object fromNode, object toNode, int distance)
        {
            objNodes[fromNode].AddConnection(objNodes[toNode], distance);
        }

        public Node GetNode(object obj)
        {
            Node node;
            objNodes.TryGetValue(obj, out node);
            return node;
        }
    }

    public class NodeConnection
    {
        internal Node Target { get; private set; }
        internal double Distance { get; private set; }

        internal NodeConnection(Node target, double distance)
        {
            Target = target;
            Distance = distance;
        }
    }

    public class Node
    {
        IList<NodeConnection> _connections;
        internal object obj;
        internal double DistanceFromStart { get; set; }

        internal IEnumerable<NodeConnection> Connections
        {
            get { return _connections; }
        }

        internal Node(object obj)
        {
            this.obj = obj;
            _connections = new List<NodeConnection>();
        }

        internal void AddConnection(Node targetNode, double distance)
        {
            if (targetNode == null) throw new ArgumentNullException("targetNode");
            if (targetNode == this) throw new ArgumentException("Node may not connect to itself.");
            if (distance <= 0) throw new ArgumentException("Distance must be positive.");

            _connections.Add(new NodeConnection(targetNode, distance));
        }
    }

    //dijkstra algohiritm pathinding
    public class DistanceCalculator
    {
        public IDictionary<object, double> CalculateDistances(Graph graph, object startingNode, object targetNode)
        {
            if (!graph.objNodes.Any(n => n.Key.Equals(startingNode)))
                throw new ArgumentException("Starting node must be in graph: " + startingNode);

            InitialiseGraph(graph, startingNode);
            ProcessGraph(graph, startingNode);
            return ExtractDistances(graph, targetNode);
        }

        //fill all distances by infinite & start = 0
        private void InitialiseGraph(Graph graph, object startingNode)
        {
            foreach (Node node in graph.objNodes.Values)
                node.DistanceFromStart = double.PositiveInfinity;
            graph.objNodes[startingNode].DistanceFromStart = 0;
        }

        //fill all nodes by distances fromStart
        private void ProcessGraph(Graph graph, object startingNode)
        {
            bool finished = false;
            var queue = graph.objNodes.Values.ToList();

            while (queue.Count > 0)
            {
                Node nextNode = queue.OrderBy(n => n.DistanceFromStart).FirstOrDefault(n => !double.IsPositiveInfinity(n.DistanceFromStart));
                if (nextNode != null)
                {
                    ProcessNode(nextNode, queue);
                    queue.Remove(nextNode);
                }
                else if (nextNode != null)
                {
                    queue.Remove(nextNode);
                }
                else
                {
                    break;
                }
            }
        }

        private void ProcessNode(Node node, List<Node> queue)
        {
            var connections = node.Connections;
            List<NodeConnection> cl = connections.ToList();
            List<NodeConnection> cl2 = connections.ToList();
            foreach (var i in cl)
            {
                if (!queue.Contains(i.Target))
                {
                    cl2.Remove(i);
                }
            }
            //var connections = node.Connections.Where(c => queue.Contains(c.Target));
            foreach (var connection in cl2)
            {
                double distance = node.DistanceFromStart + connection.Distance;
                if (distance < connection.Target.DistanceFromStart)
                {
                    connection.Target.DistanceFromStart = distance;
                }
            }
        }

        //return shorthest path distance from start to target node
        private IDictionary<object, double> ExtractDistances(Graph graph, object targetNode)
        {
            return graph.objNodes.ToDictionary(n => n.Key, n =>
            {
                if (n.Key.Equals(targetNode))
                {
                    return n.Value.DistanceFromStart;
                }
                else
                {
                    return -1;
                }
            });
        }
    }
}
#endregion