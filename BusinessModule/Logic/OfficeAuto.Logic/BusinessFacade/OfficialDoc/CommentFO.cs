using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Formula;
using OfficeAuto.Logic.Domain;

namespace OfficeAuto.Logic
{
    public class CommentFO
    {
        public static bool Add(CommentType type, string comment, string userID, string userName)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                var entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
                S_D_Comment model = entities.Set<S_D_Comment>().FirstOrDefault(c => c.IsUse == "1" && ((c.IsTemplate == "1" && c.Comment == comment) || (c.IsTemplate == "0" && c.UserID == userID && c.Comment == comment)));
                if (model == null)
                {
                    model = new S_D_Comment();
                    model.ID = FormulaHelper.CreateGuid();
                    model.IsTemplate = "0";
                    model.IsUse = "1";
                    model.Comment = comment;
                    model.UserID = userID;
                    model.UserName = userName;
                    model.Type = type.ToString();
                    model.CreateDate = DateTime.Now;
                    model.CreateUserID = userID;
                    model.CreateUserName = userName;
                    entities.Set<S_D_Comment>().Add(model);
                    entities.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public static List<S_D_Comment> GetComments(CommentType type, string userID)
        {
            var entities = FormulaHelper.GetEntities<OfficeAutoEntities>();
            string strType = type.ToString();
            List<S_D_Comment> list = entities.Set<S_D_Comment>().Where(t => t.Type == strType && t.IsUse == "1" && (t.IsTemplate == "1" || t.UserID == userID)).OrderByDescending(t => t.ID).ToList();
            return list;
        }
    }
}
