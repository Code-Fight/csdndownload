using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DAL;
using Entity;

namespace Bll
{
    public class IpCollectionBll
    {
        public string GetIp()
        {
            return string.Empty;
        }


        public void Add(IpCollectionEntity info)
        {
            IpCollectionDal dal = new IpCollectionDal();
            dal.Add(info);            
        }

        public DataTable GetAll()
        {
            IpCollectionDal dal = new IpCollectionDal();
            return dal.GetAll();
        }

         public void Update(IpCollectionEntity info)
        {
            IpCollectionDal dal = new IpCollectionDal();
             dal.Update(info);
        }

         public void Delete(int id)
         {
             IpCollectionDal dal = new IpCollectionDal();
             dal.Delete(id);
         }
    }
}
