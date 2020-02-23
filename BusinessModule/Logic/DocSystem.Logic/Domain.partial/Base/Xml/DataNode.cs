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
	/// DataNode 的摘要说明。
	/// </summary>
	public class DataNode:DataItem
	{
		#region 构造函数
		
		/// <summary>
		/// 空的DataNode构造函数
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
		/// 根据XML字符串构造DataNode
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
		/// 根据XmlElement构造DataNode
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
		/// 根据XmlDocument和XmlElement构造DataNode
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
		
		#region 公有方法
		
		/// <summary>
		/// 添加子节点
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
		/// 新增子节点
		/// </summary>
		public void NewItem(){}
		
		/// <summary>
		/// 根据index删除节点
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			XmlNode node=this.XmlEle.ChildNodes[index];
			if(node!=null)node.ParentNode.RemoveChild(node);
		}

		/// <summary>
		/// 根据传入的DataItem删除节点
		/// </summary>
		/// <param name="obj"></param>
		public void Remove(DataItem obj)
		{
			if(this.XmlEle == obj.XmlEle.ParentNode)
				obj.XmlEle.ParentNode.RemoveChild(obj.XmlEle);
		}

		/// <summary>
		/// 清除此节点下的所有子节点
		/// </summary>
		public void Clear ()
		{
			XmlNodeList nodes=this.XmlEle.ChildNodes;
			int count=nodes.Count;
			for(int i=count-1;i>=0;i--)
				this.XmlEle.RemoveChild(nodes[i]);
		}

		/// <summary>
		/// 获得子节点的数量
		/// </summary>
		/// <returns></returns>
		public int GetItemCount()
		{
			return this.XmlEle.ChildNodes.Count;
		}

		/// <summary>
		/// 根据index获取DataItem
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
		/// 根据XMLxpath获取DataItem
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
		/// 根据index获取DataItem
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
		/// 根据XMLxpath获取多个DataItem
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
		/// 获得节点名称
		/// </summary>
		/// <returns></returns>
		public new string GetName()
		{
			return this.XmlEle.GetAttribute("Name");
		}

		/// <summary>
		/// 设置节电名称
		/// </summary>
		/// <param name="name"></param>
		public void SetName(string name)
		{
			this.XmlEle.SetAttribute("Name",name);
		}

		/// <summary>
		/// 根据节点名称克隆新节点
		/// </summary>
		/// <param name="name"></param>
		public XmlNode CloneNode(bool includeChild)
		{
			XmlElement node = (XmlElement)this.XmlEle.CloneNode(includeChild);
			return node;
		}
		/// <summary>
		/// 根据节点名称克隆新节点
		/// </summary>
		/// <param name="name"></param>
		public XmlNode CloneNode(string name)
		{
			XmlElement node = (XmlElement)this.XmlEle.CloneNode(true);
			node.SetAttribute("Name",name);
			return node;
		}

		/// <summary>
		/// 按照节点标签排序
		/// </summary>
		/// <returns></returns>
		public string Sort()
		{
			return null;
		}

		/// <summary>
		/// 按照节点名称排序
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
