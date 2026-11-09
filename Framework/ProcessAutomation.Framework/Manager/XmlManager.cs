using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProcessAutomation.Framework.Manager
{
    public static class XmlManager
    {
        public static XmlDocument ToXml(this string rawXml)
        {
            XmlDocument xmlDoc = null;
            if (!string.IsNullOrWhiteSpace(rawXml))
            {
                try
                {
                    xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(rawXml);
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
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFile);

            if (xmlDocument != null)
            {
                xmlResult = xmlDocument.InnerXml;
            }

            return xmlResult;
        }

        public static void IterateXml(this XmlDocument doc, object data, Func<XmlNode, string, object, bool> elementVisitor)
        {
            if (doc != null && elementVisitor != null)
            {
                foreach (XmlNode node in doc.ChildNodes)
                {
                    DoIterateNode(node, node.Name, data, elementVisitor);
                }
            }
        }

        public static void IterateNode(this XmlNode node, object data, Func<XmlNode, string, object, bool> elementVisitor)
        {
            if (node != null && elementVisitor != null)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    DoIterateNode(childNode, childNode.Name, data, elementVisitor);
                }
            }
        }

        private static void DoIterateNode(XmlNode node, string path, object data, Func<XmlNode, string, object, bool> elementVisitor)
        {
            bool skipChildNodes = elementVisitor(node, path, data);

            if (!skipChildNodes)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    DoIterateNode(childNode, path + $"/{childNode.Name}", data, elementVisitor);
                }
            }
        }

        public static string IndentXml(string rawXml)
        {
            string result = null;

            if (!string.IsNullOrWhiteSpace(rawXml))
            {
                const string detailRoot = "DetailRootXml";
                result = System.Xml.Linq.XElement.Parse($"<{detailRoot}>" + rawXml + $"</{detailRoot}>").ToString();
                result = result.Replace($"<{detailRoot}>", string.Empty).Replace($"</{detailRoot}>", string.Empty).Trim();
            }

            return result;
        }

        public static string RetrieveXmlValueLabel(XmlDocument document, string tag, string attribute)
        {
            string result = null;

            if (tag != null)
            {
                if (attribute != null && string.IsNullOrEmpty(attribute) == false)
                    result = XMLAttributeValue<string>(document.ChildNodes, tag, attribute);
                else
                    result = XMLValueTag<string>(document.ChildNodes, tag, false);
            }

            return result;
        }

        public static T XMLAttributeValue<T>(XmlNodeList nodes, string tag, string attribute)
        {
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return (T)Convert.ChangeType(null, typeof(T));

            foreach (XmlNode node in nodes)
            {
                T value = XMLAttributeValue<T>(node.ChildNodes, tag, attribute);
                if (tag.ToLower() == node.Name.ToLower() && node.Attributes != null)
                {
                    XmlAttribute xmlAttribute = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(f => f.Name.ToLower() == attribute.ToLower());
                    if (xmlAttribute != null)
                    {
                        return (T)Convert.ChangeType(xmlAttribute.Value, typeof(T));
                    }
                }

                if (!object.Equals(value, default(T)))
                    return (T)Convert.ChangeType(value, typeof(T));
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        public static T XMLAttributeValue<T>(XmlNode node, string attribute)
        {
            T result = default(T);

            if (node != null)
            {
                if (node.Attributes != null)
                {
                    XmlAttribute xmlAttribute = node.Attributes.Cast<XmlAttribute>().FirstOrDefault(f => f.Name.ToLower() == attribute.ToLower());
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

        public static void XMLNodeListByTag(XmlNodeList nodes, string tag, ref IList<XmlNode> nodeDictionary)
        {
            if (nodeDictionary == null)
                nodeDictionary = new List<XmlNode>();

            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return;

            foreach (XmlNode node in nodes)
            {
                XMLNodeListByTag(node.ChildNodes, tag, ref nodeDictionary);
                if (tag.ToLower() == node.LocalName.ToLower() && node.Attributes != null)
                {
                    nodeDictionary.Add(node);
                }
            }
        }

        public static void XMLNodeListByNodeInfo(XmlNodeList nodes, XmlNodeInfo xmlNodeInfo, ref IList<XmlNode> nodeDictionary)
        {
            if (nodeDictionary == null)
                nodeDictionary = new List<XmlNode>();

            if (xmlNodeInfo == null || string.IsNullOrEmpty(xmlNodeInfo.Name) == true)
                return;

            foreach (XmlNode node in nodes)
            {
                XMLNodeListByNodeInfo(node.ChildNodes, xmlNodeInfo, ref nodeDictionary);

                switch (xmlNodeInfo.FindType)
                {
                    case DTO.Enum.FindTypeEnum.TAG_NAME:
                        {
                            if (xmlNodeInfo.Name.ToLower() == node.LocalName.ToLower() && node.Attributes != null)
                            {
                                SearchNodeByNodeInfo(xmlNodeInfo, ref nodeDictionary, node);
                            }
                        }
                        break;
                }
            }
        }

        private static void SearchNodeByNodeInfo(XmlNodeInfo xmlNodeInfo, ref IList<XmlNode> nodeDictionary, XmlNode node)
        {
            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                if (xmlNodeInfo.SearchAttribute != null)
                {
                    foreach (XmlNodeSearchAttribute searchAttribute in xmlNodeInfo.SearchAttribute.Where(f => f.SearchActivated))
                    {
                        if (searchAttribute.ExactSearch)
                        {
                            if (node.Attributes.Cast<XmlNode>().Any(a => a.LocalName == searchAttribute.Name && a.Value == searchAttribute.Value))
                            {
                                nodeDictionary.Add(node);
                            }
                        }
                        else
                        {
                            if (node.Attributes.Cast<XmlNode>().Any(a => a.LocalName == searchAttribute.Name && a.Value.Contains(searchAttribute.Value)))
                            {
                                nodeDictionary.Add(node);
                            }
                        }
                    }
                }
                else
                {
                    nodeDictionary.Add(node);
                }
            }
        }

        public static T XMLValueTag<T>(XmlNodeList nodes, string tag, bool cutPrefix = false)
        {
            int indexPrefix = 0;
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return (T)Convert.ChangeType(null, typeof(T));

            foreach (XmlNode node in nodes)
            {
                if (cutPrefix)
                {
                    indexPrefix = node.ParentNode.Name.LastIndexOf(':') + 1;
                }

                T value = XMLValueTag<T>(node.ChildNodes, tag, cutPrefix);
                if ((cutPrefix && indexPrefix > 0 && tag.ToString() == node.ParentNode.Name.Substring(indexPrefix))
                    || tag.ToString() == node.ParentNode.Name)
                    return (T)Convert.ChangeType(node.Value, typeof(T));

                if (!object.Equals(value, default(T)))
                    return (T)Convert.ChangeType(value, typeof(T));
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        public static XmlNodeList XMLTagChildren(XmlNodeList nodes, string tag)
        {
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return null;

            foreach (XmlNode node in nodes)
            {
                XmlNodeList value = XMLTagChildren(node.ChildNodes, tag);
                if (tag.ToString() == node.LocalName)
                    return node.ChildNodes;

                if (value != null)
                    return value;
            }

            return null;
        }

        public static List<XmlNode> XMLTagChildren(XmlNode node, string tag, bool recursive = false)
        {
            List<XmlNode> xmlNodeList = null;
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return null;

            xmlNodeList = new List<XmlNode>();
            foreach (XmlNode chidNode in node.ChildNodes)
            {
                if (tag.ToString() == chidNode.LocalName)
                    xmlNodeList.Add(chidNode);
            }

            return xmlNodeList;
        }

        public static XmlNode XMLTag(XmlNodeList nodes, string tag)
        {
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return null;

            foreach (XmlNode node in nodes)
            {
                XmlNode value = XMLTag(node.ChildNodes, tag);
                if (tag.ToString() == node.Name)
                    return node;

                if (value != null)
                    return value;
            }

            return null;
        }
    }
}
