using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorBlind : MonoBehaviour
{
    public GameObject StartPoint;

    public GameObject wall;
    public GameObject floor;
    public GameObject roomFloor;
    public GameObject roomWall;
    public GameObject border;
    public GameObject exitRoom;
    public GameObject exitRoomSmall;
    public GameObject enemy;

    [SerializeField] private GameObject miner;

    public int howManyCycles;
    public int gridSize = 5;

    public int widthSize;
    public int hightSize;

    public float corridorChance = 5;
    public float roomChance = 5;
    public int[] roomWidthRange;
    public int[] roomHightRange;
    public float enemyChance;

    private int direction = 0;

    private void Start()
    {
        miner.transform.position = StartPoint.transform.position;
        StartCoroutine(GenerateLvl());
    }

    IEnumerator GenerateWalls()
    {
        for (int y = -hightSize; y <= hightSize; y++)
        {
            for (int x = -widthSize; x <= widthSize; x++)
            {
                Instantiate(wall, miner.transform.position + new Vector3(x, 0, y) * gridSize, Quaternion.identity);
            }
        }
        yield return 0;
    }
    IEnumerator GenerateBorders()
    {
        //down border
        for (int x = -widthSize - 1; x <= widthSize + 1; x++)
        {
            Instantiate(border, miner.transform.position + new Vector3(x, 0, -hightSize) * gridSize, Quaternion.identity);
        }

        for (int y = -hightSize; y <= hightSize; y++)
        {
            Instantiate(border, miner.transform.position + new Vector3(-widthSize - 1, 0, y) * gridSize, Quaternion.identity);
            Instantiate(border, miner.transform.position + new Vector3(widthSize + 1, 0, y) * gridSize, Quaternion.identity);
        }
        //upper border
        for (int x = -widthSize - 1; x <= widthSize + 1; x++)
        {
            Instantiate(border, miner.transform.position + new Vector3(x, 0, hightSize) * gridSize, Quaternion.identity);
        }

        yield return 0;
    }

    IEnumerator GenerateLvl()
    {

        Debug.Log("generating level");

        yield return StartCoroutine(GenerateWalls());
        yield return StartCoroutine(GenerateBorders());

        MakeRoom(Random.Range(roomWidthRange[0], roomWidthRange[1] + 1), Random.Range(roomHightRange[0], roomHightRange[1] + 1));

        for (int i = 0; i < howManyCycles; i++)
        {
            if (CheckIfHitBorder())
            {
                ChangeDirection(2);
            }
            else
            {
                Move();
                Dig(0, 0, floor);

                if (Random.Range(0, 100) < corridorChance)
                {
                    ChangeDirection(0);
                    corridorChance = 0;
                }
                else
                {
                    corridorChance += 2;
                }
                if (Random.Range(0, 100) < roomChance)
                {
                    MakeRoom(Random.Range(roomWidthRange[0], roomWidthRange[1] + 1), Random.Range(roomHightRange[0], roomHightRange[1] + 1));
                    roomChance = 0;
                }
                else
                {
                    roomChance += 2;
                }
            }

        }
        MakeExit();
        yield return 0;
    }


    void MakeRoom(int width, int hight)
    {

        Debug.Log("making room [ " + width + "," + hight + "]");
        //floor
        for (int y = -hight + 1; y <= hight - 1; y++)
        {
            for (int x = -width + 1; x <= width - 1; x++)
            {
                Dig(x, y, roomFloor);
                //try spawn enemy
                if (Random.Range(0, 100) < enemyChance)
                {
                    SpawnEnemy(x, y);
                }
            }
        }
    }

    
    private bool CheckIfHitBorder()
    {
        //Collider[] hits = Physics.OverlapBox(miner.position + miner.forward * gridSize, new Vector3(1, 1, 1) / 2, miner.rotation, 1 << 14); //szuka tylko floor w obszarze boxa (3,1,1)/3 bloki przed minerem
        bool ishit = Physics.CheckBox(miner.transform.position + miner.transform.forward * gridSize, new Vector3(1, 1, 1) / 2, Quaternion.identity, 1 << 14);
        if (ishit)
        {
            return true;
        }
        else
        {

            return false;
        }
    }
    private void Dig(int x, int y, GameObject prefab)
    {
        Vector3 offset = new Vector3(x, 0, y) * gridSize;

        Collider[] hit = Physics.OverlapBox(miner.transform.position + offset, new Vector3(1, 10, 1) / 2, miner.transform.rotation); //szuka tylko scian w obszarze boxa (1,1,1)

        bool minerPath = false;
        foreach (Collider i in hit)
        {
            if (i.gameObject.layer == 12 || i.gameObject.layer == 13) //wall or roomwall
            {
                GameObject targetedTile = i.gameObject;
                Destroy(targetedTile);
            }
            if (i.gameObject.layer == 10) //level
            {
                minerPath = true;
            }
        }
        if (!minerPath)
        {
            Instantiate(prefab, miner.transform.position + offset, Quaternion.identity);
        }
        minerPath = false;
    }

    private void SpawnEnemy(int x, int y)
    {
        Vector3 offset = new Vector3(x * gridSize, 1, y * gridSize);
        Instantiate(enemy, miner.transform.position + offset, Quaternion.identity);
    }

    private void MakeExit()
    {
        Vector3 center = miner.transform.position + miner.transform.forward * gridSize * 2;
        //Destroy tiles
        for (int y = -2; y <= 2; y++)
        {
            for (int x = -2 + 1; x <= 2 - 1; x++)
            {
                Collider[] hit = Physics.OverlapBox(center + new Vector3(x, 0, y) * gridSize, new Vector3(1, 1, 1) / 2, miner.transform.rotation); //szuka tylko scian w obszarze boxa (1,1,1)
                foreach (Collider i in hit)
                {
                    GameObject targetedTile = i.gameObject;
                    Destroy(targetedTile);
                }
            }
        }
        //place exit
        Instantiate(exitRoom, center, miner.transform.rotation);

    }

    private void ChangeDirection(int dir)
    {
        switch (dir)
        {
            case 0:
                //rotate random
                direction = Random.Range(1, 5);
                miner.transform.Rotate(Vector3.up, direction * 90f);
                Debug.Log("Rotate: " + (direction * 90f));
                break;
            case 1:
                //rotate 90
                miner.transform.Rotate(Vector3.up, 90f);
                Debug.Log("Rotate: " + 90f);
                break;
            case 2:
                //rotate 180
                miner.transform.transform.Rotate(Vector3.up, 180f);
                Debug.Log("Rotate: " + 180f);
                break;
            case 3:
                //rotate 270
                miner.transform.Rotate(Vector3.up, 270f);
                Debug.Log("Rotate: " + 270f);
                break;
            case 4:
                // no rotate
                Debug.Log("Rotate: " + 0f);
                break;
            default:
                break;
        }

    }
    private void Move()
    {
        miner.transform.position = miner.transform.position + miner.transform.forward * gridSize;
        Debug.Log("[ " + miner.transform.position + "]");
    }
}
