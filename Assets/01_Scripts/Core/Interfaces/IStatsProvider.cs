namespace AniDrag.Core
{
    public interface IStatsProvider
    {
        Stats GetBaseStats();
        Stats GetTotalStats(); // including equipment
    }
}