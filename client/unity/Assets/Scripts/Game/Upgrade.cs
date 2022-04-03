using Shared;
using Shared.Table;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Upgrade
    {
        private Dictionary<Ability, int> _startFrames = new Dictionary<Ability, int>();

        public Player owner { get; private set; }
        public Ability abilities { get; private set; } = Ability.NONE;

        public Upgrade(Player owner)
        {
            this.owner = owner;
        }

        public void Update(Frame frame)
        {
            var completedList = _startFrames.Where(pair =>
            {
                var ability = pair.Key;
                var startFrame = pair.Value;

                var time = Table.From<TableUnitUpgradeCost>()[ability].Time;
                var requiredFrame = time / GameController.TimeDelta;
                
                return startFrame + requiredFrame < frame.currentFrame;
            }).Select(x => x.Key).ToList();

            foreach (var ability in completedList)
            {
                this.abilities |= ability;
                this._startFrames.Remove(ability);

                this.owner.listener?.OnFinishUpgrade(ability);
            }
        }

        public void Start(Ability ability, int currentFrame)
        {
            if (ability == Ability.NONE)
                return;

            this._startFrames[ability] = currentFrame;
        }
    }
}
