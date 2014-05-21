using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veis.Data
{
    // TODO: Find how this was implemented in one of your projects.
    public interface IRepository<T>
    {
        IEnumerable<T> Find(params Specification<T>[] specifications);

       // Should be fine... 
        int Insert(T item);
        int Insert(IEnumerable<T> items);
       
       // Also uuuhh..
        int Delete(T item);
        int Delete(IEnumerable<T> items);
        
       // Uuuh... 
        int Update(T oldItem, T newItem, params Specification<T>[] specifications);
        int Update(IDictionary<T, T> updateMap, params Specification<T>[] specifications);

    }
}
