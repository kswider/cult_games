using UnityEngine;

public class ActionDestroyByBoundary : MonoBehaviour
{
	private void OnTriggerExit (Collider other) 
	{
		Destroy(other.gameObject);
	}
}