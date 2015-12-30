using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;

namespace Bll
{
    public class IPCollectionBll
    {
        public string GetIp()
        {
            return string.Empty;
        }

        public void AddTest()
        {
            IPCollectionDal dal=new IPCollectionDal();
            dal.AddTest();
        }
    }
}
