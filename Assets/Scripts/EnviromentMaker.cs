using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnviromentMaker : MonoBehaviour
{

    [SerializeField] float x_max_len;
    [SerializeField] float y_max_len;
    [SerializeField] GameObject npc;
    [SerializeField] GameObject player;
    [SerializeField] Tilemap platforms_tilemap;
    [SerializeField] Tile platform_tile;
    [SerializeField] Tile ground_tile;


    public void RemakeEnviroment(IAVersion currentVersion){
        platforms_tilemap.ClearAllTiles();
        switch (currentVersion){
            case IAVersion.Only_shoting_no_movement:
            case IAVersion.Only_shoting_with_movement:
                PlaceInitialTesting();
            break;
            case IAVersion.Shoting_and_npc_movement:
                PlaceNPC();
                PlacePlayer();
            break;
            case IAVersion.Shoting_platforms_and_npc_movement:
            case IAVersion.Shoting_and_movemnt_player_no_IA:
            case IAVersion.Everything:
                PlaceNPC();
                PlacePlayer();
                PlacePlatforms(); //first steps of testing without this
            break;
        }
    }

    void PlaceNPC(){
        npc.transform.position = new Vector3(Random.Range(-x_max_len,x_max_len),
                                        Random.Range(-1f,y_max_len),-0.5f);
    }

    void PlaceInitialTesting(){
        npc.transform.position = new Vector3(-0.4f,0.3f,-0.5f);
        if (Random.value < 0.5f){
            player.transform.position = new Vector3(-4f,-3f,-0.5f);
        } else {
            player.transform.position = new Vector3(3.2f,-3f,-0.5f);
        }

        if (Random.value < 0.5f){
            platforms_tilemap.SetTile(new Vector3Int(1,-2,1),platform_tile);
        } else {
            platforms_tilemap.SetTile(new Vector3Int(-3,-2,1),platform_tile);
        }
    }

    void PlacePlayer(){
        float x_min = Random.Range(-x_max_len, npc.transform.position.x);
        float x_max = Random.Range(npc.transform.position.x, x_max_len);
        if (Random.value < 0.5f){
            player.transform.position = new Vector3(x_min,-1.9f,-0.5f);
        } else {
            player.transform.position = new Vector3(x_max,-1.9f,-0.5f);
        }
    }

    void PlacePlatforms(){
        MakeHills();
        MakePlatforms();
    }

    void MakeHills(){
        int offset = 10;
        // ground hills on tile -4 
        float n_hills = Random.value;

        if (n_hills < 0.33){ //two hills
            int start_hill_1 = Random.Range(1,16);
            int start_hill_2 = Random.Range(start_hill_1 + 1, 16);
            int len_hill_1 = Random.Range(1,start_hill_1 - start_hill_2);
            int len_hill_2 = Random.Range(1, 18 - start_hill_2);

            start_hill_1 = start_hill_1 - offset;
            start_hill_2 = start_hill_2 - offset;

            for(int i = -9; i < 8; i++){
                if (i >= start_hill_1 && i <= start_hill_1+len_hill_1 
                ||  i >= start_hill_2 && i <= start_hill_2+len_hill_2 ){
                    platforms_tilemap.SetTile(new Vector3Int(i,-4,1),ground_tile);
                } 
            }
        } else if (n_hills < 0.66) { //one hill
            int start_hill_1 = Random.Range(1,16);
            int len_hill_1 = Random.Range(1, 16 - start_hill_1);

            start_hill_1 = start_hill_1 - offset;

            for(int i = -9; i < 8; i++){
                if (i >= start_hill_1 && i <= start_hill_1+len_hill_1){
                    platforms_tilemap.SetTile(new Vector3Int(i,-4,1),ground_tile);
                } 
            }
        } // else no hills
            
    }

    void MakePlatforms(){
        int offset = 10;
        // make platforms
        // to -2 to 2
        int number_of_platforms = 0; //maximum 3 platforms
        for (int i = -2; i <= 2; i++){
            if (Random.value < 0.5 && number_of_platforms < 3){ // we create a platform on the row
                int start_platform = Random.Range(1,16);
                int len_platform = Random.Range(0, 4);

                start_platform = start_platform - offset;

                for (int j = start_platform; j < len_platform + start_platform ; j++){
                    platforms_tilemap.SetTile(new Vector3Int(j,i,1),platform_tile);
                }
                number_of_platforms++;
            }
        }
    }



}
