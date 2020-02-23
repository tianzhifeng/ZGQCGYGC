using System;
using System.Xml;
using System.Web.UI.WebControls;

namespace DocSystem.Logic
{
	/// <summary>
	/// DataEnum 的摘要说明。
	/// </summary>
	public class DataEnum:DataList
	{
		#region 构造函数

		/// <summary>
		/// 空的DataEnum构造函数
		/// </summary>
		public DataEnum():base()
		{
			this.Type="Enum";
			this.Init();
		}

		/// <summary>
		/// 根据XML字符串构造DataEnum
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataEnum(string xmlsrc):base(xmlsrc)
		{
			this.Type="Enum";
		}

		/// <summary>
		/// 根据XmlElement构造DataEnum
		/// </summary>
		/// <param name="ele"></param>
		public DataEnum(XmlElement ele):base(ele)
		{
			this.Type="Enum";
		}

		/// <summary>
		/// 根据DataEnum构造新的DataEnum
		/// </summary>
		/// <param name="ele"></param>
		public DataEnum(DataEnum dataEnum):base(dataEnum.ToString())
		{
			this.Type="Enum";
		}

		/// <summary>
		/// 根据XmlDocument和XmlElement构造DataEnum
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		internal  DataEnum(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			this.Type="Enum";
		}
		#endregion

		#region 共有方法
		/// <summary>
		/// 添加子节点
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
		/// 根据text获得子节点的值
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
		/// 根据设置子节点的值
		/// </summary>
		public void SetValue(){}

		/// <summary>
		/// 根据value获得子节点的text
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
		/// 根据index获得SubEnum
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
		/// 根据枚举值value获得SubEnum
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
		/// 根据dataItem获得SubEnum
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
		/// 获取数据属性列
		/// </summary>
		/// <returns></returns>
		public new string GetFields()
		{
			return null;
		}
		
		/// <summary>
		/// 设置数据属性列
		/// </summary>
		public void SetFields(){}

		/// <summary>
		/// 根据name克隆DataEnum对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataEnum Clone(bool includeChild)
		{
			XmlNode node=this.CloneNode(includeChild);
			return new DataEnum(this.XmlDoc,(XmlElement)node);
		}
		/// <summary>
		/// 根据name克隆DataEnum对象
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
