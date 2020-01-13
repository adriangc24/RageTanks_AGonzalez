using UnityEngine;
using System.Collections;
public class DeathTriggerScript : MonoBehaviour
{
   
    void OnTriggerEnter2D(Collider2D collidedObject)
    {
        Debug.Log("HiDeathTrigger");
        collidedObject.SendMessage("hitDeathTrigger",SendMessageOptions.DontRequireReceiver);
    }
}