using System;
namespace AniDrag.Core
{
    public interface IDefense
    {
        int DefenseValue { get; }
        void AddDefense(int amount);
        void RemoveDefense(int amount);
        void SetBaseDefense(int value);
        
        event Action<IDefense> OnDefenseChanged;
    }
}