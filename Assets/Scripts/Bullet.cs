using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
     private GameControler gameControler;

    private void Start() {
        gameControler = FindObjectOfType<GameControler>();
    }

   private void OnCollisionEnter2D(Collision2D other) {
       if (!other.gameObject.GetComponent<NPCAgent>()){
           Destroy(gameObject);
       }
   }
}
