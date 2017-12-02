#GoogleCardBoardVRToad

**Google Cardboard_SDK**

**C# in Unity**

![alt text](https://i.imgur.com/OWyaiKF.png)

**Game Design**

- Build a Crossy Road clone
- We will call our clone Squashy Toad
- Understand a basic VR camera
- Simple one-button VR movement
- Overview the Game Deign Document (GDD)

**Concept**

-  a toad trying to survive crossing an infinite number of lanes of hazards, including roads and rivers.

- You can't win, and you're scored on the number of lanes you cross before getting squashed. 

**Rules**

- You can look and hop in any direction
- If you sit still too long, you get burned!
- You die by falling in water, or getting squashed.
- Optional: ability to duck under by look down.

**Spawning Cars Randomly**

- Spawning cars with a time interval
- Spawning at a given location
- Randomizing the time interval

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour {

	public GameObject prefab;

	public float minTime = 2;
	public float meanTime = 10;
	float nextSpawnTime = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextSpawnTime) 
		{
			Spawn();
			nextSpawnTime = Time.time + -Mathf.Log(Random.value) * meanTime;
		}
	}

	void Spawn()
	{
		var instance = Instantiate(prefab, transform.position, transform.rotation, transform); 
	}
}
```
**Random Spawning Cars**

![alt text](https://i.imgur.com/pNK4rBo.png)

**Frog MovementScript**

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogMovement : MonoBehaviour {

	public float jumpElevationInDegrees = 45;
	public float jumpSpeedInCMPS = 5;
	public float jumpGroundClearance = 2;
	public float jumpSpeedTolerance = 5;

	public int collisionCount = 0;

	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter()
	{
		collisionCount++;
	}

	void OnCollisionExit()
	{
		collisionCount--;
	}
	
	// Update is called once per frame
	void Update () {
		bool isOnGround = collisionCount > 0;
		if (GvrViewer.Instance.Triggered && isOnGround)
		{
			var camera = GetComponentInChildren<Camera>();
			var projectedLookDirection = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
			var radianToRotate = Mathf.Deg2Rad * jumpElevationInDegrees;
			var unnormalizedJumpDirection = Vector3.RotateTowards(projectedLookDirection, Vector3.up, radianToRotate, 0);
			var jumpVector = unnormalizedJumpDirection.normalized * jumpSpeedInCMPS;
			GetComponent<Rigidbody> ().AddForce(jumpVector, ForceMode.VelocityChange);
		}
	}
}
```


**Creating Rotation from Vectors**

***Quaternion are used to represent rotations.*** 

Quaternion Euler(float x, float y, float z);
x degress around the x axis, and y degrees around
the y axis. 

- Within this code im rotating the Cubes Pivot point. 
By getting the **var camera = transform.parent.GetComponentInChildren<Camera>();**

- Then using **var projectedLookDirection = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);**
 
- so that it updates the look direction of the camera when I rotate the direction my Toad is looking in. 
![Imgur](https://i.imgur.com/bDRO5Vz.png)

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var camera = transform.parent.GetComponentInChildren<Camera>();
		var projectedLookDirection = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
		transform.rotation = Quaternion.LookRotation(projectedLookDirection);
	}
}
```

