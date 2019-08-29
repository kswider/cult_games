using UnityEngine;
using System.Collections;

public class ActionMover : MonoBehaviour
{
	public float speed;

	private void Start ()
	{
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
}
