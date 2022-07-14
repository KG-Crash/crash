using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    public class TeamCollection : IReadOnlyDictionary<int, Team>
    {
        private Dictionary<int, Team> _teams = new Dictionary<int, Team>();
        private Team.Listener _listener;

        #region override method
        public Team this[int key]
        {
            get
            {
                if (this._teams.TryGetValue(key, out var team) == false)
                    return null;

                return team;
            }
        }
        
        public IEnumerable<int> Keys => this._teams.Keys;

        public IEnumerable<Team> Values => this._teams.Values;

        public int Count => this._teams.Count;

        public bool ContainsKey(int key) => this._teams.ContainsKey(key);

        public IEnumerator<KeyValuePair<int, Team>> GetEnumerator() => this._teams.GetEnumerator();

        public bool TryGetValue(int key, out Team value) => this._teams.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => this._teams.GetEnumerator();
        #endregion

        #region properties
        public GameState owner { get; private set; }
        public List<Player> allPlayers => this.SelectMany(x => x.Value.players.Values).ToList();
        #endregion

        public TeamCollection(GameState owner, Team.Listener listener)
        {
            this.owner = owner;
            _listener = listener;
        }

        public Team Add(int id)
        {
            if (this._teams.ContainsKey(id))
                throw new Exception("asd");

            var team = new Team(id, _listener);
            _teams.Add(id, team);
            return team;
        }

        public void Remove(int id)
        {
            if (this._teams.ContainsKey(id) == false)
                return;

            this._teams.Remove(id);
        }

        public Player Find(int playerID) => this.SelectMany(x => x.Value.players.Values).FirstOrDefault(x => x.id == playerID);
    }
}
