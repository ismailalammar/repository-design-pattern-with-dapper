using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.BaseRepo
{
    public interface IBaseRepo<T>
    {
        Task<T> GetById(dynamic Id);
        Task<List<T>> GetAll();
        Task<T> Insert(T param);
        Task<T> Update(T param);
        Task DeleteById(dynamic Id);
        Task<List<T>> InsertBulk(List<T> parameters);
        Task<List<T>> UpdateBulk(List<T> parameters);
    }
}
