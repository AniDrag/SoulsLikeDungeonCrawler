using UnityEngine;

namespace AniDrag.CharacterComponents {
    /// <summary>
    /// This ScriptableObject defines a character class with base stats and growth factors.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterClass", menuName = "AniDrag/Character/Character Class")]
    public class CharacterClass : ScriptableObject
    {
        [Header("========================\n" +
        "    Weapon details      \n" +
        "========================")]
        [field: SerializeField] public string className { get; private set; }
        [field: SerializeField] public Stats baseStats { get; private set; }
        [field: SerializeField] public Stats growthFactors { get; private set; }


        private void OnValidate()
        {
#if UNITY_EDITOR
            className = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}