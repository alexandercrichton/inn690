using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veis.Data.Entities;
using System.Data;

namespace Veis.Data.Repositories
{
    public class AccessRecordRepository : Repository<AccessRecord>
    {
        private const string SelectQuery = "SELECT world_key, last_updated FROM access_records;";

        public AccessRecordRepository(IDataAccess dataAccess) : base(dataAccess) { }
            
        public override IEnumerable<AccessRecord> Find(params Specification<AccessRecord>[] specifications)
        {
            Logging.Logger.BroadcastMessage(this, "Find()");
            return Select(SelectQuery, x => x, Convert, specifications);
        }

        private AccessRecord Convert(IDataReader reader)
        {
            return new AccessRecord
            {
                WorldKey = reader.GetInt32(0),
                LastUpdated = reader.GetDateTime(1)
            };
        }

        #region Unused

        public override int Insert(AccessRecord item)
        {
            throw new NotImplementedException();
        }

        public override int Insert(IEnumerable<AccessRecord> items)
        {
            throw new NotImplementedException();
        }

        public override int Delete(AccessRecord item)
        {
            throw new NotImplementedException();
        }

        public override int Delete(IEnumerable<AccessRecord> items)
        {
            throw new NotImplementedException();
        }

        public override int Update(AccessRecord oldItem, AccessRecord newItem, params Specification<AccessRecord>[] specifications)
        {
            throw new NotImplementedException();
        }

        public override int Update(IDictionary<AccessRecord, AccessRecord> updateMap, params Specification<AccessRecord>[] specifications)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
