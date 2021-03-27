using System;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Definiowane w panelu")]

    /// <zrób UI>
    ///     public Text ammoTypeText;
            public Text ammoCountText;
    ///     public Text itemTypeText;
    ///     public Text itemCountText;
    ///     public Text cashCountText;
    /// </summary>

    [SerializeField] private Slot[] _ammoInventorySlots;
    [SerializeField] private Slot[] _weaponsInventorySlots;
    [SerializeField] private Slot[] _collectableInventorySlots; //tu na razie są zebrane pieniądze
    [SerializeField] private Slot[] _usableInventorySlots; //wszystkie przedmioty które gracz może użyć (na razie apteczki i sterydy)

    private int currentItem;
    private GunDefinition _gun;

    public bool noItem = false;
    public bool noAmmo = false;

    CharacterController controller = null;


    private void Awake()
    {
        
        if (_usableInventorySlots == null)
        {
            Debug.LogWarning("Inventory slots aren't connected with invetnory script in " + this.gameObject.name + " object");
        }

        controller = GetComponent<CharacterController>();
        _gun = GetComponentInChildren<GunDefinition>();
        ChangeItem(0, ref _usableInventorySlots, false);  //szuka itemu o ilosci != 0 w usableItemsSlots[] zaczynajac od [0] i idzie rosnaco [0],[1],[2]...
        //updateUI(ref _collectableInventorySlots, 0); //update UI z kasa
    }


    private void Update()
    {
        if (Input.GetButtonDown("NextItem"))
        {
            ChangeItem(currentItem + 1, ref _usableInventorySlots, false);
        }
        if (Input.GetButtonDown("PrevItem"))
        {
            ChangeItem(currentItem - 1, ref _usableInventorySlots, true);
        }
    }
    private bool EquipItem(Item item, ref Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item.itemType == item.itemType)
            {
                if (slots[i].amoutOfItems + item.itemCount <= slots[i].maximumAmountOfItemsInSlot)
                {
                    slots[i].amoutOfItems = slots[i].amoutOfItems + item.itemCount;
                    updateUI(ref slots, i);
                    return true;
                }
                else
                {
                    slots[i].amoutOfItems = slots[i].maximumAmountOfItemsInSlot;
                    updateUI(ref slots, i);
                    return true;
                }
            }
        }
        return false;
    }
    private void UseItem()
    {
        Debug.Log("Używam " + _usableInventorySlots[currentItem].item.name + " [" + currentItem + "]");
        TakeItem(_usableInventorySlots[currentItem].item, 1, ref _usableInventorySlots);  //(co?, ile?, skąd?)

    }
    public int TakeItem(Item item, int amount, ref Slot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item.itemType == item.itemType)
            {
                if (amount <= slots[i].amoutOfItems)
                {
                    slots[i].amoutOfItems = slots[i].amoutOfItems - amount;
                    updateUI(ref slots, i);
                    return amount;
                }
                else
                {
                    Debug.Log("Nie ma tyle [" + amount + "] w ekwipunku: " + slots[i].item.name);
                    return amount;
                }
            }
        }
        Debug.Log("Nie znalazłem w ekwipunku przedmiotu: " + item.name);
        return 0;
    }
    
    private void UpradgeWeapon()
    {
        //ulepszanie broni
    }

    private void EquipGranade()
    {

    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        Item item = collision.gameObject.GetComponent<Item>();
        
        if (item != null)
        {
            switch (item.itemUsage)
            {
                case ItemUsage.aid:
                    PutToInventory(collision, item, ref _usableInventorySlots);
                    if (noItem) { ChangeItem(0, ref _usableInventorySlots, false); }
                    break;
                case ItemUsage.ammo:
                    PutToInventory(collision, item, ref _ammoInventorySlots);
                    break;
                case ItemUsage.weapon:
                    break;
                case ItemUsage.cash:
                    break;

            }
        }
    }

    private void PutToInventory(ControllerColliderHit collision, Item item, ref Slot[] slots)
    {
        if (EquipItem(item, ref slots))
        {
            Destroy(collision.gameObject);
        }
    }

    public void updateUI(ref Slot[] slots, int nr)
    {
        switch (slots[nr].item.itemUsage)
        {
            case ItemUsage.aid:
                if (currentItem == nr)
                {
                    //itemTypeText.text = "" + _usableInventorySlots[nr].item.name;
                    //itemCountText.text = "" + _usableInventorySlots[nr].amoutOfItems + "/" + _usableInventorySlots[nr].maximumAmountOfItemsInSlot;
                }
                break;
            case ItemUsage.ammo:
                ammoCountText.text = "" + _ammoInventorySlots[nr].amoutOfItems + "/" + _ammoInventorySlots[nr].maximumAmountOfItemsInSlot;
                break;
            case ItemUsage.weapon:
                //update nazwy broni przy zmianie
                break;
            case ItemUsage.cash:
                //cashCountText.text = "" + _collectableInventorySlots[nr].amoutOfItems;
                break;
            default:
                break;
        }
    }

    private void ChangeItem(int nr, ref Slot[] slots, bool prev)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            //-----zapetlanie----
            if (nr > slots.Length - 1) { nr = 0; }
            if (nr < 0) { nr = slots.Length - 1; }
            //-----pomijanie pustych slotow-----
            if (slots[nr].amoutOfItems > 0) 
            {
                currentItem = nr;
                Debug.Log("zmieniam item na " + slots[nr].item.name + " [" + nr + "]");
                updateUI(ref slots, nr);
                return; 
            }
            else 
            {
                if (prev) { nr--; }
                else { nr++; }
            }
        }
        noItem = true; //jesli tu dotarlo znaczy ze gracz nie ma zadnego itemu
    }


    public Slot[] GetAmmoSlots()
    {
        return _ammoInventorySlots;
    }
    public Slot[] GetUsableSlots()
    {
        return _usableInventorySlots;
    }
    public int GetCurrentItem()
    {
        return currentItem;
    }
    public int GetCash(int amount)
    {
        return TakeItem(_collectableInventorySlots[0].item, amount, ref _collectableInventorySlots);
    }
    
    // ----- pobieranie slotow dla PlayerRemains.cs -----
    public Slot GetCashSlot()
    {
        return _collectableInventorySlots[0];
    }

}
