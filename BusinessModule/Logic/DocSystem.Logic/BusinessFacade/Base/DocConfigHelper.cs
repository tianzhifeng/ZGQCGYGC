using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using Formula.Helper;
using Formula;
using DocSystem.Logic.Domain;


namespace DocSystem.Logic
{
    public class DocConfigHelper
    {
        const string cacheKey = "S_C_DocSpace_";

        public static void PublishSpaceBySpaceID(string spaceID)
        {
            var space = CreateConfigSpaceByID(spaceID);

            space.SaveDataBase();

            CacheHelper.Remove(cacheKey + spaceID);

        }

        public static void PublishSpaceBySpaceKey(string spaceKey)
        {
            var space = CreateConfigSpaceByID(spaceKey);

            space.SaveDataBase();

            CacheHelper.Remove(cacheKey + spaceKey);
        }

        public static S_DOC_Space CreateConfigSpaceByID(string spaceID = "")
        {
            string defaultSpaceID = ConfigurationManager.AppSettings["DefaultDocSpaceID"];
            if (String.IsNullOrEmpty(spaceID))
                spaceID = defaultSpaceID;
            var configSpace = CacheHelper.Get(cacheKey + spaceID);
            if (configSpace == null)
            {
                var entities = FormulaHelper.GetEntities<DocConfigEntities>();
                var space = entities.Set<S_DOC_Space>().Include("S_DOC_TreeConfig").Include("S_DOC_Node").Include("S_DOC_Node.S_DOC_NodeAttr").Include("S_DOC_Node.S_DOC_FileNodeRelation").
                    Include("S_DOC_File").Include("S_DOC_File.S_DOC_FileAttr").Include("S_DOC_ListConfig").
                    Include("S_DOC_ListConfig.S_DOC_ListConfigDetail").Include("S_DOC_ListConfig.S_DOC_QueryParam").Include("S_DOC_NodeStrcut").
                    FirstOrDefault(d => d.ID == spaceID);
                CacheHelper.Set(cacheKey + spaceID, space);
                return space;
            }
            else
                return configSpace as S_DOC_Space;
        }

        public static S_DOC_Space CreateConfigSpaceByKey(string spaceKey = "")
        {
            string defaultSpaceKey = ConfigurationManager.AppSettings["DefaultDocSpaceKey"];
            if (String.IsNullOrEmpty(spaceKey))
                spaceKey = defaultSpaceKey;
            var configSpace = CacheHelper.Get(cacheKey + spaceKey);
            if (configSpace == null)
            {
                var entities = FormulaHelper.GetEntities<DocConfigEntities>();

                var space = entities.Set<S_DOC_Space>().Include("S_DOC_Node").Include("S_DOC_Node.S_DOC_NodeAttr").Include("S_DOC_Node.S_DOC_FileNodeRelation").
                    Include("S_DOC_File").Include("S_DOC_File.S_DOC_FileAttr").Include("S_DOC_ListConfig").
                    Include("S_DOC_ListConfig.S_DOC_ListConfigDetail").Include("S_DOC_ListConfig.S_DOC_QueryParam").Include("S_DOC_NodeStrcut").
                    FirstOrDefault(d => d.SpaceKey == spaceKey);
                CacheHelper.Set(cacheKey + spaceKey, space);
                return space;
            }
            else
                return configSpace as S_DOC_Space;
        }

        public static ArrayList GetMatchList(string dataType)
        {
            ArrayList result = new ArrayList();
            if (dataType.ToLower().StartsWith("varchar") || dataType.ToLower().StartsWith("nvarchar"))
            {
                result.Add(CreateEnumHash(QueryType.LK.ToString(), "模糊匹配"));
                result.Add(CreateEnumHash(QueryType.EQ.ToString(), "等于"));
                result.Add(CreateEnumHash(QueryType.SW.ToString(), "以...开始"));
                result.Add(CreateEnumHash(QueryType.EW.ToString(), "以...结束"));
            }
            else if (dataType.ToLower().StartsWith("int") || dataType.ToLower().StartsWith("decimal"))
            {
                result.Add(CreateEnumHash(QueryType.EQ.ToString(), "等于"));
                result.Add(CreateEnumHash(QueryType.FR.ToString(), "大于等于"));
                result.Add(CreateEnumHash(QueryType.GT.ToString(), "大于"));
                result.Add(CreateEnumHash(QueryType.TO.ToString(), "小于等于"));
                result.Add(CreateEnumHash(QueryType.LT.ToString(), "小于"));
                result.Add(CreateEnumHash(QueryType.UE.ToString(), "不等于"));
            }
            else if (dataType.ToLower().StartsWith("dateTime"))
            {
                result.Add(CreateEnumHash(QueryType.EQ.ToString(), "等于"));
                result.Add(CreateEnumHash(QueryType.FR.ToString(), "大于等于"));
                result.Add(CreateEnumHash(QueryType.GT.ToString(), "大于"));
                result.Add(CreateEnumHash(QueryType.TO.ToString(), "小于等于"));
                result.Add(CreateEnumHash(QueryType.LT.ToString(), "小于"));
            }
            else
            {
                //result =Formula.Helper.EnumBaseHelper.get .SystemEnumToArrayList(typeof(QueryType));
            }
            return result;
        }

        internal static Hashtable CreateEnumHash(string value, string text)
        {
            var hash = new Hashtable();
            hash["value"] = value;
            hash["text"] = text;
            return hash;
        }
    }
}
