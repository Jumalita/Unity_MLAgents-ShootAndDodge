using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IAVersion {
    Only_shoting_no_movement,
    Only_shoting_with_movement,
    Shoting_and_npc_movement,
    Shoting_platforms_and_npc_movement,
    Shoting_and_movemnt_player_no_IA,
    Everything,
}

public class GameControler : MonoBehaviour
{    

    [SerializeField] public IAVersion currentVersion;
    [SerializeField] Player player;
    [SerializeField] NPCAgent npc;
    [SerializeField] EnviromentMaker enviromentMaker;

    float nextEpisode = 10f;
    float timeTillNextEpisode ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IAVersion GetCurrentVersion(){
        return currentVersion;
    }

    private void Update() {
        /*TESTING
        if (Input.GetKeyUp(KeyCode.X))
        {
            RemakeEnviroment();
        }
        //*/
        RoundManagement();
    }

    private void ResetTimeTillNextEpisode(){
        timeTillNextEpisode = Time.time + nextEpisode;
    }
    
    private void RoundManagement(){
        switch (currentVersion) {

            case IAVersion.Only_shoting_no_movement:
            case IAVersion.Only_shoting_with_movement:
                if (Time.time > timeTillNextEpisode ){
                    _EndEpisode();
                }
                break;

            default:
            break;
            
        }
        
    }

    public void _SetReward(float reward){
        npc.SetReward(reward);
    }

    public void _EndEpisode(){
        npc.EndEpisode();

    }

    public void _StartEpisode(){
        ResetTimeTillNextEpisode();
        enviromentMaker.RemakeEnviroment(currentVersion);
    }



}