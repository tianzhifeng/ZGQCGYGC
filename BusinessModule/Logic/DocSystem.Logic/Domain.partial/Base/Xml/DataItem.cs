using System;
using System.Xml;

namespace DocSystem.Logic
{
	/// <summary>
	/// DataItem ��ժҪ˵����
	/// </summary>
	public class DataItem
	{
		public XmlDocument XmlDoc;
		public XmlElement XmlEle;
		public string Type;

		/// <summary>
		/// ����յ�DataItem
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
		/// ����һ��XmlDocument��һ��XmlElement����DataItem
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		public DataItem(XmlDocument doc,XmlElement ele)
		{
			if(doc!=null)this.XmlDoc = doc;
			if(ele!=null)this.XmlEle = ele;
		}

		#region ���з���
		/// <summary>
		/// ����index��ýڵ�ֵ
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
		/// ��������name��ýڵ�ֵ
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
		/// ����index���ýڵ�ֵ
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void SetAttr(int index,string value)
		{
			XmlNode node=this.XmlEle.Attributes.Item(index);
			node.Value=value;
		}

		/// <summary>
		/// ��������name���ýڵ�ֵ
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetAttr(string name,string value)
		{
			this.XmlEle.SetAttribute(name,value);
		}

		/// <summary>
		/// ����indexɾ���ڵ�
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAttr(int index)
		{
			XmlNode node=this.XmlEle.Attributes.Item(index);
			this.XmlEle.Attributes.RemoveNamedItem(node.Name);
		}

		/// <summary>
		/// ��������nameɾ���ڵ�
		/// </summary>
		/// <param name="name"></param>
		public void RemoveAttr(string name)
		{
			this.XmlEle.Attributes.RemoveNamedItem(name);
		}

		/// <summary>
		/// ����index��ýڵ�����
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetAttrName(int index)
		{
			return this.XmlEle.Attributes.Item(index).Name;
		}

		/// <summary>
		/// ������Խڵ�����
		/// </summary>
		/// <returns></returns>
		public int GetAttrCount()
		{
			return this.XmlEle.Attributes.Count;
		}

		/// <summary>
		/// ��ø��ڵ�����
		/// </summary>
		/// <returns></returns>
		public string GetName()
		{
			return this.XmlEle.Name;
		}
		
		/// <summary>
		/// ��ð����������DataStore
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
		/// ��ñ��ڵ��ֵ
		/// </summary>
		/// <returns></returns>
		public string GetValue()
		{
			return DataStore.FilterNull(this.XmlEle.InnerText);
		}

		/// <summary>
		/// ���ñ��ڵ��ֵ
		/// </summary>
		/// <param name="value"></param>
		public void SetValue(string value)
		{
			this.XmlEle.InnerText=value;
		}

		/// <summary>
		/// ��ñ��ڵ��XML
		/// </summary>
		/// <returns></returns>
		public string GetXmlValue()
		{
			return this.XmlEle.InnerXml;
		}

		/// <summary>
		/// ���ñ��ڵ��XML
		/// </summary>
		/// <param name="xmlsrc"></param>
		public void SetXmlValue(string xmlsrc)
		{
			this.XmlEle.InnerXml = xmlsrc;
		}

		/// <summary>
		/// ����ӽڵ㣬ֻ�������һ���ӽڵ�
		/// ���磺һ��DataItem����һ��DataListʱ��ֻ�������һ��DataList�������������������
		/// </summary>
		/// <returns></returns>
		public XmlNode GetSubXmlNode()
		{	if(this.XmlEle.ChildNodes.Count>0)
				return this.XmlEle.ChildNodes[0];
			else 
				return null;
		}

		/// <summary>
		/// �����DataForm����ֻ�������һ���ӽڵ�
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
		/// �����DataList����ֻ�������һ���ӽڵ�
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
		/// �����DataEnum����ֻ�������һ���ӽڵ�
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
		/// �����DataParam����ֻ�������һ���ӽڵ�
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
        ///// ��������ת����ΪStringParam
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
		/// ��������ת����ΪDataForm
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
		/// ��������ת����Ϊ�ַ���
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
