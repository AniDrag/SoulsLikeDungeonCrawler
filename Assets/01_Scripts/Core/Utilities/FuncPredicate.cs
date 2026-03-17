using System;

namespace AniDrag.Core
{
    /// <summary>
    ///  Call .Evaluate to see if condition is true.
    /// </summary>
    public class FuncPredicate : IPredicate
    {
        readonly Func<bool> func;

        public FuncPredicate(Func<bool> pFunc)
        {
            func = pFunc;
        }

        public bool Evaluate() => func.Invoke();
    }
}