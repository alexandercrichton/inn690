using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Services.Interfaces;

namespace Veis.Tests
{
    public class TestSceneService : ISceneService
    {
        public Common.Math.Vector3 GetPositionOfObject(string name)
        {
            if (name.EndsWith("chair")) return null;
            return new Common.Math.Vector3(0, 0, 0);
        }

        public bool PlaceObjectAt(string assetKey, string placemarkerName)
        {
            return true;
        }

        public bool PlaceObjectAt(string assetKey, Common.Math.Vector3 pos)
        {
            return true;
        }


        public string GetAssetKey(string name)
        {
            throw new NotImplementedException();
        }

        public string GetAssetName(string assetKey)
        {
            throw new NotImplementedException();
        }

        public string GetUserNameById(string id)
        {
            return String.Empty;
        }

        public void ShowResponse(string response, bool inWorld)
        {
            // DO nothing ?
        }
    }
}
