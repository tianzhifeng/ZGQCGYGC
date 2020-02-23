using System;
using System.Xml;


namespace DocSystem.Logic
{
	/// <summary>
	/// DataForm ��ժҪ˵����
	/// </summary>
	public class DataForm:DataNode
	{
		#region ���캯��

		/// <summary>
		/// �յ�DataForm���캯��
		/// </summary>
		public DataForm():base()
		{
			this.Type="Form";
			this.Init();
		}

		/// <summary>
		/// ����XML�ַ�������DataForm
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataForm(string xmlsrc):base(xmlsrc)
		{
			this.Type="Form";
		}

		/// <summary>
		/// ����DataForm�����µ�DataForm
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataForm(DataForm dataForm):base(dataForm.ToString())
		{
			this.Type="Form";
		}
		/// <summary>
		/// ����XmlElement����DataForm
		/// </summary>
		/// <param name="ele"></param>
		public DataForm(XmlElement ele):base(ele)
		{
			this.Type="Form";
		}

		/// <summary>
		/// ����XmlDocument��XmlElement����DataForm
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		internal DataForm(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			this.Type="Form";
		}

		#endregion


		#region ���з���
		/// <summary>
		/// ����ӽڵ�
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
		/// �½��ӽڵ�
		/// </summary>
		/// <returns></returns>
		public new DataItem NewItem()
		{
			return null;
		}

		/// <summary>
		/// ����index��ȡ�ӽڵ��ֵ
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
		/// ����name��ȡ�ӽڵ��ֵ
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
		/// ����name�����ӽڵ��ֵ
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
		/// ����name��ȡ�ӽڵ��XMLֵ
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
		/// ����name�����ӽڵ��XMLֵ
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
		/// ��DataFormת��ΪDataItem
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
		/// ����name��¡DataForm����
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataForm Clone(bool includeChild)
		{
			XmlElement node=(XmlElement)this.CloneNode(includeChild);
			return new DataForm(this.XmlDoc,node);
		}
		/// <summary>
		/// ����name��¡DataForm����
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
