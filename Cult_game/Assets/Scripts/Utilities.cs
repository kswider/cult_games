public static class Utilities
{
    public static PlayerController FindPlayer()
    {
        return Singleton<PlayerController>.Instance;
    }
}
