using Config;
using Formula;
using Formula.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Workflow.Logic;
using System.Collections;
using System.IO;
using EPC.Logic.Domain;

namespace EPC.Areas.Manage.Controllers
{
    public class NetWorkController : EPCController<S_P_NetWork>
    {
        //
        // GET: /Basic/NetWork/
        public ActionResult Svg()
        {
            string projectID = Convert.ToString(Request["ProjectID"]);
            ViewBag.workJson = getNetWork(Request["beginWorkTime"], projectID);
            ViewBag.relation = JsonHelper.ToJson(nodeList);
            if (!string.IsNullOrEmpty(projectID))
            {
                var item = entities.Set<S_I_Engineering>().Where(o => o.ID == projectID).SingleOrDefault();
                ViewBag.title = item != null ? item.Name : "";
            }
            return View();
        }

        List<MainWork> list = new List<MainWork>();
        private void setList(string code, string path, Double workTime)
        {
            list.Remove(list.Where(c => c.Code == code).SingleOrDefault());
            MainWork mainWork = new MainWork();
            mainWork.Code = code;
            mainWork.Path = path;
            mainWork.WorkTime = Math.Abs(workTime);
            list.Add(mainWork);
        }

        List<NodeY> nodeList = new List<NodeY>();
        private void setNodeList(int number, string precedence, string tight, int precedenceWork, Double Y)
        {
            nodeList.Remove(nodeList.Where(c => c.Number == number).SingleOrDefault());
            NodeY node = new NodeY();
            node.Number = number;
            node.Precedence = precedence;
            node.Tight = tight;
            node.PrecedenceWork = precedenceWork;
            node.Y = Y;
            nodeList.Add(node);
        }
        private NetWork getNetWork(S_P_NetWork item)
        {
            NetWork work = new NetWork();
            work.ID = item.ID;
            work.Code = item.Code.ToUpper();
            work.Name = item.Name;
            work.Precedence = !string.IsNullOrEmpty(item.Precedence) ? item.Precedence.ToUpper() : "";
            work.Time = Math.Abs(Convert.ToDouble(item.Time));
            work.VirtualTime = Math.Abs(Convert.ToDouble(item.VirtualTime));
            return work;
        }

        private void setNodeY(int nodeID, Double Y)
        {
            nodeList.Where(c => c.Number == nodeID).SingleOrDefault().Y = Y;
        }

        private Double getNodeY(int nodeID)
        {
            return nodeList.Where(c => c.Number == nodeID).SingleOrDefault().Y;
        }

        private NodeY getNode(int nodeID)
        {
            return nodeList.Where(c => c.Number == nodeID).SingleOrDefault();
        }

        Dictionary<string, int> isCalcNode = new Dictionary<string, int>();
        private void setCalcNode(int key, int value)
        {
            string keys = key.ToString() + "." + value.ToString();
            if (!isCalcNode.ContainsKey(keys))
            {
                isCalcNode.Add(keys, value);
            }
        }
        private Boolean getCalcNode(int nodeID, int type)
        {
            string keys = nodeID.ToString() + "." + type.ToString();
            return isCalcNode.Where(c => c.Key == keys && c.Value == type).ToArray().Length > 0 ? true : false;
        }

        /*前编号算法：无紧前则编号为1，有唯一紧前则后编号为前编号，否则取最大小时对应的编号
        后编号算法：无紧前则编号为MAX+1，有两个以上则按工时编号，工时小的编号小。有唯一紧前则以紧前
        的后编号为前编号，按时间MAX+1。*/
        private void calcNode(Dictionary<string, NetWork> table, DateTime beginTime)
        {
            table = table.OrderBy(c => c.Value.EndTime).ToDictionary(c => c.Key, o => o.Value);
            int maxNum = 2;
            foreach (KeyValuePair<string, NetWork> work in table)
            {
                work.Value.Last = maxNum;
                maxNum = maxNum + 1;
            }
            //无紧前算法
            var netWork = table.Where(c => c.Value.Precedence == "" || c.Value.Precedence == null).ToArray();
            var single = table.Where(c => c.Value.Precedence.Split(',').Length == 1 && c.Value.Precedence != "").OrderBy(o => o.Value.Time).ToArray();
            var multi = table.Where(c => c.Value.Precedence.Split(',').Length > 1).ToArray();
            if (netWork.Length > 0)
            {
                foreach (KeyValuePair<string, NetWork> item in netWork)
                {
                    NetWork work = item.Value;
                    work.First = 1;
                    setList(work.Code, work.First + "-" + work.Last, work.Time);
                }
            }

            //唯一紧前(前工作的紧前没有多个)
            if (single.Length > 0)
            {
                foreach (KeyValuePair<string, NetWork> sing in single.OrderBy(o => o.Value.BeginTime))
                {
                    NetWork work = sing.Value;
                    foreach (KeyValuePair<string, NetWork> item in table.OrderBy(o => o.Value.BeginTime))
                    {
                        if (item.Value.Code == work.Precedence && item.Value.Precedence.Split(',').Length <= 1)
                        {
                            work.First = item.Value.Last;
                            MainWork main = list.Where(c => c.Code == item.Value.Code).SingleOrDefault();
                            if (main != null && !string.IsNullOrEmpty(main.Code))
                            {
                                string path = string.IsNullOrEmpty(main.Path) ? work.Last.ToString() : main.Path + "-" + work.Last.ToString();
                                setList(work.Code, path, main.WorkTime + work.Time);
                            }
                        }
                    }
                }
            }

            //多个紧前
            if (multi.Length > 0)
            {
                foreach (KeyValuePair<string, NetWork> mul in multi)
                {
                    int nultiMax = 0;
                    DateTime maxTime = beginTime;
                    NetWork work = mul.Value;
                    string[] pres = work.Precedence.Split(',');
                    foreach (string pre in pres)
                    {
                        foreach (KeyValuePair<string, NetWork> item in table)
                        {
                            if (item.Value.Code == pre)
                            {
                                if (maxTime < item.Value.EndTime)
                                {
                                    maxTime = item.Value.EndTime;
                                    nultiMax = item.Value.Last;

                                    MainWork main = list.Where(c => c.Code == item.Value.Code).SingleOrDefault();
                                    if (main != null && !string.IsNullOrEmpty(main.Code))
                                    {
                                        string path = string.IsNullOrEmpty(main.Path) ? work.Last.ToString() : main.Path + "-" + work.Last.ToString();
                                        setList(work.Code, path, main.WorkTime + work.Time);
                                    }
                                }
                            }
                        }
                    }
                    work.First = nultiMax;

                    //再次处理唯一紧前(紧前工作紧前多个)
                    if (single.Length > 0)
                    {
                        foreach (KeyValuePair<string, NetWork> sing in single)
                        {
                            NetWork works = sing.Value;
                            foreach (KeyValuePair<string, NetWork> item in table)
                            {
                                if (item.Value.Code == works.Precedence && item.Value.Precedence.Split(',').Length > 1 && mul.Value.Code == item.Value.Code)
                                {
                                    works.First = item.Value.Last;
                                }
                            }
                        }
                    }
                }
            }

        }

        //计算虚工作：查找存在多个紧前工作的节点，循环每个节点的紧前工作是否存在实工作，不存在则是虚工作
        private void virtualWork(Dictionary<string, NetWork> table)
        {
            foreach (KeyValuePair<string, NetWork> virtualWork in table.Where(c => c.Value.Precedence.Split(',').Length > 1))
            {
                Double time = 0;
                NetWork work = virtualWork.Value;
                string[] pres = work.Precedence.Split(',');
                foreach (string pre in pres)
                {
                    foreach (KeyValuePair<string, NetWork> item in table)
                    {
                        if (item.Value.Code.Trim() == pre.Trim())
                        {
                            MainWork main = list.Where(c => c.Code == item.Value.Code).SingleOrDefault();
                            if (item.Value.Last != work.First)
                            {
                                Boolean isExistWork = false;
                                foreach (KeyValuePair<string, NetWork> vir in table)
                                {
                                    if (item.Value.Last == vir.Value.First && work.First == vir.Value.Last)
                                    {
                                        isExistWork = true;
                                        break;
                                    }
                                }
                                if (!isExistWork)
                                {
                                    string virt = item.Value.Last.ToString() + ":" + work.First.ToString();
                                    work.VirtualWork = !string.IsNullOrEmpty(work.VirtualWork) ? work.VirtualWork + "," + virt : virt;

                                    if (main != null && !string.IsNullOrEmpty(main.Code))
                                    {
                                        if (main.WorkTime > time)
                                        {
                                            time = main.WorkTime;
                                            string path = string.IsNullOrEmpty(main.Path) ? work.First.ToString() : main.Path + "-" + work.First.ToString();
                                            setList(work.Code, path, main.WorkTime + work.Time);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //算实工作
                                if (main != null && !string.IsNullOrEmpty(main.Code))
                                {
                                    if (main.WorkTime + work.Time > time)
                                    {
                                        string path = string.IsNullOrEmpty(main.Path) ? work.Last.ToString() : main.Path + "-" + work.Last.ToString();
                                        time = main.WorkTime + work.Time;
                                        setList(work.Code, path, time);
                                    }
                                }
                            }
                        }
                    }
                }
                //再次处理唯一紧前(紧前工作紧前多个)
                var single = table.Where(c => c.Value.Precedence.Split(',').Length == 1 && c.Value.Precedence != "").OrderBy(o => o.Value.Time).ToArray();
                var multi = table.Where(c => c.Value.Precedence.Split(',').Length > 1).ToArray();
                if (multi.Length > 0)
                {
                    foreach (KeyValuePair<string, NetWork> mul in multi)
                    {
                        if (single.Length > 0)
                        {
                            foreach (KeyValuePair<string, NetWork> sing in single.OrderBy(o => o.Value.BeginTime))
                            {
                                NetWork works = sing.Value;
                                foreach (KeyValuePair<string, NetWork> item in table.OrderBy(o => o.Value.BeginTime))
                                {
                                    if (item.Value.Code == works.Precedence && item.Value.Precedence.Split(',').Length > 1 && mul.Value.Code == item.Value.Code)
                                    {
                                        MainWork main = list.Where(c => c.Code == item.Value.Code).SingleOrDefault();
                                        if (main != null && !string.IsNullOrEmpty(main.Code))
                                        {
                                            string path = string.IsNullOrEmpty(main.Path) ? works.Last.ToString() : main.Path + "-" + works.Last.ToString();
                                            setList(works.Code, path, main.WorkTime + works.Time);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //处理没有线路的路径
                foreach (KeyValuePair<string, NetWork> tb in table)
                {
                    NetWork works = tb.Value;
                    if (list.Where(c => c.Code == tb.Value.Code).Count() == 0)
                    {
                        foreach (KeyValuePair<string, NetWork> item in table.OrderBy(o => o.Value.BeginTime))
                        {
                            if (item.Value.Code == works.Precedence)
                            {
                                if (item.Value.Precedence.Split(',').Length == 1)
                                {
                                    MainWork main = list.Where(c => c.Code == item.Value.Code).SingleOrDefault();
                                    if (main != null && !string.IsNullOrEmpty(main.Code))
                                    {
                                        string path = string.IsNullOrEmpty(main.Path) ? works.Last.ToString() : main.Path + "-" + works.Last.ToString();
                                        setList(works.Code, path, main.WorkTime + works.Time);
                                    }
                                }
                                else {
                                    double max = 0;
                                    MainWork main = null;
                                    string[] strs = item.Value.Precedence.Split(',');
                                    foreach (string pre in strs)
                                    {
                                        var ls = list.SingleOrDefault(c => c.Code == pre);
                                        if (ls != null)
                                        {
                                            if (max < ls.WorkTime)
                                            {
                                                max = ls.WorkTime;
                                                main = ls;
                                            }
                                        }            
                                    }
                                    if (main != null && !string.IsNullOrEmpty(main.Code))
                                    {
                                        string path = string.IsNullOrEmpty(main.Path) ? works.Last.ToString() : main.Path + "-" + works.Last.ToString();
                                        setList(works.Code, path, main.WorkTime + works.Time);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            int maxNode = Convert.ToInt32(nodeList.Max(o => o.Number));
            foreach (KeyValuePair<string, NetWork> item in table)
            {
                NetWork work = item.Value;
                if (item.Value.Code.Contains("_@VIR"))
                {
                    string vir = work.First + ":" + work.Last;
                    work.VirtualWork = string.IsNullOrEmpty(work.VirtualWork) ? vir : work.VirtualWork + "," + vir;
                    work.IsVir = true;
                }
                if (item.Value.Precedence.Contains("_@VIR") && work.Last != maxNode)
                {
                    bool isVirExist = false;
                    var precedence = table.Where(o => o.Value.Precedence.Contains(item.Value.Code)).ToArray();
                    foreach (var pre in precedence)
                    {
                        foreach (string str in pre.Value.Precedence.Split(','))
                        {
                            if (str == item.Value.Code && !str.Contains("_@VIR"))
                            {
                                isVirExist = true;
                            }
                        }
                    }
                    if (!isVirExist)
                    {
                        work.VirtualWork = work.Last + ":" + maxNode;
                    }
                }
                if (table.Where(o => o.Value.Precedence.Contains(work.Code)).ToArray().Length <= 0 && work.Last != maxNode)
                {
                    work.VirtualWork = work.Last + ":" + maxNode;
                }
            }
        }


        /*开始时间算法：无紧前则开始工作时间，只有一个紧前开始时间+1+工时，否则紧前最大时间+1+工时
        结束时间算法：开始时间+工时 */
        private void calcTime(Dictionary<string, NetWork> table, DateTime beginTime)
        {
            foreach (KeyValuePair<string, NetWork> item in table)
            {
                NetWork work = item.Value;
                if (!string.IsNullOrEmpty(work.ID))
                {
                    if (string.IsNullOrEmpty(work.Precedence))
                    {
                        Double day = Convert.ToDouble(work.Time);
                        work.BeginTime = beginTime;
                        work.EndTime = beginTime.AddDays(day).AddDays(-1);
                    }
                }
            }

            while (table.Where(c => c.Value.EndTime.Year == 0001).Count() > 0)
            {
                foreach (KeyValuePair<string, NetWork> item in table.Where(c => c.Value.EndTime != beginTime && c.Value.EndTime.Year == 0001))
                {
                    NetWork work = item.Value;
                    if (!string.IsNullOrEmpty(work.ID))
                    {
                        if (!string.IsNullOrEmpty(work.Precedence))
                        {
                            string[] pres = work.Precedence.Split(',');
                            DateTime maxTime = beginTime;
                            beginTime = beginTime.AddDays(-1);
                            if (pres.Length > 0)
                            {
                                bool isTrue = true;
                                foreach (string pre in pres)
                                {
                                    foreach (KeyValuePair<string, NetWork> p in table)
                                    {
                                        if (p.Value.Code.Trim() == pre.Trim())
                                        {
                                            if (p.Value.EndTime.Year == 0001)
                                                isTrue = false;
                                            if (maxTime < p.Value.EndTime)
                                            {
                                                maxTime = p.Value.EndTime;
                                            }
                                        }
                                    }
                                }
                                if (maxTime != beginTime && isTrue)
                                {
                                    work.BeginTime = maxTime.AddDays(1);
                                    Double day = Convert.ToDouble(work.Time);
                                    work.EndTime = maxTime.AddDays(day);
                                }
                            }
                        }
                    }
                }
            }
        }

        Double DFY = 50; //每层先设定一个默认高度
        List<string> level = new List<string>();
        private void getLevel(string nodeID)
        {
            var list = nodeList.Where(c => c.Number == Convert.ToInt32(nodeID)).SingleOrDefault();
            if (!level.Contains(nodeID))
            {
                level.Add(nodeID);
            }
            if (list.PrecedenceWork != 1 && !string.IsNullOrEmpty(Convert.ToString(list.PrecedenceWork)))
            {
                getLevel(list.PrecedenceWork.ToString());
            }
        }

        //查询从节点N的所有紧后
        List<string> tightAll = null;
        private void findAllTight(string nodeID)
        {
            if (!tightAll.Contains(nodeID))
            {
                tightAll.Add(nodeID);
            }
            var list = nodeList.Where(c => c.Number == Convert.ToInt32(nodeID)).SingleOrDefault();
            foreach (string node in list.Tight.Split(','))
            {
                if (!tightAll.Contains(nodeID))
                {
                    tightAll.Add(node);
                }
                if (!string.IsNullOrEmpty(list.Tight))
                {
                    findAllTight(node);
                }
            }

        }

        private void setFirstNodeY(NodeY node, int type)
        {
            if (type == 3)
            {
                setNodeY(node.Number, getNodeY(node.PrecedenceWork) + DFY);

                if (node.PrecedenceWork == 1 || nodeList.Where(c => c.Number == node.PrecedenceWork).SingleOrDefault().Precedence == "1")
                {
                    tightAll = new List<string>();
                    findAllTight(Convert.ToString(node.PrecedenceWork));
                }
                string keys = node.PrecedenceWork.ToString() + "." + type.ToString();

                for (int i = node.Number - 1; i > 0; i--)
                {
                    if (!tightAll.Contains(i.ToString()) && getNodeY(node.Number) <= getNodeY(i))
                    {
                        setNodeY(i, getNodeY(i) + DFY);
                    }
                }
                setCalcNode(node.PrecedenceWork, type);
            }
            else if (type == 1)
            {
                setNodeY(node.Number, getNodeY(node.PrecedenceWork));
                for (int i = node.Number - 1; i > 0; i--)
                {
                    setNodeY(i, getNodeY(i) + DFY);
                }
                setCalcNode(node.PrecedenceWork, type);
            }
        }

        //计算节点Y轴坐标
        private void calcNodeY(Dictionary<string, NetWork> table)
        {
            //把各节点的紧前、后关系填写到对象中，包括虚工作  
            string firstNode = "";
            foreach (KeyValuePair<string, NetWork> item in table)
            {
                NetWork work = item.Value;
                string precedence = work.First.ToString(), tight = "";
                if (work.First == 1)
                    firstNode = string.IsNullOrEmpty(firstNode) ? work.Last.ToString() : firstNode + "," + work.Last.ToString();
                foreach (KeyValuePair<string, NetWork> items in table)
                {
                    if (work.Last == items.Value.First)
                    {
                        tight = string.IsNullOrEmpty(tight) ? items.Value.Last.ToString() : tight + "," + items.Value.Last.ToString();
                    }

                }
                setNodeList(work.Last, precedence, tight, work.First, 0);
            }
            setNodeList(1, "", firstNode, 0, 0);

        }


        private string getType(string nl, int startNode, int endNode)
        {
            Double val = 0;
            string type = "";
            string[] main = null;
            foreach (MainWork mainWork in list)
            {
                if (val < mainWork.WorkTime)
                {
                    val = mainWork.WorkTime;
                    main = mainWork.Path.Split('-');
                }
            }
            if (nl == "node")
            {
                if (main != null && main.Length > 0)
                {
                    foreach (string s in main)
                    {
                        if (Convert.ToInt32(s) == startNode)
                        {
                            type = "main";
                        }
                    }
                }
            }
            else if (nl == "path")
            {
                if (main != null && main.Length > 0 && main.Contains(startNode.ToString()) && main.Contains(endNode.ToString()))
                {
                    type = "main";
                }
            }
            else if (nl == "virtual")
            {
                if (main != null && main.Length > 0 && main.Contains(startNode.ToString()) && main.Contains(endNode.ToString()))
                {
                    type = "d&m";
                }
                else
                {
                    type = "dashed";
                }
            }

            return type;
        }

        //生成JSON
        public string getNetWork(string beginWorkTime, string projectID)
        {
            string def = "{\"nodes\":[{\"id\":\"1\",\"code\":\"1\",\"type\":\"main\",\"y\":330,\"datatime\":\"2011-01-03\"},{\"id\":\"2\",\"code\":\"2\",\"y\":200,\"type\":\"main\",\"datatime\":\"2011-01-06\"},{\"id\":\"3\",\"code\":\"3\",\"type\":\"main\",\"y\":100,\"datatime\":\"2011-02-05\"},{\"id\":\"5\",\"code\":\"5\",\"y\":200,\"datatime\":\"2011-02-05\"},{\"id\":\"4\",\"code\":\"4\",\"y\":300,\"datatime\":\"2011-02-05\"},{\"id\":\"6\",\"code\":\"6\",\"y\":400,\"datatime\":\"2011-01-06\"},{\"id\":\"7\",\"code\":\"7\",\"y\":400,\"datatime\":\"2011-02-05\"},{\"id\":\"9\",\"code\":\"9\",\"y\":200,\"datatime\":\"2011-02-28\"},{\"id\":\"8\",\"code\":\"8\",\"y\":400,\"datatime\":\"2011-02-28\"},{\"id\":\"10\",\"code\":\"10\",\"y\":560,\"datatime\":\"2011-02-28\"},{\"id\":\"11\",\"code\":\"11\",\"y\":200,\"type\":\"main\",\"datatime\":\"2011-03-03\"},{\"id\":\"12\",\"code\":\"12\",\"y\":200,\"type\":\"main\",\"datatime\":\"2011-04-15\"},{\"id\":\"13\",\"code\":\"13\",\"y\":200,\"type\":\"main\",\"datatime\":\"2011-04-29\"},{\"id\":\"15\",\"code\":\"15\",\"type\":\"main\",\"y\":200,\"datatime\":\"2011-06-20\"},{\"id\":\"14\",\"code\":\"14\",\"y\":300,\"datatime\":\"2011-06-20\"},{\"id\":\"16\",\"code\":\"16\",\"type\":\"main\",\"y\":200,\"datatime\":\"2011-09-08\"}],\"lines\":[{\"from\":\"1\",\"to\":\"2\",\"name\":\"A\",\"type\":\"main\",\"time\":\"2\"},{\"from\":\"1\",\"to\":\"6\",\"name\":\"D\",\"time\":\"2\"},{\"from\":\"2\",\"to\":\"3\",\"name\":\"C\",\"type\":\"main\",\"time\":\"3\"},{\"from\":\"2\",\"to\":\"4\",\"name\":\"B\",\"time\":\"2\"},{\"from\":\"6\",\"to\":\"7\",\"name\":\"\",\"type\":\"dashed\",\"time\":\"7\"},{\"from\":\"6\",\"to\":\"10\",\"name\":\"G\",\"time\":\"2\"},{\"from\":\"3\",\"to\":\"5\",\"name\":\"\",\"type\":\"dashed\",\"time\":\"1\"},{\"from\":\"5\",\"to\":\"4\",\"name\":\"\",\"type\":\"dashed\",\"time\":\"2\"},{\"from\":\"4\",\"to\":\"7\",\"name\":\"\",\"type\":\"dashed\",\"time\":\"25\"},{\"from\":\"3\",\"to\":\"11\",\"name\":\"I\",\"type\":\"main\",\"time\":\"6\"},{\"from\":\"5\",\"to\":\"9\",\"name\":\"E\",\"time\":\"2\"},{\"from\":\"7\",\"to\":\"8\",\"name\":\"F\",\"time\":\"2\"},{\"from\":\"9\",\"to\":\"11\",\"name\":\"H\",\"time\":\"2\"},{\"from\":\"10\",\"to\":\"12\",\"name\":\"K\",\"type\":\"dashed\",\"time\":\"1\"},{\"from\":\"11\",\"to\":\"12\",\"name\":\"J\",\"type\":\"main\",\"time\":\"2\"},{\"from\":\"12\",\"to\":\"13\",\"name\":\"L\",\"type\":\"main\",\"time\":\"3\"},{\"from\":\"13\",\"to\":\"15\",\"name\":\"N\",\"type\":\"main\",\"time\":\"2\"},{\"from\":\"13\",\"to\":\"14\",\"name\":\"M\",\"time\":\"1.5\"},{\"from\":\"14\",\"to\":\"15\",\"name\":\"\",\"type\":\"dashed\",\"time\":\"2\"},{\"from\":\"15\",\"to\":\"16\",\"name\":\"P\",\"type\":\"main\",\"time\":\"2\"},{\"from\":\"8\",\"to\":\"10\",\"name\":\"J\",\"type\":\"dashed\",\"time\":\"2\"}]}";
            if (string.IsNullOrEmpty(projectID) || string.IsNullOrEmpty(beginWorkTime))
            {
                throw new Exception("项目开始时间与项目ID不能为空！");
            }
            var netWork = entities.Set<S_P_NetWork>().Where(o => o.ProjectID == projectID).ToArray();
            DateTime beginTime = Convert.ToDateTime(beginWorkTime);
            StringBuilder sb = new StringBuilder();
            Dictionary<string, NetWork> table = new Dictionary<string, NetWork>();
            if (netWork.Length > 0)
            {
                foreach (var item in netWork)
                {
                    if (!table.ContainsKey(item.ID))
                    {
                        if (!string.IsNullOrEmpty(item.VirtualTime.ToString()) && Math.Abs(Convert.ToDouble(item.VirtualTime)) > 0)
                        {
                            S_P_NetWork virWork = new S_P_NetWork();
                            virWork.ID = FormulaHelper.CreateGuid();
                            virWork.Name = item.Name;
                            virWork.Code = item.Code + "_@VIR";
                            virWork.Precedence = item.Precedence;
                            virWork.ProjectID = item.ProjectID;
                            virWork.Time = Math.Abs(Convert.ToDouble(item.VirtualTime));
                            table.Add(virWork.ID, getNetWork(virWork));
                            S_P_NetWork nWork = new S_P_NetWork();
                            nWork.ID = FormulaHelper.CreateGuid();
                            nWork.Name = item.Name;
                            nWork.Code = item.Code;
                            nWork.Precedence = item.Code + "_@VIR";
                            nWork.ProjectID = item.ProjectID;
                            nWork.Time = Math.Abs(Convert.ToDouble(item.Time));
                            table.Add(nWork.ID, getNetWork(nWork));
                        }
                        else
                        {
                            table.Add(item.ID, getNetWork(item));
                        }
                    }
                }
                calcTime(table, beginTime);
                calcNode(table, beginTime);
                calcNodeY(table);
                virtualWork(table);
            }

            if (table.Keys.Count > 0)
            {
                sb.Append("{\"nodes\":[{");
                sb.AppendFormat("\"id\":\"1\",\"code\":\"1\",\"type\":\"main\",\"y\":{0},\"datatime\":\"{1}\""
                    , getNodeY(1), beginTime.ToString("yyyy-MM-dd"));
                sb.Append("},");
                int i = 1, j = 1;
                foreach (KeyValuePair<string, NetWork> item in table.OrderBy(c => c.Value.Last))
                {
                    sb.Append("{");
                    sb.AppendFormat("\"id\":\"{0}\",\"code\":\"{1}\",\"type\":\"{2}\",\"y\":{3},\"datatime\":\"{4}\""
                        , item.Value.Last, item.Value.Last, getType("node", item.Value.Last, 0), getNodeY(item.Value.Last), item.Value.EndTime.ToString("yyyy-MM-dd"));
                    sb.AppendFormat("{0}", i == table.Keys.Count ? "}" : "},");
                    i++;
                }
                sb.Append("],");
                sb.Append("\"lines\":[");
                foreach (KeyValuePair<string, NetWork> item in table)
                {
                    if (!string.IsNullOrEmpty(item.Value.VirtualWork))
                    {
                        string[] pres = item.Value.VirtualWork.Split(',');
                        foreach (string pre in pres)
                        {
                            string[] from_to = pre.Split(':');
                            sb.Append("{");
                            sb.AppendFormat("\"from\":\"{0}\",\"to\":\"{1}\",\"name\":\"{2}\",\"type\":\"{3}\",\"time\":\"{4}\",\"isVir\":\"{5}\""
                                , from_to[0], from_to[1], "", getType("virtual", Convert.ToInt32(from_to[0]), Convert.ToInt32(from_to[1])), "", item.Value.IsVir);
                            sb.Append("},");
                        }
                    }
                    if (!item.Value.Code.Contains("_@VIR"))
                    {
                        sb.Append("{");
                        sb.AppendFormat("\"from\":\"{0}\",\"to\":\"{1}\",\"name\":\"{2}\",\"type\":\"{3}\",\"time\":\"{4}\",\"isVir\":\"{5}\""
                            , item.Value.First, item.Value.Last, item.Value.Name, getType("path", item.Value.First, item.Value.Last), item.Value.Time, item.Value.IsVir);
                        sb.AppendFormat("{0}", j == table.Keys.Count ? "}" : "},");
                    }
                    j++;
                }
                sb.Append("]}");

            }

            def = sb.Length > 0  ? sb.ToString() : "[]";
            return def;
        }

        public JsonResult GetData()
        {
            string projectID = Request["ProjectID"];
            var netWork = entities.Set<S_P_NetWork>().ToArray();
            if (!string.IsNullOrEmpty(projectID))
            {
                netWork = netWork.Where(o => o.ProjectID == projectID).ToArray();
            }
            return Json(netWork);
        }

        public void SaveWork()
        {
            SQLHelper sqlHelper = SQLHelper.CreateSqlHelper(ConnEnum.Engineering);
            StringBuilder sb = new StringBuilder();
            var list = JsonHelper.ToObject<List<Dictionary<string, string>>>(Request["Data"]);
            foreach (var item in list)
            {
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = item["_state"] != null ? item["_state"].ToString() : "";
                String id = state != "added" ? item["ID"] != null ? item["ID"].ToString() : "" : "";

                if (state == "added" || id == "")
                {
                    string newid = FormulaHelper.CreateGuid();
                    sb.Append(item.CreateInsertSql(ConnEnum.Engineering.ToString(), "S_P_NetWork", newid));
                }
                else if (state == "removed" || state == "deleted")
                {
                    sb.AppendFormat("Delete From S_P_NetWork Where ID='{0}'", id);
                }
                else if (state == "modified" || state == "")
                {
                    sb.Append(item.CreateUpdateSql(ConnEnum.Engineering.ToString(), "S_P_NetWork", item["ID"]));
                }
            }
            string projectID = GetQueryString("ProjectID");
            if (!string.IsNullOrEmpty(projectID))
                sb.AppendFormat("Update S_P_NetWork Set BeginWorkTime='{0}' Where ProjectID='{1}'", Request["BeginWorkTime"], projectID);

            if (sb.Length > 0)
            {
                sqlHelper.ExecuteNonQuery(sb.ToString());
            }
        }


    }
    public class NetWork
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Precedence { get; set; }
        public int First { get; set; }
        public int Last { get; set; }
        public bool IsVir { get; set; }
        public string VirtualWork { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public Double Time { get; set; }
        public Double VirtualTime { get; set; }
    }
    //总工路线：查找存在多个紧前工作的节点，找出当前节点的最长用时的路径，直到最后一节点结束
    public class MainWork
    {
        public string Code { get; set; }
        public string Path { get; set; }
        public Double WorkTime { get; set; }
    }

    public class NodeY
    {
        public int Number { get; set; }
        public string Precedence { get; set; }
        public string Tight { get; set; }
        public int PrecedenceWork { get; set; }
        public Double Y { get; set; }
    }
}
