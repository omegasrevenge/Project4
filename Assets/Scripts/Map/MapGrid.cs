using System.Collections.Generic;
using UnityEngine;

public class MapGrid : SceneRoot3D
{
    
    public const int TileSize = 256;
    public int MapRadius = 3;
    public const int PixelsToUnit = 100;
    public static readonly float UnitsPerTile = (float)TileSize / PixelsToUnit;

    public Transform Root;
    public List<RenderCell> Cells;
    public SmartGrid<RenderCell, RefCountedSprite> Grid;
    public LRUSpriteDictionary Dict;
    public MapUtils.ProjectedPos CurrentPosition;
    private MapUtils.ProjectedPos _loadedPosition;
    public int ZoomLevel = 17;

    private Vector3 _position;

	public int grid_version = 0;

    public float Width { get; set; }
    public float Height { get; set; }

    void Awake()
    {
        if (Root == null)
        {
            GameObject obj = new GameObject("root");
            obj.transform.parent = transform;
            Root = obj.transform;
            Root.localRotation = Quaternion.identity;
        }
        GameManager.CreateController<AssetLoader>("controller_assetloader");
        //transform.localEulerAngles = new Vector3(90f,0f,0f);

        int dim = 2 * MapRadius + 1;
        InitCells(dim, dim);
        Width = dim * UnitsPerTile;
        Height = dim * UnitsPerTile;
        Dict = new LRUSpriteDictionary((dim + 1) * (dim + 1));
        Grid = new SmartGrid<RenderCell, RefCountedSprite>(Cells.ToArray(), dim, dim);
        _loadedPosition = new MapUtils.ProjectedPos(MapRadius, MapRadius, ZoomLevel, 0, 0);
        _position = new Vector3();
    }

    void InitCells(int x, int y)
    {
        Cells = new List<RenderCell>();
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                GameObject obj = new GameObject("tile_" + j + "_" + i);
                SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
                renderer.material = (Material)Resources.Load("Materials/MapTile", typeof(Material));
                renderer.sortingOrder = -1;
                Cells.Add(obj.AddComponent<RenderCell>());
                Vector3 position = new Vector3();
                position.x = j * UnitsPerTile;
                position.y = -i * UnitsPerTile;
                obj.transform.parent = Root;
                obj.transform.localPosition = position;
                obj.transform.localRotation = Quaternion.identity;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_loadedPosition != CurrentPosition)
            LoadNewTiles();
        MoveToCenter();
    }

    private void LoadNewTiles()
    {
        if (Equals(CurrentPosition, default(MapUtils.ProjectedPos)))
            return;
        Grid.Shift(_loadedPosition.X - CurrentPosition.X, _loadedPosition.Y - CurrentPosition.Y);
        _loadedPosition = CurrentPosition;
        for (int i = _loadedPosition.Y - MapRadius; i <= _loadedPosition.Y + MapRadius; i++)
        {
            for (int j = _loadedPosition.X - MapRadius; j <= _loadedPosition.X + MapRadius; j++)
            {
                if (Grid[j, i] == null)
                {
                    Grid[j, i] = SpawnTile(j, i);
                }
            }
        }
		grid_version++;
    }

    private void MoveToCenter()
    {
        _position.x = -((MapRadius + (float)CurrentPosition.LocalX) * UnitsPerTile);
        _position.y = ((MapRadius + (float)CurrentPosition.LocalY) * UnitsPerTile);
        Root.localPosition = _position;
    }

    public RefCountedSprite SpawnTile(int x, int y)
    {
        RefCountedSprite sprite;
        if (Dict.TryGetValue(new LRUSpriteDictionary.SpriteID(x, y), out sprite))
        {
            return sprite;
        }
        sprite = new RefCountedSprite();
        Dict.Add(new LRUSpriteDictionary.SpriteID(x, y), sprite);
        //AssetLoader.Loader.Enqueue(string.Format("http://mts1.google.com/vt/lyrs=m@245168067&src=apiv3&hl=de&x={0}&y={1}&z={2}&apistyle=s.e%3Al%7Cp.v%3Aoff%2Cp.il%3Atrue%7Cp.w%3A2.4%7Cp.h%3A%2300f6ff%7Cp.s%3A54%2Cs.t%3A6%7Cp.c%3A%23ff92c2d6&style=47,37%7Csmartmaps%20(256x256)", x, y, ZoomLevel), sprite.SetSprite);
        if (GameManager.Singleton.Player.CurrentFaction == Player.Faction.NCE)
        {
            AssetLoader.Loader.Enqueue(string.Format("http://mt0.googleapis.com/vt?lyrs=m@258014715&src=apiv3&hl=de&x={0}&y={1}&z={2}&apistyle=p.v%3Aon%2Cs.e%3Al%7Cp.v%3Aoff%2Cp.il%3Atrue%7Cp.s%3A1%7Cp.h%3A%23ff0000%7Cp.g%3A1.69%7Cp.l%3A20%2Cs.t%3A6%7Cp.il%3Atrue%7Cp.s%3A-100%7Cp.w%3A2.1%7Cp.c%3A%23ff68a3ad%7Cp.l%3A17&style=47,37%7Csmartmaps", x, y, ZoomLevel), sprite.SetSprite);
        }
        else
        {
            AssetLoader.Loader.Enqueue(string.Format("http://mt1.googleapis.com/vt?lyrs=m@248009395&src=apiv3&hl=de&x={0}&y={1}&z={2}&apistyle=s.e%3Al%7Cp.v%3Aoff%2Cs.t%3A2&style=47,37%7Csmartmaps", x, y, ZoomLevel), sprite.SetSprite);
        }
        
        return sprite;
    }

    public Vector2 GetPosition(MapUtils.ProjectedPos pos)
    {
        pos.X -= Grid.OffsetX;
        pos.Y -= Grid.OffsetY;
        float x = (float)(pos.X + pos.LocalX) * UnitsPerTile + _position.x;
        float y = -((float)(pos.Y + pos.LocalY) * UnitsPerTile) + _position.y;
        return new Vector2(x, y);
    }
}
