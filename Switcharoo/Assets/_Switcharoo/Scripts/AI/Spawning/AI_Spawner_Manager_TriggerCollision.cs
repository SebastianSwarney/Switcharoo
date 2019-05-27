using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
    /*  This spawn manager will only start spawning enemies
        after the player has collided with a trigger that is attached to this object
    
     */
///<Summary>
public class AI_Spawner_Manager_TriggerCollision : AI_Spawner_Manager_Base
{
    public string playerTag = "Player";
    Collider2D[] cols;
    void Start()
    {
        cols = GetComponents<Collider2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == playerTag)
        {
            StartAllSpawners();
            foreach(Collider2D col in cols){
                col.enabled = false;
            }
        }
    }

    public override void DeintializeAllSpawners()
    {
    }
}
