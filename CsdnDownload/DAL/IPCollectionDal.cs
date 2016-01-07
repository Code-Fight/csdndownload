using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Entity;
using FTools.SQL.SqLite;

namespace DAL
{
    public class IpCollectionDal : SqLiteHelper
    {

        #region 增加数据

        public void Add(IpCollectionEntity info)
        {
            try
            {
                StringBuilder strSql = new StringBuilder();
//                strSql.Append("insert into t_ip_acquisition_rule (rule_name,rule_url,rule_rex)values(");
//                strSql.Append("'" + info.RuleName + "',");
//                strSql.Append("'" + info.RuleUrl + "',");
//                strSql.Append("'" + info.RuleRex + "')");

                strSql.Append(
                    "insert into t_ip_acquisition_rule (rule_name,rule_url,rule_rex)values(@name,@url,@rex)");
                SQLiteParameter[] parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@name", info.RuleName),
                    new SQLiteParameter("@url", info.RuleUrl),
                    new SQLiteParameter("@rex", info.RuleRex)
                };
                ExecuteNonQuery(strSql.ToString(), parameters);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion


        #region 获取所有数据

        public DataTable GetAll()
        {
            try
            {
                string str = "select * from t_ip_acquisition_rule";
                return ExecuteDataTable(str);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region 更新数据

        public void Update(IpCollectionEntity info)
        {
            try
            {
                string str = "update t_ip_acquisition_rule set rule_name=@name,rule_url=@url,rule_rex=@rex where id=@id";
                SQLiteParameter[] parameters = new SQLiteParameter[]
                {
                    new SQLiteParameter("@name", info.RuleName),
                    new SQLiteParameter("@url", info.RuleUrl),
                    new SQLiteParameter("@rex", info.RuleRex),
                    new SQLiteParameter("@id", info.Id)
                };
                ExecuteNonQuery(str, parameters);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region 删除一条数据

        public void Delete(int id)
        {
            try
            {
                string str = "delete from t_ip_acquisition_rule where id=@id";
                ExecuteNonQuery(str, new[] {new SQLiteParameter("@id", id)});
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

    }
}
