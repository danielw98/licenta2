using System;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public static CellEntity[,] Tiles { get; set; }

    public Map()
    {
        
    }
    
    /// <summary>
    /// Gets all the path cells that are adjacent
    /// </summary>
    /// <param name="tile">The tile to check against</param>
    /// <returns>The tiles marked as path, adjacent to the source tile</returns>
    public static List<CellEntity> GetAdjacentPathTiles(CellEntity tile)
    {
        try
        {
            var adjacentTiles = new List<CellEntity>();
            if (tile.X > 0)
                if (!Tiles[tile.X - 1, tile.Y].HasBuilding)
                    adjacentTiles.Add(Tiles[tile.X - 1, tile.Y]);
            if (tile.X < Game.Instance.mapWidth - 1)
                if (!Tiles[tile.X + 1, tile.Y].HasBuilding)
                    adjacentTiles.Add(Tiles[tile.X + 1, tile.Y]);
            if (tile.Y > 0)
                if (!Tiles[tile.X, tile.Y - 1].HasBuilding)
                    adjacentTiles.Add(Tiles[tile.X, tile.Y - 1]);
            if (tile.Y < Game.Instance.mapHeight - 1)
                if (!Tiles[tile.X, tile.Y + 1].HasBuilding)
                    adjacentTiles.Add(Tiles[tile.X, tile.Y + 1]);
            return adjacentTiles;
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    /// <summary>
    /// Calculates the normalized distance between two points
    /// </summary>
    /// <param name="source">Source point</param>
    /// <param name="destination">Destination point</param>
    /// <returns>The normalized distance between source and destination point</returns>
    public static int GetDistance(CellEntity source, CellEntity destination)
    {
        return 10 * (Math.Abs(source.Y - destination.Y) + Math.Abs(source.X - destination.X));
    }
    
    /// <summary>
    /// Gets the cell located at the specified world position
    /// </summary>
    /// <param name="position">The position to check, in world space</param>
    /// <returns>The Cell located at the specified position</returns>
    public static CellEntity GetCellFromWorldPosition(Vector3 position)
    {
        if (Tiles.Length > 0)
            for (int x = 0; x <= Tiles.GetUpperBound(0); x++)
                for (int y = 0; y <= Tiles.GetUpperBound(1); y++)
                    if ((int)position.x == x && (int)position.z == y) // if ((int)position.x == x && (Game.Instance.mapHeight - 1) - (int)position.z == y)
                        return Tiles[x, y];
        return null;
    }
}