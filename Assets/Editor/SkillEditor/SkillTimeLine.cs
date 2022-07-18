using System.Xml;
using System.Collections.Generic;
using UnityEngine;



struct skillLogicName
{
    public string name;
    public float beginTime;
    public float endTime;
    public Dictionary<string , object> skillParams;
    public void setXmlAttr(XmlDocument xmldoc, XmlNode parentNode) {
        CreateNode(xmldoc, parentNode, "beginTime", beginTime.ToString());
        CreateNode(xmldoc, parentNode, "endTime", endTime.ToString());
    }

    public void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
    {
        //创建对应Xml节点元素
        XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
        node.InnerText = value;
        parentNode.AppendChild(node);
    }
}


class SkillTimeLine
{
    public string path = "";
    public string name;
    public List<skillLogicName> nodes;
    public List<Vector3> movePath;
}

