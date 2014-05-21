using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;
using System.Data;

namespace Veis.Data.Repositories
{
    public class MethodRepository : CommonRepository
    {
        public MethodRepository(IDataAccess dataAccess)
            : base(dataAccess)
        {
        }

        public const string SelectMethodQuery = "SELECT name FROM activity_methods";
        public const string SelectMethodParameterQuery = "SELECT method_name, variable FROM method_parameters";
        public const string SelectMethodPostconditionQuery = "SELECT method_name, predicate, variable, `state` FROM method_post_conditions";
        public const string SelectMethodPreconditionQuery = "SELECT method_name, predicate, variable FROM method_preconditions";

        public IEnumerable<ActivityMethod> FindMethods(params Specification<ActivityMethod>[] specifications)
        {
            return this.Select<ActivityMethod>(SelectMethodQuery, x => x, MethodConvert, specifications);
        }
        
        public IEnumerable<MethodParameter> FindMethodParameters(params Specification<MethodParameter>[] specifications)
        {
            return this.Select<MethodParameter>(SelectMethodParameterQuery, x => x, MethodParameterConvert, specifications);
        }

        public IEnumerable<MethodPostcondition> FindMethodPostconditions(params Specification<MethodPostcondition>[] specifications)
        {
            return this.Select<MethodPostcondition>(SelectMethodPostconditionQuery, x => x, MethodPostconditionConvert, specifications);
        }

        public IEnumerable<MethodPrecondition> FindMethodPreconditions(params Specification<MethodPrecondition>[] specifications)
        {
            return this.Select<MethodPrecondition>(SelectMethodPreconditionQuery, x => x, MethodPreconditionConvert, specifications);
        }

        public ActivityMethod MethodConvert(IDataReader reader)
        {
            return new ActivityMethod
            {
                Name = reader.GetString(0)
            };
        }

        public MethodParameter MethodParameterConvert(IDataReader reader)
        {
            return new MethodParameter
            {
                MethodName = reader.GetString(0),
                Variable = reader.GetString(1)
            };
        }

        public MethodPostcondition MethodPostconditionConvert(IDataReader reader)
        {
            return new MethodPostcondition
            {
                MethodName = reader.GetString(0),
                Predicate = reader.GetString(1),
                Variable = reader.GetString(2),
                State = reader.GetInt16(3)
            };
        }

        public MethodPrecondition MethodPreconditionConvert(IDataReader reader)
        {
            return new MethodPrecondition
            {
                MethodName = reader.GetString(0),
                Predicate = reader.GetString(1),
                Variable = reader.GetString(2)
            };
        }

        public class ByMethodNameSpecification<T> : Specification<T>
        {
            private string _methodName;

            public ByMethodNameSpecification(string methodName)
            {
                _methodName = methodName;
            }

            public override string Condition
            {
                get { return "`method_name`=@METHOD_NAME"; }
            }

            public override IDictionary<string, object> Parameters
            {
                get { return From("@METHOD_NAME", _methodName); }
            }
        }

        public class ByAssetSpecification : Specification<ActivityMethod>
        {
            private string _assetName;

            public ByAssetSpecification(string assetName)
            {
                _assetName = assetName;
            }

            public override string Condition
            {
	            get { return "`name` IN ( SELECT method_name FROM asset_methods WHERE asset_name=@ASSET_NAME)"; }
            }

            public override IDictionary<string,object>  Parameters
            {
	            get { return From("@ASSET_NAME", _assetName); }
            }

        }
    }
}
