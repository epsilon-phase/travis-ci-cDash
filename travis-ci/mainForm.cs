using System;
using System.Windows.Forms;
using System.Net;
namespace travisci
{
	public class mainForm:Form
	{
		HttpWebRequest req;
		Label thing;
		public mainForm ()
		{
			initializeComponents();

		}
		public void initializeComponents(){
			this.Text="Travis";
			req=(HttpWebRequest)WebRequest.Create(new Uri("http://api.travis-ci.org/"));
			req.Accept="application/vnd.travis-ci.2+json";
			var q=req.GetResponse();
			thing=new Label();
			thing.Text="This is here";
			Controls.Add(thing);
			Console.WriteLine(q.ToString());
			
		}
	}
}

