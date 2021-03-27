using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMenager : MonoBehaviour
{
    [Header("Definiowane w panelu")]
    public GameObject[] weaponUI;
    [Header("Definiowane dynamicznie")]
    public int currentWeapon = 0;
    public List<GameObject> weaponList;

    void Start()
    {
        foreach (Transform child in transform)
        {
            weaponList.Add(child.gameObject);
        }

        for (int i = 1; i < weaponList.Count; i++) { weaponList[i].SetActive(false); } //wylacza reszte broni
        ChangeWeapon(0, true);
    }
    void Update()
    {

        if (Input.GetButtonDown("NextWeapon"))
        {
            ChangeWeapon(currentWeapon + 1, false);
        }
        if (Input.GetButtonDown("PrevWeapon"))
        {
            ChangeWeapon(currentWeapon - 1, true);
        }
    }

    private void ChangeWeapon(int nr, bool prev)
    {
        //-----zapetlanie----
        if (nr > weaponList.Count - 1) { nr = 0; }
        if (nr < 0) { nr = weaponList.Count - 1; }
        //-----pomijanie pustych slotow (do zrobienia)-----
        
        Debug.Log("zmieniam bron na " + weaponList[nr].name + " [" + nr + "]");
        weaponList[currentWeapon].SetActive(false);
        weaponList[nr].SetActive(true);
        currentWeapon = nr;
        //updateUI(ref slots, nr);
        return;


    }

}
