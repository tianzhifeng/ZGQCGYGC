using System;
using System.Xml;
using System.Collections;

namespace DocSystem.Logic
{
	/// <summary>
	/// DataList 的摘要说明。
	/// </summary>
	public class DataList:DataNode
	{
		#region 构造函数

		/// <summary>
		/// 空的DataList构造函数
		/// </summary>
		public DataList():base()
		{
			this.Type="List";
			this.Init();
		}

		/// <summary>
		/// 根据XML字符串构造DataList
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataList(string xmlsrc):base(xmlsrc)
		{
			this.Type="List";
		}

		/// <summary>
		/// 根据XmlElement构造DataList
		/// </summary>
		/// <param name="ele"></param>
		public DataList(XmlElement ele):base(ele)
		{
			this.Type="List";
		}
		/// <summary>
		/// 根据DataList构造新的DataList
		/// </summary>
		/// <param name="ele"></param>
		public DataList(DataList dataList):base(dataList.ToString())
		{
			this.Type="List";
		}

		/// <summary>
		/// 根据XmlDocument和XmlElement构造DataForm
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		internal  DataList(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			this.Type="List";
		}
		#endregion

		#region 共有方法

		public DataItem GetItemById(string id)
		{	XmlElement ele = (XmlElement)this.XmlEle.SelectSingleNode("*[@Id='"+id+"']");
			if(ele == null)return null;
			return new DataItem(this.XmlDoc,ele);
		}

		/// <summary>
		/// 新建子节点
		/// </summary>
		/// <returns></returns>
		public new DataItem NewItem()
		{
			XmlElement node = this.XmlDoc.CreateElement("Item");
			this.XmlEle.AppendChild(node);
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// 根据index取得子节点DataItem的名称为name的属性值
		/// </summary>
		/// <param name="index"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetValue(int index,string name)
		{
			XmlElement node=(XmlElement)this.XmlEle.ChildNodes[index];
			if(node==null)return null;
			string value=node.GetAttribute(name);
			if(value==null)return null;
			return DataStore.FilterNull(value);
		}

		/// <summary>
		/// 根据index设置子节点DataItem的名称为name的属性值
		/// </summary>
		/// <param name="index"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetValue(int index,string name,string value)
		{
			XmlElement node=(XmlElement)this.XmlEle.ChildNodes[index];
			if(node==null)return;
			node.SetAttribute(name,value);
		}

		/// <summary>
		/// 取得属性列数组
		/// </summary>
		/// <returns></returns>
		public ArrayList GetFields()
		{
			string fldstr=this.XmlEle.GetAttribute("Fields");
			ArrayList rtnary = new ArrayList();
			if(fldstr==null)return rtnary;
			string[] ary = fldstr.Split(',');
			int j = 0;
			for (int i=0; i<ary.Length;i++)
				if (ary[i] != "")rtnary[j++] = ary[i];
			return rtnary;
		}

		/// <summary>
		/// 设置属性列数组
		/// </summary>
		/// <param name="fld">字符串</param>
		public void SetFields(string fld)
		{
			this.XmlEle.SetAttribute("Fields",fld);
		}

		/// <summary>
		/// 设置属性列数组
		/// </summary>
		/// <param name="fld">数组</param>
		public void SetFields(string[] fld)
		{
			string field="";
			for(int i=0;i<fld.Length;i++)
			{	field += fld[i];
				if(i<fld.Length-1)
					field = field + ",";
			}
			this.XmlEle.SetAttribute("Fields",field);
		}

		/// <summary>
		/// 根据name克隆DataList对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataList Clone(bool includeChild)
		{
			XmlElement node=(XmlElement)this.CloneNode(includeChild);
			return new DataList(this.XmlDoc,node);
		}
		/// <summary>
		/// 根据name克隆DataList对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataList Clone(string name)
		{
			XmlElement node=(XmlElement)this.CloneNode(name);
			return new DataList(this.XmlDoc,node);
		}
		/// <summary>
		/// 对当前对象的XmlElement进行排序
		/// </summary>
		/// <param name="columnName"></param>
		public void Sort(string columnName,bool isAsc)
		{
			XmlElement ele = this.XmlEle;
			ArrayList arrayList = new ArrayList();
			for(int i=0;i<ele.ChildNodes.Count;i++)
			{
				arrayList.Add(ele.ChildNodes[i]);
			}

			arrayList.Sort(new Comparer(columnName,isAsc));

			for(int i=0;i<arrayList.Count;i++)
			{
				ele.AppendChild((XmlNode)arrayList[i]);
			}
		}
		#endregion
	}


	public class Comparer:IComparer
	{
		private string columnName;
		private bool isAsc;

		#region IComparer 构造函数

		public Comparer(string columnName,bool isAsc)
		{
			this.columnName = columnName;
			this.isAsc = isAsc;
		}
		#endregion

		#region IComparer 成员

		public int Compare(object x, object y)
		{
			// TODO:  添加 Comparer.Compare 实现
			int iResult = 0;
			XmlElement diA = (XmlElement)x;
			XmlElement diB = (XmlElement)y;
			if(this.isAsc)
				iResult = diA.GetAttribute(this.columnName).CompareTo(diB.GetAttribute(this.columnName));
			else
				iResult = diB.GetAttribute(this.columnName).CompareTo(diA.GetAttribute(this.columnName));

			return iResult;
		}
		#endregion
	}
}
