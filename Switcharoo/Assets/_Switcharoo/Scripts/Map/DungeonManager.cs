using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance { get; private set; }
    public GameObject m_playerGameObject;
    [HideInInspector]
    public CameraController_Base m_cameraController;
    PlayerController m_playerCont;
    public GameObject m_currentLoadedTilemap;

    Coroutine m_roomTransitionCoroutine;
    WaitForSeconds m_delay;

    [Header("Room Transition")]
    public float m_roomTransitionTime;

    void Awake()
    {
        m_cameraController = Camera.main.GetComponent<CameraController_Base>();
        if (m_cameraController.m_useLevelBounds)
        {
            m_cameraController.CalculateNewCameraBounds(m_currentLoadedTilemap.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>());            
        }
        
        m_playerCont = m_playerGameObject.GetComponent<PlayerController>();
        m_delay = new WaitForSeconds(m_roomTransitionTime);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadNewMap(RoomManager_Base p_loadMap, Vector3 p_playerSpawnPos)
    {
        m_playerCont.m_velocity = Vector2.zero;
        p_loadMap.gameObject.SetActive(true);
        m_cameraController.enabled = false;
        m_playerCont.m_states.m_movementControllState = PlayerController.MovementControllState.MovementDisabled;
        m_roomTransitionCoroutine = StartCoroutine(RoomTransition(m_cameraController.transform.position, p_playerSpawnPos, p_loadMap));

    }

    IEnumerator RoomTransition(Vector3 p_camStartPos, Vector3 p_playerSpawnPos, RoomManager_Base p_loadMap)
    {
        m_playerGameObject.transform.position = p_playerSpawnPos;
        GameObject deloadTilemap = m_currentLoadedTilemap.transform.parent.parent.parent.gameObject;
        m_currentLoadedTilemap = p_loadMap.m_currentLoadedTilemap.transform.GetChild(0).gameObject;
        m_cameraController.CalculateNewCameraBounds(m_currentLoadedTilemap.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>());


        float currentLerpTime = 0;

        Vector3 lerpPoint = m_cameraController.m_cameraBoundsArea.ClosestPoint(p_playerSpawnPos);
        Debug.DrawLine(p_camStartPos, lerpPoint, Color.red, 10);
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
