using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDefinition : MonoBehaviour
{
    [Header("Definiowane w panelu")]
    public int gunID;
    public Transform firePoint;
    public GameObject shotFX;
    public GameObject getShotedFX;

    public int damage;
    public float delayBetweenShots;
    public float range;

    [Header("Definiowane dynamicznie")]
    [SerializeField]
    private bool isShooting = false;

    private Slot[] _ammoInventorySlots;
    private Inventory _inventory;
    private Camera playerCamera;

    private void Awake()
    {
        playerCamera = GetComponentInParent<Camera>();
        _inventory = GetComponentInParent<Inventory>();
        _ammoInventorySlots = _inventory.GetAmmoSlots();
    }
    private void OnEnable()
    {
        isShooting = false;
        //aktualizacja UI z ammo przy wybraniu pistoletu
        _inventory.updateUI(ref _ammoInventorySlots, gunID);
    }
    private void Update()
    {

        if (Input.GetButtonDown("Shoot") && !isShooting && Time.timeScale != 0)
        {
            if (_ammoInventorySlots[gunID].amoutOfItems > 0)
            {
                StartCoroutine(ShootOrder());
            }
            else
            {
                //UI komunikat o braku ammo
                Debug.Log("Brak amunicji " + _ammoInventorySlots[gunID].item.name + " [" + gunID + "]");
            }
        }
    }
    IEnumerator ShootOrder()
    {
        isShooting = true;
        yield return StartCoroutine(Shoot());
        isShooting = false;
    }
    IEnumerator Shoot()
    {
        //audio start
        shotFX.GetComponent<ParticleSystem>().Play();


        RaycastHit hitInfo;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, range))
        {
            Debug.Log(hitInfo.transform.name);
            EnemyDef enemyDef = hitInfo.transform.GetComponent<EnemyDef>();
            if (enemyDef != null)
            {
                enemyDef.TakeDmg(damage);
            }
        }

        _ammoInventorySlots[gunID].amoutOfItems--;
        //aktualizacja UI z ammo
        _inventory.updateUI(ref _ammoInventorySlots, gunID);

        GameObject getshottedGO = Instantiate(getShotedFX, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        Destroy(getshottedGO, 2f);

        yield return new WaitForSeconds(delayBetweenShots);
    }  

    void OnDisable()
    {
        isShooting = false;
    }
}
