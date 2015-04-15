using System;
using System.Windows.Forms;
using System.Web;
using System.ComponentModel.Composition;
namespace travisci
{
	[Export(typeof(cDashboard.IPlugin))]
	[ExportMetadata("name","travis-ci")]
	class Travisci:cDashboard.IPlugin{
		System.Collections.Generic.List<EmptyClass> things;
		public static bool save=false;
		public bool DisposeOnClose{
			get{
				return true;
			}
		}
		public Travisci(){
			things=new System.Collections.Generic.List<EmptyClass>();
		}
		public bool NeedsSaving{
			get{
				return Travisci.save;
			}
		}
		public Form GetForm(){
			var i=new EmptyClass();
			things.Add(i);
			return i;
		}
		private Form GetForm(System.Drawing.Point p,string url){
			var i=(EmptyClass)GetForm();
			i.Location= p ;
			i.seturl(url);
			return i;
		}

		public void SavePlugin(string settingslocation){
			if(System.IO.File.Exists(settingslocation+"travisci.cDash"))
				System.IO.File.Delete(settingslocation+"travisci.cDash");
			var g=System.IO.File.CreateText(settingslocation+"travisci.cDash");
			foreach(var c in things){
				g.Write(c.Location.X+" ");
				g.Write(c.Location.Y+" ");
				g.Write(c.geturl()+" ");
				g.Write("\r\n");
			}
			g.Close();
		}
		public void LoadPlugin(string settingslocation,cDashboard.cDashboard c)
		{
			if(!System.IO.File.Exists(settingslocation+"travisci.cDash"))
				return;

		}
		public Type getFormType(){
			return typeof(EmptyClass);
		}

	}
	class imagePanel:Panel{
		System.Net.WebClient getter;
		bool cando,successful;
		public bool Successful{
			get{
				return successful;
			}
		}
		public imagePanel(){
			base.Paint+=paintEventHandler;
			Dock=DockStyle.Fill;
			getter=new System.Net.WebClient();
			getter.DownloadDataCompleted+=DownloadDataCompleted;
			cando=true;
		}
		void paintEventHandler(object sender,PaintEventArgs e){
			var g=e.Graphics;
			g.Clear(System.Drawing.SystemColors.ControlLight);
			if(img!=null)			
			{
				img.Width=this.Width;
				img.Height=this.Height;
				var q=img.Draw();
				q.Save("hello.png");
				g.DrawImage(q,new System.Drawing.RectangleF(new System.Drawing.PointF(0,0),new System.Drawing.SizeF(Width,Height)));

			}
			else{
				var red=System.Drawing.Pens.Red;
				g.DrawLine(red,new System.Drawing.PointF(0,0),new System.Drawing.PointF(Width,Height));
				g.DrawLine(red,new System.Drawing.PointF(0,Height),new System.Drawing.PointF(Width,0));
			}
		}
		public string Url{
			get{
				return url;
			}
			set{
				this.url=value;
				this.img=null;
				this.updatepicture();
			}
		}
		public void updatepicture(){

			try{
				if(cando){
				getter.DownloadDataAsync(new System.Uri(url));
					successful=true;
					cando=false;
				}
			
			}catch(System.UriFormatException e){
				Console.WriteLine(e.Message);
				successful=false;
			}
		}
		void DownloadDataCompleted(object sender,
			System.Net.DownloadDataCompletedEventArgs e){
			cando=true;

			if(e.Result!=null)
			{
				var d=e.Result;
				
				var str = System.Text.Encoding.Default.GetString(d);
				var qqq=System.IO.File.CreateText("ff");
				qqq.Write(str);
				qqq.Close();
				img=Svg.SvgDocument.Open("ff");
				this.Refresh();
				successful=true;
			}else{
				Console.WriteLine("download completed with error");
				Console.WriteLine(e.Error.Message);
			}
		}
		string url;
		private Svg.SvgDocument img;

	}
	public class EmptyClass:Form
	{
		string goodurl;
		public string geturl(){
			return String.Format("https://travis-ci.org/{0}/{1}.svg?branch=master",user.Text,repon.Text);
			
		}
		public void seturl(string d){
			user.Text=d;
			g.Url=d;
			if(g.Successful)
				goodurl=d;
		}
		TableLayoutPanel thing;
		TabControl page;
		TabPage view,config;
		imagePanel g;
		TextBox user,repon;
		Timer checkagain;
		Label username,repo;

		void OnMove(object sender,EventArgs e){
			Travisci.save=true;
		}
		void OnTick(object sender,EventArgs e){
			g.updatepicture();
		}
		public EmptyClass ()
		{
			page=new TabControl();
			page.Dock=DockStyle.Fill;
			checkagain=new Timer();
			checkagain.Enabled=true;
			checkagain.Interval=20*60*1000;
			checkagain.Tick+=OnTick;
			this.Move+=OnMove;
			view=new TabPage();
			g=new imagePanel();
			g.Dock=DockStyle.Fill;
			view.Controls.Add(g);
			view.Text="View";
			thing=new TableLayoutPanel();
			thing.Dock=DockStyle.Fill;

			config=new TabPage();
			config.Text="Config";
			config.Controls.Add(thing);

			user=new TextBox();
			user.Dock=DockStyle.Fill;
			thing.Controls.Add(user);
			thing.SetRow(user,0);
			thing.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			thing.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			thing.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			thing.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			thing.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			thing.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			thing.SetColumn(user,1);
			username=new Label();
			username.Text="Username";

			thing.Controls.Add(username);
			thing.SetRow(username,0);
			thing.SetColumn(username,0);

			user.TextChanged+=onTextAltered;
			repon=new TextBox();
			repon.TextChanged+=onTextAltered;
			thing.Controls.Add(repon);
			thing.SetRow(repon,1);
			thing.SetColumn(repon,1);
			repo=new Label();
			repo.Text="repository";
			thing.Controls.Add(repo);
			thing.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			thing.SetRow(repo,1);
			thing.SetColumn(repo,0);
			page.Controls.Add(view);
			page.Controls.Add(config);
			Controls.Add(page);
			status=new Label();
			status.Text="";
			thing.Controls.Add(status);
			thing.RowStyles[2]=new RowStyle(SizeType.AutoSize);
			thing.SetRow(status,1);
			thing.SetColumnSpan(status,2);
			thing.SetColumn(status,0); 
			thing.RowCount=3;
		}
		Label status;
		private void onTextAltered(object sender,EventArgs e){
			g.Url=geturl();
			
			status.Text=geturl();

			/*if(g.Successful){
				goodurl=user.Text;
				Travisci.save=true;
			}*/
		}
	}
}

