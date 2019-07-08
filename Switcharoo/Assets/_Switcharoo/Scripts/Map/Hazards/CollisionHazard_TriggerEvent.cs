
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_TriggerEvent : MonoBehaviour
{
    PlayerController m_player;

    public OnActivationEvent m_active;
    bool activated;
    private void Start()
    {
        m_player = DungeonManager.instance.m_playerGameObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!activated)
            {
                activated = true;
                m_active.Invoke();
            }
        }

    }

    public void DisablePlayerControl()

    {
        m_player.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;
        m_player.GetComponent<PlayerInput>().enabled = false;
        m_player.m_velocity = new Vector3(0f, m_player.m_velocity.y, 0f);
        m_player.m_usingMovementAbility = false;
    }
}
