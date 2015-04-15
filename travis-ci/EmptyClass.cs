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
				g.DrawImage(this.img,new System.Drawing.RectangleF(new System.Drawing.PointF(0,0),new System.Drawing.SizeF(this.Width,this.Height)));
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
				this.updatepicture();
			}
		}
		private void updatepicture(){
			successful=false;
			try{
				if(cando){
				getter.DownloadDataAsync(new System.Uri(url));
					cando=false;
				}
			
			}catch(System.UriFormatException e){}
		}
		void DownloadDataCompleted(object sender,
			System.Net.DownloadDataCompletedEventArgs e){
			cando=true;
			if(e.Result!=null)
			{
				var d=new System.IO.MemoryStream(e.Result);
				img=System.Drawing.Image.FromStream(d);
				this.Refresh();
				successful=true;
			}
		}
		string url;
		private System.Drawing.Image img;

	}
	public class EmptyClass:Form
	{
		public string geturl(){
			return url.Text;
		}
		public void seturl(string d){
			url.Text=d;
		}
		TableLayoutPanel thing;
		imagePanel g;
		TextBox url;
		void OnMove(object sender,EventArgs e){
			Travisci.save=true;
		}
		public EmptyClass ()
		{
			this.Move+=OnMove;
			this.thing=new TableLayoutPanel();
			thing.RowCount=2;
			thing.Dock=DockStyle.Fill;
			this.Controls.Add(thing);
			this.g=new imagePanel();
			thing.Controls.Add(g);
			thing.SetRow(g,0);
			url=new TextBox();
			url.Dock=DockStyle.Fill;
			thing.Controls.Add(url);
			thing.SetRow(url,1);
			url.TextChanged+=onTextAltered;
		}
		private void onTextAltered(object sender,EventArgs e){
			g.Url=this.url.Text;
			if(g.Successful){
				Travisci.save=true;
			}
		}
	}
}

