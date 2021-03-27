using UnityEngine;

public class Item : MonoBehaviour
{
    /*Pomyśleć nad zachowaniem pocisków. Czy będą obiektami podczas wystrzału?*/
    [SerializeField] private ItemType _itemType;//typ definiujący przedmiot. Możemy też to zrobić klasami dla danych przedmiotów
    public int itemCount;
    public ItemType itemType
    {
        get
        {
            return _itemType;
        }
    }

    [SerializeField] private ItemUsage _itemUsage;//typ użytkowy przedmiotu
    public ItemUsage itemUsage
    {
        get
        {
            return _itemUsage;
        }
    }
}
