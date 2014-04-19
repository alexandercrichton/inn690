using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Bots;
using Veis.Unity.Scene;

namespace Veis.Unity.Bots
{
    public class UnityNPCAvatar : BotAvatar
    {

        public UUID UUID { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        private SceneService _sceneService;

        public UnityNPCAvatar(UUID newNPCUUID, string p1, string p2, SceneService _sceneService)
        {
            this.UUID = newNPCUUID;
            this.FirstName = p1;
            this.LastName = p2;
            this._sceneService = _sceneService;
        }

        #region Agent Control Functions

        public override void Despawn()
        {
            throw new NotImplementedException();
        }

        public override void Drop()
        {
            throw new NotImplementedException();
        }

        public override void FlyToLocation(Common.Math.Vector3 position)
        {
            throw new NotImplementedException();
        }

        public override void PickUp(string objectName)
        {
            throw new NotImplementedException();
        }

        public override void PlayAnimation(string animationName)
        {
            throw new NotImplementedException();
        }

        public override void Say(string message)
        {
            throw new NotImplementedException();
        }

        //public override void SendTextBox(string message, int chatChannel, string objectname, UUID ownerID, string ownerFirstName, string ownerLastName, UUID objectId)
        //{
        //    throw new NotImplementedException();
        //}

        public override void SitOn(string objectName)
        {
            throw new NotImplementedException();
        }

        public override void StandUp()
        {
            throw new NotImplementedException();
        }

        public override void StopAnimation()
        {
            throw new NotImplementedException();
        }

        public override void Touch(string objectName)
        {
            throw new NotImplementedException();
        }

        public override void WalkTo(string areaName)
        {
            throw new NotImplementedException();
        }

        public override void WalkToLocation(Common.Math.Vector3 position)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
