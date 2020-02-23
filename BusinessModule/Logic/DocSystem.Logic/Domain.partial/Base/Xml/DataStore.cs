using System;
using System.Xml;



namespace DocSystem.Logic
{
	/// <summary>
	/// DataStore 的摘要说明。
	/// </summary>
	public class DataStore:DataItem
	{
		
		#region 构造函数
		
		/// <summary>
		/// 空的DataStore构造函数
		/// </summary>
		public DataStore():base(null,null)
		{
			this.XmlDoc = new XmlDocument();
			this.XmlDoc.LoadXml("<DataStore></DataStore>");
			this.XmlEle=this.XmlDoc.DocumentElement;
		}

		/// <summary>
		/// 根据XML字符串构造DataStore
		/// </summary>
		/// <param name="xmlsrc"></param>
		/// <param name="type">字符串格式</param>
		public DataStore(string xmlsrc,string type):base(null,null)
		{
			this.XmlDoc = new XmlDocument();
			string xmlstr="";
			switch(type)
			{
				case "":xmlstr = xmlsrc;break;
				case "b64":xmlstr=FromBase64(xmlsrc);break;
				case "zb64":xmlstr=FromZipBase64(xmlsrc);break;
			}
			this.XmlDoc.LoadXml(xmlsrc);
			this.XmlEle = this.XmlDoc.DocumentElement;
		}
		/// <summary>
		/// 根据XML字符串构造DataStore
		/// </summary>
		/// <param name="xmlsrc"></param>
		/// <param name="type">字符串格式</param>
		public DataStore(string xmlsrc):base(null,null)
		{
			this.XmlDoc = new XmlDocument();
			this.XmlDoc.LoadXml(xmlsrc);
			this.XmlEle = this.XmlDoc.DocumentElement;
		}

		/// <summary>
		/// 根据XML字符串构造DataStore
		/// </summary>
		/// <param name="dataStore"></param>
		public DataStore(DataStore dataStore):base(null,null)
		{
			this.XmlDoc = new XmlDocument();
			this.XmlDoc.LoadXml(dataStore.ToString());
			this.XmlEle = this.XmlDoc.DocumentElement;
		}

		/// <summary>
		/// 根据XmlElement构造DataStore
		/// </summary>
		/// <param name="ele"></param>
		public DataStore(XmlElement ele):base(null,null)
		{
			this.XmlDoc = ele.OwnerDocument ;
			this.XmlEle = ele;
		}

		#endregion
		
		public string this[string paramName]
		{
			get
			{		return this[paramName,null];
			}
			set
			{	this[paramName,null]=value;
			}
		}
		public string this[string paramName,string defValue]
		{
			get
			{
				DataParam dp = (DataParam)this.Get("Param",paramName);
				if(dp == null) 
				{
						if(defValue!=null)
							return defValue;
						else
							return null;
				}
				return dp.GetValue();
			}set
			{	DataParam dp = (DataParam)this.Get("Param",paramName);
				if(dp == null)	
				{
					dp = new DataParam();
					dp.SetName(paramName);
					dp.SetValue(value);
					this.Add(dp);
				}
				else
					dp.SetValue(value);
			}
		}


		#region 公有方法
		
		/// <summary>
		/// 根据index获取DataStore中的数据元素
		/// </summary>
		/// <param name="type"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataNode Get(string type,int index)
		{
			type=type.ToLower();
			switch(type)
			{
				case "form":return this.Forms(index);
				case "list":return this.Lists(index);
				case "enum":return this.Enums(index);
				case "param":return this.Params(index);
			}
			return null;
		}
	
		/// <summary>
		/// 根据name获取DataStore中的数据元素
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataNode Get(string type,string name)
		{
			type=type.ToLower();
			switch(type)
			{
				case "form":return this.Forms(name);
				case "list":return this.Lists(name);
				case "enum":return this.Enums(name);
				case "param":return this.Params(name);
			}
			return null;
		}

		/// <summary>
		/// 根据index获取XML节点
		/// </summary>
		/// <param name="type"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public XmlNode GetXmlNode(string type,int index)
		{
			XmlNode node=null;
			string tag = this.GetTag(type,false);
//			type=this.GetTag(type,true);
			node = this.XmlEle.SelectSingleNode(tag).ChildNodes[index];
			return node;
		}

		/// <summary>
		/// 根据name获取XML节点
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public XmlNode GetXmlNode(string type,string name)
		{
			XmlNode node=null;
			string tag = this.GetTag(type,false);
			type=this.GetTag(type,true);
			node = this.XmlEle.SelectSingleNode(tag + "/" + type + "[@Name='"+name+"']");
			return node;
		}

		/// <summary>
		/// 向本DataStore中添加数据节点
		/// </summary>
		/// <param name="dnode"></param>
		public void Add(DataNode dnode)
		{
			this.Add(dnode,null);	
		}

		public void Add(DataNode dnode,string name)
		{	string type=dnode.Type.ToLower();
			string tag=this.GetTag(type,false);
			XmlNode pnode=this.XmlEle.SelectSingleNode(tag);
			if(pnode==null)
			{	pnode = this.XmlDoc.CreateElement(tag);
				this.XmlEle.AppendChild(pnode);
			}
			XmlElement newItem = this.XmlDoc.CreateElement(this.GetTag(type,true));
			XmlNode node = pnode.AppendChild(newItem);
            Function.CopyXmlAttrs(dnode.XmlEle, newItem);
			newItem.InnerXml = dnode.XmlEle.InnerXml;
			if(name!=null)
				newItem.SetAttribute("Name",name);
		}

		/// <summary>
		/// 在本DataStore中新建数据节点
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public DataNode New(string type)
		{
			type=type.ToLower();
			string tag=this.GetTag(type,false);
			XmlNode pnode=this.XmlEle.SelectSingleNode(tag);
			XmlNode node=null;
			DataNode nodeobj=null;
			if(pnode==null)
			{
				pnode = this.XmlDoc.CreateElement(tag);
				this.XmlEle.AppendChild(pnode);
			}
			switch(type)
			{
					case "form":
					node=this.XmlDoc.CreateElement("Form");
					pnode.AppendChild(node);
					nodeobj = new DataForm(this.XmlDoc,(XmlElement)node);
					break;
				case "list":
					node=(XmlElement)this.XmlDoc.CreateElement("List");
					pnode.AppendChild(node);
					nodeobj = new DataList(this.XmlDoc,(XmlElement)node);
					break;
				case "enum":
					node=(XmlElement)this.XmlDoc.CreateElement("Enum");
					pnode.AppendChild(node);
					nodeobj = new DataEnum(this.XmlDoc,(XmlElement)node);
					break;
				case "param":
					node=(XmlElement)this.XmlDoc.CreateElement("Param");
					pnode.AppendChild(node);
					nodeobj = new DataParam(this.XmlDoc,(XmlElement)node);
					break;
			}
			return nodeobj;
		}

		/// <summary>
		/// 根据index获取一个DataForm
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataForm Forms(int index)
		{
			XmlNode node = this.GetXmlNode("Form",index);
			if(node==null)return null;
			return new DataForm(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 根据name获取一个DataForm
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataForm Forms(string name)
		{
			XmlNode node = this.GetXmlNode("Form",name);
			if(node==null)return null;
			return new DataForm(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 根据index获取一个DataList
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataList Lists(int index)
		{
			XmlNode node = this.GetXmlNode("List",index);
			if(node==null)return null;
			return new DataList(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 根据name获取一个DataList
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataList Lists(string name)
		{
			XmlNode node = this.GetXmlNode("List",name);
			if(node==null)return null;
			return new DataList(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 根据index获取一个DataEnum
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataEnum Enums(int index)
		{
			XmlNode node = this.GetXmlNode("Enum",index);
			if(node==null)return null;
			return new DataEnum(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 根据name获取一个DataEnum
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataEnum Enums(string name)
		{
			XmlNode node = this.GetXmlNode("Enum",name);
			if(node==null)return null;
			return new DataEnum(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 根据index获取一个DataParam
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public DataParam Params(int index)
		{
			XmlNode node = this.GetXmlNode("Param",index);
			if(node==null)return null;
			return new DataParam(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 根据name获取一个DataParam
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public DataParam Params(string name)
		{
			XmlNode node = this.GetXmlNode("Param",name);
			if(node==null)return null;
			return new DataParam(this.XmlDoc,(XmlElement)node);	
		}

		/// <summary>
		/// 删除传入的DataNode
		/// </summary>
		/// <param name="obj"></param>
		public void Remove(DataNode objNode)
		{
			if(this.XmlDoc == objNode.XmlDoc)
				objNode.XmlEle.ParentNode.RemoveChild(objNode.XmlEle);
		}

		/// <summary>
		/// 根据index删除DataNode
		/// </summary>
		/// <param name="type"></param>
		/// <param name="index"></param>
		public void Remove(string type,string name)
		{
			XmlNode node=null;
			node=this.GetXmlNode(type,name);
			if(node!=null)node.ParentNode.RemoveChild(node);
		}

		/// <summary>
		/// 根据index删除DataNode
		/// </summary>
		/// <param name="type"></param>
		/// <param name="index"></param>
		public void Remove(string type,int index)
		{
			XmlNode node=null;
			node=this.GetXmlNode(type,index);
			if(node!=null)node.ParentNode.RemoveChild(node);
		}

		/// <summary>
		/// 清空一个DataStore
		/// </summary>
		public void Clear()
		{
			XmlNodeList nodes=this.XmlEle.ChildNodes;
			XmlNode pnode=this.XmlEle;

			int count=nodes.Count;
			for(int i=count-1;i>=0;i--)
				pnode.RemoveChild(nodes[i]);
		}
		
		/// <summary>
		/// 清空一个DataStore中的一种type的节点
		/// </summary>
		/// <param name="type"></param>
		public void Clear(string type)
		{
			string tag = this.GetTag(type,false);
			XmlNode pnode=this.XmlEle.SelectSingleNode(tag);
			XmlNodeList nodes = pnode.ChildNodes;
			int count=nodes.Count;
			for(int i=count-1;i>=0;i--)
				pnode.RemoveChild(nodes[i]);
		}

		/// <summary>
		/// 字符串ZipBase64编码
		/// </summary>
		/// <returns></returns>
		public string ToZipBase64()
		{
			return this.ToString();
		}

		/// <summary>
		/// 字符串Base64编码
		/// </summary>
		/// <returns></returns>
		public string ToBase64()
		{
			return this.ToString();
		}

		/// <summary>
		/// 继承的克隆节点方法
		/// </summary>
		/// <param name="deep"></param>
		/// <returns></returns>
		public new DataStore  Clone(bool deep)
		{
			return new DataStore(this.XmlEle.CloneNode(deep).OuterXml,"");
		}

		#endregion
		
		public int GetCount()
		{
				return this.GetNodeCount("Form")+this.GetNodeCount("Enum")+this.GetNodeCount("List")+this.GetNodeCount("Param");
		}
		public int GetNodeCount(string nodeType)
		{	string type = nodeType.ToLower();
			DataNodeType ntype=0;
			switch(type)
			{ 
				case "list":
					ntype=DataNodeType.DataList;
					break;
				case "form":
					ntype=DataNodeType.DataForm;
					break;
				case "enum":
					ntype=DataNodeType.DataEnum;
					break;
				case "param":
					ntype=DataNodeType.DataParam;
					break;
			}
			return this.GetNodeCount(ntype);
		}

		public int GetNodeCount(DataNodeType nodeType)
		{
			string xpath="";
			switch(nodeType)
			{ 
				case DataNodeType.DataList:
					xpath="Lists";
					break;
				case DataNodeType.DataForm:
					xpath="Forms";
					break;
				case DataNodeType.DataEnum:
					xpath="Enums";
					break;
				case DataNodeType.DataParam:
					xpath="Params";
					break;
			}
			XmlNode node= this.XmlEle.SelectSingleNode(xpath);
			if(node==null)
				return 0;
			else
				return node.ChildNodes.Count;
		}

		#region 私有方法

		/// <summary>
		/// 格式化一个字符串，将首字母变成大写
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		private string IsFormat(string str)
		{
			return str.Substring(0,1).ToUpper()+str.Substring(1);
		}
		
		/// <summary>
		/// 根据type取得节点标签
		/// </summary>
		/// <param name="type"></param>
		/// <param name="isformat"></param>
		/// <returns></returns>
		private string GetTag(string type,bool isformat)
		{
			type=type.ToLower();
			if(isformat)
				type=IsFormat(type);
		
			switch(type)
			{
				case "form":type="Forms";break;
				case "list":type="Lists";break;
				case "enum":type="Enums";break;
				case "param":type="Params";break;
			}
			return type;
		}

		#endregion

		#region 静态方法
		/// <summary>
		/// 过滤Null
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string FilterNull(string value)
		{
			if(value.ToLower()=="null")
				return "";
			else
				return value;
		}

		/// <summary>
		/// 字符串Base64解码
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string FromBase64(string value)
		{
			return null;
		}
		
		/// <summary>
		/// 字符串ZipBase64解码
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string FromZipBase64(string value)
		{
			return null;
		}


		#endregion
	}
}
