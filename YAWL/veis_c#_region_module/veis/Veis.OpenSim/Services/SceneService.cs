using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Services.Interfaces;
using OpenSim.Region.Framework.Scenes;
using OpenMetaverse;
using Veis.OpenSim.Bots;
using OpenSim.Framework;

namespace Veis.Services
{
    public class SceneService : ISceneService
    {
        private Scene _scene;

        public SceneService(Scene scene)
        {
            _scene = scene;
        }

        public void SetScene(Scene scene)
        {
            _scene = scene;
        }
    
        public Veis.Common.Math.Vector3 GetPositionOfObject(string name)
        {
            var obj = _scene.GetSceneObjectGroup(name);
            if (obj == null) return null;

            return obj.AbsolutePosition.ToLocal();
        }

        public bool PlaceObjectAt(string assetKey, string placemarkerName)
        {
            var placemarker = _scene.GetSceneObjectGroup(placemarkerName);
            if (placemarker == null) return false;

            UUID assetUUID;
            if (!UUID.TryParse(assetKey, out assetUUID)) return false;
            var asset = _scene.GetSceneObjectGroup(assetUUID);
            if (asset == null) return false;
            
            var pos =  placemarker.AbsolutePosition;
            var rot = placemarker.GroupRotation;
            asset.UpdateGroupRotationPR(pos, rot);
            return true;
        }

        public bool PlaceObjectAt(string assetKey, Veis.Common.Math.Vector3 pos)
        {
            UUID assetUUID;
            if (!UUID.TryParse(assetKey, out assetUUID)) return false;
            var asset = _scene.GetSceneObjectGroup(assetUUID);
            if (asset == null) return false;

            asset.UpdateGroupPosition(pos.ToOpenSim());
            return true;
        }

        public string GetAssetKey(string name)
        {
            var asset = _scene.GetSceneObjectGroup(name);
            if (asset == null) return string.Empty;
            return asset.UUID.ToString();
        }

        public string GetAssetName(string assetKey)
        {
            UUID assetUUID;
            if (!UUID.TryParse(assetKey, out assetUUID)) return string.Empty;

            var asset = _scene.GetSceneObjectGroup(assetUUID);
            if (asset == null) return string.Empty;
            return asset.Name;            
        }

        public string GetUserNameById(string id)
        {
            var avatar = _scene.GetScenePresence(UUID.Parse(id));
            if (avatar != null)
            {
                return avatar.Name;
            }
            return string.Empty;
        }

        public void ShowResponse(string response, bool inWorld)
        {

        }
        /*            if (response == "") return;
            if (inWorld)
            {
                UUID m_owner = _scene.RegionInfo.EstateSettings.EstateOwner;

                MainConsole.Instance.Output(m_owner.ToString());

                IClientAPI ownerAPI = null;
                if (_scene.TryGetClient(m_owner, out ownerAPI))
                {
                    ownerAPI.SendBlueBoxMessage(m_owner, "osboids", response);
                }
                else
                {
                    //MainConsole.Instance.Output("Cannot communicate with Client");
                }
            }
            else
            {
                MainConsole.Instance.Output(response);
            }
        }
         * */
    }
}
