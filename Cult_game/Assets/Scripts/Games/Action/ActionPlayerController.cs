using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class ActionPlayerController : MonoBehaviour
{
	public float speed;
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	 
	private float _nextFire;

	private Quaternion _calibrationQuaternion;

	private void Start()
	{
		CalibrateAccelerometer();
	}
	
	void Update()
	{
		if (!Input.GetButton("Fire1") || !(Time.time > _nextFire)) return;
		
		_nextFire = Time.time + fireRate;
		Instantiate(shot, shotSpawn.transform.position, shotSpawn.transform.rotation); //as GameObject;
	}
	
	private void FixedUpdate ()
	{
		Vector3 accelerationRaw = Input.acceleration; // by  Vector3(0.0f, 0.0f, -1.0f)
		Vector3 acceleration = FixAcceleration(accelerationRaw); // by initial zero point

		Vector3 movement = new Vector3(acceleration.x, 0.0f, acceleration.y);
		GetComponent<Rigidbody>().velocity = movement * speed;
		
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
	
	//Used to calibrate the Input.acceleration input
	private void CalibrateAccelerometer()
	{
		Vector3 accelerationSnapshot = Input.acceleration;
		Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
		_calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
	}

	//Get the 'calibrated' value from the Input
	private Vector3 FixAcceleration(Vector3 acceleration)
	{
		Vector3 fixedAcceleration = _calibrationQuaternion * acceleration;
		return fixedAcceleration;
	}
}
