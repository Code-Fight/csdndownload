using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using FTools.Log;
using FTools.SQL.SqLite;

namespace DAL
{
    public class CsdnUserDal : SqLiteHelper
    {

        #region 增加数据
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <param name="email"></param>
        public void AddUser(string name, string pwd, string email)
        {
            try
            {
                string strSql = "insert into t_csdn_user(username,userpwd,useremail)values(@name,@pwd,@email)";
                SQLiteParameter[] parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@name", name),
                    new SQLiteParameter("@pwd", pwd),
                    new SQLiteParameter("@email", email)
                };
                ExecuteNonQuery(strSql, parameters);
            }
            catch (Exception e)
            {
                Log.WriteLog(this.GetType(), e.Message);
                throw;
            }
        }

        #endregion

    }
}
