using System;
using System.Xml;


namespace DocSystem.Logic
{
	/// <summary>
	/// DataForm 的摘要说明。
	/// </summary>
	public class DataForm:DataNode
	{
		#region 构造函数

		/// <summary>
		/// 空的DataForm构造函数
		/// </summary>
		public DataForm():base()
		{
			this.Type="Form";
			this.Init();
		}

		/// <summary>
		/// 根据XML字符串构造DataForm
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataForm(string xmlsrc):base(xmlsrc)
		{
			this.Type="Form";
		}

		/// <summary>
		/// 根据DataForm构造新的DataForm
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataForm(DataForm dataForm):base(dataForm.ToString())
		{
			this.Type="Form";
		}
		/// <summary>
		/// 根据XmlElement构造DataForm
		/// </summary>
		/// <param name="ele"></param>
		public DataForm(XmlElement ele):base(ele)
		{
			this.Type="Form";
		}

		/// <summary>
		/// 根据XmlDocument和XmlElement构造DataForm
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		internal DataForm(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			this.Type="Form";
		}

		#endregion


		#region 共有方法
		/// <summary>
		/// 添加子节点
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public DataItem AddItem(string name,string value)
		{
			XmlElement node=this.XmlDoc.CreateElement(name);
			this.XmlEle.AppendChild(node);
			node.InnerText=value;
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// 新建子节点
		/// </summary>
		/// <returns></returns>
		public new DataItem NewItem()
		{
			return null;
		}

		/// <summary>
		/// 根据index获取子节点的值
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetValue(int index)
		{
			XmlNode node=this.XmlEle.ChildNodes[index];
			if(node==null) return null;
			return DataStore.FilterNull(node.InnerText);
		}

		/// <summary>
		/// 根据name获取子节点的值
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetValue(string name)
		{
			XmlNode node=this.XmlEle.SelectSingleNode(name);
			if(node==null) return null;
			return DataStore.FilterNull(node.InnerText);
		}

		/// <summary>
		/// 根据name设置子节点的值
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetValue(string name,string value)
		{
			XmlNode node=this.XmlEle.SelectSingleNode(name);
			if(node==null)
			{
				node=this.XmlDoc.CreateElement(name);
				this.XmlEle.AppendChild(node);
			}
			node.InnerText=value;
		}

		/// <summary>
		/// 根据name获取子节点的XML值
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetXmlValue(string name)
		{
			XmlNode node=this.XmlEle.SelectSingleNode(name);
			if(node==null) return null;
			return DataStore.FilterNull(node.InnerXml);
		}

		/// <summary>
		/// 根据name设置子节点的XML值
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetXmlValue(string name,string value)
		{
			XmlNode node=this.XmlEle.SelectSingleNode(name);
			if(node==null)
			{
				node=this.XmlDoc.CreateElement(name);
				this.XmlEle.AppendChild(node);
			}
			node.InnerXml=value;
		}

		/// <summary>
		/// 将DataForm转换为DataItem
		/// </summary>
		/// <param name="tag"></param>
		/// <returns></returns>
		public DataItem ToDataItem(string tag)
		{
			if(tag=="")tag="Item";
			XmlElement node=this.XmlDoc.CreateElement(tag);
			XmlNodeList nodes=this.XmlEle.ChildNodes;
			int count = nodes.Count;
			for(int i=0;i<count;i++)
				node.SetAttribute(nodes[i].Name,nodes[i].InnerText);
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// 根据name克隆DataForm对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataForm Clone(bool includeChild)
		{
			XmlElement node=(XmlElement)this.CloneNode(includeChild);
			return new DataForm(this.XmlDoc,node);
		}
		/// <summary>
		/// 根据name克隆DataForm对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataForm Clone(string name)
		{
			XmlElement node=(XmlElement)this.CloneNode(name);
			return new DataForm(this.XmlDoc,node);
		}

		#endregion
	}
}
