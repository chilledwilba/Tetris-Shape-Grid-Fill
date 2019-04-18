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

    [SerializeField]
    int[] counts = new int[] { 0, 0, 0, 0, 0, 0 };

    void Start()
    {
        for (int i = 0; i < repeat; i++)
        {
            grid = new bool[gridx, gridy];
            shapes = new List<Shape>();

            GenerateGrid();
            //print((((gridx * gridy) / 4) - shapes.Count) * 4);
            counts[(((gridx * gridy) / 4) - shapes.Count)]++;
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

            //List<Block> neighbourBlocks = GetNeighbours(currentGridPosition);
            List<Block> neighbourBlocks = GetAllNeighbours(shape);

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

    #region Get Neighbour Blocks
    List<Block> GetAllNeighbours(Shape shape)
    {
        List<Block> blocks = new List<Block>();

        foreach (Block item in shape.blocks)
        {
            List<Block> tempBlocks = GetNeighbours(item);

            foreach (Block tempBlock in tempBlocks)
            {
                bool success = CheckDuplicate(tempBlock, blocks);
                if (success) blocks.Add(tempBlock);
            }
        }
        return blocks;
    }

    List<Block> GetNeighbours(Block currentGridPosition)
    {
        List<Block> blocks = new List<Block>();

        int x = currentGridPosition.x;
        int y = currentGridPosition.y;

        if (x + 1 >= 0 && x + 1 < gridx && grid[x + 1, y] == false) blocks.Add(new Block(x + 1, y));
        if (x - 1 >= 0 && x - 1 < gridx && grid[x - 1, y] == false) blocks.Add(new Block(x - 1, y));

        if (y + 1 >= 0 && y + 1 < gridy && grid[x, y + 1] == false) blocks.Add(new Block(x, y + 1));
        if (y - 1 >= 0 && y - 1 < gridy && grid[x, y - 1] == false) blocks.Add(new Block(x, y - 1));

        return blocks;
    }

    bool CheckDuplicate(Block block, List<Block> blocks)
    {
        foreach (Block item in blocks)
        {
            if (item.x == block.x && item.y == block.y)
            {
                return false;
            }
        }
        return true;
    }
    #endregion
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
