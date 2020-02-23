using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Data;
using Formula;
using Config;
using Base.Logic.BusinessFacade.Portal;


namespace Portal.PublicInfo
{
    public partial class PicNews : System.Web.UI.Page
    {
        protected string PicHTML = string.Empty;
        protected string IconBallHTML = string.Empty;
        protected string TextBallHTML = string.Empty;
        protected string StyleType = string.Empty;
        protected bool IsUseNewPortal = false;

        private void Page_Load(object sender, System.EventArgs e)
        {
            SetHTML();
        }

        private void SetHTML()
        {
            StringBuilder sb = new StringBuilder();

            //得到所有为上条件的所有信息
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = @"select c.ID,c.Title,b.ID as NewsImageID,Remark from
                            (
                            select GroupID,min(SortIndex) as SortIndex from S_I_NewsImage group by GroupID
                            ) a join S_I_NewsImage b on a.GroupID=b.GroupID and a.SortIndex=b.SortIndex 
                            right join S_I_NewsImageGroup c on c.ID=b.GroupID 
                            where isnull (c.DeptDoorId,'') ='' 
                            order by c.SortIndex,c.CreateTime desc";
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            IsUseNewPortal = PortalMain.IsUseNewPortal;
            StringBuilder pics = new StringBuilder();
            StringBuilder icons = new StringBuilder();
            StringBuilder texts = new StringBuilder();
            string active = " class=\"active\"";
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < (dt.Rows.Count < 5 ? dt.Rows.Count : 5); i++)
                    {
                        DataRow item = dt.Rows[i];
                        pics.Append("<li><a href=\"javascript:void(0);\" onclick=\"openGallary('" + item["ID"].ToString() + "','" + item["Title"].ToString() + "')\"><img  alt=\"" + item["Title"].ToString() + "\"  title=\"\"  src=\"/Base/PortalBlock/NewsImage/GetPic?ID=" + item["NewsImageID"].ToString() + "\" /></a></li>");
                        string icon = "<li{0}>" + (i + 1).ToString() + "</li>";
                        string title = "<li{0}><div class=\"textBall_title\">" + item["Title"].ToString() + "</div>{1}</li>";
                        string content = "<div class=\"textBall_content\">" + item["Remark"].ToString() + "</div>";
                        if (i > 0)
                        {
                            active = "";
                        }
                        icons.Append(string.Format(icon, active));
                        texts.Append(string.Format(title, active, IsUseNewPortal ? content : ""));

                    }
                }
                else
                {
                    pics.Append("<li></li>");
                    icons.Append("<li>1</li>");
                    texts.Append("<li></li>");
                }
            }
            PicHTML = pics.ToString();
            IconBallHTML = icons.ToString();
            TextBallHTML = texts.ToString();
            string css = "<link href='/PortalLTE/PublicInfo/css/{0}' rel='stylesheet' type='text/css' />";
            StyleType = IsUseNewPortal ? string.Format(css, "new.css") : string.Format(css, "old.css");
        }
    }
}