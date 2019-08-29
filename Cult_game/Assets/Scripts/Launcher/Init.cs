using UnityEngine;

namespace Launcher
{
    public sealed class Init : MonoBehaviour
    {
        private void Start ()
        {
            SceneController sm = Utilities.FindSceneController();
            sm.GoToScene(SceneController.SCN_MAIN_MENU);
        }
    }
}
