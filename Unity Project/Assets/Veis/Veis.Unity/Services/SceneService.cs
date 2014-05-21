using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Services.Interfaces;
using Veis.Unity.Bots;
using UnityEngine;
using Veis.Data.Entities;

using System.ComponentModel;
using System.Threading;

namespace Veis.Unity.Scene
{
    public class SceneService : ISceneService
    {
        protected Queue<AssetServiceRoutine> assetServiceRoutinesToHandle;

        public SceneService()
        {
            assetServiceRoutinesToHandle = new Queue<AssetServiceRoutine>();
        }

        public void AddAssetServiceRoutineToHandle(AssetServiceRoutine assetServiceRoutine)
        {
            assetServiceRoutinesToHandle.Enqueue(assetServiceRoutine);
        }

        public void HandleAssetServiceRoutines()
        {
            while (assetServiceRoutinesToHandle.Count > 0)
            {
                AssetServiceRoutine assetServiceRoutine = assetServiceRoutinesToHandle.Dequeue();
                HandleMoveAsset(assetServiceRoutine);
                Veis.Unity.Logging.UnityLogger.BroadcastMesage(this, "Handled routine");
            }
        }

        protected bool HandleMoveAsset(AssetServiceRoutine assetServiceRoutine)
        {
            // first check if its something other than the asset that needs to be moved. Will be after "Move", before ":", eg. Move goods:Truck to=Bay 05
            var movepart = assetServiceRoutine.ServiceRoutine.Split(':')[0];
            var assetKey = string.Empty;
            var assetName = string.Empty;
            if (movepart.Length > "Move".Length)
            {
                assetName = movepart.Substring("Move".Length + 1);
                assetKey = GetAssetKey(assetName);
            }
            if (string.IsNullOrEmpty(assetKey))
            {
                assetKey = assetServiceRoutine.AssetKey;
                assetName = GetAssetName(assetKey);
            }

            // Now process the location, including sub-location based on name. no underscores

            // Move:Bed to=Bay 1
            // Get the basic location. We want everything after the = sign
            var locationName = assetServiceRoutine.ServiceRoutine.Split('=').Last();

            // Check for the location in this order: 
            // "Location <asset name> <location name>"
            // "Location <location name>"
            // "<location name>"
            var locationStrings = new List<string> 
            {
                string.Format("Location {0} {1}", assetName, locationName),
                string.Format("Location {0}", locationName),
                locationName
            };
            bool success = false;

            foreach (var locationString in locationStrings)
            {
                success = PlaceObjectAt(assetKey, locationString);
                if (success) return true;
            }

            return false;
        }

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
