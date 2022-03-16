﻿using KG.Collection;
using KG.Util;
using Network;
using Newtonsoft.Json;
using Protocol.Response;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public partial class GameController
    {
        private FileLogger _logger;
        private ActionBuffer _actions = new ActionBuffer();

        
        [FlatBufferEvent]
        public async Task<bool> OnActionQueue(ActionQueue response)
        {
            if (_logger != null)
            {
                _logger.Info($"[{response.User}] : {JsonConvert.SerializeObject(response.Actions)}");
            }

            Debug.Log($"receive queue : {response.Turn}, {response.User}, me?={response.User==Client.Instance.id}");

            foreach (var x in response.Actions.GroupBy(x => x.Frame))
                _actions.Add(response.User, response.Turn, x.Key, x.Select(x => x));

            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnInGameChat(InGameChat response)
        {
            if (_logger != null)
            {
                _logger.Info($"[{response.User}] : {JsonConvert.SerializeObject(response)}");
            }

            Debug.Log($"chat receive queue : {response.Turn}, {response.User},{response.Message} me?={response.User == Client.Instance.id}");
            
            _actions.Add(response.User, response.Turn, response.Frame, response);
            

            return true;
        }

        public void OnActionSelf(ActionQueue myAction)
        {
            
        }
        
        [FlatBufferEvent]
        public async Task<bool> OnReady(Ready response)
        {
            var allReady = (response.ReadyState.Count == response.Users.Count);
            if (allReady)
            {
                ready = true;
                _logger = new FileLogger($"log/{DateTime.Now}.txt".Replace(":", "_"));
            }

            // team : users
            var users = response.Users
                .GroupBy(x => x.Team)
                .ToDictionary(x => x.Key, x => x.ToList());

            // random seed
            Client.Instance.seed = response.Seed;
            Debug.Log($"OnReady, myname is {Client.Instance.id}"); 

            foreach (var user in response.Users)
            {
                Debug.Log($"OnReady, user.Id={user.Id}");
            }

            return true;
        }

        [FlatBufferEvent]
        public async Task<bool> OnResume(Resume response)
        {
            if (!paused)
            {
                Debug.LogError("receive resume but not paused.");
                return true;
            }

            Debug.Log($"user {response.User} resume.");
            paused = false;
            return true;
        }

        public void SendResume()
        {
            _ = Client.Send(new Protocol.Request.Resume());
        }

        public void ReservePause()
        {
            EnqueueAction(new Protocol.Request.Action
            {
                Frame = InputFrame,
                Id = (int)Shared.ActionKind.Pause
            });
        }
    }
}
