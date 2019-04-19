using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomNamespace;

public class GridFill : MonoBehaviour
{
    public GridSize gridSize = new GridSize(8, 7);
    public float positionOffset = 1.15f;
    public Transform blockPrefab;
    public Theme colors;
    int colorIndex = 0;

    public enum SearchType { LastBlock, AllBlocks };
    public SearchType neighbourCellsSearch;

    // Lists
    bool[,] grid;
    List<Shape> shapes;

    void Start()
    {
        grid = new bool[gridSize.x, gridSize.y];
        shapes = new List<Shape>();
        colorIndex = Random.Range(0, colors.colors.Length);

        GenerateGrid();
        LoadShapeObjects();

        // Print number of empty cells
        print((((gridSize.x * gridSize.y) / 4) - shapes.Count) * 4);
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y] == false)
                {
                    bool success = GetShape(out Shape shape, new Cell(x, y));

                    if (success)
                        shapes.Add(shape);
                    else
                        foreach (Cell cell in shape.blocks)
                            grid[cell.x, cell.y] = false;
                }
            }
        }
    }

    bool GetShape(out Shape shape, Cell firstBlock)
    {
        shape = new Shape();
        Cell newShapeBlock = firstBlock;

        for (int i = 0; i < 3; i++)
        {
            // Add Cell
            shape.blocks.Add(newShapeBlock);
            grid[newShapeBlock.x, newShapeBlock.y] = true;

            // Get List of Neighbour Cells
            List<Cell> neighbourCells;

            if (neighbourCellsSearch == SearchType.AllBlocks) neighbourCells = Neighbours.Shape(shape, gridSize, grid);
            else neighbourCells = Neighbours.Block(newShapeBlock, gridSize, grid);

            // Choose a random Neighbour Cell
            if (neighbourCells.Count > 0) newShapeBlock = neighbourCells[Random.Range(0, neighbourCells.Count)];
            else return false;
        }

        // Add Cell
        shape.blocks.Add(newShapeBlock);
        grid[newShapeBlock.x, newShapeBlock.y] = true;

        return true;
    }

    void LoadShapeObjects()
    {
        Transform shapeParent = new GameObject("Shapes").transform;

        foreach (Shape shape in shapes)
        {
            // Color
            Color color = colors.colors[colorIndex];
            colorIndex++;
            if (colorIndex >= colors.colors.Length) colorIndex = 0;

            Transform shapeObj = ShapeObject.CreateShape(shape, blockPrefab, positionOffset, color);
            shapeObj.SetParent(shapeParent);
        }
    }
}
