using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomNamespace
{
    public class Neighbours : MonoBehaviour
    {
        /// <summary>
        /// Get available cells around each block in shape
        /// </summary>
        public static List<Cell> Shape(Shape shape, GridSize gridSize, bool[,] grid)
        {
            List<Cell> cells = new List<Cell>();

            foreach (Cell block in shape.blocks)
            {
                List<Cell> cellNeighbours = Block(block, gridSize, grid);

                foreach (Cell cell in cellNeighbours)
                {
                    bool success = CheckDuplicate(cell, cells);
                    if (success) cells.Add(cell);
                }
            }
            return cells;
        }

        static bool CheckDuplicate(Cell newCell, List<Cell> cells)
        {
            foreach (Cell cell in cells)
            {
                if (cell.x == newCell.x && cell.y == newCell.y)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  Get available cells around given block
        /// </summary>
        public static List<Cell> Block(Cell currentGridPosition, GridSize gridSize, bool[,] grid)
        {
            List<Cell> cells = new List<Cell>();

            int x = currentGridPosition.x;
            int y = currentGridPosition.y;

            if (x + 1 >= 0 && x + 1 < gridSize.x && grid[x + 1, y] == false) cells.Add(new Cell(x + 1, y));
            if (x - 1 >= 0 && x - 1 < gridSize.x && grid[x - 1, y] == false) cells.Add(new Cell(x - 1, y));

            if (y + 1 >= 0 && y + 1 < gridSize.y && grid[x, y + 1] == false) cells.Add(new Cell(x, y + 1));
            if (y - 1 >= 0 && y - 1 < gridSize.y && grid[x, y - 1] == false) cells.Add(new Cell(x, y - 1));

            return cells;
        }
    }

    public class ShapeObject : MonoBehaviour
    {
        /// <summary>
        /// Create Shape Object
        /// </summary>
        public static Transform CreateShape(Shape shape, Transform prefab, float positionOffset, Color color)
        {
            Transform shapeObj = new GameObject("Shape").transform;

            foreach (Cell block in shape.blocks)
            {
                Transform blockObj = Instantiate(prefab, shapeObj);
                blockObj.position = new Vector3(block.x * positionOffset, block.y * positionOffset);
                blockObj.GetComponent<SpriteRenderer>().color = color;
            }

            return shapeObj;
        }
    }
}

[System.Serializable]
public struct GridSize
{
    public int x;
    public int y;

    public GridSize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Shape
{
    public List<Cell> blocks = new List<Cell>();
}

public struct Cell
{
    public int x;
    public int y;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}