using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;

namespace Veis.Data.Services
{
    public interface IActivityMethodService
    {
        IList<ActivityMethod> GetMethodsByAsset(string assetName);
    }
}