using PI.Common;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        //private DbContext _dbContext;
        private GPIEntities _dbContext;
        private readonly IDbSet<T> dbset;

        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected Repository(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            dbset = DbContext.Set<T>();
        }

        public GPIEntities DbContext
        {
            get { return _dbContext ?? (_dbContext = DatabaseFactory.MakeDbContext()); }
        }

        public virtual T Add(T entity)
        {
            dbset.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            dbset.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual void Delete(T entity)
        {
            dbset.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbset.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
                dbset.Remove(obj);
        }

        public virtual T GetById(long id)
        {
            var model = dbset.Find(id);
            DbContext.Configuration.ProxyCreationEnabled = false;
            DbContext.Entry(model).State = EntityState.Detached;
            return model;
        }

        public virtual T GetById(string id)
        {
            return dbset.Find(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            DbContext.Configuration.ProxyCreationEnabled = false;
            return dbset.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            var list =  dbset.Where(where).ToList();
            DbContext.Configuration.ProxyCreationEnabled = false;
            /*if (list != null & list.Count>0)
                DbContext.Entry(list).State = EntityState.Detached;*/
            return list;
        }

        //pageNumber == 0 >> all records
        public virtual PagedList GetPage<TOrder>(int pageNumber, int pageSize, Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
        {
            var result = new PagedList();
            
            // temporarily here
            // fixes error: A circular reference was detected while serializing an object of type 
            DbContext.Configuration.ProxyCreationEnabled = false;

            result.TotalItems = (where != null) ?
                dbset.Where(where).Count()
                : dbset.Count();

            if (pageNumber == 0)
            {
                pageSize = result.TotalItems;
                pageNumber = 1;
            }
            
            var results = (where != null) ? 
                dbset.OrderBy(order).Where(where).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
                : dbset.OrderBy(order).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            result.DataSet = results;

            //result.TotalItems = results == null ? 0 : total;

            var pageCount = result.TotalItems > 0 ? (int)Math.Ceiling(result.TotalItems / (double)pageSize) : 0;

            result.HasPreviousPage = pageNumber > 1;
            result.HasNextPage = pageNumber < pageCount;
            result.IsFirstPage = pageNumber == 1;
            result.IsLastPage = pageNumber >= pageCount;

            return result;
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            var entity = dbset.Where(where).FirstOrDefault<T>();
            DbContext.Configuration.ProxyCreationEnabled = false;
            if (entity !=null)
                DbContext.Entry(entity).State = EntityState.Detached;
            return entity;
        }


        //private static string connectionString = System.Configuration.ConfigurationManager.AppSettings["GPIEntities"].ToString();
        private static string connectionString = @"data source= 192.168.1.9\SQLSERVER2014;initial catalog=GPIDEV;User ID=erpi;Password=Erp!;integrated security=false;";

        /// <summary>
        /// Dapper support
        /// </summary>
        /// <returns></returns>
        public static SqlConnection GetOpenConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
