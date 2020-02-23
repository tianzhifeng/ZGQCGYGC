using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Workflow.DTO
{
    public class AbstractDTO : ICloneable
    {
        public string ID { get; set; }

        /// <summary>
        /// 克隆对象，并返回一个已克隆对象的引用
        /// </summary>
        /// <returns>引用新的克隆对象</returns>
        public object Clone()
        {
            //首先我们建立指定类型的一个实例
            object newObject = Activator.CreateInstance(this.GetType());
            //我们取得新的类型实例的字段数组。
            PropertyInfo[] properties = newObject.GetType().GetProperties();
            int i = 0;
            foreach (PropertyInfo fi in this.GetType().GetProperties())
            {
                //我们判断字段是否支持ICloneable接口。
                Type ICloneType = fi.PropertyType.GetInterface("ICloneable", true);
                if (ICloneType != null)
                {
                    //取得对象的Icloneable接口。
                    ICloneable IClone = (ICloneable)fi.GetValue(this, null);
                    if (IClone != null)
                        //我们使用克隆方法给字段设定新值。
                        properties[i].SetValue(newObject, IClone.Clone(), null);
                }
                //                 else if (fi == this.GetType().GetProperty("XRefs") || fi == this.GetType().GetProperty("Useds"))
                //                 {
                //  
                // 
                //                 }
                else
                {
                    // 如果该字段部支持Icloneable接口，直接设置即可。
                    object obj = fi.GetValue(this, null);
                    if (obj != null)
                        properties[i].SetValue(newObject, obj, null);
                }
                i++;
                //现在我们检查该对象是否支持IEnumerable接口，如果支持，
                //我们还需要枚举其所有项并检查他们是否支持IList 或 IDictionary 接口。
                //                 Type IEnumerableType = fi.PropertyType.GetInterface("IEnumerable", true);
                //                 if (IEnumerableType != null)
                //                 {
                //                     //取得该字段的IEnumerable接口
                //                     IEnumerable IEnum = (IEnumerable)fi.GetValue(this, null);
                //                     Type IListType = properties[i].PropertyType.GetInterface("IList", true);
                //                     Type IDicType = properties[i].PropertyType.GetInterface("IDictionary", true);
                //                     int j = 0;
                //                     if (IListType != null)
                //                     {
                //                         //取得IList接口。
                //                         IList list = (IList)properties[i].GetValue(newObject, null);
                //                         foreach (object obj in IEnum)
                //                         {
                //                             //查看当前项是否支持支持ICloneable 接口。
                //                             ICloneType = obj.GetType().GetInterface("ICloneable", true);
                //                             if (ICloneType != null)
                //                             {
                //                                 //如果支持ICloneable 接口，
                //                                 //我们用它李设置列表中的对象的克隆
                //                                 ICloneable clone = (ICloneable)obj;
                //                                 list[j] = clone.Clone();
                //                             }
                //                             //注意：如果列表中的项不支持ICloneable接口，那么
                //                             //在克隆列表的项将与原列表对应项相同
                //                             //（只要该类型是引用类型）
                //                             j++;
                //                         }
                //                     }
                //                     else if (IDicType != null)
                //                     {
                //                         //取得IDictionary 接口
                //                         IDictionary dic = (IDictionary)properties[i].GetValue(newObject, null);
                //                         j = 0;
                //                         foreach (DictionaryEntry de in IEnum)
                //                         {
                //                             //查看当前项是否支持支持ICloneable 接口。
                //                             ICloneType = de.Value.GetType().
                //                             GetInterface("ICloneable", true);
                //                             if (ICloneType != null)
                //                             {
                //                                 ICloneable clone = (ICloneable)de.Value;
                //                                 dic[de.Key] = clone.Clone();
                //                             }
                //                             j++;
                //                         }
                //                     }
                //                 }
                //                 i++;
            }

            return newObject;
        }
        public static T Clone<T>(T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization序列化与反序列化完成引用对象的复制  
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }
    }
}
