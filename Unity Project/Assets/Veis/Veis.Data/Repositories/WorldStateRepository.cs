using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;
using System.Data;

namespace Veis.Data.Repositories
{
    public class WorldStateRepository : Repository<WorldState>
    {
        private const string SelectQuery = "SELECT asset_name, predicate_label, value, world_key FROM world_states";

        public WorldStateRepository(IDataAccess dataAccess) : base(dataAccess) { }

        public override IEnumerable<WorldState> Find(params Specification<WorldState>[] specifications)
        {
            return Select(SelectQuery, x => x, Convert, specifications);
        }

        private WorldState Convert(IDataReader reader)
        {
            return new WorldState
            {
                AssetName = reader.GetString(0),
                PredicateLabel = reader.GetString(1),
                Value = reader.GetString(2),
                WorldKey = reader.GetInt32(3),
            };
        }

        private const string UpdateQuery
            = "UPDATE world_states SET value = @VALUE "
            + "WHERE asset_name = @ASSET_NAME "
            + "AND predicate_label = @PREDICATE_LABEL";

        public int Update(string assetName, string predicate, string value)
        {
            UpdateSpecification spec = new UpdateSpecification(
                assetName, predicate, value);
            return Update(UpdateQuery, null, spec);
        }

        public void ResetAssetWorldStates()
        {
            ExecuteProcedure("reset_world_state");
        }

        #region Specifications

        public class ByAssetSpecification : Specification<WorldState>
        {
            private string _assetName;

            public ByAssetSpecification(string assetName)
            {
                _assetName = assetName;
            }

            public override string Condition
            {
                get { return "asset_name=@ASSET_NAME"; }
            }

            public override IDictionary<string, object> Parameters
            {
                get { return From("@ASSET_NAME", _assetName); }
            }
        }

        public class UpdateSpecification : Specification<WorldState>
        {
            private string _assetName;
            private string _predicate;
            private string _value;

            public UpdateSpecification(string assetName, string predicate, string value)
            {
                _assetName = assetName;
                _predicate = predicate;
                _value = value;
            }

            public override string Condition
            {
                get { return string.Empty; }
            }

            public override IDictionary<string, object> Parameters
            {
                get
                {
                    return new Dictionary<string, object>()
                    {
                        { "@ASSET_NAME", _assetName },
                        { "@PREDICATE_LABEL", _predicate },
                        { "@VALUE", _value }
                    };
                }
            }
        }

        #endregion

        #region Unused

        public override int Insert(WorldState item)
        {
            throw new NotImplementedException();
        }

        public override int Insert(IEnumerable<WorldState> items)
        {
            throw new NotImplementedException();
        }

        public override int Delete(WorldState item)
        {
            throw new NotImplementedException();
        }

        public override int Delete(IEnumerable<WorldState> items)
        {
            throw new NotImplementedException();
        }

        public override int Update(WorldState oldItem, WorldState newItem, params Specification<WorldState>[] specifications)
        {
            throw new NotImplementedException();
        }

        public override int Update(IDictionary<WorldState, WorldState> updateMap, params Specification<WorldState>[] specifications)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
