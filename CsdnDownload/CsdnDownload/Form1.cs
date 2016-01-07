using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using Bll;
using Entity;
using FTools.Log;

namespace CsdnDownload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // GetEmail();
            //Init();
            
            Log.WriteLog(typeof(Form1),"测试");

        }

        /// <summary>
        /// 程序初始化
        /// </summary>
        private void Init()
        {
            Init_Ip_Rule();
        }

        private void Init_Ip_Rule()
        {
            IpCollectionBll bll = new IpCollectionBll();
            DataTable dt= bll.GetAll();
            dgv_ip_rule.DataSource = dt;
          
        }

        public void GetEmail()
        {
            CookieContainer cc = new CookieContainer();
            EmailBll bll = new EmailBll(cc);
            bll.FirstQuest();
        }

        private void dgv_ip_rule_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex<0)
            {
                return;
            }
            txt_ip_rule_name.Text = dgv_ip_rule.Rows[e.RowIndex].Cells[1].Value.ToString();
            txt_ip_rule_url.Text = dgv_ip_rule.Rows[e.RowIndex].Cells[2].Value.ToString();
            rtxt_ip_rule_rex.Text = dgv_ip_rule.Rows[e.RowIndex].Cells[3].Value.ToString();
            lb_ip_rule_id.Text = dgv_ip_rule.Rows[e.RowIndex].Cells[0].Value.ToString(); 
        }

        #region ip采集规则编辑

        private void btn_add_ip_rule_Click(object sender, EventArgs e)
        {
            IpCollectionBll bll = new IpCollectionBll();
            IpCollectionEntity info = new IpCollectionEntity()
            {
                RuleName = txt_ip_rule_name.Text,
                RuleUrl = txt_ip_rule_url.Text,
                RuleRex = rtxt_ip_rule_rex.Text
            };
            bll.Add(info);
            Init_Ip_Rule();
        }

        private void btn_edit_ip_rule_Click(object sender, EventArgs e)
        {
            IpCollectionEntity info = new IpCollectionEntity()
            {
                RuleName = txt_ip_rule_name.Text,
                RuleUrl = txt_ip_rule_url.Text,
                RuleRex = rtxt_ip_rule_rex.Text,
                Id = int.Parse(lb_ip_rule_id.Text)
            };
            IpCollectionBll bll = new IpCollectionBll();
            bll.Update(info);
            Init_Ip_Rule();
        }

        private void btn_del_ip_rule_Click(object sender, EventArgs e)
        {
            IpCollectionBll bll = new IpCollectionBll();
            bll.Delete(int.Parse(lb_ip_rule_id.Text));
            Init_Ip_Rule();
        }

        #endregion


        
    }
}
