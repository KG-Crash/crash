using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class Team
    {
        public interface Listener : Player.Listener
        { }

        public int id { get; private set; }
        public PlayerCollection players { get; private set; }

        public Team(int id, Listener listener)
        {
            this.id = id;
            this.players = new PlayerCollection(this, listener);
        }
    }
}