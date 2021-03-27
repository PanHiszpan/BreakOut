using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Definiowane w panelu")]
    public float speed;
    public int damage;
    public float timeToDestroy;


    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        //------ Niszczenie pocisku po x czasu -------
        Destroy(gameObject, timeToDestroy);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("Trafiony " + collision.name); //pobiera name ze sceny, ustaw na nameEnemy w enemy stats

            float[] temp = new float[3];
            temp[0] = damage;//obrażenia
            temp[1] = 1;//Czy ma być odrzut po otrzymaniu obrażeń 1-tak, 0-nie
            //temp[2] = GameObject.Find("Player").transform.position.x;//Pozycja gracza ( potrzebne do odrzutu)
            if (!collision.gameObject.tag.Equals("PoisonousBarrel")) collision.SendMessage("TakeDamage", temp);
            else
            { 
                float leftOrRight;
                if(transform.position.x > collision.transform.position.x) leftOrRight = 1f;
                else leftOrRight = 0f;


                float[] args = {1f, transform.position.x, transform.position.y, leftOrRight};
                collision.SendMessage("TakeDamage", args);
            }
        }

        Destroy(gameObject);
    }
}
