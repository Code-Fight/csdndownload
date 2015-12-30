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
            IPCollectionBll bll=new IPCollectionBll();
            bll.AddTest();
        }


        public void GetEmail()
        {
            CookieContainer cc = new CookieContainer();
            EmailBll bll = new EmailBll(cc);
            bll.FirstQuest();
        }
    }
}
