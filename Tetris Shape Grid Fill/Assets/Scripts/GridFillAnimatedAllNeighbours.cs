using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridFillAnimatedAllNeighbours : MonoBehaviour
{
    [Header("Grid")]
    public int gridx = 8;
    public int gridy = 7;
    public float positionOffset = 1.15f;

    [Header("Blocks")]
    public Transform blockPrefab;
    public Transform[] tempShapeBlocks;
    public Transform[] tempNeighbourBlocks;

    [Header("Color")]
    public Color[] colors;
    int colorIndex = 0;

    [Header("Animation Time")]
    public float animTime = 0.5f;

    // Lists
    bool[,] grid;
    List<Shape> shapes;

    void Start()
    {
        grid = new bool[gridx, gridy];
        shapes = new List<Shape>();
        colorIndex = Random.Range(0, colors.Length);

        StartCoroutine(GenerateGrid());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator GenerateGrid()
    {
        for (int x = 0; x < gridx; x++)
        {
            for (int y = 0; y < gridy; y++)
            {
                if (grid[x, y] == false)
                {
                    Shape shape = new Shape();

                    // Set first block
                    SetBlock(shape, x, y);

                    bool success = true;
                    int numberOfBlocks = 3;
                    while (numberOfBlocks > 0 && success)
                    {
                        numberOfBlocks--;

                        Block currentGridPosition = shape.blocks[shape.blocks.Count - 1];
                        List<Block> neighbourBlocks = GetNeighbourBlocks(shape);

                        yield return new WaitForSeconds(animTime);

                        if (neighbourBlocks.Count > 0)
                        {
                            int randomIndex = Random.Range(0, neighbourBlocks.Count);
                            Block block = neighbourBlocks[randomIndex];

                            SetBlock(shape, block.x, block.y);
                        }
                        else
                        {
                            success = false;
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
                        CreateShape(shape);
                        yield return new WaitForSeconds(animTime);
                    }
                    else
                    {
                        foreach (Block block in shape.blocks)
                        {
                            grid[block.x, block.y] = false;
                        }
                    }
                }
            }
        }
    }

    void SetBlock(Shape shape, int x, int y)
    {
        shape.blocks.Add(new Block(x, y));
        grid[x, y] = true;

        Transform tempShapeBlock = tempShapeBlocks[shape.blocks.Count - 1];
        tempShapeBlock.gameObject.SetActive(true);
        tempShapeBlock.position = new Vector3(x * positionOffset, y * positionOffset);
    }

    #region Get Neighbour Blocks
    List<Block> GetNeighbourBlocks(Shape shape)
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

        // Set Neighbour Block Positions
        foreach (Transform block in tempNeighbourBlocks)
        {
            block.gameObject.SetActive(false);
        }

        for (int i = 0; i < blocks.Count; i++)
        {
            Block block = blocks[i];
            tempNeighbourBlocks[i].gameObject.SetActive(true);
            tempNeighbourBlocks[i].position = new Vector3(block.x * positionOffset, block.y * positionOffset);
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

    void CreateShape(Shape shape)
    {
        // Color
        Color color = colors[colorIndex];
        colorIndex++;
        if (colorIndex >= colors.Length) colorIndex = 0;

        Transform shapeObj = new GameObject("Shape").transform;

        foreach (Block block in shape.blocks)
        {
            Transform blockObj = Instantiate(blockPrefab, shapeObj);
            blockObj.position = new Vector3(block.x * positionOffset, block.y * positionOffset);
            blockObj.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
