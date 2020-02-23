using System;
using System.Xml;
using System.Collections;
namespace DocSystem.Logic
{
	/// <summary>
	/// DataParam ��ժҪ˵����
	/// </summary>
	public class DataParam:DataNode
	{
		#region ���캯��

		/// <summary>
		/// �յ�DataParam���캯��
		/// </summary>
		public DataParam():base()
		{
			this.Type="Param";
			this.Init();
		}

		/// <summary>
		/// ����XML�ַ�������DataParam
		/// </summary>
		/// <param name="xmlsrc"></param>
		public DataParam(string xmlsrc):base(xmlsrc)
		{
			this.Type="Param";
		}

		/// <summary>
		/// ����XmlElement����DataParam
		/// </summary>
		/// <param name="ele"></param>
		public DataParam(XmlElement ele):base(ele)
		{
			this.Type="Param";
		}

		/// <summary>
		/// ����DataParam�����µ�DataParam
		/// </summary>
		/// <param name="ele"></param>
		public DataParam(DataParam dataParam):base(dataParam.ToString())
		{
			this.Type="Param";
		}
		/// <summary>
		/// ����XmlElement����DataParam
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
		/// ����XmlDocument��XmlElement����DataParam
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ele"></param>
		internal  DataParam(XmlDocument doc,XmlElement ele):base(doc,ele)
		{
			this.Type="Param";
		}

		#endregion

		#region ���з���
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="dItem"></param>
		public new DataItem AddItem(DataItem dItem){return null;}
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="index"></param>
		public new void Remove(int index){}
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="index"></param>
		public new void Remove(DataItem obj){}
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="index"></param>
		public new void Clear (){}
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="index"></param>
		public new int GetItemCount(){return 0;}
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="index"></param>
		public new DataItem GetItem(int index){return null;}
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="index"></param>
		public new DataItem GetItem(string xpath){return null;}
		/// <summary>
		/// ���θ���ķ���
		/// </summary>
		/// <param name="index"></param>
		public new ArrayList GetItems(string xpath){return null;}
		/// <summary>
		/// ����name��¡DataList����
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public new DataParam  Clone(bool includeChild)
		{
			XmlElement node = (XmlElement)this.CloneNode(includeChild);
			return new DataParam(this.XmlDoc,node);
		}

		/// <summary>
		/// ����name��¡DataList����
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
