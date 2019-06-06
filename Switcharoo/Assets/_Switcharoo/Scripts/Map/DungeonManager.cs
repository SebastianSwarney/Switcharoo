using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance { get; private set; }

    public GameObject m_playerGameObject;
    Vector3 m_playerRespawnPoint;
    Health m_playerHealth;

    [HideInInspector]
    public CameraController_Base m_cameraController;
    PlayerController m_playerCont;
    public GameObject m_currentLoadedTilemap;



    [Header("Room Transition")]
    public float m_roomTransitionTime;
    Coroutine m_roomTransitionCoroutine;
    public RoomManager_Base m_currentRoom;



    void Awake()
    {
        if(Camera.main == null)
        {
            Debug.Log("DUNGEON MANAGER ERROR! The Camera is either disabled or nonexistant");
        }
        m_cameraController = Camera.main.GetComponent<CameraController_Base>();

        if (m_cameraController.m_useLevelBounds)
        {
            m_cameraController.CalculateNewCameraBounds(m_currentLoadedTilemap.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>());
        }


        m_playerHealth = m_playerGameObject.GetComponent<Health>();
        m_playerCont = m_playerGameObject.GetComponent<PlayerController>();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        m_playerRespawnPoint = m_playerGameObject.transform.position;
    }

    private void Update()
    {
        CheckPlayerHealth();
    }
    void CheckPlayerHealth()
    {
        if (m_playerHealth.m_isDead)
        {
            Debug.Log("TODO: Place fancy died transition here");
            m_playerHealth.m_isDead = false;
            m_playerHealth.ResetHealth();
            m_currentRoom.gameObject.SetActive(false);
            m_currentRoom.ResetRoom();
            m_currentRoom.gameObject.SetActive(true);
            m_playerGameObject.transform.position = m_playerRespawnPoint;
            m_cameraController.transform.position = m_cameraController.m_cameraBoundsArea.ClosestPoint(m_playerGameObject.transform.position);

        }
    }

    public void LoadNewMap(RoomManager_Base p_loadMap, Vector3 p_playerSpawnPos)
    {
        m_playerCont.m_usingMovementAbility = false;

            
        m_currentRoom = p_loadMap;
        m_playerCont.m_velocity = Vector2.zero;
        p_loadMap.gameObject.SetActive(true);
        m_cameraController.enabled = false;
        m_playerCont.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;
        m_roomTransitionCoroutine = StartCoroutine(RoomTransition(m_cameraController.transform.position, p_playerSpawnPos, p_loadMap));
        m_playerRespawnPoint = p_playerSpawnPos;



    }

    IEnumerator RoomTransition(Vector3 p_camStartPos, Vector3 p_playerSpawnPos, RoomManager_Base p_loadMap)
    {
        m_playerGameObject.transform.position = p_playerSpawnPos;
        GameObject deloadTilemap = m_currentLoadedTilemap.transform.parent.parent.parent.gameObject;
        m_currentLoadedTilemap = p_loadMap.m_currentLoadedTilemap.transform.GetChild(0).gameObject;
        m_cameraController.CalculateNewCameraBounds(m_currentLoadedTilemap.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>());


        float currentLerpTime = 0;

        Vector3 lerpPoint = m_cameraController.m_cameraBoundsArea.ClosestPoint(p_playerSpawnPos);
        while (m_cameraController.transform.position.x != lerpPoint.x)
        {
            currentLerpTime += Time.deltaTime;

            m_cameraController.transform.position = new Vector3(Mathf.Lerp(p_camStartPos.x, lerpPoint.x, currentLerpTime / m_roomTransitionTime), Mathf.Lerp(p_camStartPos.y, lerpPoint.y, currentLerpTime / m_roomTransitionTime), m_cameraController.transform.position.z);
            yield return null;
        }

        m_playerCont.m_states.m_movementControllState = PlayerController.MovementControllState.MovementEnabled;
        deloadTilemap.SetActive(false);


        m_cameraController.enabled = true;
        m_playerCont.m_velocity = Vector2.zero;

    }
}
