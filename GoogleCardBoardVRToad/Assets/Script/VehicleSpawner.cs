using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour {

	public GameObject prefab;

	public float minTime = 2;
	public float meanTime = 4;
	float nextSpawnTime = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextSpawnTime)
		{
			Spawn();
			nextSpawnTime = Time.time + minTime -Mathf.Log(Random.value) * meanTime;
		}
	}
		
	void Spawn()
	{
		var instance = Instantiate (prefab, transform.position, transform.rotation, transform);
	}
}
