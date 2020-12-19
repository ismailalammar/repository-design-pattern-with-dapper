using DAL.DbConnection;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.BaseRepo
{
    public class BaseRepo<T> : IBaseRepo<T> where T : class
    {
        private readonly IConnection _db;
        private string _tablename;
        private string _primaryKey;
        private bool _isGuid;

        public BaseRepo(IConnection db, string tablename, string primaryKey, bool isGuid = false)
        {
            _db = db;
            _tablename = tablename;
            _primaryKey = primaryKey;
            _isGuid = isGuid;
        }


        public async Task<T> GetById(dynamic Id)
        {
            try
            {
                string query = $"SELECT * FROM {_tablename} WHERE {_primaryKey} = @Id";

                return await _db.LContext.QueryFirstOrDefaultAsync<T>(query, new { Id = Id });
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<List<T>> GetAll()
        {
            try
            {
                string query = $"SELECT * FROM {_tablename}";

                var result = await _db.LContext.QueryAsync<T>(query);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public async Task<T> Insert(T param)
        {
            T result;
            try
            {
                if (_db.LContext.State == ConnectionState.Closed)
                    _db.LContext.Open();

                int skip = 0;

                if (!_isGuid)
                    skip = 1;
                else
                    param.GetType().GetProperty(_primaryKey).SetValue(param, Guid.NewGuid());

                string properties = String.Join(",", typeof(T).GetProperties().Select(p => p.Name).Skip(skip));
                string paramaetrs = String.Join(",", typeof(T).GetProperties().Select(p => "@" + p.Name).Skip(skip));



                string query = $"INSERT INTO {_tablename} ({properties}) VALUES ({paramaetrs})";

                if (!_isGuid)
                    query += "; SELECT CAST(SCOPE_IDENTITY() as int)";

                using var tran = _db.LContext.BeginTransaction();
                try
                {
                    result = await _db.LContext.QueryFirstOrDefaultAsync<T>(query, param, transaction: tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_db.LContext.State == ConnectionState.Open)
                    _db.LContext.Close();
            }

            return param;
        }

        public async Task<T> Update(T param)
        {
            T result;
            try
            {
                if (_db.LContext.State == ConnectionState.Closed)
                    _db.LContext.Open();


                string props = String.Join(",", typeof(T).GetProperties().Select(p => p.Name + "=@" + p.Name).Skip(1));

                var Id = param.GetType().GetProperty(_primaryKey).GetValue(param);

                string query = "";

                if (_isGuid)
                    query = $"UPDATE {_tablename} SET {props} WHERE {_primaryKey} = '{Id}'";
                else
                    query = $"UPDATE {_tablename} SET {props} WHERE {_primaryKey} = {Id}";


                using var tran = _db.LContext.BeginTransaction();
                try
                {
                    result = await _db.LContext.QueryFirstOrDefaultAsync<T>(query, param, transaction: tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (_db.LContext.State == ConnectionState.Open)
                    _db.LContext.Close();
            }

            return param;
        }

        public async Task DeleteById(dynamic Id)
        {
            T result;
            try
            {
                if (_db.LContext.State == ConnectionState.Closed)
                    _db.LContext.Open();


                string query = $"DELETE FROM {_tablename} WHERE {_primaryKey} = @Id";


                using var tran = _db.LContext.BeginTransaction();
                try
                {
                    await _db.LContext.QueryFirstOrDefaultAsync<T>(query, new { Id = Id }, transaction: tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_db.LContext.State == ConnectionState.Open)
                    _db.LContext.Close();
            }
        }


        public async Task<List<T>> InsertBulk(List<T> parameters)
        {
            try
            {
                if (_db.LContext.State == ConnectionState.Closed)
                    _db.LContext.Open();

                int skip = 0;

                if (!_isGuid)
                    skip = 1;
                else
                {
                    foreach (var param in parameters)
                        param.GetType().GetProperty(_primaryKey).SetValue(param, Guid.NewGuid());
                }


                string properties = String.Join(",", typeof(T).GetProperties().Select(p => p.Name).Skip(skip));
                string paramaetrs = String.Join(",", typeof(T).GetProperties().Select(p => "@" + p.Name).Skip(skip));


                string query = $"INSERT {_tablename} ({properties}) VALUES ({paramaetrs})";


                using var tran = _db.LContext.BeginTransaction();
                try
                {
                    await _db.LContext.ExecuteAsync(query, parameters, transaction: tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_db.LContext.State == ConnectionState.Open)
                    _db.LContext.Close();
            }
            return parameters;
        }

        public async Task<List<T>> UpdateBulk(List<T> parameters)
        {
            try
            {
                if (_db.LContext.State == ConnectionState.Closed)
                    _db.LContext.Open();


                string props = String.Join(",", typeof(T).GetProperties().Select(p => p.Name + "=@" + p.Name).Skip(1));


                string query = $"UPDATE {_tablename} SET {props} WHERE {_primaryKey} = @{_primaryKey}";


                using var tran = _db.LContext.BeginTransaction();
                try
                {
                    await _db.LContext.ExecuteAsync(query, parameters, transaction: tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_db.LContext.State == ConnectionState.Open)
                    _db.LContext.Close();
            }
            return parameters;
        }
    }
}
