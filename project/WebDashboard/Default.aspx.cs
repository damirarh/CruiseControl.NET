using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Runtime.Remoting;
using tw.ccnet.core;
using tw.ccnet.remote;

namespace tw.ccnet.webdashboard
{
	public class Default : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label ExceptionTitleLabel;
		protected System.Web.UI.WebControls.DataGrid ExceptionGrid;
		protected System.Web.UI.WebControls.DataGrid StatusGrid;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			IList urls = (IList) ConfigurationSettings.GetConfig("projectURLs");
			ArrayList statusses = new ArrayList();
			ArrayList connectionExceptions = new ArrayList();

			foreach (string url in urls)
			{
				try
				{
					ICruiseManager remoteCC = (ICruiseManager) RemotingServices.Connect(typeof(ICruiseManager), url);
					statusses.Add(remoteCC.GetProjectStatus());
				}
				catch (Exception f)
				{
					connectionExceptions.Add(new ConnectionException(url, f));
				}
			}

			if (statusses.Count > 0)
			{
				StatusGrid.DataSource = statusses;
				StatusGrid.DataBind();
				StatusGrid.Visible = true;
			}
			else
			{
				StatusGrid.Visible = false;
			}

			if (connectionExceptions.Count > 0)
			{
				ExceptionGrid.DataSource = connectionExceptions;
				ExceptionGrid.DataBind();
				ExceptionGrid.Visible = true;
				ExceptionTitleLabel.Visible = true;
			}
			else
			{
				ExceptionGrid.Visible = false;
				ExceptionTitleLabel.Visible = false;
			}
		}

		private void StatusGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if ((ListItemType)e.Item.ItemType == ListItemType.Item)
			{
				ProjectStatus thisProjectsStatus = (ProjectStatus) e.Item.DataItem;
				TableCell buildStatusCell = (TableCell)e.Item.Controls[1];

				if (thisProjectsStatus.BuildStatus == IntegrationStatus.Success)
				{
					buildStatusCell.ForeColor = Color.Green;
				}
				else if (thisProjectsStatus.BuildStatus == IntegrationStatus.Unknown)
				{
					buildStatusCell.ForeColor = Color.Yellow;
				}
				else
				{
					buildStatusCell.ForeColor = Color.Red;
				}
			}

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}
		
		private void InitializeComponent()
		{    
			this.StatusGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.StatusGrid_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}

	public struct ConnectionException
	{
		private string _url;
		private Exception _exception;

		public ConnectionException(string URL, Exception exception)
		{
			this._url = URL;
			this._exception = exception;
		}

		public string URL
		{
			get { return _url; }
		}

		public string Message
		{
			get { return _exception.Message; }
		}

		public Exception Exception
		{
			get { return _exception; }
		}
	}
}
