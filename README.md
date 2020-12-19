# Install dependencies : 
1 - install Dapper package using package manager console 
```bash
Install-Package Dapper -Version 2.0.78
```
2 - install db provider package : (in our project we are using sql server)
```bash
Install-Package System.Data.SqlClient -Version 4.8.2
```

# How it works : 
1 - create interface that takes generic parameter and name it IBaseRepo
```C#
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
``` 
this interface will include the common functions that might use in other repositories.

2 - Create class that implement IBaseRepo and name it BaseRepo
```C#
    public BaseRepo(IConnection db, string tablename, string primaryKey, bool isGuid = false)
    {
       _db = db;
       _tablename = tablename;
       _primaryKey = primaryKey;
       _isGuid = isGuid;
   }
```
3 - define the functions in BaseRepo calss : (will define one here and you can find the rest in the solution) \
```C#
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
                // add some logs 
                return null;
          }
    }
```
4 - create other repositores that inherate BaseRepo class : 
  4-1 create interface for user class and name it IUserRepo and inherate from IBaseRepo and the gerneric type will be user model:
  ```C#
    public interface IUserRepo : IBaseRepo<UserModel>
    {
    }
  ``` 
 4-2 create class that inherate BaseRepo class and implement IUserRepo interface and name it UserRepo:
 ```C#
    public class UserRepo : BaseRepo<UserModel>, IUserRepo
    {
        private IConnection _db;

        public UserRepo(IConnection db) : base(db, "Users", "ID", true)
        {
            _db = db;
        }
    }
 ``` 
note: we are passing table info like (table name , primary key name , if the pk is guid) from UserRepo class to BaseRepo

5 - create Users table: 
```sql
  CREATE TABLE Users
  (
	ID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	UserName NVARCHAR(50) NOT NULL,
	Email NVARCHAR(50) NOT NULL,
	Password NVARCHAR(200) NOT NULL
  )
```

6 - insert dummy data in Users table:
```sql
  INSERT INTO [Users] VALUES (newid() , 'test user 1' , 'user1@test.comm' , '123')
  INSERT INTO [Users] VALUES (newid() , 'test user 2' , 'user2@test.comm' , '321')
  INSERT INTO [Users] VALUES (newid() , 'test user 3' , 'user3@test.comm' , '213')
```

## How to start:
1 - clone the solution and restore all the dependencies. \
2 - change the connection string to you database credential in appsetting.json \
3 - create db models in your solution since dapper is micro orm and can't auto create it for you like EF do.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.



## License
[MIT](https://choosealicense.com/licenses/mit/)
 
