using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;
using OpenSim.Framework;
using Veis.Bots;
using Veis.OpenSim.Bots;
using Veis.Chat;
using OpenSim.Region.Framework.Scenes;

namespace Veis.OpenSim.Chat
{
    public class OpenSimChatHandler : ChatHandler
    {
        public const string CommandName = "OPENSIM";

        private readonly OpenSimNPCAvatar _owner;
        private readonly Scene _scene;

        public OpenSimChatHandler(OpenSimNPCAvatar owner, Scene scene)
        {
            _owner = owner;
            _scene = scene;
        } 
        
        public override bool CanHandleMessage(string message)
        {
            return message.Split(':')[0] == CommandName;
        }
        
        protected override string ProcessMessage(string answer, string pre, string post)
        {
            string output = "I've been told to say 'Feature not implemented', whatever that means.";

            char[] sep2 = { ':' };
            string request = answer.Split(':')[1];
            
            if (request == "NAME") {
                output = pre + _owner.FirstName + post;
            } else if (request == "APPEARANCE") {
                output = pre + _owner.Appearance + post;
            } else if (request == "REGION") {
                output = pre + _scene.RegionInfo.RegionName + post;
            } else if (request == "LOCATION") {

                ILandObject obj = _scene.LandChannel.GetLandObject(_owner.NPCBase.Position.X, _owner.NPCBase.Position.Y);

                if (obj != null) {
                    output = pre + obj.LandData.Name + post;
                } else {
                    output = pre + "UNKNOWN" + post;
                }
            } else if (request == "COORDINATES") {
                output = pre + "(" + _owner.NPCBase.Position.X + ", " + _owner.NPCBase.Position.Y + ")" + post;
            } else if (request == "HASANIMATION") {
                // TODO when animation is implemented on Bot
                if (_owner.GetDefaultAnimation(answer.Split(':')[2]) != UUID.Zero) {
                    output = pre + "Yes" + post;
                } else {
                    output = pre + "No" + post;
                }
            } else if (request == "TASK") {
                // TODO this may have something to do with physical action planning
                _owner.AddTaskToQueue(answer.Split(sep2, 3)[2]);
                output = pre + post;
            } else if (request == "NEARBY_PARCEL_AREIS") {
                // TODO we may have to investigate data snapshot a bit more
                List<ILandObject> nearbyParcels = _scene.LandChannel.ParcelsNearPoint(_owner.NPCBase.Position);

                String nameList = "";

                if (nearbyParcels.Count > 0) {

                    foreach (ILandObject obj in nearbyParcels) {
                        nameList += obj.LandData.Name + ", ";
                    }

                    nameList = nameList.Substring(0, nameList.Length - 2);

                    int toAndPos = nameList.LastIndexOf(", ");
                    if (toAndPos > 0) {
                        nameList.Remove(toAndPos, 1);
                        nameList.Insert(toAndPos, " and");
                    }

                    output = pre + nameList + " are" + post;
                } else {
                    output = pre + "nothing is" + post;
                }
            } else if (request == "PARCEL_OBJECTS") {
                // TODO Obviously
                EntityBase[] objs = _scene.GetEntities();

                String nameList = "";

                if (objs.Length > 0) {

                    int unknowns = 0;

                    ILandObject mParc = _scene.LandChannel.GetLandObject(_owner.NPCBase.Position.X, _owner.NPCBase.Position.Y);

                    foreach (EntityBase entity in objs) {
                        if (!mParc.ContainsPoint((int)Math.Round(entity.AbsolutePosition.X), (int)Math.Round(entity.AbsolutePosition.Y))) {
                            continue;
                        }

                        if (entity.Name.Length > 0) {
                            nameList += entity.Name + ", ";
                        } else {
                            unknowns++;
                        }
                    }

                    nameList = nameList.Substring(0, nameList.Length - 2);

                    if (unknowns == 0) {
                        int toAndPos = nameList.LastIndexOf(", ");
                        if (toAndPos > 0) {
                            nameList.Remove(toAndPos, 1);
                            nameList.Insert(toAndPos, " and");
                        }
                    } else {
                        nameList += " and " + unknowns + " unamed objects";
                    }

                    output = pre + nameList + " are" + post;
                } else {
                    output = pre + "nothing is" + post;
                }
            } else {
                output = pre + "{ERROR:PROC:OPENSIM}" + post;
            }

            return output;
        }
    }
}
