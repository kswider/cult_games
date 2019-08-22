using UnityEngine;

public class OptionsController : MonoBehaviour
{
    private PlayerController _playerController;
    private void Start()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            _playerController = player.GetComponent<PlayerController>();
        }
    }

    public void ResetData()
    {
        _playerController.DeleteSave();
    }
}
