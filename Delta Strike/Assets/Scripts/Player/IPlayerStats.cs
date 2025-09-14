using Game.Core.Config;

namespace Game.Player
{
    public interface IPlayerStats
    {
        float GetStatValue(StatType stat); 
        int GetLevel(StatType stat);
    }
}