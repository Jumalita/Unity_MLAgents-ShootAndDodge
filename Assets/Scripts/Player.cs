using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] GameControler gameControler;

    [SerializeField] float moveSpeed = 0.02f;
 
    public float decisionTime = 500f;
    internal float decisionTimeCount = 0;
    int currentMoveDirection = 0; //0: Drt -  1: Esq


    void Update(){
        switch (gameControler.GetCurrentVersion()){
            case IAVersion.Only_shoting_with_movement:
                LinealMovment(gameControler.GetCurrentVersion());
            break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.GetComponent<Bullet>()){
            Die();
        }
    }

    private void Die(){
        gameControler._SetReward(1f);
        gameControler._EndEpisode();
    }

    private void LinealMovment(IAVersion currentVersion){
        this.transform.position += 
        (currentMoveDirection == 0 ? Vector3.right : Vector3.left) * Time.deltaTime * moveSpeed;

        if (decisionTimeCount > 0) decisionTimeCount -= Time.deltaTime;
        else
        {
            decisionTimeCount = decisionTime;
            currentMoveDirection = currentMoveDirection == 0 ? 1 : 0;
        }
    }


}
