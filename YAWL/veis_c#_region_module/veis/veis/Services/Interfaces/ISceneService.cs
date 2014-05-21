using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Common.Math;
using Veis.Data.Entities;

namespace Veis.Services.Interfaces
{
    public interface ISceneService
    {
        void AddAssetServiceRoutineToHandle(AssetServiceRoutine assetServiceRoutine);

        Vector3 GetPositionOfObject(string name);

        bool PlaceObjectAt(string assetKey, string placemarkerName);

        bool PlaceObjectAt(string assetKey, Veis.Common.Math.Vector3 pos);

        string GetAssetKey(string name);

        string GetAssetName(string assetKey);

        string GetUserNameById(string id);

        void ShowResponse(string response, bool inWorld);
    }
}
