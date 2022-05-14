using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum DIRECTION : int
{
    Left = 0,
    Right = 1,
    Up = 2,
    Down = 3
}



public class WorldGenController : MonoBehaviour
{
    // Start is called before the first frame update

    public Tilemap ground;
    public Tilemap walls;
    public Tilemap pits;
    public TileBase tileGround;
    public Room roommap;
    //[SerializeField]
    public TileBase[] tileDictionary;
    public Vector3Int startVector;
    public Enemy[] enemySmallDictionary;
    public Enemy[] enemyLargeDictionary;
    public Enemy[] enemyBossDictionary;
    public PlayerController player;
    public CameraController gamecamera;
    List<(int, int, int, int)> availableRooms = new List<(int, int, int, int)>();
    List<(int, int, int, int)> generatedRooms = new List<(int, int, int, int)>();
    (int, int, int, int) bossRoom;
    ScenePayload payload;
    public GameObject key;
    public GameObject keyhole;
    bool hasSpawnedWalls;
    public BossController boss;



    int[,] map = new int[20, 20];
    int currentPosX, currentPosY;
    private void Start()
    {
        payload = FindObjectOfType<ScenePayload>();
        //ground.SetTile(new Vector3Int(0,0,0), tileGround);
        readLevelLayout();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void readLevelLayout()
    {
        if (payload)
        {
            tileDictionary = new TileBase[3];
            tileDictionary[0] = payload.ground;
            tileDictionary[1] = payload.wall;
            tileDictionary[2] = payload.pit;
        }
        //ground.BoxFill(new Vector3Int(-10, -10, 0), tileDictionary[1], 0, 0, 300, 220);
        Vector3Int currentpos = startVector;
        currentPosX = 10;
        currentPosY = 10;
        List<DIRECTION> availableDirections = new List<DIRECTION>();
        availableDirections = new List<DIRECTION>();
        availableDirections.Add(DIRECTION.Left);
        availableDirections.Add(DIRECTION.Right);
        availableDirections.Add(DIRECTION.Up);
        availableDirections.Add(DIRECTION.Down);
        //Start room generation
        GenerateStartRoom(ref currentpos, ref currentPosX, ref currentPosY);
        //Regular room generation
        if (payload)
            GenerateRegularRooms(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections, payload.roomnumber);
        else
            GenerateRegularRooms(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections, 15);
        //Key Room generation
        KeyRoomGeneration(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections);
        //Boss Room Generation
        BossRoomGeneration(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections);
        //Patch up holes
        PatchUp();

    }

    void GenerateStartRoom(ref Vector3Int currentpos, ref int currentPosX, ref int currentPosY)
    {
        if (roommap != null)
        {
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    ground.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.startroom[i, j]]);
                    if (roommap.startroom[i, j] == 1)
                    {
                        walls.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.startroom[i, j]]);
                        //walls.SetColliderType(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), Tile.ColliderType.Sprite);
                    }
                    else if (roommap.startroom[i, j] == 2)
                    {
                        pits.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.startroom[i, j]]);
                    }
                }
            }
        }
        map[currentPosX, currentPosY] = 1;
        PlayerController spawnedPlayer = Instantiate(player, new Vector3(7f, 5f, 0f) + (Vector3Int)currentpos + new Vector3(0.5f, 0.5f, 0f), Quaternion.Euler(0, 0, 0));
        gamecamera.player = spawnedPlayer;
        //availableRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
    }

    void GenerateRegularRooms(ref Vector3Int currentpos, ref int currentPosX, ref int currentPosY, ref List<DIRECTION> availableDirections, int max)
    {
        DIRECTION nextdirection = (DIRECTION)Random.Range(0, 4);
        int k = 0;
        while (k < max)
        {
            int roomnumber = Random.Range(0, 10);
            GenerateAvailableDirections(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections);
            //Debug.Log(availableDirections.Count);
            //Debug.Log((currentPosX, currentPosY, currentpos.x, currentpos.y));

            if (availableDirections.Count > 0)
            {
                GenerateNextDirection(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections, ref nextdirection);
                if (roommap != null)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            ground.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.rooms[roomnumber][i, j]]);
                            if (roommap.rooms[roomnumber][i, j] == 1)
                            {
                                walls.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.rooms[roomnumber][i, j]]);
                            }
                            else if (roommap.rooms[roomnumber][i, j] == 2)
                            {
                                pits.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.rooms[roomnumber][i, j]]);
                            }
                            //ground.SetColliderType(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), Tile.ColliderType.None);

                        }
                    }
                    for (int l = 0; l < roommap.enemyPositions[roomnumber].Count; l++)
                    {
                        Transform tempTransform = gameObject.transform;
                        tempTransform.position = new Vector3((float)roommap.enemyPositions[roomnumber][l].Item2.x + (float)currentpos.x + 0.5f, (float)roommap.enemyPositions[roomnumber][l].Item2.y + (float)currentpos.y + 0.5f, 0);
                        tempTransform.rotation = new Quaternion(0, 0, 0, 0);
                        tempTransform.localScale = new Vector3(1, 1, 1);
                        //  Debug.Log(" "tempTransform.position);

                        if (roommap.enemyPositions[roomnumber][l].Item1 == EnemyType.small)
                        {
                            Instantiate<Enemy>(enemySmallDictionary[Random.Range(0, enemySmallDictionary.Length)], (Vector3)(Vector3Int)roommap.enemyPositions[roomnumber][l].Item2 + currentpos + new Vector3(0.5f, 0.5f, 0f), tempTransform.rotation = new Quaternion(0, 0, 0, 0));
                        }
                        else if (roommap.enemyPositions[roomnumber][l].Item1 == EnemyType.large)
                        {
                            //  Instantiate<Enemy>(enemyLargeDictionary[Random.Range(0, enemySmallDictionary.Length)], );
                        }
                        else
                        {
                            //   Instantiate<Enemy>(enemyBossDictionary[Random.Range(0, enemySmallDictionary.Length)],);
                        }
                    }

                }
                map[currentPosX, currentPosY] = 2;
                k++;
            }
            else
            {
                GetNewAvailablePosition(ref currentpos, ref currentPosX, ref currentPosY);
            }
            if (!availableRooms.Contains((currentPosX, currentPosY, currentpos.x, currentpos.y))) availableRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
        }
    }

    void KeyRoomGeneration(ref Vector3Int currentpos, ref int currentPosX, ref int currentPosY, ref List<DIRECTION> availableDirections)
    {
        DIRECTION nextdirection = (DIRECTION)Random.Range(0, 4);
        bool hasGenerated = false;
        while (hasGenerated == false)
        {
            (int, int, int, int) keyPos = availableRooms[Random.Range(0, availableRooms.Count)];
            currentpos.x = keyPos.Item3;
            currentpos.y = keyPos.Item4;
            currentPosX = keyPos.Item1;
            currentPosY = keyPos.Item2;
            GenerateAvailableDirections(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections);

            if (availableDirections.Count > 0)
            {
                GenerateNextDirection(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections, ref nextdirection);
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        ground.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.keyroom[i, j]]);
                        if (roommap.keyroom[i, j] == 1)
                        {
                            walls.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.keyroom[i, j]]);
                            //walls.SetColliderType(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), Tile.ColliderType.Sprite);
                        }
                        else if (roommap.keyroom[i, j] == 2)
                        {
                            pits.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.keyroom[i, j]]);
                        }
                    }
                }
                map[currentPosX, currentPosY] = 3;
                if (!generatedRooms.Contains((currentPosX, currentPosY, currentpos.x, currentpos.y))) generatedRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
                availableRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
                Instantiate(key, new Vector3(7f, 5f, 0f) + (Vector3Int)currentpos + new Vector3(0.5f, 0.5f, 0f), Quaternion.Euler(0, 0, 0));

                //spawnkey
                hasGenerated = true;
            }
            else
            {
                GetNewAvailablePosition(ref currentpos, ref currentPosX, ref currentPosY);
            }
        }
    }

    void BossRoomGeneration(ref Vector3Int currentpos, ref int currentPosX, ref int currentPosY, ref List<DIRECTION> availableDirections)
    {
        DIRECTION nextdirection = (DIRECTION)Random.Range(0, 4);
        bool hasGenerated = false;
        while (!hasGenerated)
        {
            //Boss Room generation
            (int, int, int, int) bossPos = availableRooms[Random.Range(0, availableRooms.Count)];
            currentpos.x = bossPos.Item3;
            currentpos.y = bossPos.Item4;
            currentPosX = bossPos.Item1;
            currentPosY = bossPos.Item2;
            GenerateAvailableDirections(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections);

            if (availableDirections.Count > 0)
            {
                GenerateNextDirection(ref currentpos, ref currentPosX, ref currentPosY, ref availableDirections, ref nextdirection);

                if (roommap != null)
                {
                    for (int i = 0; i < 11; i++)
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            ground.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.bossroom[i, j]]);
                            if (roommap.bossroom[i, j] == 1)
                            {
                                walls.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.bossroom[i, j]]);
                                //walls.SetColliderType(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), Tile.ColliderType.Sprite);
                            }
                            else if (roommap.bossroom[i, j] == 2)
                            {
                                pits.SetTile(new Vector3Int(currentpos.x + j, currentpos.y + i, 0), tileDictionary[roommap.bossroom[i, j]]);
                            }
                        }
                    }
                }
                map[currentPosX, currentPosY] = 3;
                if (!generatedRooms.Contains((currentPosX, currentPosY, currentpos.x, currentpos.y))) generatedRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
                availableRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
                hasGenerated = true;
                bool atBottom = false;
                bool atTop = false;
                bool atLeftSide = false;
                bool atRightSide = false;
                if (currentPosX - 1 < 0) atLeftSide = true;
                else if (currentPosX + 1 > 19) atRightSide = true;
                if (currentPosY - 1 < 0) atBottom = true;
                else if (currentPosY + 1 > 19) atTop = true;
                if (!atRightSide && map[currentPosX + 1, currentPosY] != 0) Instantiate(keyhole, new Vector3(14f, 5f, 0f) + (Vector3Int)currentpos + new Vector3(0.5f, 0.5f, 0f), Quaternion.Euler(0, 0, 0));
                if (!atLeftSide && map[currentPosX - 1, currentPosY] != 0) Instantiate(keyhole, new Vector3(0f, 5f, 0f) + (Vector3Int)currentpos + new Vector3(0.5f, 0.5f, 0f), Quaternion.Euler(0, 0, 0));
                if (!atTop && map[currentPosX, currentPosY + 1] != 0) Instantiate(keyhole, new Vector3(7f, 10f, 0f) + (Vector3Int)currentpos + new Vector3(0.5f, 0.5f, 0f), Quaternion.Euler(0, 0, 0));
                if (!atBottom && map[currentPosX, currentPosY - 1] != 0) Instantiate(keyhole, new Vector3(7f, 0f, 0f) + (Vector3Int)currentpos + new Vector3(0.5f, 0.5f, 0f), Quaternion.Euler(0, 0, 0));
                bossRoom = (currentPosX, currentPosY, currentpos.x, currentpos.y);
                Instantiate(boss, new Vector3(7f, 5f, 0f) + (Vector3Int)currentpos + new Vector3(0.5f, 0.5f, 0f), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                GetNewAvailablePosition(ref currentpos, ref currentPosX, ref currentPosY);
            }
        }
    }

    void GenerateAvailableDirections(ref Vector3Int currentpos, ref int currentPosX, ref int currentPosY, ref List<DIRECTION> availableDirections)
    {
        bool atBottom = false;
        bool atTop = false;
        bool atLeftSide = false;
        bool atRightSide = false;
        availableDirections = new List<DIRECTION>();

        if (currentPosX - 1 < 0) atLeftSide = true;
        else if (currentPosX + 1 > 19) atRightSide = true;
        if (currentPosY - 1 < 0) atBottom = true;
        else if (currentPosY + 1 > 19) atTop = true;
        if ((!atBottom || !atTop || !atLeftSide || !atRightSide) && !availableRooms.Contains((currentPosX, currentPosY, currentpos.x, currentpos.y))) availableRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
        else if (atBottom && atTop && atLeftSide && atRightSide) availableRooms.Remove((currentPosX, currentPosY, currentpos.x, currentpos.y));

        if (!atRightSide && map[currentPosX + 1, currentPosY] == 0) availableDirections.Add(DIRECTION.Right);
        if (!atLeftSide && map[currentPosX - 1, currentPosY] == 0) availableDirections.Add(DIRECTION.Left);
        if (!atTop && map[currentPosX, currentPosY + 1] == 0) availableDirections.Add(DIRECTION.Up);
        if (!atBottom && map[currentPosX, currentPosY - 1] == 0) availableDirections.Add(DIRECTION.Down);
    }

    void GenerateNextDirection(ref Vector3Int currentpos, ref int currentPosX, ref int currentPosY, ref List<DIRECTION> availableDirections, ref DIRECTION nextdirection)
    {
        if (!generatedRooms.Contains((currentPosX, currentPosY, currentpos.x, currentpos.y))) generatedRooms.Add((currentPosX, currentPosY, currentpos.x, currentpos.y));
        nextdirection = (DIRECTION)availableDirections[Random.Range(0, availableDirections.Count)];
        switch (nextdirection)
        {
            case DIRECTION.Left: currentpos.x = currentpos.x - 15; currentPosX -= 1; break;
            case DIRECTION.Right: currentpos.x = currentpos.x + 15; currentPosX += 1; break;
            case DIRECTION.Up: currentpos.y = currentpos.y + 11; currentPosY += 1; break;
            case DIRECTION.Down: currentpos.y = currentpos.y - 11; currentPosY -= 1; break;
        }
    }

    void PatchUp()
    {
        for (int i = 0; i < availableRooms.Count; i++)
        {
            (int, int, int, int) newPos = availableRooms[i];

            Vector3Int currentpos = new Vector3Int();
            currentpos.x = newPos.Item3;
            currentpos.y = newPos.Item4;
            int currentPosX = newPos.Item1;
            int currentPosY = newPos.Item2;
            if (currentPosX - 1 < 0 || map[currentPosX - 1, currentPosY] == 0)
            {
                ground.SetTile(new Vector3Int(currentpos.x, currentpos.y + 5, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(currentpos.x, currentpos.y + 5, 0), tileDictionary[1]);
            }
            if (currentPosX + 1 > 19 || map[currentPosX + 1, currentPosY] == 0)
            {
                ground.SetTile(new Vector3Int(currentpos.x + 14, currentpos.y + 5, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(currentpos.x + 14, currentpos.y + 5, 0), tileDictionary[1]);
            }
            if (currentPosY - 1 < 0 || map[currentPosX, currentPosY - 1] == 0)
            {
                ground.SetTile(new Vector3Int(currentpos.x + 7, currentpos.y, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(currentpos.x + 7, currentpos.y, 0), tileDictionary[1]);
            }
            if (currentPosY + 1 > 19 || map[currentPosX, currentPosY + 1] == 0)
            {
                ground.SetTile(new Vector3Int(currentpos.x + 7, currentpos.y + 10, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(currentpos.x + 7, currentpos.y + 10, 0), tileDictionary[1]);

            }
        }
    }

    void GetNewAvailablePosition(ref Vector3Int currentpos, ref int currentPosX, ref int currentPosY)
    {
        (int, int, int, int) newPos = availableRooms[Random.Range(0, availableRooms.Count)];
        currentpos.x = newPos.Item3;
        currentpos.y = newPos.Item4;
        currentPosX = newPos.Item1;
        currentPosY = newPos.Item2;
    }

    public bool SpawnBossWall()
    {
        if (!hasSpawnedWalls)
        {
            Vector3Int bosspos = new Vector3Int();
            int bossPosX = bossRoom.Item1;
            int bossPosY = bossRoom.Item2;
            bosspos.x = bossRoom.Item3;
            bosspos.y = bossRoom.Item4;
            if (bossPosX - 1 > 0 && map[bossPosX - 1, bossPosY] != 0)
            {
                ground.SetTile(new Vector3Int(bosspos.x, bosspos.y + 5, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(bosspos.x, bosspos.y + 5, 0), tileDictionary[1]);
               
            }
            if (bossPosX + 1 < 19 && map[bossPosX + 1, bossPosY] != 0)
            {
                ground.SetTile(new Vector3Int(bosspos.x + 14, bosspos.y + 5, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(bosspos.x + 14, bosspos.y + 5, 0), tileDictionary[1]);
               
            }
            if (bossPosY - 1 > 0 && map[bossPosX, bossPosY - 1] != 0)
            {
                ground.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y, 0), tileDictionary[1]);
               
            }
            if (bossPosY + 1 < 19 && map[bossPosX, bossPosY + 1] != 0)
            {
                ground.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y + 10, 0), tileDictionary[1]);
                walls.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y + 10, 0), tileDictionary[1]);
               

            }
            ground.SetTile(new Vector3Int(bosspos.x + 1, bosspos.y + 5, 0), tileDictionary[2]);
            pits.SetTile(new Vector3Int(bosspos.x + 1, bosspos.y + 5, 0), tileDictionary[2]);
            ground.SetTile(new Vector3Int(bosspos.x + 13, bosspos.y + 5, 0), tileDictionary[2]);
            pits.SetTile(new Vector3Int(bosspos.x + 13, bosspos.y + 5, 0), tileDictionary[2]);
            ground.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y + 1, 0), tileDictionary[2]);
            pits.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y + 1, 0), tileDictionary[2]);
            ground.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y + 9, 0), tileDictionary[2]);
            pits.SetTile(new Vector3Int(bosspos.x + 7, bosspos.y + 9, 0), tileDictionary[2]);
            return true;
        }
        else
        {
            return false;
        }
       
    }
}
