using System;
using System.Collections;
using System.Collections.Generic;
using Network;

namespace Game
{
    public class PlayerCollection : IReadOnlyDictionary<int, Player>
    {
        #region override methods
        private Dictionary<int, Player> _players = new Dictionary<int, Player>();

        public Player this[int key]
        {
            get
            {
                if (this._players.TryGetValue(key, out var player) == false)
                    return null;

                return player;
            }
        }

        public IEnumerable<int> Keys => this._players.Keys;

        public IEnumerable<Player> Values => this._players.Values;

        public int Count => this._players.Count;

        public bool ContainsKey(int key) => this._players.ContainsKey(key);

        public IEnumerator<KeyValuePair<int, Player>> GetEnumerator() => this._players.GetEnumerator();

        public bool TryGetValue(int key, out Player value) => this._players.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => this._players.GetEnumerator();
        #endregion

        #region private fields
        private Team _team;
        private Player.Listener _listener;
        private BaseClient _client;
        #endregion

        public PlayerCollection(Team team, Player.Listener listener, BaseClient client)
        {
            _team = team;
            _listener = listener;
            _client = client;
        }

        public Player Add(int id, int spawnIndex)
        {
            if (this._players.ContainsKey(id))
                throw new Exception("asd");

            var player = new Player(id, _team, spawnIndex, _listener, _client);
            this._players.Add(id, player);
            return player;
        }

        public void Remove(int id)
        {
            if (this._players.ContainsKey(id) == false)
                return;

            this._players.Remove(id);
        }
    }
}
