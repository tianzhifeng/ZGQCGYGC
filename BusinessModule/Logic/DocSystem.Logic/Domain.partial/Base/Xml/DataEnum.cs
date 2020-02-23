using System;
using System.Xml;
using System.Web.UI.WebControls;

namespace DocSystem.Logic
{
	/// <summary>
	/// DataEnum ��ժҪ˵����
	/// </summary>
	public class DataEnum:DataList
	{
		#region ���캯��

		/// <summary>
		/// �յ�DataEnum���캯��
		/// </summary>
		public DataEnum():base()
		{
			this.Type="Enum";
			this.Init();
		}

		/// <summary>
		/// ����XML�ַ�������DataEnum
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataEnum(string xmlsrc):base(xmlsrc)
		{
			this.Type="Enum";
		}

		/// <summary>
		/// ����XmlElement����DataEnum
		/// </summary>
		/// <param name="ele"></param>
		public DataEnum(XmlElement ele):base(ele)
		{
			this.Type="Enum";
		}

		/// <summary>
		/// ����DataEnum�����µ�DataEnum
		/// </summary>
		/// <param name="ele"></param>
		public DataEnum(DataEnum dataEnum):base(dataEnum.ToString())
		{
			this.Type="Enum";
		}

		/// <summary>
		/// ����XmlDocument��XmlElement����DataEnum
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		internal  DataEnum(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			this.Type="Enum";
		}
		#endregion

		#region ���з���
		/// <summary>
		/// ����ӽڵ�
		/// </summary>
		/// <param name="text"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public DataItem AddItem(string text,string value)
		{
			XmlElement node=this.XmlDoc.CreateElement("Item");
			this.XmlEle.AppendChild(node);
			node.SetAttribute("Text",text);
			node.SetAttribute("Value",value);
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// �½��ӽڵ�
		/// </summary>
		/// <returns></returns>
		public new DataItem NewItem()
		{
			XmlElement node = this.XmlDoc.CreateElement("Item");
			this.XmlEle.AppendChild(node);
			return new DataItem(this.XmlDoc,node);
		}

		/// <summary>
		/// ����text����ӽڵ��ֵ
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string GetValue(string text)
		{
			XmlElement node=null;
			node = (XmlElement)this.XmlEle.SelectSingleNode("Item[@Text='"+text+"']");
			if(node==null)return null;
			return DataStore.FilterNull(node.GetAttribute("Value"));
		}

		/// <summary>
		/// ���������ӽڵ��ֵ
		/// </summary>
		public void SetValue(){}

		/// <summary>
		/// ����value����ӽڵ��text
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string GetText(string value)
		{
			XmlElement node=null;
			node = (XmlElement)this.XmlEle.SelectSingleNode("Item[@Value='"+value+"']");
			if(node==null)return null;
			return DataStore.FilterNull(node.GetAttribute("Text"));
		}

		/// <summary>
		/// ����index���SubEnum
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetSubEnum(int index)
		{
			XmlElement node;
			node = (XmlElement)this.XmlEle.ChildNodes[index];
			if(node==null)return null;
			string se=null;
			if(node!=null)se = node.GetAttribute("SubEnum");
			if(se.Trim()=="")return null;
			return se;
		}

		/// <summary>
		/// ����ö��ֵvalue���SubEnum
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetSubEnum(string value)
		{
			XmlElement node;
			node = (XmlElement)this.XmlEle.SelectSingleNode("Item[@Value='"+value+"']");
			if(node==null)return null;
			string se=null;
			if(node!=null)se = node.GetAttribute("SubEnum");
			if(se.Trim()=="")return null;
			return se;
		}

		/// <summary>
		/// ����dataItem���SubEnum
		/// </summary>
		/// <param name="dataItem"></param>
		/// <returns></returns>
		public string GetSubEnum(DataItem dataItem)
		{
			XmlElement node;
			node = dataItem.XmlEle;
			if(node==null)return null;
			string se=null;
			if(node!=null)se = node.GetAttribute("SubEnum");
			if(se.Trim()=="") return null;
			return se;
		}
		
		/// <summary>
		/// ��ȡ����������
		/// </summary>
		/// <returns></returns>
		public new string GetFields()
		{
			return null;
		}
		
		/// <summary>
		/// ��������������
		/// </summary>
		public void SetFields(){}

		/// <summary>
		/// ����name��¡DataEnum����
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataEnum Clone(bool includeChild)
		{
			XmlNode node=this.CloneNode(includeChild);
			return new DataEnum(this.XmlDoc,(XmlElement)node);
		}
		/// <summary>
		/// ����name��¡DataEnum����
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataEnum Clone(string name)
		{
			XmlNode node=this.CloneNode(name);
			return new DataEnum(this.XmlDoc,(XmlElement)node);
		}
		#endregion
	}
}
