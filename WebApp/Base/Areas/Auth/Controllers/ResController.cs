using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Base.Logic;
using Formula.Exceptions;
using Formula.Helper;
using Config;
using System.Data;
using System.IO;
using System.Text;
using Formula;
using Config.Logic;

namespace Base.Areas.Auth.Controllers
{
    public class ResController : BaseController<S_A_Res>
    {
        public override JsonResult GetTree()
        {
            string sql = string.Format("select * from S_A_Res where FullID like '{0}%' order by SortIndex", Request["RootFullID"]);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition());
            return Json(dt);
        }

        public override JsonResult SaveNode()
        {
            var entity = UpdateNode<S_A_Res>();

            if (entity.Type == ResType.Data.ToString())
            {
                string data = ResType.Data.ToString();
                if (entities.Set<S_A_Res>().Where(c => c.Url == entity.Url && c.Type == data && c.DataFilter == entity.DataFilter && c.ID != entity.ID).Count() > 0)
                    throw new BusinessException("不能增加重复的数据权限");
            }

            if (entity.Type == ResType.Button.ToString())
            {
                string button = ResType.Button.ToString();
                if (entities.Set<S_A_Res>().Where(c => c.Url == entity.Url && c.Type == button && c.ButtonID == entity.ButtonID && c.ID != entity.ID).Count() > 0)
                    throw new BusinessException("不能增加重复的按钮权限");
            }

            return base.JsonSaveNode<S_A_Res>(entity);
        }

        public JsonResult GetRule(string id)
        {
            return JsonGetModel<S_A_Res>(id);
        }

        public override JsonResult DeleteNode()
        {
            string fullID = Request["FullID"];
            var role = entities.Set<S_UI_RoleRes>().SingleOrDefault(c => c.FullID == fullID);
            if (role != null)
            {
                entities.Set<S_UI_RoleRes>().Remove(role);
                entities.SaveChanges();
            }
            return base.JsonDeleteNode<S_A_Res>(fullID);
        }
        #region 移动端配置


        public ActionResult MobileEnumEdit()
        {
            return View();
        }
        public ActionResult MobileTree()
        {
            return View();
        }    
        public  JsonResult GetMobileTree()
        {
            string sql = string.Format("select ID,ParentID,Name,Type,SortIndex from S_A_Res where Code ='Mobile'");
            SQLHelper sqlHelpers = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            List<S_A_Res> sares = sqlHelpers.ExecuteList<S_A_Res>(sql).AsQueryable().ToList(); ;


            sql = string.Format(@"select ID,MenuIcon,
                case  when ParentID=''  then '1' when ParentID is NULL then '1' else ParentID end 
                as ParentID,Name,GroupName,GroupIndex,SortIndex from S_S_EntryMenu With(NoLock) ");
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Terminal);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition()).AsEnumerable()
                .Select(x=>new {
                    ID = x["ID"].ToString(),
                    MenuIcon= checkfile(x["MenuIcon"].ToString()),
                    ParentID=x["ParentID"].ToString(),
                     Name= sares.Where(a=>a.ID== x["ID"].ToString()).Count()>0?"["+ x["GroupName"].ToString()  + "]"+x["Name"].ToString()+ "√ " : "[" + x["GroupName"].ToString() + "]" + x["Name"].ToString(),
                    GroupIndex = x["GroupIndex"].ToString(),
                    SortIndex = x["SortIndex"].ToString()                    
                }).OrderBy(x=>x.GroupIndex).ThenBy(x=>x.SortIndex).ToList();
            
            dt.Add(new {
                ID="1",
                MenuIcon= "/base/Scripts/Auth/menuIcons/wx-news.png",
                ParentID ="",
                Name="移动端菜单",
                GroupIndex="1",
                SortIndex="1"
            });
            return Json(dt);
        }
        public string checkfile(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory+ "/Scripts/Auth/";
                if (System.IO.File.Exists(path + filename))
                {
                    return "/base/Scripts/Auth/"+ filename;

                }
            }
            return "/base/Scripts/Auth/menuIcons/wx-news.png";
        }
        public JsonResult GetMobileIcon()
        {
            var path = Request["Path"];
            path = System.AppDomain.CurrentDomain.BaseDirectory + path;
            DirectoryInfo root = new DirectoryInfo(path);
             FileInfo[] files = root.GetFiles();
            return Json(files, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Icons()
        {
            return View();
        }
        
        public JsonResult GetMobileModel()
        {
            var id = Request["ID"];
            string sql = string.Format(@"select ID as OID,ID as NewsID,ID,ParentID,Name,Code,MenuIcon,Url,IsInnerLink,
               IsPanelItem,PanelSize,PanelIcon,SortIndex,Remark,Type,FunctionID,
                 Component,GroupName,GroupIndex from S_S_EntryMenu With(NoLock) where ID='" + id+"'");
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Terminal);
            var dt = sqlHelper.ExecuteDataTable(sql, new SearchCondition()).AsEnumerable().FirstOrDefault();
            if (id =="1")
            {
               var dts = new
                {
                    OID = id,
                    ID = id,
                    ParentID = "",
                    Name = "移动端菜单",
                    Code = "",
                    MenuIcon = "menuIcons/wx-news.png",
                    Url = "",
                    IsInnerLink = "",
                    IsPanelItem = "",
                    PanelSize = "",
                    PanelIcon ="",
                    SortIndex = "",
                    Remark = "移动端菜单配置",
                    Type = "",
                    FunctionID = "",
                    Component = "",
                    GroupName = "",
                    GroupIndex ="",
                };
                return Json(dts);
            }
            return Json(dt);
        }
        public  JsonResult SaveMobileNode(string data)
        {        
            var dic = JsonHelper.ToObject<Dictionary<string, string>>(data);
            string id = dic.GetValue("ID");
            string oid = dic.GetValue("OID");
            string pid = dic.GetValue("ParentID");
            if (!string.IsNullOrEmpty(id) && id == "1")
            {
                throw new BusinessException("根节点不能编辑!");
            }
           
            if (!string.IsNullOrEmpty(pid) && pid == "1")
            {
                pid = null;
            }
           
            if (string.IsNullOrEmpty(oid))
            {
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Terminal);
                if (string.IsNullOrEmpty(id))
                {
                    id = FormulaHelper.CreateGuid();                    
                }
                else
                {
                    string ssql = string.Format(@"select  ID from S_S_EntryMenu With(NoLock) where ID='" + id + "'");
                    var dt = sqlHelper.ExecuteDataTable(ssql, new SearchCondition()).AsEnumerable().FirstOrDefault();
                    if (dt != null)
                    {
                        throw new BusinessException("该功能已经添加！");
                    }
                }
                string sql = string.Format(@"insert into S_S_EntryMenu (ID,ParentID,Name,Code,MenuIcon,Url,IsInnerLink,
               IsPanelItem,PanelSize,PanelIcon,SortIndex,Remark,Type,FunctionID,
                 Component,GroupName,GroupIndex) values('{0}','{1}','{2}','{3}','{4}','{5}',
                '{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}')",
                id, pid, dic.GetValue("Name"), dic.GetValue("Code"), dic.GetValue("MenuIcon")
                , dic.GetValue("Url"), dic.GetValue("IsInnerLink"), dic.GetValue("IsPanelItem"), dic.GetValue("PanelSize"), dic.GetValue("PanelIcon")
                , dic.GetValue("SortIndex"), dic.GetValue("Remark"), dic.GetValue("Type"), dic.GetValue("FunctionID"), dic.GetValue("Component")
                , dic.GetValue("GroupName"), dic.GetValue("GroupIndex"));
                sqlHelper.ExecuteNonQuery(sql);
            }
            else
            {
                if (string.IsNullOrEmpty(id))
                {
                    id = oid;
                }
                string sql = string.Format(@"update S_S_EntryMenu set ID='{0}',ParentID='{1}',Name='{2}',Code='{3}',
                MenuIcon='{4}',Url='{5}',IsInnerLink='{6}',IsPanelItem='{7}',PanelSize='{8}',PanelIcon='{9}',
                SortIndex='{10}',Remark='{11}',Type='{12}',FunctionID='{13}',
                 Component='{14}',GroupName='{15}',GroupIndex='{16}' where ID='{17}'",
            id, pid, dic.GetValue("Name"), dic.GetValue("Code"), dic.GetValue("MenuIcon")
            , dic.GetValue("Url"), dic.GetValue("IsInnerLink"), dic.GetValue("IsPanelItem"), dic.GetValue("PanelSize"), dic.GetValue("PanelIcon")
            , dic.GetValue("SortIndex"), dic.GetValue("Remark"), dic.GetValue("Type"), dic.GetValue("FunctionID"), dic.GetValue("Component")
            , dic.GetValue("GroupName"), dic.GetValue("GroupIndex"), oid);
                SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Terminal);
                 sqlHelper.ExecuteNonQuery(sql);
            }
            var rdata = new
            {
                OID = id,
                ID = id,
                ParentID = dic.GetValue("ParentID"),
                Name = dic.GetValue("Name"),
                Code = dic.GetValue("Code"),
                MenuIcon = dic.GetValue("MenuIcon"),
                Url = dic.GetValue("Url"),
                IsInnerLink = dic.GetValue("IsInnerLink"),
                IsPanelItem = dic.GetValue("IsPanelItem"),
                PanelSize = dic.GetValue("PanelSize"),
                PanelIcon = dic.GetValue("PanelIcon"),
                SortIndex = dic.GetValue("SortIndex"),
                Remark = dic.GetValue("Remark"),
                Type = dic.GetValue("Type"),
                FunctionID = dic.GetValue("FunctionID"),
                Component = dic.GetValue("Component"),
                GroupName = dic.GetValue("GroupName"),
                GroupIndex = dic.GetValue("GroupIndex"),
            };
            return Json(rdata);
        }

        public JsonResult DeleteMobileNode()
        {
            var id = Request["ID"];
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Terminal);

            string ssql = string.Format(@"select  ID from S_S_EntryMenu With(NoLock) where ID='" + id + "'");
            var dt = sqlHelper.ExecuteDataTable(ssql, new SearchCondition()).AsEnumerable().FirstOrDefault();
            if (dt == null)
            {
                throw new BusinessException("没有找到该数据，请检查");
            }
            string sql = string.Format(@"delete from S_S_EntryMenu  where ID='" + id + "' or ParentID='" + id + "'");
            sqlHelper.ExecuteNonQuery(sql);
            var res = entities.Set<S_A_Res>().Where(c => c.ID == id || c.ParentID==id).ToList();
           foreach(var item in res)
            {
                entities.Set<S_A_Res>().Remove(item);
            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult PublicMobileNode()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Terminal);
            var sares = entities.Set<S_A_Res>().Where(c => c.Code == "Mobile"&&c.ParentID=="").FirstOrDefault();
            string mSql = "";
            if (sares == null)
            {
                 mSql = string.Format(@"select '{0}' as ID,'' as ParentID,'{0}' as FullID,'移动端' as Name,'Menu' as Type,1 as SortIndex,'' as MenuIcon
                    union all
                    select ID,
	                    case when isnull(ParentID,'')<>'' 
	                    then ParentID 
	                    else '{0}' end as ParentID,
	                    case when isnull(ParentID,'')<>'' 
	                    then '{0}.'+ ParentID + '.' + ID 
	                    else '{0}.' + ID  end as FullID,	
                    Name,'Menu' as Type,SortIndex,MenuIcon from S_S_EntryMenu
                    ", Guid.NewGuid());
            }
            else
            {
                 mSql = string.Format(@"select ID,
	                    case when isnull(ParentID,'')<>'' 
	                    then ParentID 
	                    else '{0}' end as ParentID,
	                    case when isnull(ParentID,'')<>'' 
	                    then '{0}.'+ ParentID + '.' + ID 
	                    else '{0}.' + ID  end as FullID,	
                    Name,'Menu' as Type,SortIndex,MenuIcon from S_S_EntryMenu
                    ", sares.ID);
            }
           
            SQLHelper terminalHelper = SQLHelper.CreateSqlHelper(ConnEnum.Terminal);
            var table = terminalHelper.ExecuteDataTable(mSql);
            foreach (DataRow row in table.Rows)
            {
                var resID = row["ID"].ToString();
                var res = entities.Set<S_A_Res>().Where(c => c.ID == resID).FirstOrDefault();
                if (res == null)
                {
                    entities.Set<S_A_Res>().Add(new S_A_Res
                    {
                        ID = resID,
                        ParentID = Convert.ToString(row["ParentID"]),
                        Code="Mobile",
                        IconUrl = Convert.ToString(row["MenuIcon"]),
                        FullID = Convert.ToString(row["FullID"]),
                        Name = Convert.ToString(row["Name"]),
                        Type = Convert.ToString(row["Type"]),
                        SortIndex = !string.IsNullOrEmpty(row["SortIndex"].ToString()) ? Convert.ToDouble(row["SortIndex"]) : 0
                    });
                }
                else
                {
                    if (res.Type == row["Type"].ToString() || res.Name == row["SortIndex"].ToString() || res.Name != Convert.ToString(row["Name"]))
                    {
                        res.Name = Convert.ToString(row["Name"]);
                        res.SortIndex = !string.IsNullOrEmpty(row["SortIndex"].ToString()) ? Convert.ToDouble(row["SortIndex"]) : 0;
                        res.Type = Convert.ToString(row["Type"]);
                    }
                }

            }
            entities.SaveChanges();
            return Json("");
        }

        public JsonResult GetMenuGroupEnum()
        {
            var code = Request["EnumCode"];
            if (!string.IsNullOrEmpty(code))
            {
                var defenum = entities.Set<S_M_EnumDef>().Where(c => c.Code == code).FirstOrDefault();
                if (defenum != null)
                {
                    var res = entities.Set<S_M_EnumItem>().Where(c => c.EnumDefID == defenum.ID).OrderBy(x => x.SortIndex).ToList();
                    return Json(res);
                }
            }
            return Json("");
        }
        public JsonResult RemoveMenuGroupEnum(string listIDs)
        {
            return base.JsonDelete<S_M_EnumItem>(listIDs);
        }

        public JsonResult GetMobileGroupEnum()
        {
            var id = Request["ID"];
            var model = GetEntity<S_M_EnumItem>(id);
            if (model._state == MvcAdapter.EntityStatus.added.ToString())
            {
                var code = Request["EnumCode"];
                if (!string.IsNullOrEmpty(code))
                {
                    var defenum = entities.Set<S_M_EnumDef>().Where(c => c.Code == code).FirstOrDefault();
                    if (defenum != null)
                    {
                        model.EnumDefID = defenum.ID;
                        var lastModel = entities.Set<S_M_EnumItem>().Where(c => c.EnumDefID == defenum.ID).OrderByDescending(c => c.SortIndex).FirstOrDefault();
                        if (lastModel == null)
                            model.SortIndex = 0;
                        else if (lastModel.SortIndex == null)
                            model.SortIndex = 0;
                        else
                            model.SortIndex = lastModel.SortIndex + 1;
                    }
                }
               
            }
            return Json(model);
        }
        public JsonResult EditMenuGroupEnum()
        {
            var item = UpdateEntity<S_M_EnumItem>();
            if (entities.Set<S_M_EnumItem>().SingleOrDefault(c => c.EnumDefID == item.EnumDefID && c.Code == item.Code && c.ID != item.ID) != null)
                throw new Exception("枚举编号不能重复");
            var def = entities.Set<S_M_EnumDef>().SingleOrDefault(c => c.ID == item.EnumDefID);
            def.ModifyTime = DateTime.Now;
            entities.SaveChanges();
            return Json(new { ID = item.ID });
        }
        #endregion
        #region 导出Sql

        //菜单导出Sql
        public FileResult SqlFile(string resID)
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("USE [{0}] \n", sqlHelper.DbName);

            string sql = string.Format("select * from S_A_Res where ID in('{0}')", resID);
            DataTable dt = sqlHelper.ExecuteDataTable(sql);
            var fullID = dt.Rows[0]["FullID"].ToString();
            var parentID = dt.Rows[0]["ParentID"].ToString();
            if (!string.IsNullOrEmpty(parentID))
            {
                sb.AppendFormat(@"
--父菜单不存在，不能导入
if not exists (select ID from S_A_Res where ID='{0}')
begin
    select '父菜单不存在，不能导入'
	return
end
", parentID);
            }

            sb.AppendLine();
            dt = sqlHelper.ExecuteDataTable("select * from S_A_Res where FullID like '" + fullID + "%'");
            sb.Append(SQLHelper.CreateUpdateSql("S_A_Res", dt));

            MemoryStream ms = new MemoryStream(System.Text.Encoding.Default.GetBytes(sb.ToString()));
            ms.Position = 0;
            return File(ms, "application/octet-stream ; Charset=UTF8", "ResSQL.sql");
        }


        #endregion
      

        #region 复制粘贴

        public void NodePaste(string id, string cp)
        {
            List<S_A_Res> list = new List<S_A_Res>();
            var target = entities.Set<S_A_Res>().Where(o => o.FullID.Contains(cp)).ToArray();
            var paste = entities.Set<S_A_Res>().Where(o => o.ID == id).SingleOrDefault();
            foreach (var item in target)
            {
                S_A_Res res = new S_A_Res();
                res.ID = item.ID;
                res.FullID = item.FullID;
                res.ParentID = item.ParentID;
                res.Name = item.Name;
                res.Code = item.Code;
                res.Type = item.Type;
                res.Url = item.Url;
                res.IconUrl = item.IconUrl;
                res.Target = item.Target;
                res.ButtonID = item.ButtonID;
                res.DataFilter = item.DataFilter;
                res.Description = item.Description;
                res.SortIndex = item.SortIndex;
                res.IconClass = item.IconClass;
                res.NameEN = item.NameEN;
                list.Add(res);
            }
            foreach (var item in list)
            {
                string oldID = item.ID;
                string newID = FormulaHelper.CreateGuid();
                foreach (var items in list)
                {   
                    if (items.ID == cp)
                    {
                        items.ParentID = id;
                    }
                    else if (oldID == items.ParentID)
                    {
                        items.ParentID = newID;
                    }            
                }
                if (item.ID == cp)
                {
                    item.FullID = paste.FullID + "." + newID;
                }
                else {
                    item.FullID = list.Find(o => o.ID == item.ParentID).FullID + "." + newID;
                }
                item.ID = newID;
                entities.Set<S_A_Res>().Add(item);
            }
            entities.SaveChanges();
        }

        #endregion

    }
}
