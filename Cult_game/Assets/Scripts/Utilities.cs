public static class Utilities
{
    public static PlayerController FindPlayer()
    {
        return Singleton<PlayerController>.Instance;
    }

    public static SceneController FindSceneController()
    {
        return Singleton<SceneController>.Instance;
    }
}
