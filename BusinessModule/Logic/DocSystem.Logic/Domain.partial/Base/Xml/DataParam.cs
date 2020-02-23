using System;
using System.Xml;
using System.Collections;
namespace DocSystem.Logic
{
	/// <summary>
	/// DataParam 的摘要说明。
	/// </summary>
	public class DataParam:DataNode
	{
		#region 构造函数

		/// <summary>
		/// 空的DataParam构造函数
		/// </summary>
		public DataParam():base()
		{
			this.Type="Param";
			this.Init();
		}

		/// <summary>
		/// 根据XML字符串构造DataParam
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataParam(string xmlsrc):base(xmlsrc)
		{
			this.Type="Param";
		}

		/// <summary>
		/// 根据XmlElement构造DataParam
		/// </summary>
		/// <param name="ele"></param>
		public DataParam(XmlElement ele):base(ele)
		{
			this.Type="Param";
		}

		/// <summary>
		/// 根据DataParam构造新的DataParam
		/// </summary>
		/// <param name="ele"></param>
		public DataParam(DataParam dataParam):base(dataParam.ToString())
		{
			this.Type="Param";
		}
		/// <summary>
		/// 根据XmlElement构造DataParam
		/// </summary>
		/// <param name="ele"></param>
		public DataParam(string name,string value):base()
		{
			this.Type="Param";
			this.Init();
			this.SetName(name);
			this.SetValue(value);
		}

		/// <summary>
		/// 根据XmlDocument和XmlElement构造DataParam
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		internal  DataParam(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			this.Type="Param";
		}

		#endregion

		#region 公有方法
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="dItem"></param>
		public new DataItem AddItem(DataItem dItem){return null;}
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="index"></param>
		public new void Remove(int index){}
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="index"></param>
		public new void Remove(DataItem obj){}
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="index"></param>
		public new void Clear (){}
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="index"></param>
		public new int GetItemCount(){return 0;}
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="index"></param>
		public new DataItem GetItem(int index){return null;}
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="index"></param>
		public new DataItem GetItem(string xpath){return null;}
		/// <summary>
		/// 屏蔽父类的方法
		/// </summary>
		/// <param name="index"></param>
		public new ArrayList GetItems(string xpath){return null;}
		/// <summary>
		/// 根据name克隆DataList对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataParam  Clone(bool includeChild)
		{
			XmlElement node = (XmlElement)this.CloneNode(includeChild);
			return new DataParam(this.XmlDoc,node);
		}

		/// <summary>
		/// 根据name克隆DataList对象
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataParam Clone(string name)
		{
			XmlElement node = (XmlElement)this.CloneNode(name);
			return new DataParam(this.XmlDoc,node);
		}
		#endregion
	}
}
