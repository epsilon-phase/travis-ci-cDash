using System;
using System.Windows.Forms;
using System.Web;
namespace travisci
{
	class imagePanel:Panel{
		System.Net.WebClient getter;
		bool cando;
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
			}
		}
		string url;
		private System.Drawing.Image img;

	}
	public class EmptyClass:Form
	{
		TableLayoutPanel thing;
		imagePanel g;
		TextBox url;
		public EmptyClass ()
		{
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
		}
	}
}

