using UnityEngine;
using System.Collections;
public class DestroyOnCollision : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D hitObj)
    {
        Debug.Log("DestroyOnCollision. Bala col·lisionada amb objecte "+hitObj.tag);
        DestroyObject(gameObject);
    }
}