using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
	[Header("Pickup Spawn Properites")]
	public List<Vector3> m_pickupSpawnPoints;
	public int m_numberOfPickupsToSpawn;
	[Tooltip("Only put all of one type of pickup in here.")]
	public List<Pickup_Base> m_objectsToSpawn;

	private void Start()
	{
		if (m_numberOfPickupsToSpawn > m_objectsToSpawn.Count)
		{
			m_numberOfPickupsToSpawn = m_objectsToSpawn.Count;
		}

		SpawnAtRandomLocations();
	}

	public void SpawnAtRandomLocations()
	{
		for (int i = 0; i < m_numberOfPickupsToSpawn; i++)
		{
			int chosenNumber = Random.Range(0, m_pickupSpawnPoints.Count);
			SpawnPickup(m_pickupSpawnPoints[chosenNumber]);
			m_pickupSpawnPoints.RemoveAt(chosenNumber);
		}
	}

	private void SpawnPickup(Vector3 p_spawnPosition)
	{
		if (m_objectsToSpawn.Count <= 0)
		{
			Debug.LogWarning("more spawn points then there are spawn objects");
			return;
		}

		int chosenNumber = Random.Range(0, m_objectsToSpawn.Count);
		GameObject newPickup = ObjectPooler.instance.NewObject(m_objectsToSpawn[chosenNumber].gameObject, transform);
		newPickup.transform.position = p_spawnPosition;
		m_objectsToSpawn.RemoveAt(chosenNumber);
	}

	private void OnDrawGizmos()
	{
		foreach (Vector3 vector in m_pickupSpawnPoints)
		{
			DebugExtension.DrawBounds(new Bounds(vector, Vector3.one), Color.red);
		}
	}
}
