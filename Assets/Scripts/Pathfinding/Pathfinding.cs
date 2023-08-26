using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Pathfinding : MonoBehaviour
{
    protected List<CellEntity> Path { get; private set; } = new();
    public bool isNewPathNeeded = true;
    
    /// <summary>
    /// Generates a new path, first from entry to a random tile node, then from that random tile node to exit 
    /// </summary>
    public IEnumerator PathTimer()
    {
        isNewPathNeeded = false;
        FindPath(Map.GetCellFromWorldPosition(transform.position), // should the start be the gameobject's position itself, instead of entry gate??
                 Map.GetCellFromWorldPosition(new Vector3(Game.Instance.mapWidth - 1, 0, Game.Instance.mapHeight / 2 + 0.5f)));
       // TODO: disallow placement when path is not possible
        yield return new WaitForSeconds(0.5F);
    }


    /// <summary>
    /// Calculates the shortest path between two specified path points
    /// </summary>
    /// <param name="startPosition">The A point of the path</param>
    /// <param name="endPosition">The B point of the path</param>
    public void FindPath(CellEntity startPosition, CellEntity endPosition)
    {
        if (startPosition != null && endPosition != null)
        {
            var open = new List<CellEntity>(); // list containing tiles that can still be walked
            var closed = new HashSet<CellEntity>(); // list of tiles that were already walked
            open.Add(startPosition);
            while (open.Count > 0) // if we still have tiles that can be walked
            {
                try
                {
                    CellEntity current = open[0];
                    for (int i = 0; i < open.Count(); i++) // if the current F cost is lower or equal to iterated tile cost, and iterated tile H cost is lower than current tile H cost, advance to iterated tile
                        if (open[i].F < current.F || open[i].F == current.F && open[i].H < current.H)
                            current = open[i];
                    open.Remove(current); // remove current tile from walkable list and add it to walked list
                    closed.Add(current);
                    if (current == endPosition) // if we reached destination, retrace backwards the path from our current location to the origin point
                    {
                        Path = RetracePath(startPosition, endPosition);
                        return;
                    }
                    foreach (CellEntity neighbour in Map.GetAdjacentPathTiles(current)) // get the path tiles adjacent to this tile
                    {
                        if (closed.Any(e => e.Id == neighbour.Id)) // if the current adjacent tile is in the list of walked tiles, skip it
                            continue;
                        int newMovementCostToNeighbour = current.G + Map.GetDistance(current, neighbour); // get the movement cost to the adjacent path tile
                        if (newMovementCostToNeighbour < neighbour.G || !open.Contains(neighbour)) // if the movement cost is smaller than the G cost of the adjacent path tile, and the adjacent tile is not in the walkable list
                        {
                            neighbour.G = newMovementCostToNeighbour; // update the G cost of the neighbor path tile
                            neighbour.H = Map.GetDistance(neighbour, endPosition); // set the H cost too
                            neighbour.Parent = current; // set this tile as the parent of the adjacent path tile, so we can retrace the path to origin point, when we find the path
                            if (!open.Contains(neighbour)) // if walkable tiles list doesn't contain adjacent path tile, add it
                                open.Add(neighbour);
                        }
                    }
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// Returns the list of tiles of the shortest path between two points, starting from end node
    /// </summary>
    /// <param name="startNode">The starting node of the path</param>
    /// <param name="endNode">The ending node of the path</param>
    /// <returns>List of Cell containing path tiles</returns>
    public static List<CellEntity> RetracePath(CellEntity startNode, CellEntity endNode)
    {
        var path = new List<CellEntity>();
        CellEntity current = endNode;
        while (current != startNode)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }
}