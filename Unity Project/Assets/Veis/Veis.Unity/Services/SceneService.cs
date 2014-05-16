using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Services.Interfaces;
using Veis.Unity.Bots;
using UnityEngine;

namespace Veis.Unity.Scene
{
    public class SceneService : ISceneService
    {
        public SceneService()
        {

        }

        //private Scene _scene;

        //public SceneService(Scene scene)
        //{
        //    _scene = scene;
        //}

        //public void SetScene(Scene scene)
        //{
        //    _scene = scene;
        //}

        public Veis.Common.Math.Vector3 GetPositionOfObject(string name)
        {
            GameObject gameObject = GameObject.Find(name);
            if (gameObject == null)
            {
                return null;
            }
            Vector3 position = gameObject.transform.position;
            return new Veis.Common.Math.Vector3(position.x, position.y, position.z);
        }

        public string GetAssetKey(string name)
        {
            Asset[] assets = GameObject.FindObjectsOfType<Asset>();
            foreach (Asset asset in assets)
            {
                if (asset.AssetName == name)
                {
                    return asset.AssetKey;
                }
            }
            return null;
        }

        public string GetAssetName(string assetKey)
        {
            Asset[] assets = GameObject.FindObjectsOfType<Asset>();
            foreach (Asset asset in assets)
            {
                if (asset.AssetKey == assetKey)
                {
                    return asset.AssetName;
                }
            }
            return null;
        }

        public string GetUserNameById(string id)
        {
            throw new NotImplementedException();
        }

        public bool PlaceObjectAt(string assetKey, string placemarkerName)
        {
            Logging.UnityLogger.BroadcastMesage(this, "PlaceObjectAt placeMarker: " + placemarkerName);
            PlaceMarker[] placeMarkers = GameObject.FindObjectsOfType<PlaceMarker>();
            foreach (PlaceMarker placeMarker in placeMarkers)
            {
                if (placeMarker.PlaceMarkerName == placemarkerName)
                {
                    Vector3 pos = placeMarker.transform.position;
                    return PlaceObjectAt(assetKey, new Common.Math.Vector3(pos.x, pos.y, pos.z));
                }
            }
            return false;
        }

        public bool PlaceObjectAt(string assetKey, Common.Math.Vector3 pos)
        {
            Logging.UnityLogger.BroadcastMesage(this, "PlaceObjectAt position: " + pos.ToString());
            Asset[] assets = GameObject.FindObjectsOfType<Asset>();
            foreach (Asset asset in assets)
            {
                if (asset.AssetKey == assetKey)
                {
                    asset.transform.position = new Vector3(pos.X, pos.Y, pos.Z);
                    return true;
                }
            }
            return false;
        }

        public void ShowResponse(string response, bool inWorld)
        {
            throw new NotImplementedException();
        }
    }
}
