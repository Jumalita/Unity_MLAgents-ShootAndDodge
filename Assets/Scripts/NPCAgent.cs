using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class NPCAgent : Agent
{
    [SerializeField] GameObject body;
    [SerializeField] GameControler gameControler;
    [SerializeField] GameObject player;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform cannonTransform;
    [SerializeField] float cooldownTime = 0.5f;
    [SerializeField] float moveSpeed = 1.5f;
    float minTimeNextShoot;

#region beheviour

    void Update()
    {
        if (player != null){
            FaceDirection();
        }

        if (Input.GetKeyDown(KeyCode.K)){
            Shoot();
        }
        
    }

    void FaceDirection(){
        if (this.transform.localPosition.x < player.transform.localPosition.x){
            body.transform.localScale = new Vector3(-1,1,1);
        }
        else{
            body.transform.localScale = new Vector3(1,1,1);
        }
    }
    

    public void Shoot(){
        if (CanShoot()){
            minTimeNextShoot = Time.time + cooldownTime;
            GameObject bullet = Instantiate(bulletPrefab,cannonTransform.position,Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(
                this.transform.localPosition.x < player.transform.localPosition.x ? 1 : -1 ,
                -1 );
        }       
    }

    private bool CanShoot(){
        return Time.time > minTimeNextShoot;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "GroundNdPlatforms"){
            //gameControler._SetReward(-0.01f);
        } else if (other.gameObject.tag == "Player"){
            gameControler._SetReward(-1f);
            gameControler._EndEpisode();
        }
    }

#endregion

#region ML

    public override void OnEpisodeBegin()
    {
        gameControler._StartEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //dist
        float dist = Vector3.Distance(this.transform.localPosition,player.transform.localPosition);
        sensor.AddObservation(dist);

        var directionBullets =  body.transform.localScale.x == -1 ? new Vector2(1,-1): new Vector2(-1,-1);
        LayerMask mask = LayerMask.GetMask("Player");

        if (Physics2D.Raycast(cannonTransform.position, directionBullets, 6f, mask))
        {
            sensor.AddObservation(1);
        } else {
            sensor.AddObservation(0);
        }

        if (Physics2D.Raycast(transform.position, Vector2.down, 7.5f, mask))
        {
            sensor.AddObservation(1);
        } else {
            sensor.AddObservation(0);
        }

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.0001f);

        if (actions.DiscreteActions[0] == 1) {
            AddReward(-0.01f);
            Shoot();
        }

        if(gameControler.GetCurrentVersion() != IAVersion.Only_shoting_no_movement &&
         gameControler.GetCurrentVersion() != IAVersion.Only_shoting_with_movement){
            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];
            this.transform.localPosition += new Vector3(moveX,moveY) * Time.deltaTime * moveSpeed;
        }

        var direction_bullets =  body.transform.localScale.x == -1 ? new Vector2(1,-1): new Vector2(-1,-1);
        Debug.DrawRay(cannonTransform.position, direction_bullets* 6f, Color.green, 0.01f);

        LayerMask mask = LayerMask.GetMask("Player");
        if (Physics2D.Raycast(cannonTransform.position, direction_bullets, 6f, mask))
        {
            AddReward(0.0001f);
        }

        Debug.DrawRay(transform.position, Vector2.down* 7.5f, Color.green, 0.01f);

        if (Physics2D.Raycast(transform.position, Vector2.down, 7.5f, mask))
        {
            AddReward(-0.001f);
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
        //ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        //discreteActions[0] = Input.GetKey(KeyCode.Z) ? 1 : 0;
        ActionSegment<float> continiousActions = actionsOut.ContinuousActions;
        continiousActions[0] = Input.GetAxisRaw("Horizontal");
        continiousActions[1] = Input.GetAxisRaw("Vertical");
    }

#endregion

}
