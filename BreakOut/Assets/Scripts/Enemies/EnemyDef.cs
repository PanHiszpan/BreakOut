using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDef : MonoBehaviour
{

    [SerializeField] private int hp;

    public void TakeDmg(int damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
