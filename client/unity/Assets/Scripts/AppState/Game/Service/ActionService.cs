using KG.Collection;
using Network;
using System.Collections.Generic;
using System.Linq;

namespace Game.Service
{
    public class ActionService
    {
        public interface Listener
        {
            void OnAction(int userId, Protocol.Response.Action action);
            void OnChat(int userId, Protocol.Response.InGameChat chat);
        }

        private Listener _listener;

        public Protocol.Request.ActionQueue Request { get; private set; } = new Protocol.Request.ActionQueue
        {
            Actions = new List<Protocol.Request.Action>()
        };

        public ActionBuffer Actions { get; private set; }

        public ActionService(Listener listener)
        {
            _listener = listener;
        }

        public void Setup(IEnumerable<int> sequences)
        {
            Actions = new ActionBuffer(sequences);
        }

        public void Leave(int sequence)
        {
            if (Actions == null)
                return;

            Actions.Remove(sequence);
        }

        public void Send(Protocol.Request.Action action) => Request.Actions.Add(action);

        public void Send(string message)
        {
            _ = Client.Send(new Protocol.Request.InGameChat
            {
                Frame = LockStep.Frame.In,
                Turn = LockStep.Turn.In,
                Message = message
            });
        }

        public void Flush()
        {
            Request.Turn = LockStep.Turn.In;
            _ = Client.Send(Request);
            Request.Actions.Clear();
        }

        public void Receive(Protocol.Response.ActionQueue response)
        {
            if (Actions == null)
                return;

            foreach (var x in response.Actions.GroupBy(x => x.Frame))
                Actions.Add(response.Sequence, response.Turn, x.Key, x.Select(x => x));
        }

        public void Receive(Protocol.Response.InGameChat response)
        {
            if (Actions == null)
                return;

            Actions.Add(response.Sequence, response.Turn, response.Frame, response);
        }

        public bool Update()
        {
            var buffers = Actions.Pop(LockStep.Turn.Out);
            if (buffers == null)
                return false;

            foreach (var pair in buffers)
            {
                var userId = pair.Key;
                var buffer = pair.Value;

                var protocols = buffer.Protocols.GroupBy(x => x.GetType())
                    .ToDictionary(x => x.Key, x => x.ToList());

                if (protocols.TryGetValue(typeof(Protocol.Response.Action), out var actions) == false)
                {
                    actions = new List<Protocol.IProtocol>();
                }
                if (protocols.TryGetValue(typeof(Protocol.Response.InGameChat), out var chats) == false)
                {
                    chats = new List<Protocol.IProtocol>();
                }
                foreach (var action in actions.Select(x => x as Protocol.Response.Action))
                {
                    _listener?.OnAction(userId, action);
                }
                foreach (var chat in chats.Select(x => x as Protocol.Response.InGameChat))
                {
                    _listener?.OnChat(userId, chat);
                }
            }

            return true;
        }
    }
}
