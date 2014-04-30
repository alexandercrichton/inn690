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
            Logging.Logger.BroadcastMessage(this, "Convert()");
            return new WorldState
            {
                AssetName = reader.GetString(0),
                PredicateLabel = reader.GetString(1),
                Value = reader.GetString(2),
                WorldKey = reader.GetInt32(3),
            };
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
