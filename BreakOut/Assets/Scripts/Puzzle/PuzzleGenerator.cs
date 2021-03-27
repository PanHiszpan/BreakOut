using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    public GameObject StartPoint;

    public GameObject wall;
    public GameObject floor;
    public GameObject border;
    public GameObject exitRoomSmall;
    public GameObject ball;

    [SerializeField] private GameObject miner;

    public int howManyCycles;
    public float gridSize = 5;

    public int widthSize;
    public int hightSize;

    public float corridorChance = 5;


    private int direction = 0;

    private void Start()
    {
        miner.transform.position = StartPoint.transform.position;
        GenerateLvl();
    }

    private void GenerateWalls()
    {
        for (int y = -hightSize; y <= hightSize; y++)
        {
            for (int x = -widthSize; x <= widthSize; x++)
            {
                Instantiate(wall, miner.transform.position + new Vector3(x, 0, y) * gridSize, Quaternion.identity);
            }
        }

    }
    private void GenerateBorders()
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


    }

    private void GenerateLvl()
    {

        Debug.Log("generating level");

        GenerateWalls();
        GenerateBorders();

        Dig(0, 0, floor);
        Instantiate(ball, miner.transform.position, Quaternion.identity);
        for (int i = 0; i < howManyCycles; i++)
        {
            if (CheckIfHitBorder())
            {
                ChangeDirection(0);
            }
            else
            {
                Move();
                Dig(0, 0, floor);

                if (Random.Range(0, 100) < corridorChance)
                {
                    ChangeDirection(5);
                    corridorChance = 0;
                }
                else
                {
                    corridorChance += 5;
                }
            }

        }
        MakeExit();
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

        Collider[] hit = Physics.OverlapBox(miner.transform.position + offset, new Vector3(1, 1, 1) / 2, miner.transform.rotation); //szuka tylko scian w obszarze boxa (1,1,1)

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


    private void MakeExit()
    {
        Vector3 center = miner.transform.position + miner.transform.forward * gridSize * 2;


        Collider[] hit = Physics.OverlapBox(miner.transform.position * gridSize, new Vector3(1, 1, 1) / 2, miner.transform.rotation);
        foreach (Collider i in hit)
        {
            GameObject targetedTile = i.gameObject;
            Destroy(targetedTile);
        }

        Instantiate(exitRoomSmall, miner.transform.position, miner.transform.rotation);

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
                direction = Random.Range(0, 2);
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
    }

}
