using UnityEngine;
using System.Collections;

public class ActionRandomRotator : MonoBehaviour 
{
	public float tumble;

	private void Start ()
	{
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
	}
}