using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;

    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;

    public int MinFood = 3;
    public int MaxFood = 7;
    public FoodObject[] FoodPrefabs;
    public WallObject[] WallPrefabs;

    public ExitCellObject ExitCellPrefab;
    private List<Vector2Int> m_EmptyCellsList;

    public Enemy[] EnemyPrefabs;


    public void Init(int level)
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();

        m_BoardData = new CellData[Width, Height];

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        m_EmptyCellsList.Remove(endCoord);

        GenerateWall(level);
        GenerateFood(level);
        GenerateEnemy(level);
        PolygonCollider2D confiner = GetComponent<PolygonCollider2D>();
        if (confiner != null)
        {
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(0, 0);          // ta7t 4mal
            points[1] = new Vector2(Width, 0);      // ta7t ymin
            points[2] = new Vector2(Width, Height); // fo2 ymin
            points[3] = new Vector2(0, Height);     // fo2 4mal
            confiner.SetPath(0, points);

            Unity.Cinemachine.CinemachineConfiner2D camConfiner = FindAnyObjectByType<Unity.Cinemachine.CinemachineConfiner2D>();
            if (camConfiner != null)
            {
                camConfiner.InvalidateBoundingShapeCache();
            }
        }
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    void GenerateWall(int level)
    {
        int wallCount = Random.Range(15, 20 + (level * 2));
        for (int i = 0; i < wallCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) return;
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            WallObject newWall = Instantiate(WallPrefabs[Random.Range(0, WallPrefabs.Length)]);
            AddObject(newWall, coord);
        }
    }

    void GenerateFood(int level)
    {
        int foodCount;

        
        if (level % 5 == 0)
        {
            foodCount = Random.Range(1, 4);
        }
        else
        {
            
            foodCount = Random.Range(8, 12 + level);
        }

        for (int i = 0; i < foodCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) return;
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject newFood = Instantiate(FoodPrefabs[Random.Range(0, FoodPrefabs.Length)]);
            AddObject(newFood, coord);
        }
    }

    void GenerateEnemy(int level)
    {
       
        int enemyCount = 1 + (level / 2);

        for (int i = 0; i < enemyCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) return;

            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            Enemy newEnemy = Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)]);
            AddObject(newEnemy, coord);
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public void Clean()
    {
        if (m_BoardData == null)
            return;


        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = m_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }

}