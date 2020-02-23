using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Base.Logic.Domain;
using Base.Logic;
using Formula.Exceptions;
using Config;
using MvcAdapter;
using Formula;

namespace Base.Areas.Auth.Controllers
{
    public class RoleController : BaseController<S_A_Role>
    {
        public override JsonResult GetList(QueryBuilder qb)
        {
            if (Request["CorpID"] == "{CurrentUserCompanyID}")
                qb.Add("CorpID", QueryMethod.Equal, FormulaHelper.GetUserInfo().AdminCompanyID);
            return base.GetList(qb);
        }


        public JsonResult GetRoleUserList(string roleIDs, QueryBuilder qb)
        {
            string sql = @"select a.*,case when exists(select 1 from S_A__RoleUser where UserID=a.ID and RoleID=a.RoleID) then 1 else 0 end Effective from (
select S_A_User.ID,Code,Name,Sex,WorkNo,Phone,MobilePhone,RTX,DeptName,RoleID from S_A__OrgRole 
join S_A__OrgUser on S_A__OrgUser.OrgID=S_A__OrgRole.OrgID and RoleID in ('{0}')
join S_A_User on S_A_User.ID=S_A__OrgUser.UserID
union
select S_A_User.ID,Code,Name,Sex,WorkNo,Phone,MobilePhone,RTX,DeptName,RoleID  from S_A_User join S_A__RoleUser on ID=UserID and RoleID in ('{0}')
union
select S_A_User.ID,Code,Name,Sex,WorkNo,Phone,MobilePhone,RTX,DeptName,RoleID  from S_A_User join S_A__OrgRoleUser on ID=UserID and RoleID in ('{0}')
) a";
            sql = string.Format(sql, roleIDs);
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        public override JsonResult Save()
        {
            var entity = base.UpdateEntity<S_A_Role>();

            if (entities.Set<S_A_Role>().Count(c => c.Code == entity.Code && c.ID != entity.ID) > 0)
                throw new Exception("角色编号不能重复！");

            if (entity.Type == Base.Logic.RoleType.OrgRole.ToString())
            {
                entity.S_A__RoleUser.Clear();
            }

            if (!string.IsNullOrEmpty(Request["CorpID"]))
                entity.CorpID = Request["CorpID"];

            entities.SaveChanges();
            return Json("");
        }


        public JsonResult GetRoleRes(string nodeID)
        {
            return base.JsonGetRelationAll<S_A_Role, S_A__RoleRes, S_A_Res>(nodeID);
        }

        public JsonResult SetRoleRes(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__RoleRes>().Where(c => c.RoleID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.MenuRooID));
            return base.JsonSetRelation<S_A_Role, S_A__RoleRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        public JsonResult SetRoleRule(string nodeFullID, string relationData, string fullRelation)
        {
            var originalList = entities.Set<S_A__RoleRes>().Where(c => c.RoleID == nodeFullID && c.S_A_Res.FullID.StartsWith(Config.Constant.RuleRootID));
            return base.JsonSetRelation<S_A_Role, S_A__RoleRes, S_A_Res>(nodeFullID, relationData, originalList);
        }

        public JsonResult GetRoleUser(string nodeID)
        {
            return base.JsonGetRelationAll<S_A_Role, S_A__RoleUser, S_A_User>(nodeID);
        }

        public JsonResult SetRoleUser()
        {
            string nodeFullID = Request["nodeFullID"], relationData = Request["relationData"];
            return base.JsonSetRelation<S_A_Role, S_A__RoleUser, S_A_User>(nodeFullID, relationData, "False");
        }
        public JsonResult GetUserList(string roleIDs, QueryBuilder qb)
        {
            var role = entities.Set<S_A_Role>().Where(c => c.Type == "OrgRole" && roleIDs.Contains(c.ID));
            if (role != null && role.Count() > 0)
            {
                return GetRoleUserList(roleIDs, qb);
            }
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Base);
            string sql = string.Format(@"select a.*,case when a.ID=b.ID then 1 else 0 end Effective from S_A_User a left join (
                select a.ID  from S_A_User a left join S_A__RoleUser b on a.ID = b.UserID 
                left join S_A_Role c on b.RoleID = c.ID where c.ID in ('{0}')
                group by a.ID) b on a.ID =b.ID where a.IsDeleted!='1' order by Effective desc", roleIDs);

            return Json(sqlHelper.ExecuteGridData(sql, qb));
        }

        [ValidateInput(false)]
        public JsonResult SaveRoleUser()
        {
            string nodeFullID = Request["nodeFullID"], relationData = Request["relationData"];
            if (string.IsNullOrEmpty(nodeFullID) || string.IsNullOrEmpty(relationData))
                return Json("");
            var isTrue = GetValues(relationData, "Effective").First();

            string userID = GetValues(relationData, "ID").First();
            var role = entities.Set<S_A__RoleUser>().SingleOrDefault(c => c.RoleID == nodeFullID && c.UserID == userID);
            if (role == null && Convert.ToBoolean(isTrue))
            {
                S_A__RoleUser roleUser = new S_A__RoleUser();
                roleUser.RoleID = nodeFullID;
                roleUser.UserID = userID;
                entities.Set<S_A__RoleUser>().Add(roleUser);
            }
            else
            {
                entities.Set<S_A__RoleUser>().Remove(role);
            }
            var user = entities.Set<S_A_User>().FirstOrDefault(a => a.ID == userID);
            if (user != null) user.ModifyTime = DateTime.Now;

            entities.SaveChanges();
            return Json("ok");
        }

        [ValidateInput(false)]
        public JsonResult SaveRoleUsers(bool isSelect)
        {
            string nodeFullID = Request["nodeFullID"], relationData = Request["relationData"];
            if (string.IsNullOrEmpty(nodeFullID) || string.IsNullOrEmpty(relationData))
                return Json("");
            string[] userIDs = GetValues(relationData, "ID");
            if (userIDs.Length > 0)
            {
                foreach (var userID in userIDs)
                {
                    var role = entities.Set<S_A__RoleUser>().SingleOrDefault(c => c.RoleID == nodeFullID && c.UserID == userID);
                    if (role == null && isSelect)
                    {
                        S_A__RoleUser roleUser = new S_A__RoleUser();
                        roleUser.RoleID = nodeFullID;
                        roleUser.UserID = userID;
                        entities.Set<S_A__RoleUser>().Add(roleUser);
                    }
                    else
                    {
                        if (role != null && !isSelect)
                            entities.Set<S_A__RoleUser>().Remove(role);
                    }
                    var user = entities.Set<S_A_User>().FirstOrDefault(a => a.ID == userID);
                    if (user != null) user.ModifyTime = DateTime.Now;
                }
                entities.SaveChanges();
            }
            return Json("设置成功");
        }

    }
}
