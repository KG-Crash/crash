using System.Collections.Generic;
using System.Linq;
using Network;

namespace Game
{
    public class Team
    {
        public interface Listener : Player.Listener
        { }

        public int id { get; private set; }
        public PlayerCollection players { get; private set; }

        public Team(int id, Listener listener, BaseClient client)
        {
            this.id = id;
            this.players = new PlayerCollection(this, listener, client);
        }
    }
}