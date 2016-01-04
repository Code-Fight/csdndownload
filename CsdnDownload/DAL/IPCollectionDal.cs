using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FTools.SQL;
using FTools.SQL.SqLite;

namespace DAL
{
    public class IPCollectionDal : SqLiteHelper
    {
        public void AddTest()
        {
            const string str = "insert into t_ip_acquisition_rule (rule_name,rule_url,rule_rex)values('测试name','测试url','测试rex')";
            ExecuteNonQuery(str);
        }
    }
}
