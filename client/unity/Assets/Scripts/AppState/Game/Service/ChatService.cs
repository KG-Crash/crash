using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Service
{
    public class ChatService
    {

        public virtual bool Send(ActionService actionService, string msg )
        {
            if (string.IsNullOrEmpty(msg))
                return false;

            actionService.Send(msg);
            return true;
        }

        public virtual bool Recv(string msg, string user ) // chatlog ³ªÁß¿¡ Ãªºä·Î º¯°æ
        {
           // chatlog. += $"\n {user}  :  {msg}";
            return true;
            //_scrollRect.verticalNormalizedPosition = 0.0f;
        }
    }
}