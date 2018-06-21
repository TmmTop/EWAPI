using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace EWAPI.Tool
{
    public  class DBDapper
    {
      
        //初始化连接对象
        public SqlConnection conn = null;
        public  DBDapper(IHostingEnvironment environment)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(environment.ContentRootPath).AddJsonFile("appsettings.json");
            var config = builder.Build();
            var sqlconnct = config["ConnectionStrings:conn"].ToString();
            conn = new SqlConnection(sqlconnct);
        }
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        private void OpenConnect()
        {
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        private void CloseConnect()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
        /// <summary>
        /// 查询语句，必要形参sql语句或存储过程名称，后面参数用于扩展可以不写，若两边有参数中间用null占位
        /// </summary>
        /// <typeparam name="T">强类型的类</typeparam>
        /// <param name="sql">sql执行语句或存储过程名称</param>
        /// <param name="parameter">sql参数，可匿名类型，可对象类型</param>
        /// <param name="transaction">执行事务</param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>对象集合</returns>
        public IEnumerable<T> GetInfoList<T>(string sql, object parameter = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            try
            {
                OpenConnect();
                //可以让结果转换成其他集合形式 例：list、array等集合，方法： ToList<>、ToArray<>
                IEnumerable<T> result = conn.Query<T>(sql, parameter, transaction, buffered, commandTimeout, commandType);
                CloseConnect();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 插入、更新或删除语句，必要形参sql语句或存储过程名称，后面参数用于扩展可以不写，若两边有参数中间用null占位
        /// </summary>
        /// <typeparam name="T">强类型的类</typeparam>
        /// <param name="sql">sql执行语句或存储过程名称</param>
        /// <param name="parameter">sql参数，可匿名类型，可对象类型</param>
        /// <param name="transaction">执行事务</param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>成功：true;失败：false</returns>
        public bool UpdateSql(string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            try
            {
                OpenConnect();
                int result = conn.Execute(sql, parameter, transaction, commandTimeout, commandType);
                CloseConnect();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 插入数据库返回新增id
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameter"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int Insert(string sql, object parameter = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            try
            {
                OpenConnect();
                sql += "SELECT CAST(SCOPE_IDENTITY() as int)";
                int result = Convert.ToInt32(conn.ExecuteScalar(sql, parameter, transaction, commandTimeout, commandType));

                CloseConnect();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 根据条件获取数据库表中列表数量,必要形参sql,后面参数用于扩展可以不写，若两边有参数中间用null占位
        /// </summary>
        /// <param name="sql">sql执行语句或存储过程名称</param>
        /// <param name="parameter">sql参数，可匿名类型，可对象类型</param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int GetInfoCounts(string sql, object parameter = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            try
            {
                OpenConnect();
                //注意：sql语句应该是这种形式 select count(*) as rows from table
                int result = conn.Query<int>(sql, parameter, transaction, buffered, commandTimeout, commandType).First();
                CloseConnect();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
