using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomNamespace;

public class GridFillAnimation : MonoBehaviour
{
    public GridSize gridSize = new GridSize(8, 7);
    public float positionOffset = 1.15f;
    public Transform blockPrefab;
    public Theme colors;
    int colorIndex = 0;

    public enum SearchType { LastBlock, AllBlocks };
    public SearchType neighbourCellsSearch;

    public float animTime = 0.5f;
    public Transform[] tempShapeBlocks;
    public Transform[] tempNeighbourBlocks;

    // Lists
    bool[,] grid;
    List<Shape> shapes;

    void Start()
    {
        grid = new bool[gridSize.x, gridSize.y];
        shapes = new List<Shape>();
        colorIndex = Random.Range(0, colors.colors.Length);

        StartCoroutine(GenerateGrid());
    }

    IEnumerator GenerateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y] == false)
                {
                    Shape shape = new Shape();
                    Cell newShapeBlock = new Cell(x, y);

                    bool success = true;
                    int numberOfBlocks = 4;
                    while (numberOfBlocks > 0 && success)
                    {
                        numberOfBlocks--;

                        // Add Cell
                        shape.blocks.Add(newShapeBlock);
                        grid[newShapeBlock.x, newShapeBlock.y] = true;

                        // Get List of Neighbour Cells
                        List<Cell> neighbourCells;

                        if (neighbourCellsSearch == SearchType.AllBlocks) neighbourCells = Neighbours.Shape(shape, gridSize, grid);
                        else neighbourCells = Neighbours.Block(newShapeBlock, gridSize, grid);

                        // Animation
                        SetTempShapeBlock(shape, newShapeBlock.x, newShapeBlock.y);
                        SetTempNeighbourBlocks(neighbourCells);

                        yield return new WaitForSeconds(animTime);

                        if (numberOfBlocks > 0)
                        {
                            // Choose a random Neighbour Cell
                            if (neighbourCells.Count > 0) newShapeBlock = neighbourCells[Random.Range(0, neighbourCells.Count)];
                            else success = false;
                        }
                    }

                    // Deactivate temp neighbour blocks
                    foreach (Transform block in tempNeighbourBlocks) block.gameObject.SetActive(false);

                    // Wait for last temp shape block
                    if (success) yield return new WaitForSeconds(animTime);

                    // Deactivate temp shape blocks
                    foreach (Transform block in tempShapeBlocks) block.gameObject.SetActive(false);

                    if (success)
                    {
                        shapes.Add(shape);

                        // Color
                        Color color = colors.colors[colorIndex];
                        colorIndex++;
                        if (colorIndex >= colors.colors.Length) colorIndex = 0;

                        ShapeObject.CreateShape(shape, blockPrefab, positionOffset, colors.colors[colorIndex]);

                        yield return new WaitForSeconds(animTime);
                    }
                    else
                    {
                        foreach (Cell cell in shape.blocks)
                        {
                            grid[cell.x, cell.y] = false;
                        }
                    }
                }
            }
        }
    }

    void SetTempShapeBlock(Shape shape, int x, int y)
    {
        Transform tempShapeBlock = tempShapeBlocks[shape.blocks.Count - 1];
        tempShapeBlock.gameObject.SetActive(true);
        tempShapeBlock.position = new Vector3(x * positionOffset, y * positionOffset);
    }

    void SetTempNeighbourBlocks(List<Cell> neighbourBlocks)
    {
        foreach (Transform block in tempNeighbourBlocks)
        {
            block.gameObject.SetActive(false);
        }

        for (int i = 0; i < neighbourBlocks.Count; i++)
        {
            Cell block = neighbourBlocks[i];
            tempNeighbourBlocks[i].gameObject.SetActive(true);
            tempNeighbourBlocks[i].position = new Vector3(block.x * positionOffset, block.y * positionOffset);
        }
    }
}