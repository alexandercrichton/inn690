using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;
using Veis.Unity.Scene;
using UnityEngine;
using Veis.Unity.Simulation;

namespace Veis.Unity.Bots
{
    public class UnityBotAvatar : BotAvatar
    {

		public navAgent botAgentMovement;
        //public UUID UUID { get; set; }
        //public string Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        private UnitySceneService _sceneService;

        public UnityBotAvatar(string id, string name, string role, UnitySceneService sceneService)
        {
            //this.UUID = uuid;
            this.ID = id;
            this.Name = name;
            this.Role = role;
            this._sceneService = sceneService;
        }

        #region Agent Control Functions

        public override void Despawn()
        {
            throw new NotImplementedException();
        }

        public override void Say(string message)
        {
            Logging.UnityLogger.BroadcastMesage(this, "Bot[" + Name + "] says: " + message);

        }

		public override void DefineTask (string task) {
			botAgentMovement.SetTask(task);
		}

        public override void Touch(string objectName)
        {
            throw new NotImplementedException();
        }

        public override void WalkTo(string assetName)
        {
            //MainThread.QueueAction(()=> {
            //Veis.Unity.Logging.UnityLogger.BroadcastMesage(this, "Current object: " + this.ToString());
			botAgentMovement.SetTarget(GameObject.Find(assetName));
			
            //});
        }

		public void SendBotValues() {
			botAgentMovement.SetBotInfo (this);
		}

        #endregion

        public override bool IsAt(string assetName)
        {
            Vector3 location = GameObject.Find(assetName).transform.position;
            return (Vector3.Distance(botAgentMovement.transform.position, location) < 2f);
        }
    }
}
