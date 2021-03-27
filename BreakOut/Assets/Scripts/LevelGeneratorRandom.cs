using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorRandom : MonoBehaviour
{
    public GameObject StartPoint;

    public GameObject wall;
    public GameObject floor;

    [SerializeField] private GameObject miner;

    public int gridSize = 5;

    public int widthSize;
    public int hightSize;

    public float roomChance = 5;

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
    

    IEnumerator GenerateLvl()
    {

        Debug.Log("generating level");

        yield return StartCoroutine(GenerateWalls());

        Dig(0, 0, floor);

        for (int y = -hightSize; y <= hightSize; y++)
        {
            for (int x = -widthSize; x <= widthSize; x++)
            {
                if (Random.Range(0, 100) < roomChance)
                {
                    Dig(x, y, floor);
                }

            }
        }
        yield return 0;

        yield return 0;
    }

    private void Dig(int x, int y, GameObject prefab)
    {
        Vector3 offset = new Vector3(x, 0, y) * gridSize;

        Collider[] hit = Physics.OverlapBox(miner.transform.position + offset, new Vector3(1, 10, 1) / 2, miner.transform.rotation); //szuka tylko scian w obszarze boxa (1,1,1)

        bool minerPath = false;
        foreach (Collider i in hit)
        {
            if (i.gameObject.layer == 12) //wall
            {
                GameObject targetedTile = i.gameObject;
                Destroy(targetedTile);
            }

        }
        Instantiate(prefab, miner.transform.position + offset, Quaternion.identity);

    }
}
