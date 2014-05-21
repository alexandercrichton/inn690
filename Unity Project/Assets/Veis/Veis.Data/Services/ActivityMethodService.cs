using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;
using Veis.Data.Repositories;

namespace Veis.Data.Services
{
    public class ActivityMethodService : IActivityMethodService
    {
        private readonly MethodRepository _methodRepos;

        public ActivityMethodService(MethodRepository methodRepos)
        {
            _methodRepos = methodRepos;
        }
        
        public IList<ActivityMethod> GetMethodsByAsset(string assetName)
        {
            var methods = _methodRepos.FindMethods(new MethodRepository.ByAssetSpecification(assetName)).ToList();
            foreach (var method in methods)
            {
                var methodParameters = _methodRepos.FindMethodParameters(
                    new MethodRepository.ByMethodNameSpecification<MethodParameter>(method.Name)).ToList();
                var methodPostConditions = _methodRepos.FindMethodPostconditions(
                    new MethodRepository.ByMethodNameSpecification<MethodPostcondition>(method.Name)).ToList();
                var methodPreConditions = _methodRepos.FindMethodPreconditions(
                    new MethodRepository.ByMethodNameSpecification<MethodPrecondition>(method.Name)).ToList();
                method.Parameters = methodParameters;
                method.Postconditions = methodPostConditions;
                method.Preconditions = methodPreConditions;
            }
            return methods;
        }
    }
}
