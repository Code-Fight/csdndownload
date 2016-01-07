using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    /// <summary>
    /// ip采集实体
    /// </summary>
    public class IpCollectionEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// 采集规则名称
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// 采集地址
        /// </summary>
        public string RuleUrl { get; set; }
        /// <summary>
        /// 采集正则
        /// </summary>
        public string RuleRex { get; set; }
    }
}
