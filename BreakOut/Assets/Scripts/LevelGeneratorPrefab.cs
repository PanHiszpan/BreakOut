using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorPrefab : MonoBehaviour
{
    public GameObject StartPoint;

    public GameObject wall;
    public GameObject border;
    public GameObject enemy;

    public GameObject[] rooms;
    public GameObject[] corridors;

    public int howManyCycles;


    [SerializeField] private GameObject miner;

    public int gridSize = 5;

    public int widthSize;
    public int hightSize;


    private int direction = 0;

    [SerializeField] private bool roomPlaced = false;
    [SerializeField] private bool corridorPlaced = false;

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

        //yield return StartCoroutine(GenerateWalls());
        //yield return StartCoroutine(GenerateBorders());
        for (int i = 0; i < howManyCycles; i++)
        {
            ChangeDirection(0);
            roomPlaced = false;
            corridorPlaced = false;
            PlaceRoom();
            PlaceCorridor();
        }
        /*
        do
        {
            
        } while (roomPlaced || corridorPlaced);
        */
        yield return 0;
    }


    private void PlaceRoom()
    {
        bool[] prefabChecked = new bool[rooms.Length];
        bool sthUnchecked = true;

        //try placing random room froom rooms[], tries all in random order
        while (sthUnchecked == true)
        {
            int rand = Random.Range(0, rooms.Length);
            prefabChecked[rand] = true;
            if (!CheckIfHitRoom(rooms[rand].GetComponents<BoxCollider>()))
            {
                DestroyTiles(rooms[rand].GetComponents<BoxCollider>());
                Instantiate(rooms[rand], miner.transform.position, miner.transform.rotation);
                roomPlaced = true;
                break;
            }
            else
            {
                sthUnchecked = false; //reset
                //check if sth not checked
                for (int i = 0; i < prefabChecked.Length; i++)
                {
                    if (prefabChecked[i] == false)
                    {
                        sthUnchecked = true;
                        break;
                    }
                }
            }

        }

    }

    private void PlaceCorridor()
    {
        bool[] prefabChecked = new bool[rooms.Length];
        bool sthUnchecked = true;
        //try placing random corridor froom corridors[], tries all in random order
        while (sthUnchecked == true)
        {
            int rand = Random.Range(0, rooms.Length);
            prefabChecked[rand] = true;
            if (!CheckIfHitRoom(corridors[rand].GetComponents<BoxCollider>()) && !CheckIfHitCorridor(corridors[rand].GetComponents<BoxCollider>()))
            {
                DestroyTiles(corridors[rand].GetComponents<BoxCollider>());
                GameObject prefab = Instantiate(corridors[rand], miner.transform.position, miner.transform.rotation);
                GameObject endPoint = prefab.GetComponent<Transform>().GetChild(0).gameObject;
                miner.transform.rotation = endPoint.transform.rotation;
                miner.transform.position = endPoint.transform.position;
                break;
            }
            else
            {
                sthUnchecked = false; //reset
                //check if sth not checked
                for (int i = 0; i < prefabChecked.Length; i++)
                {
                    if (prefabChecked[i] == false)
                    {
                        sthUnchecked = true;
                        break;
                    }
                }
            }
        }
 
    }

    private bool CheckIfHitRoom(BoxCollider[] colliders)
    {
        foreach (BoxCollider i in colliders)
        {
            if (Physics.CheckBox(miner.transform.position + i.center * gridSize, i.size / 2, Quaternion.identity, 1 << 11))
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckIfHitCorridor(BoxCollider[] colliders)
    {
        foreach (BoxCollider i in colliders)
        {
            if (Physics.CheckBox(miner.transform.position + i.center * gridSize, i.size / 2, Quaternion.identity, 1 << 10))
            {
                return true;
            }
        }      
        return false;
    }

    private void DestroyTiles(BoxCollider[] colliders)
    {
        foreach (BoxCollider i in colliders)
        {
            Collider[] hit = Physics.OverlapBox(miner.transform.position + i.center * gridSize, i.size / 2, Quaternion.identity);

            foreach (Collider j in hit)
            {
                if (j.gameObject.layer == 12) //wall
                {
                    GameObject targetedTile = j.gameObject;
                    Destroy(targetedTile);
                }
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

}
