using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMenager : MonoBehaviour
{
    public GameObject ammoUI;
    public bool puzzleWon;
    public bool atExit;


    private void Start()
    {
        FindObjectOfType<PlayerController>().transform.position = new Vector3(0, 1.2f, 0);

    }
    private void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            puzzleWon = false;
            atExit = false;
            ResetLvl();
        }
        if (atExit && puzzleWon)
        {
            puzzleWon = false;
            atExit = false;
            ResetLvl();
        }
    }

    public void ResetLvl()
    {
        StartCoroutine(LoadLevelAsync(SceneManager.GetActiveScene().buildIndex));
    }
    IEnumerator LoadLevelAsync(int nr)
    {
        AsyncOperation loadLevelAsync = SceneManager.LoadSceneAsync(nr);

        //wait until loaded
        while (!loadLevelAsync.isDone)
        {
            yield return null;
        }
    }
}
