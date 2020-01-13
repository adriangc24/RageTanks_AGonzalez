using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitByBoss : MonoBehaviour
{
    //Definicio de delegat i event per tractar impacte de bala
    public delegate void hitByPlayerBullet();
    public event hitByPlayerBullet hitByBullet;
    //Tractament de col.lisio amb DefenseCollider: si
    //es amb una bala, generar event.
    void OnTriggerEnter2D(Collider2D collidedObject)
    {
        if (collidedObject.tag == "Boss")
        {
            Debug.Log("HitByBoss");
           
                /*ParticleSystem deathFxParticle =
                (ParticleSystem)Instantiate(deathFxParticlePrefab);*/
                Vector3 enemyPos = transform.position;
                Vector3 particlePosition =
                new Vector3(enemyPos.x, enemyPos.y, enemyPos.z + 1.0f);
               // deathFxParticle.transform.position = particlePosition;
               
                Destroy(gameObject, 0.1f);
            
        }
    }

}
