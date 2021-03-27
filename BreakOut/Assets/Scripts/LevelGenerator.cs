using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
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
                Instantiate(wall, miner.transform.transform.position + new Vector3(x, 0, y) * gridSize, Quaternion.identity);
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
            if (roomChance >= 100)
            {
                break;
            }
            if (CheckIfHitBorder())
            {
                ChangeDirection(5);
            }
            else
            {
                Move();
                Debug.Log("Room%: " + roomChance);
                Dig(0, 0, floor);

                if (Random.Range(0, 100) < corridorChance)
                {
                    ChangeDirection(5);
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

        Debug.Log("making room [ "+width+","+hight+"]");
        if (CheckIfHaveSpace(miner.transform.position, width, hight))
        {
            //down wall
            for (int x = -width; x <= width; x++)
            {
                Dig(x, -hight, roomWall);
            }
            //floor
            for (int y = -hight+1; y <= hight-1; y++)
            {
                for (int x = -width+1; x <= width-1; x++)
                {
                    //left wall
                    Dig(-width,y,roomWall);
                    Dig(x,y,roomFloor);
                    //try spawn enemy
                    if (Random.Range(0,100) < enemyChance)
                    {
                        SpawnEnemy(x,y);
                    }
                    //right wall
                    Dig(width, y, roomWall);
                }
            }
            //upper wall
            for (int x = -width; x <= width; x++)
            {
                Dig(x, hight, roomWall);
            }
            /*
            //get out from the room
            ChangeDirection(0);
            if (miner.eulerAngles.y == 90f || miner.eulerAngles.y == 270f)
            {
                for (int i = 0; i < width; i++)
                {
                    if (CheckIfHitBorder()) //turn around and get out
                    {
                        Debug.LogWarning("hit border: " + miner.eulerAngles.y);
                        ChangeDirection(2);
                        Debug.LogWarning("Angle: " + miner.eulerAngles.y);
                        for (int j = 0; j < width * 2; j++)
                        {
                            Move();
                            Dig(0, 0, roomFloor);
                        }
                        break;
                    }
                    Move();
                    Dig(0, 0, roomFloor);
                }
            }
            else if (miner.eulerAngles.y == 180f || miner.eulerAngles.y == 360f || miner.eulerAngles.y == 0f)
            {
                for (int i = 0; i < hight; i++)
                {
                    if(!CheckIfHitBorder()) //turn around and get out
                    {
                        Debug.LogWarning("hit border: " + miner.position);
                        ChangeDirection(2);
                        Debug.LogWarning("Angle: " + miner.eulerAngles.y);
                        for (int j = 0; j < hight * 2; j++)
                        {
                            Move();
                            Dig(0, 0, roomFloor);
                        }
                        break;
                    }
                    Move();
                    Dig(0, 0, roomFloor);
                }
            }
            */
        }  
    }

    private bool CheckIfHaveSpace(Vector3 center, int width, int hight)
    {
        //RaycastHit hit;
        //bool ishit = Physics.BoxCast(miner.position, (new Vector3(width, 1, hight) * gridSize) / 2, miner.up, out hit, miner.rotation, 10);
        bool isRoomWallHit = Physics.CheckBox(center, (new Vector3((float)width, 1f, (float)hight) * (float)gridSize) / 2f, miner.transform.rotation, 1 << 13); //te chuje zrobili tak ze ptrzyjmuje nie warstwe a bitmaski, przesuniecie bitowe, czyli jedynke ...0001 przesuwa o dziesieć w prawo ...010000000000, czyli robi checkboxa tylko warstwie 10 - level
        bool isRoomHit = Physics.CheckBox(center, (new Vector3((float)width, 1f, (float)hight) * (float)gridSize) / 2f, miner.transform.rotation, 1 << 11);
        bool isBorderHit = Physics.CheckBox(center, (new Vector3((float)width, 1f, (float)hight) * (float)gridSize) / 2f, miner.transform.rotation, 1 << 14);


        if (isRoomWallHit || isRoomHit || isBorderHit)
        {
            Debug.Log("Brak miejsca na pokój");
            return false;
        }
        else
        {
            Debug.Log("Jest miejsce na pokój");
            return true;
        }
    }
    /*
    private bool CheckIfCanDig()
    {
        Collider[] hits = Physics.OverlapBox(miner.position + miner.forward * gridSize, new Vector3(1, 1, 1) / 2, miner.rotation, 1 << 13); //szuka tylko floor w obszarze boxa (3,1,1)/3 bloki przed minerem
        //bool ishit = Physics.CheckBox(miner.position + miner.forward * gridSize, new Vector3(1, 1, 1) / 2, Quaternion.identity, 1 << 10);
        if (hits == null)
        {
            return true;
        }
        else
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log("Stop, can't dig: " + hits[i].transform.position);
            }
            
            return false; 
        }
    }
    */
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
    private bool CheckIfHitRoomWall()
    {
        //Collider[] hits = Physics.OverlapBox(miner.position + miner.forward * gridSize, new Vector3(1, 1, 1) / 2, miner.rotation, 1 << 14); //szuka tylko floor w obszarze boxa (3,1,1)/3 bloki przed minerem
        bool ishit = Physics.CheckBox(miner.transform.position + miner.transform.forward * gridSize, new Vector3(1, 1, 1) / 2, Quaternion.identity, 1 << 13);
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
        if (CheckIfHaveSpace(center, 2, 2))
        {
            //Destroy tiles
            for (int y = -2; y <= 2; y++)
            {
                for (int x = -2; x <= 2; x++)
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
        else
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    Collider[] hit = Physics.OverlapBox(miner.transform.position + new Vector3(x, 0, y) * gridSize, new Vector3(1, 1, 1) / 2, miner.transform.rotation); //szuka tylko scian w obszarze boxa (1,1,1)
                    foreach (Collider i in hit)
                    {
                        GameObject targetedTile = i.gameObject;
                        Destroy(targetedTile);
                    }
                }
                Instantiate(exitRoomSmall, miner.transform.position, miner.transform.rotation);
            }
        }

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
                miner.transform.Rotate(Vector3.up, 180f);
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
            case 5:
                //rotate random
                direction = Random.Range(0,2);
                if (direction == 1)
                {
                    miner.transform.Rotate(Vector3.up, 90f);
                    Debug.Log("Rotate: " + (90f));
                }
                else
                {
                    miner.transform.Rotate(Vector3.up, -90f);
                    Debug.Log("Rotate: " + (-90f));
                }
                
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
