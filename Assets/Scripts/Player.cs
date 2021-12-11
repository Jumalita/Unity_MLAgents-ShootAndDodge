using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] GameControler gameControler;
    [SerializeField] float moveSpeed = 0.02f;
    [SerializeField] Transform npcTransform;

    [SerializeField] Collider2D feet;
 
    public float decisionTime = 500f;
    internal float decisionTimeCount = 0;
    int currentMoveDirection = 0; //0: Drt -  1: Esq

    void Update(){
        switch (gameControler.GetCurrentVersion()){
            case IAVersion.Only_shoting_with_movement:
                LinealMovment();
                break;
            case IAVersion.Shoting_and_npc_movement:
                Follow();
            break;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.GetComponent<Bullet>()){
            Die();
        } else if (gameControler.GetCurrentVersion() == IAVersion.Shoting_and_npc_movement
                && other.gameObject.layer == 9){
            currentMoveDirection = currentMoveDirection == 0 ? 1 : 0;
        }
    }

    private void Die(){
        gameControler._SetReward(0.75f);
        gameControler._EndEpisode();
    }

    private void LinealMovment(){
        this.transform.localPosition += 
        (currentMoveDirection == 0 ? Vector3.right : Vector3.left) * Time.deltaTime * moveSpeed;

        if (decisionTimeCount > 0) decisionTimeCount -= Time.deltaTime;
        else
        {
            decisionTimeCount = decisionTime;
            currentMoveDirection = currentMoveDirection == 0 ? 1 : 0;
        }
    }

    //not good enough
    private void LinealMovmentTillCollision(){
        this.transform.localPosition += 
        (currentMoveDirection == 0 ? Vector3.right : Vector3.left) * Time.deltaTime * moveSpeed;
    }

    private void Follow(){
        Debug.DrawRay(transform.position, Vector2.up, Color.blue, 0.1f);

        LayerMask mask = LayerMask.GetMask("NPC");
        if (IsGrounded() && Physics2D.Raycast(transform.position, Vector2.up, 500f, mask))
        {
            if (GetComponent<Rigidbody2D>().velocity.y == 0){
                GetComponent<Rigidbody2D>().velocity = new Vector2(0,7f);
            }
        } else {
            this.transform.localPosition += 
            (npcTransform.position.x > transform.position.x ? Vector3.right : Vector3.left) 
            * Time.deltaTime * moveSpeed;
        }
    }

    private bool IsGrounded(){
        LayerMask mask = LayerMask.GetMask("GroundPlatforms");
        RaycastHit2D hit = Physics2D.Raycast(feet.bounds.center,Vector2.down,feet.bounds.extents.y + 1f,mask);
        return hit.collider != null;
    }

}
