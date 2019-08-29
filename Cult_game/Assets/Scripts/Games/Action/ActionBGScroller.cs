using UnityEngine;
using System.Collections;

public class ActionBGScroller : MonoBehaviour
{
	public float scrollSpeed;
	public float tileSizeZ;

	private Vector3 _startPosition;

	void Start ()
	{
		_startPosition = transform.position;
	}

	void Update ()
	{
		float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
		transform.position = _startPosition + Vector3.forward * newPosition;
	}
}