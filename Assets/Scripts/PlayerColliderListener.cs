using UnityEngine;
using System.Collections;
public class PlayerColliderListener : MonoBehaviour { 
    public PlayerStateListener targetStateListener = null; 
    void OnTriggerEnter2D(Collider2D collidedObject) 
    { 
        switch (collidedObject.tag) { 
            case "Platform": // Quan el Player cau en una plataforma, canviar l'estat. 
                targetStateListener.onStateChange( PlayerStateController.playerStates.landing); 
            break;
            case "DeathTrigger":
                // El Player ha caigut sobre el DeathTrigger. El matem.
                Debug.Log("Col.lisio amb DeathTrigger. Tag: " + collidedObject.tag);
                targetStateListener.onStateChange(PlayerStateController.playerStates.kill);
                break;
        } 
    } 
}