using KG;
using Shared.Type;
using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "UpgradeIconTable", menuName = "Crash/UpgradeIconTable", order = 0)]
    public class UpgradeIconTable : UnityTable
    {
        [SerializeField]
        private SerializableDictionary<Ability, Sprite> _abilitySpriteDict =
            new SerializableDictionary<Ability, Sprite>();
        
        public Sprite this[Ability ability]
        {
            get => _abilitySpriteDict[ability];
            set => _abilitySpriteDict[ability] = value;
        }

        public void Refresh()
        {
            var abilities = (Ability[]) Enum.GetValues(typeof(Ability));
            var keys = _abilitySpriteDict.Keys.ToArray();

            Array.ForEach(keys, (key) =>
            {
                if (Array.FindIndex(abilities, (other) => other == key) < 0)
                    _abilitySpriteDict.Remove(key);
            });

            Array.ForEach(abilities, (ability) =>
            {
                if (!_abilitySpriteDict.ContainsKey(ability))
                    _abilitySpriteDict.Add(ability, null);
            });
        }
    }
}