using System;
using System.Xml;
using System.Collections;


namespace DocSystem.Logic
{
	public enum DataNodeType
	{	DataForm=1,
		DataList=2,
		DataEnum=3,
		DataParam=4
	}
	/// <summary>
	/// DataNode ��ժҪ˵����
	/// </summary>
	public class DataNode:DataItem
	{
		#region ���캯��
		
		/// <summary>
		/// �յ�DataNode���캯��
		/// </summary>
		protected DataNode():base(null,null)
		{
			this.Type="Node";
		}
		
		protected void Init()
		{
			this.XmlDoc = new XmlDocument();
			switch(this.Type.ToLower())
			{
				case "form":this.XmlDoc.LoadXml("<Form Name='' ></Form>");break;
				case "list":this.XmlDoc.LoadXml("<List Name='' Fields=''></List>");break;
				case "enum":this.XmlDoc.LoadXml("<Enum Name='' ></Enum>");break;
				case "param":this.XmlDoc.LoadXml("<Param Name=''></Param>");break;
			}
			this.XmlEle=this.XmlDoc.DocumentElement;
		}

		/// <summary>
		/// ����XML�ַ�������DataNode
		/// </summary>
		/// <param name="xmlsrc"></param>
		protected DataNode(string xmlsrc):base(null,null)
		{
			this.Type="Node";
			this.XmlDoc = new XmlDocument();
			this.XmlDoc.LoadXml(xmlsrc);
			this.XmlEle = this.XmlDoc.DocumentElement;
		}

		/// <summary>
		/// ����XmlElement����DataNode
		/// </summary>
		/// <param name="ele"></param>
		protected DataNode(XmlElement ele):base(null,null)
		{
//			this.Type="Node";
			XmlNode node=ele.CloneNode(true);
			this.XmlDoc = node.OwnerDocument;
			this.XmlEle = (XmlElement)node;
		}

		/// <summary>
		/// ����XmlDocument��XmlElement����DataNode
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		protected DataNode(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			if(doc!=null)this.XmlDoc=doc;
			if(ele!=null)this.XmlEle=ele;
		}

		#endregion

		//----------------------------------------------------------------------
		
		#region ���з���
		
		/// <summary>
		/// ����ӽڵ�
		/// </summary>
		public DataItem AddItem(DataItem dItem)
		{
			XmlElement newItem = this.XmlDoc.CreateElement("Item");
			XmlNode node = this.XmlEle.AppendChild(newItem);
            Function.CopyXmlAttrs(dItem.XmlEle, newItem);
			newItem.InnerXml = dItem.XmlEle.InnerXml;
			dItem.XmlDoc = this.XmlDoc;
			dItem.XmlEle = newItem;
			return dItem;
		}

		/// <summary>
		/// �����ӽڵ�
		/// </summary>
		public void NewItem(){}
		
		/// <summary>
		/// ����indexɾ���ڵ�
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			XmlNode node=this.XmlEle.ChildNodes[index];
			if(node!=null)node.ParentNode.RemoveChild(node);
		}

		/// <summary>
		/// ���ݴ����DataItemɾ���ڵ�
		/// </summary>
		/// <param name="obj"></param>
		public void Remove(DataItem obj)
		{
			if(this.XmlEle == obj.XmlEle.ParentNode)
				obj.XmlEle.ParentNode.RemoveChild(obj.XmlEle);
		}

		/// <summary>
		/// ����˽ڵ��µ������ӽڵ�
		/// </summary>
		public void Clear ()
		{
			XmlNodeList nodes=this.XmlEle.ChildNodes;
			int count=nodes.Count;
			for(int i=count-1;i>=0;i--)
				this.XmlEle.RemoveChild(nodes[i]);
		}

		/// <summary>
		/// ����ӽڵ������
		/// </summary>
		/// <returns></returns>
		public int GetItemCount()
		{
			return this.XmlEle.ChildNodes.Count;
		}

		/// <summary>
		/// ����index��ȡDataItem
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataItem GetItem(int index)
		{
			XmlElement node=null;
			node = (XmlElement)this.XmlEle.ChildNodes.Item(index);
			if(node==null)return null;
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// ����XMLxpath��ȡDataItem
		/// </summary>
		/// <param name="xpath"></param>
		/// <returns></returns>
		public DataItem GetItem(string xpath)
		{
			XmlElement node=null;
			node = (XmlElement)this.XmlEle.SelectSingleNode(xpath);
			if(node==null)return null;
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// ����index��ȡDataItem
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataItem GetItem(string attrName,string attrValue)
		{
			XmlElement node=null;
			node = (XmlElement)this.XmlEle.SelectSingleNode("*[@"+attrName+"='"+attrValue+"']");
			if(node==null)return null;
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// ����XMLxpath��ȡ���DataItem
		/// </summary>
		/// <param name="xpath"></param>
		/// <returns></returns>
		public ArrayList GetItems(string xpath)
		{
			XmlNodeList nodes=null;
			if(xpath=="")
				nodes= this.XmlEle.ChildNodes;
			else
				nodes=this.XmlEle.SelectNodes(xpath);
			ArrayList diary = new ArrayList(nodes.Count);
			for(int i=0;i<nodes.Count;i++)
				diary.Add(new DataItem(this.XmlDoc,(XmlElement)nodes[i]));
			return diary;
		}

		/// <summary>
		/// ��ýڵ�����
		/// </summary>
		/// <returns></returns>
		public new string GetName()
		{
			return this.XmlEle.GetAttribute("Name");
		}

		/// <summary>
		/// ���ýڵ�����
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			this.XmlEle.SetAttribute("Name",name);
		}

		/// <summary>
		/// ���ݽڵ����ƿ�¡�½ڵ�
		/// </summary>
		/// <param name="name"></param>
		public XmlNode CloneNode(bool includeChild)
		{
			XmlElement node = (XmlElement)this.XmlEle.CloneNode(includeChild);
			return node;
		}
		/// <summary>
		/// ���ݽڵ����ƿ�¡�½ڵ�
		/// </summary>
		/// <param name="name"></param>
		public XmlNode CloneNode(string name)
		{
			XmlElement node = (XmlElement)this.XmlEle.CloneNode(true);
			node.SetAttribute("Name",name);
			return node;
		}

		/// <summary>
		/// ���սڵ��ǩ����
		/// </summary>
		/// <returns></returns>
		public string Sort()
		{
			return null;
		}

		/// <summary>
		/// ���սڵ���������
		/// </summary>
		/// <param name="attrName"></param>
		/// <param name="dataType"></param>
		/// <returns></returns>
		public string Sort(string name,string dataType)
		{
			return null;
		}

		#endregion
	}
}
