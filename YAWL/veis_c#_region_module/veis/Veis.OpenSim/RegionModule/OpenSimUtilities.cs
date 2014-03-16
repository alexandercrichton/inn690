using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSim.Framework;
using OpenSim.Region.Framework.Scenes;
using OpenMetaverse;
using System.Xml;

namespace Veis.OpenSim.RegionModule
{
    public class OpenSimUtilities
    {
        public const string NPCBoxName = "NPCBox";  // Storage container for avatar outfits

        public const string NPCSpawnPointName = "NPCBox";

        public static Vector3 NPCSpawnPoint = Vector3.Zero;

        public static readonly Vector3 DefaultSpawnPoint = new Vector3(128, 128, 30);

        public const string DefaultAnimationLocation = "data/avataranimations.xml";

        public static ILandObject GetLandObject(string name, Scene scene)
        {
            List<ILandObject> allLand = scene.LandChannel.AllParcels();
            foreach (ILandObject obj in allLand)
            {
                if (obj.LandData.Name == name)
                {
                    return obj;
                }
            }
            return null;
        }

        public static Dictionary<String, UUID> InitDefaultAnimations()
        {
            Dictionary<String, UUID> animations = new Dictionary<String, UUID>();

            using (XmlTextReader reader = new XmlTextReader(DefaultAnimationLocation))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                if (doc.DocumentElement != null)
                    foreach (XmlNode nod in doc.DocumentElement.ChildNodes)
                    {
                        if (nod.Attributes["name"] != null)
                        {
                            string name = nod.Attributes["name"].Value.ToLower();
                            string id = nod.InnerText;
                            animations.Add(name, (UUID)id);
                        }
                    }
            }

            return animations;
        }
    }
}
