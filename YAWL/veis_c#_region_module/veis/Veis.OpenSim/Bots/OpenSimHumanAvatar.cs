using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;
using OpenSim.Region.Framework.Scenes;
using OpenSim.Framework;
using Veis.Bots;

namespace Veis.OpenSim.Bots
{
    public class OpenSimHumanAvatar : HumanAvatar
    {
        // Human has identification
        public UUID UUID { get; set; }  // UUID of this human user

        // Information about the virtual world
        public Scene Scene { get; set; }
        private IClientAPI _user { get; set; } // The in-world user of the human avatar

        #region Constructors

        public OpenSimHumanAvatar(Scene scene, UUID uuid, string name, string actingName = "")
        {
            UUID = uuid;
            Name = name;
            ActingName = actingName;
            Scene = scene;

            try 
            {
                // Set up event listeners to track completed work
                _user = scene.GetScenePresence(UUID).ControllingClient;
                _user.OnLogout += new Action<IClientAPI>(OnLogout);
                _user.OnStopMovement += new GenericCall2(OnStopMovement);
                _user.OnStopAnim += new StopAnim(OnStopAnimation);
                _user.OnObjectClickAction += new GenericCall7(OnObjectClickAction);
            }
            catch (NullReferenceException ex) {
                System.Diagnostics.Debug.Print(ex.Message + ex.Source);
            }
            
        }
        
        #endregion

        #region Action Tracking

        void OnObjectClickAction(IClientAPI remoteClient, uint localID, string message)
        {
            // Check if the user touched the right object
            //throw new NotImplementedException();
        }

        void OnStopAnimation(IClientAPI remoteClient, UUID animID)
        {
            // Check if the user just played the right animation
            
            const string JumpUUID = "709ea28e-1573-c023-8bf8-520c8bc637fa";

            if (animID.ToString() != JumpUUID) return;

            Random r = new Random();

            //int taskToComplete = r.Next(TempTaskBucket.Count());

            //if (TempTaskBucket.Count() == 0) return;

            //WorkProvider.CompleteWork(TempTaskBucket.ElementAt(taskToComplete));

            //TempTaskBucket.RemoveAt(taskToComplete);
        }

        void OnStopMovement()
        {
            // Check if at the right spot
            //throw new NotImplementedException();
        }

        public void OnLogout(IClientAPI obj)
        {
            throw new NotImplementedException("Critical");
            // Perform tear down logic on this human and transfer responsibility
            // to an NPC bot if necessary
            //throw new NotImplementedException();
        }

        #endregion

        #region Communicating With User

        public override void NotifyUser(String message)
        {
            if (_user == null) return;

            //_user.SendAgentAlertMessage(message, true);
            //GridInstantMessage im = new GridInstantMessage();

            // TODO: We want to send either a 
            // 1. Dialog  
            // 2. Direct chat message  
            // 3. Instant message from system
            // 4. Instant message from invisible manager client
            
            // Fortune, it is where the script is
            int channelToBroadcast = -689999;

            Vector3 vc = new Vector3(150, 210, 32);
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(message);
            Scene.SimChatBroadcast(b1, ChatTypeEnum.Region, channelToBroadcast, vc, "Location_Mark_Toggle", UUID.Zero, false);

            //_user.SendDialog("",UUID.Zero,UUID.Zero,"Test","User",message,UUID.Zero,0,button);
        }

        #endregion

    }
}
