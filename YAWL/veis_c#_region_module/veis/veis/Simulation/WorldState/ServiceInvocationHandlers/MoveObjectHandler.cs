using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;
using Veis.Services.Interfaces;

namespace Veis.Simulation.WorldState.ServiceInvocationHandlers
{
    /// <summary>
    /// This Service Invocation Handler handles the "Move" service call. SPECIFIC TO OPENSIM
    /// Moves the asset to the value specified in "value".
    /// This value can either be a named location (specified by an object) (both location and rotation are used)
    ///     or a vector value in the world. (No rotation).
    /// If the variable value specifies a component of the asset, that asset will be moved relative to the parent object
    /// </summary>
    public class MoveObjectHandler : IServiceInvocationHandler
    {
        private readonly ISceneService _sceneService;

        public MoveObjectHandler(ISceneService sceneService)
        {
            _sceneService = sceneService;
        }

        /// <summary>
        /// Service invocation is in the form "Move:<variable>=<location>"
        /// </summary>
        public bool CanHandle(string serviceRoutine)
        {
            return serviceRoutine.StartsWith("Move", StringComparison.OrdinalIgnoreCase);
        }

        public bool Handle(AssetServiceRoutine assetServiceRoutine)
        {
            _sceneService.AddServiceToHandle(assetServiceRoutine);
            return true;
        }

        /*public bool Handle(AssetServiceRoutine assetServiceRoutine)
        {
            // first check if its something other than the asset that needs to be moved. Will be after "Move", before ":", eg. Move goods:Truck to=Bay 05
            var movepart = assetServiceRoutine.ServiceRoutine.Split(':')[0];
            var assetKey =  string.Empty;
            var assetName = string.Empty;
            if (movepart.Length > "Move".Length)
            {
                assetName = movepart.Substring("Move".Length + 1);
                assetKey = _sceneService.GetAssetKey(assetName);
            }
            if (string.IsNullOrEmpty(assetKey))
            {                
                assetKey = assetServiceRoutine.AssetKey;
                assetName = _sceneService.GetAssetName(assetKey);
            }

            // Now process the location, including sub-location based on name. no underscores

            // Move:Bed to=Bay 1
            // Get the basic location. We want everything after the = sign
            var locationName = assetServiceRoutine.ServiceRoutine.Split('=').Last();

            var locationStrings = GetLocationSearchStrings(assetName, locationName);
            bool success = false;

            foreach (var locationString in locationStrings)
            {
                success = _sceneService.PlaceObjectAt(assetKey, locationString);
                if (success) return true;
            }

            return false;
        }*/

        /// <summary>
        /// Other formatted location search strings can be added in this function.
        //  The location object in the virtual world should have one of these formatted
        //  name strings. 
        /// </summary>
        private static List<string> GetLocationSearchStrings(string assetName, string locationName)
        {
            // Check for the location in this order: 
            // "Location <asset name> <location name>"
            // "Location <location name>"
            // "<location name>"

            const string fullLocation = "Location {0} {1}";
            const string justLocation = "Location {0}";

            var searchStrings = new List<string>
            {
                String.Format(fullLocation, assetName, locationName),
                String.Format(justLocation, locationName),
                locationName
            };

            return searchStrings;
        }

        public void Finalise()
        {
        }
    }
}
