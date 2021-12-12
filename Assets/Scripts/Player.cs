using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Player : Agent
{

    [SerializeField] GameControler gameControler;
    [SerializeField] float moveSpeed = 0.02f;
    [SerializeField] Transform npcTransform;

    [SerializeField] Collider2D feet;
 
    public float decisionTime = 500f;
    internal float decisionTimeCount = 0;
    int currentMoveDirection = 0; //0: Drt -  1: Esq

#region  beheviour
    void Update(){
        switch (gameControler.GetCurrentVersion()){
            case IAVersion.Only_shoting_with_movement:
                LinealMovment();
                break;
            case IAVersion.Shoting_and_npc_movement:
            case IAVersion.Shoting_platforms_and_npc_movement:
                Follow();
            break;
        }

        if(Input.GetKeyDown(KeyCode.J)){
            GetComponent<Rigidbody2D>().velocity = new Vector2(0,9f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.GetComponent<Bullet>()){
            Die();
        } else if (other.gameObject.GetComponent<NPCAgent>()){
            AddReward(1f);
            EndEpisode();
        }
    }

    private void Die(){
        AddReward(-1f);
        EndEpisode();
        gameControler._SetReward(1f);
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
        Debug.DrawRay(transform.position, Vector2.up*7.5f, Color.blue, 0.1f);
        Debug.DrawRay(transform.position, Vector2.right*0.5f, Color.blue, 0.1f);
        Debug.DrawRay(transform.position, Vector2.left*0.5f, Color.blue, 0.1f);

        LayerMask mask = LayerMask.GetMask("NPC");
        LayerMask maskGrounds = LayerMask.GetMask("GroundPlatforms");

        if (IsGrounded() && Physics2D.Raycast(transform.position, Vector2.up, 7.5f, mask))
        {
            if (GetComponent<Rigidbody2D>().velocity.y == 0){
                GetComponent<Rigidbody2D>().velocity = new Vector2(0,8f);
            }
        } else if (IsGrounded() && Physics2D.Raycast(transform.position, Vector2.right, 0.5f, maskGrounds) ) {
            if (GetComponent<Rigidbody2D>().velocity.y == 0){
                GetComponent<Rigidbody2D>().velocity = new Vector2(0,8f);
            }
        } else if (IsGrounded() &&  Physics2D.Raycast(transform.position, Vector2.left, 0.5f, maskGrounds)) {
            if (GetComponent<Rigidbody2D>().velocity.y == 0){
                GetComponent<Rigidbody2D>().velocity = new Vector2(0,8f);
            }
        }
        else {
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
#endregion

#region ML

    public override void OnEpisodeBegin()
    {
        //gameControler._StartEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //dist
        float dist = Vector3.Distance(this.transform.localPosition, npcTransform.localPosition);
        sensor.AddObservation(dist);

        if (IsGrounded()){
            sensor.AddObservation(1);
        } else {
            sensor.AddObservation(0);
        }

        if (GetComponent<Rigidbody2D>().velocity.y == 0){
            sensor.AddObservation(1);
        } else {
            sensor.AddObservation(0);
        }

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.0001f);

        if (actions.DiscreteActions[0] == 1) {
            if (GetComponent<Rigidbody2D>().velocity.y == 0){
                GetComponent<Rigidbody2D>().velocity = new Vector2(0,9f);
            }
        }

        this.transform.localPosition += new Vector3(actions.DiscreteActions[0],0) * Time.deltaTime * moveSpeed;

        Debug.DrawRay(transform.position, Vector2.up*7.5f, Color.blue, 0.1f);
        LayerMask mask = LayerMask.GetMask("NPC");
        if (Physics2D.Raycast(transform.position, Vector2.up, 7.5f, mask))
        {
            AddReward(0.001f);
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        //discreteActions[0] = Input.GetKey(KeyCode.Z) ? 1 : 0;
        ActionSegment<float> continiousActions = actionsOut.ContinuousActions;
        continiousActions[0] = Input.GetAxisRaw("Horizontal");
        
    }
#endregion

}
