using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;
using System.Data;
namespace Veis.Data.Repositories
{
    public class AssetServiceRoutineRepository : Repository<AssetServiceRoutine>
    {
        public const string SelectQuery = "SELECT `key`, priority, asset_key, service_routine " +
            "FROM asset_service_routines";

        public AssetServiceRoutineRepository(IDataAccess dataAccess) : base(dataAccess) { }
        
        public override IEnumerable<AssetServiceRoutine> Find(params Specification<AssetServiceRoutine>[] specifications)
        {
            return Select(SelectQuery, x => x, Convert, specifications);
        }

        private AssetServiceRoutine Convert(IDataReader reader)
        {
            return new AssetServiceRoutine
            {
                Id = reader.GetInt32(0),
                Priority = reader.GetInt32(1),
                AssetKey = reader.GetString(2),
                ServiceRoutine = reader.GetString(3)
            };
        }

        public const string DeleteQuery = "DELETE FROM asset_service_routines";

        public override int Delete(AssetServiceRoutine item)
        {
            return Delete(DeleteQuery, new ByIdSpecification(item.Id));
        }

        public override int Delete(IEnumerable<AssetServiceRoutine> items)
        {
            int total = 0;
            foreach (var item in items)
            {
                total += Delete(item);
            }
            return total;
        }

        #region Specifications

        public class ByIdSpecification : Specification<AssetServiceRoutine>
        {
            private int _id;

            public ByIdSpecification(int id)
            {
                _id = id;
            }
            
            public override string Condition
            {
                get { return "`key`=@ROUTINE_ID"; }
            }

            public override IDictionary<string, object> Parameters
            {
                get { return From("@ROUTINE_ID", _id); }
            }
        }

        public class ByWorldSpecification : Specification<AssetServiceRoutine>
        {
            private int _worldKey;

            public ByWorldSpecification(int worldKey)
            {
                _worldKey = worldKey;
            }

            public override string Condition
            {
                get { return "world_key=@WORLD_KEY"; }
            }

            public override IDictionary<string, object> Parameters
            {
                get { return From("@WORLD_KEY", _worldKey); }
            }

        }
        
        #endregion

        #region Unused

        public override int Insert(AssetServiceRoutine item)
        {
            throw new NotImplementedException();
        }

        public override int Insert(IEnumerable<AssetServiceRoutine> items)
        {
            throw new NotImplementedException();
        }

        public override int Update(AssetServiceRoutine oldItem, AssetServiceRoutine newItem, params Specification<AssetServiceRoutine>[] specifications)
        {
            throw new NotImplementedException();
        }

        public override int Update(IDictionary<AssetServiceRoutine, AssetServiceRoutine> updateMap, params Specification<AssetServiceRoutine>[] specifications)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
