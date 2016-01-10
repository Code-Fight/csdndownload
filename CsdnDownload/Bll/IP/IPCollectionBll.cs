using System.Data;
using DAL;
using Entity;

namespace Bll.IP
{
    public class IpCollectionBll
    {

        #region DBA 

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

        #endregion

    }
}
