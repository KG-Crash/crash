﻿using KG.Collection;
using KG.Util;
using Network;
using Newtonsoft.Json;
using Protocol.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public partial class GameController
    {
        private FileLogger _logger;
        private ActionBuffer _actions = new ActionBuffer();
        
        public Dictionary<int, string> uuidTable { get; private set; } = new Dictionary<int, string>();

        
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
            // 준비가 완료된 유저는 0 based의 Sequence가 발급됨
            // 준비가 안되면 -1로 설정됨
            var allReady = response.Users.All(x => x.Sequence != -1);
            if (allReady)
            {
                ready = true;
                _logger = new FileLogger($"log/{DateTime.Now}.txt".Replace(":", "_"));
            }

            uuidTable = response.Users.ToDictionary(x => x.Sequence, x => x.Id);
            Client.Instance.id = response.Users.FirstOrDefault(x => x.Id == Client.Instance.uuid)?.Sequence ?? -1;

            // team : users
            var users = response.Users
                .GroupBy(x => x.Team)
                .ToDictionary(x => x.Key, x => x.ToList());

            // random seed
            Client.Instance.seed = response.Seed;
            Debug.Log($"OnReady, myname is {Client.Instance.uuid}"); 

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
    }
}
