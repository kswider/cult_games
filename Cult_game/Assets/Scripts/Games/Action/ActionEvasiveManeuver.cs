using UnityEngine;
using System.Collections;

public class ActionEvasiveManeuver : MonoBehaviour
{
	public Boundary boundary;
	public float tilt;
	public float dodge;
	public float smoothing;
	public Vector2 startWait;
	public Vector2 maneuverTime;
	public Vector2 maneuverWait;

	private float _currentSpeed;
	private float _targetManeuver;
	
	private Rigidbody _rigidbody;
	
	private void Start ()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_currentSpeed = _rigidbody.velocity.z;
		StartCoroutine(Evade());
	}

	private IEnumerator Evade ()
	{
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y));
		while (true)
		{
			_targetManeuver = Random.Range (1, dodge) * -Mathf.Sign (transform.position.x);
			yield return new WaitForSeconds (Random.Range (maneuverTime.x, maneuverTime.y));
			_targetManeuver = 0;
			yield return new WaitForSeconds (Random.Range (maneuverWait.x, maneuverWait.y));
		}
	}

	private void FixedUpdate ()
	{
		float newManeuver = Mathf.MoveTowards (_rigidbody.velocity.x, _targetManeuver, smoothing * Time.deltaTime);
		_rigidbody.velocity = new Vector3 (newManeuver, 0.0f, _currentSpeed);
		_rigidbody.position = new Vector3
		(
			Mathf.Clamp(_rigidbody.position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp(_rigidbody.position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0, 0, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}
