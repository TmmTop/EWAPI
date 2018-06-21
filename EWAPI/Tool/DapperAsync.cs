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
    public  class DapperAsync
    {
        //初始化连接对象
        public SqlConnection conn = null;
        public DapperAsync(IHostingEnvironment environment)
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
        /// 查询集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> GetList<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            try
            {
                OpenConnect();
                Task<IEnumerable<T>> result = SqlMapper.QueryAsync<T>(conn, sql, param, transaction, commandTimeout, commandType);
                if (result.IsCompletedSuccessfully == true)//查询数据线程完成后关闭数据连接
                {
                    CloseConnect();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
