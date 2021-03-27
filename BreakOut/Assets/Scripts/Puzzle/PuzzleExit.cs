using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleExit : MonoBehaviour
{
    private SceneMenager sceneMenager;
    private void Start()
    {
        sceneMenager = FindObjectOfType<SceneMenager>();
    }
    private void OnTriggerStay(Collider coll)
    {
        BallControler ball = coll.GetComponent<BallControler>();
        if (ball != null)
        {
            sceneMenager.puzzleWon = true;
        }
    }
}
