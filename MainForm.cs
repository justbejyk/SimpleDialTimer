/*
 * Created by SharpDevelop.
 * User: quinnc
 * Date: 2019/4/23
 * Time: 21:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace read
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.timer1.Interval = 1000;
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		private List<String[]> accountList = new List<string[]>();
		
		private int len = 0;
		
		private int count = 0;
		
		private Boolean first = true;
		
		void Button1Click(object sender, EventArgs e)
		{
			DialogResult result = this.openFileDialog1.ShowDialog();
			if(result == DialogResult.OK){
				this.label2.Text = this.openFileDialog1.FileName;
			}
		}
		
		void Timer1Tick(object sender, EventArgs e)
		{
			
			if(accountList != null && len > 0){
				if(count == len){
					count = 0;
				}
				String[] myacct = accountList[count];
				count++;
				String name = this.textBox1.Text;
				String command = "rasdial "+name+" "+myacct[0]+" "+ myacct[1];
//				String command = "echo "+myacct[0]+" "+ myacct[1];
				this.richTextBox1.AppendText(DateTime.Now.ToString()+ "   ");
				this.richTextBox1.AppendText(command);
				this.richTextBox1.AppendText("\n");
				String aa = RunCmd(command);
				this.richTextBox1.AppendText("连接结果：" + aa);
			}
			if(first){
				first = false;
				this.timer1.Interval = 3600000;
			}
			
			
		}
		void MainFormSizeChanged(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.Hide();   //隐藏窗体
				notifyIcon1.Visible = true; //使托盘图标可见
			}
		}
		void NotifyIcon1DoubleClick(object sender, EventArgs e)
		{
			this.Visible = true;
			this.WindowState = FormWindowState.Normal;
			
		}
		void Button2Click(object sender, EventArgs e)
		{
			String files = this.label2.Text;
			using(FileStream fin = new FileStream(files, FileMode.Open)){
				using(StreamReader reader = new StreamReader(fin, Encoding.UTF8)){
					while(!reader.EndOfStream){
						String[] content = reader.ReadLine().Split(new char[1]{','});
						accountList.Add(content);
					}
					len = accountList.Count;
				}
			}
			
			this.timer1.Start();
			richTextBox1.AppendText("任务启动。。。。。。。\n");
		}
		
		private string RunCmd(string command)
		{
			//实例一个Process类，启动一个独立进程
			Process p = new Process();
			//Process类有一个StartInfo属性
			//设定程序名
			p.StartInfo.FileName = "cmd.exe";
			//设定程式执行参数
			p.StartInfo.Arguments = "/c " + command;
			//关闭Shell的使用
			p.StartInfo.UseShellExecute = false;
			//重定向标准输入
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardOutput = true;
			//重定向错误输出
			p.StartInfo.RedirectStandardError = true;
			//设置不显示窗口
			p.StartInfo.CreateNoWindow = true;
			//启动
			p.Start();
			//也可以用这种方式输入要执行的命令
			//不过要记得加上Exit要不然下一行程式执行的时候会当机
			//p.StandardInput.WriteLine(command);
			//p.StandardInput.WriteLine("exit");
			//从输出流取得命令执行结果
			return p.StandardOutput.ReadToEnd();
		}
		void Button3Click(object sender, EventArgs e)
		{
			this.timer1.Stop();
			this.first = false;
			this.timer1.Interval = 100;
			this.accountList.Clear();
		}
	}
}
