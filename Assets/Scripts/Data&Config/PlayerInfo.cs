
public class PlayerInfo : OverrodeOperator
{
    public LevelInfo[] LevelInfo
    {
        get;
        set;
    }

    public PlayerInfo()
    {
        LevelInfo = new LevelInfo[GameConfig.LevelCount];
    }
}


