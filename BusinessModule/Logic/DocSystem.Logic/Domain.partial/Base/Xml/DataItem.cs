using System;
using System.Xml;

namespace DocSystem.Logic
{
	/// <summary>
	/// DataItem 的摘要说明。
	/// </summary>
	public class DataItem
	{
		public XmlDocument XmlDoc;
		public XmlElement XmlEle;
		public string Type;

		/// <summary>
		/// 构造空的DataItem
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		public DataItem()
		{
			this.XmlDoc = new XmlDocument();
			this.XmlDoc.LoadXml("<Item/>");
			this.XmlEle=this.XmlDoc.DocumentElement;
		}

		/// <summary>
		/// 根据一个XmlDocument和一个XmlElement构造DataItem
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		public DataItem(XmlDocument doc,XmlElement ele)
		{
			if(doc!=null)this.XmlDoc = doc;
			if(ele!=null)this.XmlEle = ele;
		}

		#region 共有方法
		/// <summary>
		/// 根据index获得节点值
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetAttr(int index)
		{
			string value=null;
			XmlNode node=this.XmlEle.Attributes.Item(index);
			if(node!=null)value=DataStore.FilterNull(node.Value);
			return value;
		}

		/// <summary>
		/// 根据属性name获得节点值
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetAttr(string name)
		{
			string value=null;
			value=this.XmlEle.GetAttribute(name);
			if(value.Trim()!="")value = DataStore.FilterNull(value);
			return value;
		}

		/// <summary>
		/// 根据index设置节点值
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void SetAttr(int index,string value)
		{
			XmlNode node=this.XmlEle.Attributes.Item(index);
			node.Value=value;
		}

		/// <summary>
		/// 根据属性name设置节点值
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetAttr(string name,string value)
		{
			this.XmlEle.SetAttribute(name,value);
		}

		/// <summary>
		/// 根据index删除节点
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAttr(int index)
		{
			XmlNode node=this.XmlEle.Attributes.Item(index);
			this.XmlEle.Attributes.RemoveNamedItem(node.Name);
		}

		/// <summary>
		/// 根据属性name删除节点
		/// </summary>
		/// <param name="name"></param>
		public void RemoveAttr(string name)
		{
			this.XmlEle.Attributes.RemoveNamedItem(name);
		}

		/// <summary>
		/// 根据index获得节点名称
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetAttrName(int index)
		{
			return this.XmlEle.Attributes.Item(index).Name;
		}

		/// <summary>
		/// 获得属性节点数量
		/// </summary>
		/// <returns></returns>
		public int GetAttrCount()
		{
			return this.XmlEle.Attributes.Count;
		}

		/// <summary>
		/// 获得根节点名称
		/// </summary>
		/// <returns></returns>
		public string GetName()
		{
			return this.XmlEle.Name;
		}
		
		/// <summary>
		/// 获得包含本对象的DataStore
		/// </summary>
		/// <returns></returns>
		public DataStore GetDataStore()
		{	XmlElement ele = this.XmlEle;
			for(int i=0;i<10;i++)
			{	if(ele==null || ele.Name.ToLower()=="datastore")break;
				ele=(XmlElement)ele.ParentNode;
			}
			if(ele==null)return null;
			return new DataStore(this.XmlEle);
		}

		/// <summary>
		/// 获得本节点的值
		/// </summary>
		/// <returns></returns>
		public string GetValue()
		{
			return DataStore.FilterNull(this.XmlEle.InnerText);
		}

		/// <summary>
		/// 设置本节点的值
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(string value)
		{
			this.XmlEle.InnerText=value;
		}

		/// <summary>
		/// 获得本节点的XML
		/// </summary>
		/// <returns></returns>
		public string GetXmlValue()
		{
			return this.XmlEle.InnerXml;
		}

		/// <summary>
		/// 设置本节点的XML
		/// </summary>
		/// <param name="xmlsrc"></param>
		public void SetXmlValue(string xmlsrc)
		{
			this.XmlEle.InnerXml = xmlsrc;
		}

		/// <summary>
		/// 获得子节点，只允许包含一个子节点
		/// 例如：一个DataItem包含一个DataList时，只允许包含一个DataList，不允许包含两个以上
		/// </summary>
		/// <returns></returns>
		public XmlNode GetSubXmlNode()
		{	if(this.XmlEle.ChildNodes.Count>0)
				return this.XmlEle.ChildNodes[0];
			else 
				return null;
		}

		/// <summary>
		/// 获得子DataForm对象，只允许包含一个子节点
		/// </summary>
		/// <returns></returns>
		public DataForm GetSubDataForm()
		{
			if(this.XmlEle.ChildNodes.Count<=0)
				return null;
			if(this.XmlEle.ChildNodes[0].NodeType== XmlNodeType.Element)
				return new DataForm(this.XmlEle.OwnerDocument,(XmlElement)this.XmlEle.ChildNodes[0]);
			else
				return null;
		}
		
		/// <summary>
		/// 获得子DataList对象，只允许包含一个子节点
		/// </summary>
		/// <returns></returns>
		public DataList GetSubDataList()
		{	if(this.XmlEle.ChildNodes.Count<=0)
				return null;
			if(this.XmlEle.ChildNodes[0].NodeType== XmlNodeType.Element)
				return new DataList(this.XmlEle.OwnerDocument,(XmlElement)this.XmlEle.ChildNodes[0]);
			else
				return null;
		}
		
		/// <summary>
		/// 获得子DataEnum对象，只允许包含一个子节点
		/// </summary>
		/// <returns></returns>
		public DataEnum GetSubDataEnum()
		{
			if(this.XmlEle.ChildNodes.Count<=0)
			 return null;
			if(this.XmlEle.ChildNodes[0].NodeType== XmlNodeType.Element)
				return new DataEnum(this.XmlEle.OwnerDocument,(XmlElement)this.XmlEle.ChildNodes[0]);
			else
				return null;
		}
		
		/// <summary>
		/// 获得子DataParam对象，只允许包含一个子节点
		/// </summary>
		/// <returns></returns>
		public DataParam GetSubDataParam()
		{
			if(this.XmlEle.ChildNodes.Count<=0)
				return null;
			if(this.XmlEle.ChildNodes[0].NodeType== XmlNodeType.Element)
				return new DataParam(this.XmlEle.OwnerDocument,(XmlElement)this.XmlEle.ChildNodes[0]);
			else
				return null;
		}

        ///// <summary>
        ///// 将本对象转换成为StringParam
        ///// </summary>
        ///// <returns></returns>
        //public StringParam ToStringParam()
        //{
        //    StringParam objParam = new StringParam();
        //    for (int i = 0; i< this.XmlEle.Attributes.Count; i++)
        //        objParam.Add(this.XmlEle.Attributes.Item(i).Name,this.XmlEle.Attributes.Item(i).Value);
        //    return objParam;
        //}

		/// <summary>
		/// 将本对象转换成为DataForm
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataForm ToDataForm(string name)
		{
			XmlElement node=this.XmlDoc.CreateElement("Form");
			node.SetAttribute("Name",name);
			DataForm df = new DataForm(this.XmlDoc,node);
			for (int i = 0; i< this.XmlEle.Attributes.Count; i++)
				df.AddItem(this.XmlEle.Attributes.Item(i).Name,this.XmlEle.Attributes.Item(i).Value);
			return df;
		}

		/// <summary>
		/// 将本对象转换成为字符串
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.XmlEle.OuterXml;
		}

		public DataItem Clone(bool includeChild)
		{	XmlElement node = (XmlElement)this.XmlEle.CloneNode(includeChild);
			return new DataItem(this.XmlDoc,this.XmlEle);
		}
		#endregion
	}
}
