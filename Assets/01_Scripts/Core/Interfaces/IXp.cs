using System;

namespace AniDrag.Core
{
    public interface IXp
    {
        int CurrentXp { get; }
        int MaxXp { get; }
        int Level { get; }
        void GainXp(int amount);
        void Initialize(int startLevel);  
        void SetLevel(int level, int xp); 
        
        event Action<IXp> OnXpChanged;
        event Action<int> OnLevelUp;      // int = new level
    }
}