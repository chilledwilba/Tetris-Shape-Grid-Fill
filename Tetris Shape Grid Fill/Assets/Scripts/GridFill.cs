using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridFill : MonoBehaviour
{
    [Header("Grid")]
    public int gridx = 8;
    public int gridy = 7;
    public float positionOffset = 1.15f;
    public int repeat = 1;

    [Header("Blocks")]
    public Transform blockPrefab;

    [Header("Color")]
    public Color[] colors;

    // Lists
    bool[,] grid;
    List<Shape> shapes;

    void Start()
    {
        for (int i = 0; i < repeat; i++)
        {
            grid = new bool[gridx, gridy];
            shapes = new List<Shape>();

            GenerateGrid();
            print((((gridx * gridy) / 4) - shapes.Count) * 4);
        }

        LoadShapes();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Generate Grid
    void GenerateGrid()
    {
        for (int x = 0; x < gridx; x++)
        {
            for (int y = 0; y < gridy; y++)
            {
                if (grid[x, y] == false)
                {
                    bool success = GetShape(out Shape shape, x, y);

                    if (success) shapes.Add(shape);
                    else RevertGrid(shape);
                }
            }
        }
    }

    bool GetShape(out Shape shape, int x, int y)
    {
        // First Block
        shape = new Shape();
        shape.blocks.Add(new Block(x, y));
        grid[x, y] = true;

        // Randomly Choose 3 more blocks
        for (int i = 0; i < 3; i++)
        {
            Block currentGridPosition = shape.blocks[shape.blocks.Count - 1];
            List<Block> neighbourBlocks = GetNeighbourBlocks(currentGridPosition);

            if (neighbourBlocks.Count > 0)
            {
                int randomBlock = Random.Range(0, neighbourBlocks.Count);
                Block newBlock = neighbourBlocks[randomBlock];

                shape.blocks.Add(newBlock);
                grid[newBlock.x, newBlock.y] = true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    List<Block> GetNeighbourBlocks(Block currentGridPosition)
    {
        List<Block> neighbourBlocks = new List<Block>();

        int x = currentGridPosition.x;
        int y = currentGridPosition.y;

        if (x + 1 >= 0 && x + 1 < gridx && grid[x + 1, y] == false) neighbourBlocks.Add(new Block(x + 1, y));
        if (x - 1 >= 0 && x - 1 < gridx && grid[x - 1, y] == false) neighbourBlocks.Add(new Block(x - 1, y));

        if (y + 1 >= 0 && y + 1 < gridy && grid[x, y + 1] == false) neighbourBlocks.Add(new Block(x, y + 1));
        if (y - 1 >= 0 && y - 1 < gridy && grid[x, y - 1] == false) neighbourBlocks.Add(new Block(x, y - 1));

        return neighbourBlocks;
    }

    void RevertGrid(Shape shape)
    {
        // If a shape hits a dead end revert the grid values it changed
        foreach (Block block in shape.blocks)
        {
            grid[block.x, block.y] = false;
        }
    }
    #endregion

    void LoadShapes()
    {
        Transform shapeParent = new GameObject("Shapes").transform;

        int colorIndex = Random.Range(0, colors.Length);

        foreach (Shape shape in shapes)
        {
            Transform shapeObj = new GameObject("Shape").transform;
            shapeObj.SetParent(shapeParent);

            // Color
            Color color = colors[colorIndex];
            colorIndex++;
            if (colorIndex >= colors.Length) colorIndex = 0;

            foreach (Block block in shape.blocks)
            {
                Transform blockObj = Instantiate(blockPrefab, shapeObj);
                blockObj.position = new Vector3(block.x * positionOffset, block.y * positionOffset);
                blockObj.GetComponent<SpriteRenderer>().color = color;
            }
        }
    }
}

public class Shape
{
    public List<Block> blocks = new List<Block>();
}

public class Block
{
    public int x;
    public int y;

    public Block(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
