using System;
using Shared.Table;
using Shared.Type;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Upgrade
    {
        public Ability? currentUpdateAbility { get; private set; }
        public uint? updateStartFrame { get; private set; }
        public List<Ability> reservedAbilities { get; private set; }

        public Player owner { get; private set; }
        public Ability abilities { get; private set; } = Ability.NONE;
        public Dictionary<Ability, int[]> lastFrames { get; private set; }  = new Dictionary<Ability, int[]>();

        public bool inUpgradeProgress => currentUpdateAbility != null;

        public Upgrade(Player owner)
        {
            this.owner = owner;
            reservedAbilities = new List<Ability>();
        }

        public void UpdateAbility(Frame input)
        {
            CheckUpgradeTime(input);
            CheckMyUnitSpawn(input);
        }

        private void CheckUpgradeTime(Frame input)
        {    
            if (currentUpdateAbility == null || updateStartFrame == null)
                return;

            var ability = currentUpdateAbility.Value;
            
            var upgradeCost = Table.From<TableUnitUpgradeCost>()[ability];
            var totalFrame = (uint)(input.currentFrame + input.currentTurn * Shared.Const.Time.FramePerTurn);
            if (updateStartFrame.Value + upgradeCost.DurationSec * Shared.Const.Time.FPS < totalFrame)
            {
                lastFrames[ability] = Enumerable.Repeat(int.MinValue, Table.From<TableUnitUpgradeSpawn>()[ability].Count).ToArray();
                owner.listener?.OnFinishUpgrade(ability);

                if (reservedAbilities.Count > 0)
                {
                    currentUpdateAbility = reservedAbilities[0];
                    reservedAbilities.RemoveAt(0);
                    updateStartFrame = totalFrame;
                }
                else
                {
                    currentUpdateAbility = null;
                    updateStartFrame = null;   
                }
            }
        }
        
        private void CheckMyUnitSpawn(Frame input)
        {
            var table = Table.From<TableUnitUpgradeSpawn>();
            var sourceAbilities = (Ability[])Enum.GetValues(typeof(Ability));
            var inputFrame = input.currentFrame + input.currentTurn * Shared.Const.Time.FramePerTurn;
        
            foreach (var ability in sourceAbilities.Where(ability => (abilities & ability) > 0 && table.ContainsKey(ability)))
            {
                if (!lastFrames.TryGetValue(ability, out var lastTimes) ||
                    !table.TryGetValue(ability, out var spawns))
                    continue;

                for (var i = 0; i < spawns.Count; i++)
                {
                    var spawn = spawns[i];
                    if (spawn.CycleSec * Shared.Const.Time.FPS + lastTimes[i] > inputFrame)
                        continue;
                    
                    lastTimes[i] = inputFrame;
                    owner.listener?.OnSpawnMyUnitByUpgrade(spawn);
                }
            }
        }

        public void Start(Ability ability, uint currentFrame)
        {
            if (ability == Ability.NONE)
                return;

            if (currentUpdateAbility == null)
            {
                currentUpdateAbility = ability;
                updateStartFrame = currentFrame;
            }
            else
            {
                reservedAbilities.Add(ability);
            }
        }

        public void Reserve(Ability ability)
        {
            reservedAbilities.Add(ability);
        }

        public void AddAbility(Ability ability)
        {
            abilities |= ability;
        }

        public void RemoveAbility(Ability ability)
        {
            abilities &= ~ability;
        }

        // 리스너에 알려야함
        public void CancelAbility(Ability ability, uint currentFrame)
        {
            if (currentUpdateAbility == null)
                return;
            
            if ((currentUpdateAbility.Value & ability) > 0)
            {
                if (reservedAbilities.Count > 0)
                {
                    currentUpdateAbility = reservedAbilities[0];
                    reservedAbilities.RemoveAt(0);
                    updateStartFrame = currentFrame;
                }
                else
                {
                    currentUpdateAbility = null;
                    updateStartFrame = null;
                }
            }
            else if (reservedAbilities.FindIndex(x => x == ability) >= 0)
            {
                reservedAbilities.Remove(ability);
            }
            
            owner.listener?.OnUpgradeCancel(ability);
        }
    }
}
