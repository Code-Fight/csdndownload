namespace CsdnDownload
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_ip_rule_id = new System.Windows.Forms.Label();
            this.btn_del_ip_rule = new System.Windows.Forms.Button();
            this.btn_edit_ip_rule = new System.Windows.Forms.Button();
            this.btn_add_ip_rule = new System.Windows.Forms.Button();
            this.rtxt_ip_rule_rex = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_ip_rule_url = new System.Windows.Forms.TextBox();
            this.txt_ip_rule_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lb1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgv_ip_rule = new System.Windows.Forms.DataGridView();
            this.rule_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rule_url = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rule_rex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgv_ip_list = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_ip_check_time = new System.Windows.Forms.TextBox();
            this.txt_ip_coll_time = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btn_ip_start = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ip_rule)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ip_list)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(1, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(766, 435);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(758, 408);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(758, 408);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tabControl2);
            this.tabPage3.ImageIndex = 0;
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(758, 408);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "代理IP";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.ImageList = this.imageList1;
            this.tabControl2.Location = new System.Drawing.Point(-2, 6);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(764, 406);
            this.tabControl2.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox5);
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Controls.Add(this.groupBox3);
            this.tabPage4.ImageIndex = 2;
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(756, 379);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "工作";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.groupBox1);
            this.tabPage5.Controls.Add(this.groupBox2);
            this.tabPage5.ImageIndex = 1;
            this.tabPage5.Location = new System.Drawing.Point(4, 23);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(756, 379);
            this.tabPage5.TabIndex = 1;
            this.tabPage5.Text = "IP采集规则";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_ip_rule_id);
            this.groupBox1.Controls.Add(this.btn_del_ip_rule);
            this.groupBox1.Controls.Add(this.btn_edit_ip_rule);
            this.groupBox1.Controls.Add(this.btn_add_ip_rule);
            this.groupBox1.Controls.Add(this.rtxt_ip_rule_rex);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txt_ip_rule_url);
            this.groupBox1.Controls.Add(this.txt_ip_rule_name);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lb1);
            this.groupBox1.Location = new System.Drawing.Point(3, 290);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(750, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "操作区";
            // 
            // lb_ip_rule_id
            // 
            this.lb_ip_rule_id.AutoSize = true;
            this.lb_ip_rule_id.Location = new System.Drawing.Point(319, 63);
            this.lb_ip_rule_id.Name = "lb_ip_rule_id";
            this.lb_ip_rule_id.Size = new System.Drawing.Size(0, 12);
            this.lb_ip_rule_id.TabIndex = 9;
            this.lb_ip_rule_id.Visible = false;
            // 
            // btn_del_ip_rule
            // 
            this.btn_del_ip_rule.Location = new System.Drawing.Point(670, 63);
            this.btn_del_ip_rule.Name = "btn_del_ip_rule";
            this.btn_del_ip_rule.Size = new System.Drawing.Size(75, 23);
            this.btn_del_ip_rule.TabIndex = 8;
            this.btn_del_ip_rule.Text = "删除";
            this.btn_del_ip_rule.UseVisualStyleBackColor = true;
            this.btn_del_ip_rule.Click += new System.EventHandler(this.btn_del_ip_rule_Click);
            // 
            // btn_edit_ip_rule
            // 
            this.btn_edit_ip_rule.Location = new System.Drawing.Point(670, 37);
            this.btn_edit_ip_rule.Name = "btn_edit_ip_rule";
            this.btn_edit_ip_rule.Size = new System.Drawing.Size(75, 23);
            this.btn_edit_ip_rule.TabIndex = 7;
            this.btn_edit_ip_rule.Text = "修改";
            this.btn_edit_ip_rule.UseVisualStyleBackColor = true;
            this.btn_edit_ip_rule.Click += new System.EventHandler(this.btn_edit_ip_rule_Click);
            // 
            // btn_add_ip_rule
            // 
            this.btn_add_ip_rule.Location = new System.Drawing.Point(670, 11);
            this.btn_add_ip_rule.Name = "btn_add_ip_rule";
            this.btn_add_ip_rule.Size = new System.Drawing.Size(75, 23);
            this.btn_add_ip_rule.TabIndex = 6;
            this.btn_add_ip_rule.Text = "增加";
            this.btn_add_ip_rule.UseVisualStyleBackColor = true;
            this.btn_add_ip_rule.Click += new System.EventHandler(this.btn_add_ip_rule_Click);
            // 
            // rtxt_ip_rule_rex
            // 
            this.rtxt_ip_rule_rex.Location = new System.Drawing.Point(376, 20);
            this.rtxt_ip_rule_rex.Name = "rtxt_ip_rule_rex";
            this.rtxt_ip_rule_rex.Size = new System.Drawing.Size(287, 57);
            this.rtxt_ip_rule_rex.TabIndex = 5;
            this.rtxt_ip_rule_rex.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "采集正则：";
            // 
            // txt_ip_rule_url
            // 
            this.txt_ip_rule_url.Location = new System.Drawing.Point(66, 56);
            this.txt_ip_rule_url.Name = "txt_ip_rule_url";
            this.txt_ip_rule_url.Size = new System.Drawing.Size(237, 21);
            this.txt_ip_rule_url.TabIndex = 3;
            // 
            // txt_ip_rule_name
            // 
            this.txt_ip_rule_name.Location = new System.Drawing.Point(66, 24);
            this.txt_ip_rule_name.Name = "txt_ip_rule_name";
            this.txt_ip_rule_name.Size = new System.Drawing.Size(237, 21);
            this.txt_ip_rule_name.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "采集地址：";
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Location = new System.Drawing.Point(7, 27);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(65, 12);
            this.lb1.TabIndex = 0;
            this.lb1.Text = "规则名称：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_ip_rule);
            this.groupBox2.Location = new System.Drawing.Point(3, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(750, 278);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "规则列表";
            // 
            // dgv_ip_rule
            // 
            this.dgv_ip_rule.AllowUserToAddRows = false;
            this.dgv_ip_rule.AllowUserToDeleteRows = false;
            this.dgv_ip_rule.BackgroundColor = System.Drawing.Color.White;
            this.dgv_ip_rule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ip_rule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rule_name,
            this.rule_url,
            this.rule_rex,
            this.id});
            this.dgv_ip_rule.GridColor = System.Drawing.Color.Black;
            this.dgv_ip_rule.Location = new System.Drawing.Point(7, 20);
            this.dgv_ip_rule.Name = "dgv_ip_rule";
            this.dgv_ip_rule.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgv_ip_rule.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_ip_rule.RowTemplate.Height = 23;
            this.dgv_ip_rule.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_ip_rule.Size = new System.Drawing.Size(738, 252);
            this.dgv_ip_rule.TabIndex = 0;
            this.dgv_ip_rule.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_ip_rule_CellClick);
            // 
            // rule_name
            // 
            this.rule_name.DataPropertyName = "rule_name";
            this.rule_name.HeaderText = "规则名称";
            this.rule_name.Name = "rule_name";
            this.rule_name.ReadOnly = true;
            this.rule_name.Width = 150;
            // 
            // rule_url
            // 
            this.rule_url.DataPropertyName = "rule_url";
            this.rule_url.HeaderText = "采集地址";
            this.rule_url.Name = "rule_url";
            this.rule_url.ReadOnly = true;
            this.rule_url.Width = 200;
            // 
            // rule_rex
            // 
            this.rule_rex.DataPropertyName = "rule_rex";
            this.rule_rex.HeaderText = "采集正则";
            this.rule_rex.Name = "rule_rex";
            this.rule_rex.ReadOnly = true;
            this.rule_rex.Width = 300;
            // 
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.Visible = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ip_40px_572935_easyicon.net.png");
            this.imageList1.Images.SetKeyName(1, "settings_668px_1195254_easyicon.net.png");
            this.imageList1.Images.SetKeyName(2, "screen_399px_1173039_easyicon.net.png");
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgv_ip_list);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(286, 367);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "IP列表";
            // 
            // dgv_ip_list
            // 
            this.dgv_ip_list.BackgroundColor = System.Drawing.Color.White;
            this.dgv_ip_list.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ip_list.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dgv_ip_list.Location = new System.Drawing.Point(6, 20);
            this.dgv_ip_list.Name = "dgv_ip_list";
            this.dgv_ip_list.RowTemplate.Height = 23;
            this.dgv_ip_list.Size = new System.Drawing.Size(271, 341);
            this.dgv_ip_list.TabIndex = 1;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "IP";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 200;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txt_ip_coll_time);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txt_ip_check_time);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(298, 227);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(450, 146);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "工作配置";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "循环检测周期(秒)：";
            // 
            // txt_ip_check_time
            // 
            this.txt_ip_check_time.Location = new System.Drawing.Point(124, 58);
            this.txt_ip_check_time.Name = "txt_ip_check_time";
            this.txt_ip_check_time.Size = new System.Drawing.Size(100, 21);
            this.txt_ip_check_time.TabIndex = 1;
            // 
            // txt_ip_coll_time
            // 
            this.txt_ip_coll_time.Location = new System.Drawing.Point(123, 25);
            this.txt_ip_coll_time.Name = "txt_ip_coll_time";
            this.txt_ip_coll_time.Size = new System.Drawing.Size(100, 21);
            this.txt_ip_coll_time.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "循环采集周期(秒)：";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btn_ip_start);
            this.groupBox5.Location = new System.Drawing.Point(298, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(450, 215);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "工作台";
            // 
            // btn_ip_start
            // 
            this.btn_ip_start.Location = new System.Drawing.Point(10, 18);
            this.btn_ip_start.Name = "btn_ip_start";
            this.btn_ip_start.Size = new System.Drawing.Size(75, 23);
            this.btn_ip_start.TabIndex = 0;
            this.btn_ip_start.Text = "开始工作";
            this.btn_ip_start.UseVisualStyleBackColor = true;
            this.btn_ip_start.Click += new System.EventHandler(this.btn_ip_start_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 448);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "CsdnDownloader";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ip_rule)).EndInit();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ip_list)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_del_ip_rule;
        private System.Windows.Forms.Button btn_edit_ip_rule;
        private System.Windows.Forms.Button btn_add_ip_rule;
        private System.Windows.Forms.RichTextBox rtxt_ip_rule_rex;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_ip_rule_url;
        private System.Windows.Forms.TextBox txt_ip_rule_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb1;
        private System.Windows.Forms.DataGridView dgv_ip_rule;
        private System.Windows.Forms.DataGridViewTextBoxColumn rule_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn rule_url;
        private System.Windows.Forms.DataGridViewTextBoxColumn rule_rex;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.Label lb_ip_rule_id;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgv_ip_list;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btn_ip_start;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txt_ip_coll_time;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_ip_check_time;
        private System.Windows.Forms.Label label3;
    }
}

