using UnityEngine;
using System.Collections;

public class ActionDestroyByTime : MonoBehaviour
{
	public float lifetime;

	private void Start ()
	{
		Destroy (gameObject, lifetime);
	}
}
