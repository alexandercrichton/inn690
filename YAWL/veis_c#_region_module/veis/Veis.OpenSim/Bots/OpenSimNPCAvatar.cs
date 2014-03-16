using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Timers;
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Region.OptionalModules.World.NPC;
using OpenSim.Region.Framework.Scenes;

using Veis.OpenSim.RegionModule;
using Veis.Chat;
using Veis.Planning;
using Veis.Common;
using Veis.Bots;

namespace Veis.OpenSim.Bots
{
    public class OpenSimNPCAvatar : BotAvatar
    {        
        public UUID UUID { get; set; }          // UUID of this NPC
        public NPCAvatar NPCBase { get; set; }      // OpenSim NPCAvatar that this NPC encapsulates
        public Scene Scene { get; set; }      // OpenSim scene that this NPC belongs to

        public string FirstName { get; set; }   
        public string LastName { get; set; }
        public string Appearance { get; set; }
        public Vector3 StartPosition { get; set; }
        public string Id { get; set; }

        private uint heldItem = 0;
        private readonly Dictionary<string, UUID> _defaultAnimations = new Dictionary<string, UUID>();

        #region Constructors

        public OpenSimNPCAvatar(string firstName, string lastName, Vector3 startPosition) : base()
        {
            FirstName = firstName;
            LastName = lastName;
            StartPosition = startPosition;
            ChatHandle = new AimlChatProvider(firstName);
            _defaultAnimations = OpenSimUtilities.InitDefaultAnimations();
        }

        public OpenSimNPCAvatar(UUID uuid, string firstName, string lastName, Scene scene, NPCAvatar npcBase, string appearance = "bot") : base()
        {
            UUID = uuid;
            FirstName = firstName;
            LastName = lastName;
            StartPosition = Vector3.Zero;
            Scene = scene;
            NPCBase = npcBase;
            Appearance = appearance;
            ChatHandle = new AimlChatProvider(firstName);
            _defaultAnimations = OpenSimUtilities.InitDefaultAnimations();
        }

        #endregion

        #region Chat

        public void ReceiveChat(OSChatMessage message)
        {
            // TODO: Make sure it doesn't talk to other bots
            if (!(message.SenderUUID == this.UUID)) // Make sure its not talking to itself
            {
                // Make sure its not another bot
                ScenePresence sp = Scene.GetScenePresence(message.SenderUUID);

                Thread.Sleep(100);
                string reply = RecieveChat(message.ToLocal());

                if (!String.IsNullOrEmpty(reply)) Say(reply);
            }
        }

        public void ReceiveMessage(GridInstantMessage im)
        {
            //there are a variety of InstantMessageDialog choices.. MessageFromObject and MessageFromAgent would be the two most common
            if (im.dialog == (byte)InstantMessageDialog.MessageFromAgent ||
                im.dialog == (byte)InstantMessageDialog.MessageFromObject)
            {
                if (!new UUID(im.fromAgentID).Equals(this.UUID)) // Make sure its not talking to itself
                {
                    string reply = RecieveChat(im.ToLocal());

                    NPCBase.InstantMessage(new UUID(im.fromAgentID), reply);

                }
            }
        }

        public void ReceiveMessage(IClientAPI remoteclient, GridInstantMessage im)
        {
            //there are a variety of InstantMessageDialog choices.. MessageFromObject and MessageFromAgent would be the two most common
            if (im.dialog == (byte)InstantMessageDialog.MessageFromAgent ||
                im.dialog == (byte)InstantMessageDialog.MessageFromObject)
            {
                if (!new UUID(im.fromAgentID).Equals(this.UUID)) // Make sure its not talking to itself
                {
                    string reply = RecieveChat(im.ToLocal());

                    // Send an instant message back to the user

                    if (remoteclient != null && !String.IsNullOrEmpty(reply))
                    {
                        remoteclient.SendInstantMessage(
                                    new GridInstantMessage(
                                    Scene, new UUID(im.fromAgentID), im.fromAgentName,
                                    new UUID(im.toAgentID),
                                    (byte)InstantMessageDialog.MessageFromAgent,
                                    reply, false,
                                    new Vector3()));
                    }
                   
                }
            }
        }

        #endregion

        #region Agent Controls Functions

        public override void Say(string message)
        {
            if (NPCBase != null)
            {
                NPCBase.Say(message);
            }
        }

        public override void SendTextBox(string message, int chatChannel, string objectname, UUID ownerID, string ownerFirstName, string ownerLastName, UUID objectId)
        {
            if (NPCBase != null)
            {
                NPCBase.SendTextBoxRequest(message, chatChannel, objectname, ownerID, ownerFirstName, ownerLastName, objectId);
            }
        }

        public override void FlyToLocation(Veis.Common.Math.Vector3 position)
        {
            ScenePresence npc = Scene.GetScenePresence(UUID);
            npc.MoveToTarget(position.ToOpenSim(), false, true);
            while (npc.MovingToTarget)
            {
                Thread.Sleep(2000);
            }
        }

        public override void WalkToLocation(Veis.Common.Math.Vector3 position)
        {
            ScenePresence npc = Scene.GetScenePresence(UUID);
            npc.MoveToTarget(position.ToOpenSim(), true, true);
            // Don't do anything until the movement is finished (poll npc.MovingToTarget)
            while (npc.MovingToTarget)
            {
                Thread.Sleep(2000);
            }

        }

        public override void Despawn()
        {
            (Scene.Modules["NPCModule"] as NPCModule).DeleteNPC(UUID, Scene);
        }

        /// <summary>
        /// New from Paul - fixed by Kat
        /// </summary>
        /// <param name="parcelName"></param>
        public override void WalkTo(string parcelName)
        {
            ILandObject obj = OpenSimUtilities.GetLandObject(parcelName, Scene);
            if (obj != null)
            {
                ScenePresence npc = Scene.GetScenePresence(UUID);
                npc.MoveToTarget((obj.StartPoint + obj.EndPoint) / 2, false, true);
                while (npc.MovingToTarget)
                {
                    Thread.Sleep(4000);
                }
            }
            else
            {
                Say("{ERROR:ACTION:WALKTO:UNKNOWNLOCATION}");
            }
        }

        /// <summary>
        /// New from Paul
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="attachPoint"></param>
        public override void PickUp(string objectName, int attachPoint = 0)
        {
            AttachmentPoint attachLoc;
            if (attachPoint == 0)
            {
                attachLoc = AttachmentPoint.RightHand;
            }
            else
            {
                attachLoc = (AttachmentPoint)Enum.ToObject(typeof(AttachmentPoint), attachPoint);
            }

            SceneObjectGroup obj = Scene.GetSceneObjectGroup(objectName);
            if (obj != null)
            {
                Scene.AttachmentsModule.AttachObject(Scene.GetScenePresence(UUID), obj, (uint)attachPoint, false);
                heldItem = obj.LocalId;
            }
            else
            {
                Say("{ERROR:ACTION:PICKUP:UNKNOWNOBJECT}");
            }
        }

        /// <summary>
        /// New from Paul
        /// </summary>
        public override void Drop()
        {
            if (heldItem != 0)
            {
                Scene.AttachmentsModule.DetachSingleAttachmentToGround(Scene.GetScenePresence(UUID), (uint)heldItem);
                heldItem = 0;
            }
            else
            {
                Say("{ERROR:ACTION:DROP:NOOBJECT}");
            }
        }

        /// <summary>
        /// New from Paul - See what this does (fixed by Kat, hopefully it works)
        /// </summary>
        /// <param name="objectName"></param>
        public override void Touch(String objectName)
        {
            SceneObjectPart obj = Scene.GetSceneObjectPart(objectName);

            if (obj != null)
            {
                GrabObject handlerGrabObject = (GrabObject)NPCBase.GetType().GetField("OnGrabObject", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(NPCBase);

                if (handlerGrabObject != null)
                {
                    List<SurfaceTouchEventArgs> touchArgs = new List<SurfaceTouchEventArgs>();
                    handlerGrabObject(obj.LocalId, Vector3.Zero, NPCBase, touchArgs);
                }
            }
            else
            {
                Say("{ERROR:ACTION:TOUCH:UNKNOWNOBJECT}");
            }
        }

        /// <summary>
        /// New from Paul. (fixed by Kat)
        /// </summary>
        /// <param name="objectName"></param>
        public override void SitOn(String objectName)
        {
            SceneObjectPart obj = Scene.GetSceneObjectPart(objectName);

            if (obj != null)
            {
                AgentRequestSit handlerAgentRequestSit = (AgentRequestSit)NPCBase.GetType().GetField("OnAgentRequestSit", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(NPCBase);
                if (handlerAgentRequestSit != null)
                    handlerAgentRequestSit(NPCBase, UUID, obj.UUID, obj.SitTargetPosition);
            }
            else
            {
                Say("{ERROR:ACTION:SIT:UNKNOWNOBJECT}");
            }
        }

        public override void StandUp()
        {
            Scene.GetScenePresence(this.UUID).StandUp();
        }

        // TODO: Animation querying needs to be abstracted elsewhere
        public UUID GetDefaultAnimation(string name)
        {
            if (_defaultAnimations.ContainsKey(name))
                return _defaultAnimations[name];
            return UUID.Zero;
        }

        public override void PlayAnimation(string animationName)
        {
            animationName = animationName.ToLower();
            if (GetDefaultAnimation(animationName) != UUID.Zero)
            {
                UUID animUUID = GetDefaultAnimation(animationName);
                Scene.GetScenePresence(UUID).Animator.AddAnimation(animUUID, UUID.Zero);
            }
            else
            {
                SceneObjectPart sop = Scene.GetSceneObjectPart(OpenSimUtilities.NPCBoxName);
                if (sop != null)
                {
                    Scene.GetScenePresence(UUID).Animator.AddAnimation(animationName, sop.UUID);
                }
            }
        }

        public override void StopAnimation()
        {
            Scene.GetScenePresence(UUID).Animator.Animations.Clear();
            Scene.GetScenePresence(UUID).Animator.AddAnimation("stand_1", UUID.Zero);
        }

        #endregion


    }
}
