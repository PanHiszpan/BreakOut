using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    private SceneMenager sceneMenager;
    private void Start()
    {
        sceneMenager = FindObjectOfType<SceneMenager>();
    }
    private void OnTriggerEnter(Collider coll)
    {
        PlayerController player = coll.GetComponent<PlayerController>();
        if (player != null)
        {
            sceneMenager.atExit = true;
        }
    }
}
