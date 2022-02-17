using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KG.Collection
{
    public class FrameBuffer : IEnumerable<KeyValuePair<int, List<IProtocol>>> // int : Frame id
    {
        private readonly Dictionary<int, List<IProtocol>> _protocolSet = new Dictionary<int, List<IProtocol>>();

        #region implemented methods
        public List<IProtocol> this[int key] => _protocolSet[key];

        public IEnumerator<KeyValuePair<int, List<IProtocol>>> GetEnumerator() => _protocolSet.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _protocolSet.GetEnumerator();
        #endregion

        public IEnumerable<IProtocol> Protocols => _protocolSet.SelectMany(x => x.Value);

        public void Add(int frameId, IProtocol protocol)
        {
            if (_protocolSet.ContainsKey(frameId) == false)
                _protocolSet.Add(frameId, new List<IProtocol>());

            _protocolSet[frameId].Add(protocol);
        }

        public void AddRange(int frameId, IEnumerable<IProtocol> protocols)
        {
            if (_protocolSet.ContainsKey(frameId) == false)
                _protocolSet.Add(frameId, new List<IProtocol>());

            _protocolSet[frameId].AddRange(protocols);
        }

        public List<IProtocol> Peek(int frameId)
        {
            if (_protocolSet.TryGetValue(frameId, out var protocols) == false)
                return null;

            return protocols;
        }

        public List<IProtocol> Pop(int frameId)
        {
            var result = Peek(frameId);
            if(result != null)
                _protocolSet.Remove(frameId);

            return result;
        }
    }

    public class TurnBuffer : IEnumerable<KeyValuePair<int, FrameBuffer>> // int : turn id
    {
        private readonly Dictionary<int, FrameBuffer> _buffers = new Dictionary<int, FrameBuffer>();

        public IEnumerable<IProtocol> Protocols => _buffers.SelectMany(x => x.Value.Protocols);

        #region implemented methods
        public IEnumerator<KeyValuePair<int, FrameBuffer>> GetEnumerator() => _buffers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _buffers.GetEnumerator();
        #endregion

        public FrameBuffer this[int key] => _buffers[key];

        public void Add(int turnId, int frameId, IProtocol protocol)
        {
            if (_buffers.ContainsKey(turnId) == false)
                _buffers.Add(turnId, new FrameBuffer());

            _buffers[turnId].Add(frameId, protocol);
        }

        public void Add(int turnId, int frameId, IEnumerable<IProtocol> protocols)
        {
            if (_buffers.ContainsKey(turnId) == false)
                _buffers.Add(turnId, new FrameBuffer());

            _buffers[turnId].AddRange(frameId, protocols);
        }

        public FrameBuffer Peek(int turnId)
        {
            if (_buffers.TryGetValue(turnId, out var buffer) == false)
                return null;

            return buffer;
        }

        public FrameBuffer Pop(int turnId)
        {
            var result = Peek(turnId);
            if(result != null)
                _buffers.Remove(turnId);

            return result;
        }
    }

    public class ActionBuffer : IEnumerable<KeyValuePair<string, TurnBuffer>>
    {
        private readonly Dictionary<string, TurnBuffer> _buffers = new Dictionary<string, TurnBuffer>();

        #region implemented methods
        public IEnumerator<KeyValuePair<string, TurnBuffer>> GetEnumerator() => _buffers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _buffers.GetEnumerator();
        #endregion

        public TurnBuffer this[string key] => _buffers[key];

        public void Add(string userId, int turnId, int frameId, IEnumerable<IProtocol> protocols)
        {
            if (_buffers.ContainsKey(userId) == false)
                _buffers.Add(userId, new TurnBuffer());

            _buffers[userId].Add(turnId, frameId, protocols);
        }

        public Dictionary<string, FrameBuffer> Peek(int turnId)
        {
            return _buffers.ToDictionary(x => x.Key, x => x.Value.Peek(turnId));
        }

        public bool IsReady(int turnId)
        {
            if (_buffers.Count == 0)
                return false;

            return Peek(turnId).All(x => x.Value != null);
        }

        public Dictionary<string, FrameBuffer> Pop(int turnId)
        {
            if (IsReady(turnId) == false)
                return null;

            var result = _buffers.ToDictionary(x => x.Key, x => x.Value.Pop(turnId));
            return result;
        }
    }
}
