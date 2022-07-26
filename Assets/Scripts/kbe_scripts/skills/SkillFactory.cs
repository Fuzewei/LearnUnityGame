namespace KBEngine
{
    using GameLogic;
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using System.Reflection;


    public class XmlSkillLogicName
    {
        public string name;
        public float beginTime;
        public float endTime;
        public Dictionary<string, string> skillParams = new Dictionary<string, string>();
        public void setXmlAttr(XmlDocument xmldoc, XmlNode parentNode)
        {
            CreateNode(xmldoc, parentNode, "beginTime", beginTime.ToString());
            CreateNode(xmldoc, parentNode, "endTime", endTime.ToString());
            XmlNode paramsNode = xmldoc.CreateElement("params");
            parentNode.AppendChild(paramsNode);
            foreach (var item in skillParams)
            {
                string key = item.Key;
                string value = item.Value;
                CreateNode(xmldoc, paramsNode, key, value);
            }
        }

        public void fromXmlAttr(XmlDocument xmldoc, XmlNode node)
        {
            name = node.Name;
            var nodeBeginTime = node.SelectSingleNode("./beginTime");
            var nodeEndTime = node.SelectSingleNode("./endTime");
            float.TryParse(nodeBeginTime.InnerText, out beginTime);
            float.TryParse(nodeEndTime.InnerText, out endTime);
            var xmlparamsNode = node.SelectSingleNode("./params");
            foreach (XmlNode xmlParamNode in xmlparamsNode.ChildNodes)
            {
                skillParams[xmlParamNode.Name] = xmlParamNode.InnerText;

            }
        }

        public XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            //创建对应Xml节点元素
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
            return node;
        }
    }

    public class XmlSkillTimeLine
    {
        public string path = "";
        public string name = "";
        public List<XmlSkillLogicName> nodes = new List<XmlSkillLogicName>();
        public List<Vector3> movePath = new List<Vector3>();

        public void setXmlAttr(XmlDocument xmldoc, XmlNode parentNode)
        {
            XmlNode nodesRoot = xmldoc.CreateElement("nodes");
            parentNode.AppendChild(nodesRoot);
            foreach (var node in nodes)
            {
                XmlNode nodeRoot = xmldoc.CreateElement(node.name);
                nodesRoot.AppendChild(nodeRoot);
                node.setXmlAttr(xmldoc, nodeRoot);
            }
        }

        public void fromXmlAttr(XmlDocument xmldoc, XmlNode node)
        {
            name = node.Name;
            XmlNode xmlNodes = node.SelectSingleNode("./nodes");
            foreach (XmlNode xmlNode in xmlNodes.ChildNodes)
            {
                XmlSkillLogicName logicNode = new XmlSkillLogicName();
                logicNode.fromXmlAttr(xmldoc, xmlNode);
                nodes.Add(logicNode);
            }
        }
    }




    public static class SkillFactory
    {
        public static Dictionary<string, XmlSkillTimeLine> skillTimeLines = new Dictionary<string, XmlSkillTimeLine>();
        public static Dictionary<string, Type> name2Types = new Dictionary<string, Type>();

        public static void Init()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(Application.streamingAssetsPath + "/TimeLine/SkillTimeLine.xml");
            XmlNode root = xml.SelectSingleNode("/Root");
            foreach (XmlNode timeline in root.ChildNodes)
            {
                XmlSkillTimeLine timeLine = new XmlSkillTimeLine();
                timeLine.fromXmlAttr(xml, timeline);
                skillTimeLines[timeLine.name] = timeLine;
            }

            var types = Assembly.GetExecutingAssembly().GetTypes();//为了动态生成对象
            foreach (var item in types)
            {
                name2Types[item.Name] = item;
            }
        }

        public static skillTimeLine getTimeLineById(IServerEntity entity, string  timeLineName, SkillNodeType nodeType = SkillNodeType.P1)
        {
            skillTimeLine line = new skillTimeLine(entity);
            XmlSkillTimeLine timeLine = skillTimeLines[timeLineName];
            foreach (var item in timeLine.nodes)
            {
                var t = name2Types[item.name];
                SkillNodeBase node = (SkillNodeBase)Activator.CreateInstance(t, item.beginTime);
                node.resetNode(item);
                node.nodeType = nodeType;
                line.addNode(node);
            }
            //switch (timeLineId)
            //{
            //    case 1:
            //        PlayerAnimationNode NodeBase1 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack01");
            //        NodeBase1.nodeType = nodeType;
            //        line.addNode(NodeBase1);
            //        CommonAttack NodeBase2 = new CommonAttack(0.3f);
            //        NodeBase2.nodeType = nodeType;
            //        line.addNode(NodeBase2);
            //        StartNewSkill NodeBase3 = new StartNewSkill(1.0f, 0.4f, 2);
            //        NodeBase3.nodeType = nodeType;
            //        line.addNode(NodeBase3);
            //        TimeLineEndNode NodeBase4 = new TimeLineEndNode(2.0f);
            //        NodeBase4.nodeType = nodeType;
            //        line.addNode(NodeBase4);
            //        break;

            //    case 2:
            //        PlayerAnimationNode NodeBase6 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack02");
            //        NodeBase6.nodeType = nodeType;
            //        line.addNode(NodeBase6);
            //        CommonAttack NodeBase7 = new CommonAttack(0.3f);
            //        NodeBase7.nodeType = nodeType;
            //        line.addNode(NodeBase7);
            //        StartNewSkill NodeBase8 = new StartNewSkill(0.8f, 0.4f, 3);
            //        NodeBase8.nodeType = nodeType;
            //        line.addNode(NodeBase8);
            //        TimeLineEndNode NodeBase9 = new TimeLineEndNode(2.0f);
            //        NodeBase9.nodeType = nodeType;
            //        line.addNode(NodeBase9);
            //        break;
            //    case 3:
            //        PlayerAnimationNode NodeBase10 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack03");
            //        NodeBase10.nodeType = nodeType;
            //        line.addNode(NodeBase10);
            //        CommonAttack NodeBase11 = new CommonAttack(0.6f);
            //        NodeBase11.nodeType = nodeType;
            //        line.addNode(NodeBase11);
            //        TimeLineEndNode NodeBase13 = new TimeLineEndNode(1.5f);
            //        NodeBase13.nodeType = nodeType;
            //        line.addNode(NodeBase13);
            //        break;

            //    default:
            //        break;
            //}
            
           
            return line;
        }




    }
}
