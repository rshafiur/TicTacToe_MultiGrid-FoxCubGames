using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    //grid specifics
    private int grid;
    [SerializeField]
    private Vector2 gridSize;
    private Vector2 gridOffset;

    //about cells
    [SerializeField]
    private Sprite cellSprite;
    [SerializeField]
    private GameObject cellObject;
    private Vector2 cellSize;
    private Vector2 cellScale;
    private int cellIndex;

    void Start()
    {
        InitCells();
    }

    void InitCells()
    {
        if (PlayerPrefs.GetInt("Grid") != 0)
            grid = PlayerPrefs.GetInt("Grid");
        else
            grid = 3;

        cellIndex = 0; 

        //catch the size of the sprite
        cellSize = cellSprite.bounds.size;

        Vector2 newCellSize = new Vector2(gridSize.x / (float)grid, gridSize.y / (float)grid);

        //Geting the scales
        cellScale.x = newCellSize.x / cellSize.x;
        cellScale.y = newCellSize.y / cellSize.y;
        
        //new computed size
        cellSize = newCellSize;
        cellObject.transform.localScale = new Vector2(cellScale.x, cellScale.y);
        gridOffset.x = -(gridSize.x / 2) + cellSize.x / 2;
        gridOffset.y = -(gridSize.y / 2) + cellSize.y / 2;

        //Creating grid
        for (int row = 0; row < grid; row++)
        {
            for (int col = 0; col < grid; col++)
            {
                Vector2 pos = new Vector2(col * cellSize.x + gridOffset.x + transform.position.x, row * cellSize.y + gridOffset.y + transform.position.y);
                GameObject cO = Instantiate(cellObject, pos, Quaternion.identity) as GameObject;
                cO.transform.parent = transform;

                //Cell indexing
                cO.GetComponent<CellInfo>().cellNumber = cellIndex;
                cellIndex += 1;
            }
        }
    }
}