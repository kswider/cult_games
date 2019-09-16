using System;
using System.Collections.Generic;

[Serializable]
public class PlayersWrapper
{
    public List<PlayerInfo> playerInfos;
}
[Serializable]
public class PlayerInfo
{
    public int id;
    public string username;
    public int points;
}
