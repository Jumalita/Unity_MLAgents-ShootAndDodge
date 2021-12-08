using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class NPCAgent : Agent
{

    [SerializeField] GameControler gameControler;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform cannonTransform;
    //[SerializeField] Transform pointerTransform;
    [SerializeField] float cooldownTime = 3f;
    float minTimeNextShoot;

#region beheviour
/*
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector2(cannonTransform.position.x,cannonTransform.position.y),
            new Vector2(pointerTransform.position.x,pointerTransform.position.y) *1000f);
    }
    */

    void Update()
    {
        if (player != null){
            FaceDirection();
        }

/* TESTING 
        if (Input.GetKeyUp(KeyCode.S))
        {
            Shoot();
        }
//*/
    }

    void FaceDirection(){
        if (this.transform.position.x < player.transform.position.x){
            this.transform.localScale = new Vector3(-1,1,1);
        }
        else{
            this.transform.localScale = new Vector3(1,1,1);
        }
    }

    public void Shoot(){
        if (CanShoot()){
            minTimeNextShoot = Time.time + cooldownTime;

            GameObject bullet = Instantiate(bulletPrefab,cannonTransform.position,Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(
                this.transform.position.x < player.transform.position.x ? 1 : -1 ,
                -1);
        }       
    }

    private bool CanShoot(){
        return Time.time > minTimeNextShoot;
    }

#endregion

#region ML

    public override void OnEpisodeBegin()
    {
        gameControler._StartEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //my position
        sensor.AddObservation(transform.position);
        /*if player is in range 
        //Ray cast
        RaycastHit2D hit = Physics2D.Raycast( cannonTransform.position, pointerTransform.position);

        if (hit.collider != null && hit.transform.GetComponent<Player>()){
            sensor.AddObservation(1);
        } else {
            sensor.AddObservation(0);
        }
        */
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log(actions.DiscreteActions[0]);
        if (actions.DiscreteActions[0] == 1) {
            Shoot();
        }
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        if(gameControler.GetCurrentVersion() != IAVersion.Only_shoting_no_movement){
            if(CanShoot()){
                actionMask.SetActionEnabled(0, 1, true);
            } 
            else {
                actionMask.SetActionEnabled(0, 1, false);
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
    }

#endregion

}
