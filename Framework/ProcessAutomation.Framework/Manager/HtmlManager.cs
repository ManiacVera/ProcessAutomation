using HtmlAgilityPack;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Framework.Manager
{

    public static class HtmlManager
    {
        public static string HtmlToString(string htmlFile)
        {
            string htmlResult = null;
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.Load(htmlFile);

            if (htmlDocument != null)
            {
                htmlResult = htmlDocument.DocumentNode.OuterHtml;
            }

            return htmlResult;
        }

        public static string RetrieveHtmlValueLabel(HtmlNode document, string tag, string attribute)
        {
            string result = null;

            if (tag != null)
            {
                if (attribute != null && string.IsNullOrEmpty(attribute) == false)
                    result = HTMLAttributeValue<string>(document.ChildNodes, tag, attribute);
                else
                    result = HTMLValueTag<string>(document.ChildNodes, tag, false);
            }

            return result;
        }

        public static T HTMLAttributeValue<T>(HtmlNodeCollection nodes, string tag, string attribute)
        {
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return (T)Convert.ChangeType(null, typeof(T));

            foreach (HtmlNode node in nodes)
            {
                T value = HTMLAttributeValue<T>(node.ChildNodes, tag, attribute);
                if (tag.ToLower() == node.Name.ToLower() && node.Attributes != null)
                {
                    HtmlAttribute htmlAttribute = node.Attributes.Cast<HtmlAttribute>().FirstOrDefault(f => f.Name.ToLower() == attribute.ToLower());
                    if (htmlAttribute != null)
                    {
                        return (T)Convert.ChangeType(htmlAttribute.Value, typeof(T));
                    }
                }

                if (!object.Equals(value, default(T)))
                    return (T)Convert.ChangeType(value, typeof(T));
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        public static void HTMLNodeListByTag(HtmlNodeCollection nodes, string tag, ref IList<HtmlNode> nodeDictionary)
        {
            if (nodeDictionary == null)
                nodeDictionary = new List<HtmlNode>();

            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return;

            foreach (HtmlNode node in nodes)
            {
                HTMLNodeListByTag(node.ChildNodes, tag, ref nodeDictionary);
                if (tag.ToLower() == node.OriginalName.ToLower() && node.Attributes != null)
                {
                    nodeDictionary.Add(node);
                }
            }
        }

        public static void HTMLNodeListByNodeInfo(HtmlNodeCollection nodes, XmlNodeInfo htmlNodeInfo, ref IList<HtmlNode> nodeDictionary)
        {
            if (nodeDictionary == null)
                nodeDictionary = new List<HtmlNode>();

            if (htmlNodeInfo == null || string.IsNullOrEmpty(htmlNodeInfo.Name) == true)
                return;

            foreach (HtmlNode node in nodes)
            {
                HTMLNodeListByNodeInfo(node.ChildNodes, htmlNodeInfo, ref nodeDictionary);

                switch (htmlNodeInfo.FindType)
                {
                    case DTO.Enum.FindTypeEnum.TAG_NAME:
                        {
                            if (htmlNodeInfo.Name.ToLower() == node.OriginalName.ToLower() && node.Attributes != null)
                            {
                                SearchNodeByNodeInfo(htmlNodeInfo, ref nodeDictionary, node);
                            }
                        }
                        break;

                    //case DTO.Enum.FindTypeEnum.NAME:
                    //    {
                    //        if (node.Attributes != null)
                    //        {
                    //            SearchNodeByNodeInfo(htmlNodeInfo, ref nodeDictionary, node);
                    //        }
                    //    }
                    //    break;
                }
            }
        }

        public static T FindAttributeByXPath<T>(TagSearchInfo tagInfo, HtmlNode mainNode)
        {
            T result = default(T);

            try
            {
                if (tagInfo != null && mainNode.ParentNode != null && !string.IsNullOrEmpty(tagInfo.GetAttribute))
                {
                    HtmlNode node = null;
                    if (tagInfo.FindType == FindTypeEnum.XPATH)
                    {
                        node = mainNode.ParentNode.SelectSingleNode(tagInfo.ContainsText);
                    }

                    if (node != null)
                    {
                        HtmlAttribute htmlAttribute = node.Attributes.Cast<HtmlAttribute>().FirstOrDefault(f => f.Name.ToLower() == tagInfo.GetAttribute.ToLower());
                        if (htmlAttribute != null)
                        {
                            if (!string.IsNullOrEmpty(tagInfo.RegularExpression))
                            {
                                result = (T)Convert.ChangeType(Regex.Replace(htmlAttribute.Value, @tagInfo.RegularExpression, ""), typeof(T));
                            }
                            else
                            {
                                result = (T)Convert.ChangeType(htmlAttribute.Value, typeof(T));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }

            return result;
        }

        public static T FindPropertyByXPath<T>(TagSearchInfo tagInfo, HtmlNode mainNode)
        {
            T result = default(T);

            try
            {
                if (tagInfo != null && !string.IsNullOrEmpty(tagInfo.GetProperty))
                {
                    HtmlNode node = null;
                    if (tagInfo.FindType == FindTypeEnum.XPATH)
                    {
                        if (mainNode.ParentNode != null)
                        {
                            node = mainNode.ParentNode.SelectSingleNode(tagInfo.ContainsText);
                        }
                        else
                        {
                            node = mainNode.SelectSingleNode(tagInfo.ContainsText);
                        }

                        if (node != null)
                        {
                            switch (tagInfo.GetAttribute.ToLower())
                            {
                                case "innertext":
                                    if (!string.IsNullOrEmpty(tagInfo.RegularExpression))
                                    {
                                        result = (T)Convert.ChangeType(Regex.Replace(node.InnerText, @tagInfo.RegularExpression, ""), typeof(T));
                                    }
                                    else
                                    {
                                        result = (T)Convert.ChangeType(node.InnerText.Trim(), typeof(T));
                                    }
                                    break;
                                case "innerhtml":
                                    if (!string.IsNullOrEmpty(tagInfo.RegularExpression))
                                    {
                                        result = (T)Convert.ChangeType(Regex.Replace(node.InnerHtml, @tagInfo.RegularExpression, ""), typeof(T));
                                    }
                                    else
                                    {
                                        result = (T)Convert.ChangeType(node.InnerHtml.Trim(), typeof(T));
                                    }
                                    break;
                                //case "outerhtml":
                                //views = (T)Convert.ChangeType(Regex.Replace(node.OuterHtml, @tagInfo.RegularExpression, ""), typeof(T));
                                //break;
                                default:
                                    if (!string.IsNullOrEmpty(tagInfo.RegularExpression))
                                    {
                                        result = (T)Convert.ChangeType(Regex.Replace(node.Attributes[tagInfo.GetAttribute].Value, @tagInfo.RegularExpression, ""), typeof(T));
                                    }
                                    else
                                    {
                                        result = (T)Convert.ChangeType(node.Attributes[tagInfo.GetAttribute].Value.Trim(), typeof(T));
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }

            return result;
        }

        public static string GetTagText(HtmlNode mainNode, int levelSearch = 0)
        {
            string value = null;
            int countLevelSearch = 0;

            if (mainNode != null && mainNode.ChildNodes != null)
            {
                IEnumerable<HtmlNode> findNodeList = mainNode.ChildNodes.Cast<HtmlNode>().Where(f => f.NodeType == HtmlNodeType.Text || f.NodeType == HtmlNodeType.Element);

                if (findNodeList != null && findNodeList.Any())
                {
                    foreach (HtmlNode findNode in findNodeList)
                    {
                        if (findNode.NodeType == HtmlNodeType.Text)
                        {
                            value = findNode.InnerText;
                        }
                        else
                        {
                            value = GetTagText(findNode);
                        }

                        if (value != null && value.Trim() != string.Empty)
                        {
                            if (levelSearch == countLevelSearch)
                            {
                                break;
                            }
                            else
                            {
                                countLevelSearch++;
                            }
                        }
                    }
                }
            }
            return value;
        }

        private static void SearchNodeByNodeInfo(XmlNodeInfo htmlNodeInfo, ref IList<HtmlNode> nodeDictionary, HtmlNode node)
        {
            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                if (htmlNodeInfo.SearchAttribute != null)
                {
                    foreach (XmlNodeSearchAttribute searchAttribute in htmlNodeInfo.SearchAttribute.Where(f => f.SearchActivated))
                    {
                        if (searchAttribute.ExactSearch)
                        {
                            if ((searchAttribute.Name.ToLower() == "outerhtml" && node.OuterHtml == searchAttribute.Value)
                                || (searchAttribute.Name.ToLower() == "innerhtml" && node.InnerHtml == searchAttribute.Value)
                                || (searchAttribute.Name.ToLower() == "innertext" && node.InnerText == searchAttribute.Value))
                            {
                                nodeDictionary.Add(node);
                            }
                            else if (node.Attributes.Cast<HtmlAttribute>().Any(a => a.OriginalName == searchAttribute.Name && a.Value == searchAttribute.Value))
                            {
                                nodeDictionary.Add(node);
                            }
                        }
                        else
                        {
                            if ((searchAttribute.Name.ToLower() == "outerhtml" && node.OuterHtml.Contains(searchAttribute.Value))
                                || (searchAttribute.Name.ToLower() == "innerhtml" && node.InnerHtml.Contains(searchAttribute.Value))
                                || (searchAttribute.Name.ToLower() == "innertext" && node.InnerText.Contains(searchAttribute.Value)))
                            {
                                nodeDictionary.Add(node);
                            }
                            else if (node.Attributes.Cast<HtmlAttribute>().Any(a => a.OriginalName == searchAttribute.Name && a.Value.Contains(searchAttribute.Value)))
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

        public static T HTMLValueTag<T>(HtmlNodeCollection nodes, string tag, bool cutPrefix = false)
        {
            int indexPrefix = 0;
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return (T)Convert.ChangeType(null, typeof(T));

            foreach (HtmlNode node in nodes)
            {
                if (cutPrefix)
                {
                    indexPrefix = node.ParentNode.Name.LastIndexOf(':') + 1;
                }

                T value = HTMLValueTag<T>(node.ChildNodes, tag, cutPrefix);
                if ((cutPrefix && indexPrefix > 0 && tag.ToString() == node.ParentNode.Name.Substring(indexPrefix))
                    || tag.ToString() == node.ParentNode.Name)
                    return (T)Convert.ChangeType(node.InnerText, typeof(T));

                if (!object.Equals(value, default(T)))
                    return (T)Convert.ChangeType(value, typeof(T));
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        public static HtmlNodeCollection HTMLTagChildren(HtmlNodeCollection nodes, string tag)
        {
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return null;

            foreach (HtmlNode node in nodes)
            {
                HtmlNodeCollection value = HTMLTagChildren(node.ChildNodes, tag);
                if (tag.ToString() == node.OriginalName)
                    return node.ChildNodes;

                if (value != null)
                    return value;
            }

            return null;
        }

        public static HtmlNode HTMLTag(HtmlNodeCollection nodes, string tag)
        {
            if (tag == null || string.IsNullOrEmpty(tag) == true)
                return null;

            foreach (HtmlNode node in nodes)
            {
                HtmlNode value = HTMLTag(node.ChildNodes, tag);
                if (tag.ToString() == node.Name)
                    return node;

                if (value != null)
                    return value;
            }

            return null;
        }

        public static HtmlDocument ConnectToWebSite(string sourceUrl, ref string homeUrl, bool isLocalFile = false)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument htmlDocument = null;

            if (isLocalFile)
            {
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(sourceUrl);
            }
            else
            {
                htmlDocument = htmlWeb.Load(sourceUrl, "GET");
                homeUrl = string.Format("{0}://{1}", htmlWeb.ResponseUri.Scheme, htmlWeb.ResponseUri.Authority);
            }

            return htmlDocument;
        }

    }
}
