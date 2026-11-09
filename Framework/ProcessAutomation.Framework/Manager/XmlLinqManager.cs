using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ProcessAutomation.Framework.Manager
{
    public enum XmlActionEnum
    {
        SKIP_CHILD_NODES = 1,
        JUMP_ELEMENT = 2
    }

    public class XmlActionInfo
    {
        public int Action { get; set; }
        public int Value { get; set; }
        public Func<XElement, bool> Condition { get; set; }

        public XmlActionInfo(int action, int value, Func<XElement, bool> condition = null)
        {
            Action = action;
            Value = value;
            Condition = condition;
        }

        public XmlActionInfo(XmlActionInfo actionInfo)
        {
            if (actionInfo != null)
            {
                Action = actionInfo.Action;
                Value = actionInfo.Value;
                Condition = actionInfo.Condition;
            }
        }
    }

    public class XmlActionResult
    {
        public List<XmlActionInfo> XmlActionList { get; set; }

        public bool IsActivated(XElement element, XmlActionEnum action)
        {
            bool result = false;

            if (XmlActionList != null && XmlActionList.Count > 0)
            {
                XmlActionInfo xmlValidator = XmlActionList.FirstOrDefault(v => v.Action == (int)action);
                if (xmlValidator != null)
                {
                    if(xmlValidator.Value > 0)
                    {
                        if (xmlValidator.Condition == null)
                        {
                            result = true;
                        }
                        else
                        {
                            result = xmlValidator.Condition(element);
                        }
                    }
                }
            }

            return result;
        }
    }
    
    public static class XmlLinqManager
    {
        public static XDocument ToXmlLinq(this string rawXml)
        {
            XDocument xmlDoc = null;
            if (!string.IsNullOrWhiteSpace(rawXml))
            {
                try
                {
                    xmlDoc = XDocument.Parse(rawXml);
                }
                catch (Exception)
                {
                }
            }
            return xmlDoc;
        }

        public static string XmlToString(string xmlFile)
        {
            string xmlResult = null;
            XDocument xmlDocument = XDocument.Load(xmlFile);

            if (xmlDocument != null)
            {
                xmlResult = xmlDocument.ToString();
            }

            return xmlResult;
        }

        public static void IterateXml(this XDocument doc, object data, Func<XElement, string, object, XmlActionResult> elementVisitor)
        {
            if (doc != null && elementVisitor != null)
            {
                foreach (XElement node in doc.Elements().ToList())
                {
                    DoIterateNode(node, node.Name.LocalName, data, elementVisitor);
                }
            }
        }

        public static void IterateXml(this XElement doc, object data, Func<XElement, string, object, XmlActionResult> elementVisitor)
        {
            if (doc != null && elementVisitor != null)
            {
                XmlActionResult oldResult = null;
                foreach (XElement node in doc.Elements().ToList())
                {
                    if (oldResult != null && oldResult.IsActivated(node, XmlActionEnum.JUMP_ELEMENT))
                    {
                        continue;
                    }

                    oldResult = DoIterateNode(node, node.Name.LocalName, data, elementVisitor);
                }
            }
        }

        public static void IterateNode(this XElement node, object data, Func<XElement, string, object, XmlActionResult> elementVisitor)
        {
            if (node != null && elementVisitor != null)
            {
                foreach (XElement childNode in node.Elements().ToList())
                {
                    DoIterateNode(childNode, childNode.Name.LocalName, data, elementVisitor);
                }
            }
        }

        private static XmlActionResult DoIterateNode(XElement node, string path, object data, Func<XElement, string, object, XmlActionResult> elementVisitor)
        {
            XmlActionResult xmlResult = elementVisitor(node, path, data);

            if (xmlResult == null || !xmlResult.IsActivated(node, XmlActionEnum.SKIP_CHILD_NODES))
            {
                foreach (XElement childNode in node.Elements().ToList())
                {
                    DoIterateNode(childNode, path + $"/{childNode.Name.LocalName}", data, elementVisitor);
                }
            }

            return xmlResult;
        }

        public static T XMLAttributeValue<T>(XElement node, string attribute)
        {
            T result = default(T);

            if (node != null)
            {
                if (node.HasAttributes)
                {
                    XAttribute xmlAttribute = node.Attributes().FirstOrDefault(f => f.Name.LocalName.ToLower() == attribute.ToLower());
                    if (xmlAttribute != null)
                    {
                        try
                        {
                            result = (T)Convert.ChangeType(xmlAttribute.Value, typeof(T));
                        }
                        catch (Exception) { }
                    }
                }
            }

            return result;
        }

        public static List<XElement> XMLTagChildren(List<XElement> nodes, string tag)
        {
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return null;

            foreach (XElement node in nodes)
            {
                List<XElement> value = XMLTagChildren(node.Elements().ToList(), tag);
                if (tag.ToString() == node.Name.LocalName)
                    return node.Elements().ToList();

                if (value != null)
                    return value;
            }

            return null;
        }

        public static List<XElement> XMLTagChildren(XElement node, string tag, bool recursive = false)
        {
            List<XElement> xmlNodeList = null;
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return null;

            xmlNodeList = new List<XElement>();
            foreach (XElement chidNode in node.Elements())
            {
                if (tag.ToString() == chidNode.Name.LocalName)
                    xmlNodeList.Add(chidNode);
            }

            return xmlNodeList;
        }
    }
}
