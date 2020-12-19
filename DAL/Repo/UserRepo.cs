using DAL.BaseRepo;
using DAL.DbConnection;
using DAL.IRepo;
using DAL.Model;

namespace DAL.Repo
{
    public class UserRepo : BaseRepo<UserModel>, IUserRepo
    {
        private IConnection _db;
        public UserRepo(IConnection db) : base(db, "Users", "ID", true)
        {
            _db = db;
        }
    }
}
