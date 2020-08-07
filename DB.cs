using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static DSQLDM.DBExceprions;

namespace DSQLDM
{

    public class DBExceprions
    {
        public class BindPropertyNotFoundException : Exception
        {
            public string Proparty { get; set; }
            public BindPropertyNotFoundException(string proparty)
            {
                this.Proparty = Proparty;
            }
        }
    }

    public class DB<T> where T : new()
    {
        public string ConnectionString { get; set; }
        public DB(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }

        public async Task<List<T>> Query(string SQL, object Params)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                IEnumerable<dynamic> result = await connection.QueryAsync<dynamic>(SQL, Params);
                List<T> re = result.Select(
                    item => (T)BindQueryResult(new T(), item)).ToList();
                return re;
            }
        }

        public async Task<T> QuerySingle(string SQL, object Params)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                IEnumerable<dynamic> result = await connection.QueryAsync<dynamic>(SQL, Params);
                List<T> re = result.Select(
                    item => (T)BindQueryResult(new T(), (IDictionary<string, object>)item)).ToList();
                return re.FirstOrDefault();
            }
        }

        /// <summary>
        /// Map the selected fields to a domain model. Each field should be named/renamed using "AS" keyword to match the model's property.
        /// Example :-
        /// Model1 { Id, Name}
        /// Query1 : SELECT UserId as Id, Name From Users;
        /// 
        /// Model2 {Age, Model1}
        /// Query2 : SELECT UserId as [Model1.Id], [Model1.Name], Age From Profile JOIN Users ON Profile.UserId = Users.UserID;
        /// </summary>
        /// <param name="DBModel">Target domain model to bind the result set to</param>
        /// <param name="item">SQL result set row</param>
        private U BindQueryResult<U>(U DBModel, IDictionary<string, object> item)
        {
            if (typeof(IDictionary).IsAssignableFrom(DBModel.GetType()))
            {
                foreach (var i in item.Keys)
                {
                    ((dynamic)DBModel).Add(i, item[i]);
                }
                return DBModel;
            }
            PropertyInfo[] properties = DBModel.GetType().GetProperties();
            foreach (string key in item.Keys)
            {
                string[] tokens = key.Split(".");
                dynamic property = properties.Where(e => e.Name == tokens[0]).FirstOrDefault();
                if (property == null)
                {
                    throw new BindPropertyNotFoundException(tokens[0]);
                }
                if (tokens.Count() > 1 && tokens[0] != DBModel.GetType().Name)
                {
                    dynamic instance = property.GetValue(DBModel);
                    if (instance == null)
                    {
                        instance = Activator.CreateInstance(property.PropertyType);
                    }
                    instance = BindQueryResult(instance, new Dictionary<string, object> { { string.Join(".", tokens.Skip(1)), item[key] } });
                    property.SetValue(DBModel, instance);
                }
                else
                {
                    var val = item[key];
                    if (val != null)
                    {
                        property.SetValue(DBModel, Convert.ChangeType(val, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType));
                    }
                }
            }
            return DBModel;
        }
    }
}
