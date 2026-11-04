using HtmlAgilityPack;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ProcessAutomation.Component
{
    public class HTMLBrowserComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;
        private const int imageMaxCount = 3;
        //private readonly LoggerManager loggerManager = null;
        //private readonly ICatWebSiteSearchDAO catWebSiteSearchDAO = null;
        //private readonly ILogUserIpDAO logUserIpDAO = null;

        public HTMLBrowserComponent() : base()
        {

        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            return true;
        }

        public void EndComponent()
        {
            
        }

        public bool InitComponent()
        {
            return true;
        }

        private HtmlNode FindTagRecursive(HtmlNode video, string tagName, ref int imageCounter, bool asc = false)
        {
            HtmlNode imageInfo = null;
            HtmlNode parentNode = video;
            bool enterJustOne = false;

            for (int imageCount = 0; imageCount <= imageMaxCount; imageCount++)
            {
                if (imageCount < imageCounter)
                {
                    parentNode = parentNode.ParentNode;
                    if ((parentNode == null || imageCounter == imageMaxCount) && !enterJustOne)
                    {
                        enterJustOne = true;
                        imageCounter = 0;
                        imageCount = -1;
                    }

                    continue;
                }

                if (parentNode != null && parentNode.ChildNodes != null)
                {
                    try
                    {
                        imageInfo = HtmlManager.HTMLTag(parentNode.ChildNodes, tagName);
                    }
                    catch (Exception exception)
                    {
                        string message = exception.Message;
                    }

                    if (imageInfo != null)
                    {
                        break;
                    }
                    else
                    {
                        parentNode = parentNode.ParentNode;
                    }
                }

                imageCounter++;
            }

            if (imageInfo == null)
            {
                imageCounter = 0;
            }

            return imageInfo;
        }

        private void AddVideo(HtmlDocument htmlDocument, IList<HtmlNode> nodeDictionary, SiteHeaderInfo siteHeader, HTMLBrowserSearchInfo webSiteSearch, SiteInfo site, List<VideoInfo> videoList)
        {
            HtmlNode imageNode = null;
            string webSiteUrl = string.Format(webSiteSearch.UrlSearch, webSiteSearch.TextSearch, webSiteSearch.InitPageNumberZero ? webSiteSearch.PageNumber - 1 : webSiteSearch.PageNumber);
            int imageCounter = webSiteSearch.ImageTag != null ? webSiteSearch.ImageTag.DepthCount : 0;
            int levelTagText = webSiteSearch.VideoTag != null ? webSiteSearch.VideoTag.DepthCount : 0;
            int views = 0;
            string duration = null;
            string imageUrl = null;
            string videoUrl = null;
            VideoInfo videoCompare = null;

            foreach (HtmlNode nodeVideo in nodeDictionary)
            {
                imageUrl = null;
                string text = HtmlManager.GetTagText(nodeVideo, levelTagText);

                if (!string.IsNullOrEmpty(text))
                {
                    text = HttpUtility.HtmlDecode(text);

                    if (webSiteSearch.TitleTag != null && !string.IsNullOrEmpty(webSiteSearch.TitleTag.GetProperty))
                    {
                        text = HtmlManager.FindPropertyByXPath<string>(webSiteSearch.TitleTag, nodeVideo);
                    }

                    if (imageUrl == null)
                    {
                        imageNode = FindTagRecursive(nodeVideo, webSiteSearch.ImageTag.TagSearch, ref imageCounter);
                        webSiteSearch.ImageTag.DepthCount = imageCounter;

                        if (imageNode != null)
                        {
                            if (!string.IsNullOrEmpty(webSiteSearch.ImageTag.GetProperty) && imageNode.Attributes[webSiteSearch.ImageTag.GetProperty] != null)
                            {
                                imageUrl = imageNode.Attributes[webSiteSearch.ImageTag.GetProperty].Value;
                            }
                            else if (imageNode.Attributes["data-src"] != null)
                            {
                                imageUrl = imageNode.Attributes["data-src"].Value;
                            }
                            else if (imageNode.Attributes["data-original"] != null)
                            {
                                imageUrl = imageNode.Attributes["data-original"].Value;
                            }
                            else if (imageNode.Attributes["data-thumbnail"] != null)
                            {
                                imageUrl = imageNode.Attributes["data-thumbnail"].Value;
                            }
                            else if (imageNode.Attributes["data-thumb_url"] != null)
                            {
                                imageUrl = imageNode.Attributes["data-thumb_url"].Value;
                            }
                            else if (imageNode.Attributes["src"] != null)
                            {
                                imageUrl = imageNode.Attributes["src"].Value;
                            }
                        }
                    }

                    views = HtmlManager.FindPropertyByXPath<int>(webSiteSearch.ViewsTag, nodeVideo);
                    duration = HtmlManager.FindPropertyByXPath<string>(webSiteSearch.DurationTag, nodeVideo);

                    if (imageUrl != null)
                    {
                        // Luis vera validar si hay otra pagina que contenga algo similar que XVideos
                        imageUrl = imageUrl.Replace("THUMBNUM", "8");

                        //if (!videoFinded)
                        //{
                        //    if (site != null && site.LastVideo != null)
                        //    {
                        //        if (imageUrl != site.LastVideo)
                        //        {
                        //            continue;
                        //        }
                        //        else
                        //        {
                        //            videoFinded = true;
                        //            continue;
                        //        }
                        //    }
                        //}

                        //if (firstImageUrl == null)
                        //{
                        //    firstImageUrl = imageUrl;
                        //}

                        //if (site.FirstVideo != null && firstImageUrl == site.FirstVideo)
                        //{
                        //    break;
                        //}

                        // Se debe de comparar la url, si ya existe obtenerla, cambiarle el texto y la imagen
                        videoUrl = HttpUtility.HtmlDecode(nodeVideo.Attributes["href"].Value);

                        videoCompare = videoList.FirstOrDefault(f => f.VideoUrl == videoUrl);

                        if (videoCompare == null)
                        {
                            //lock (this)
                            {
                                videoList.Add(new VideoInfo
                                {
                                    IdSite = site.IdSite,
                                    WebSiteUrl = webSiteUrl,
                                    ImageUrl = imageUrl,
                                    VideoUrl = string.Format("{0}{1}", webSiteSearch.HomeUrl, videoUrl),
                                    Text = text,
                                    ViewCount = views,
                                    Duration = duration,
                                    OuterHTML = null //nodeVideo.OuterHtml
                                });

                                //if ((++maxBySiteCount) == webSiteSearch.MaxShow)
                                //{
                                //    break;
                                //}
                            }
                        }
                        else
                        {
                            videoCompare.Text = text;
                            videoCompare.ImageUrl = imageUrl;
                        }
                    }
                }
            }

            //site.FirstVideo = firstImageUrl;
            //site.LastVideo = imageUrl;
        }

        private IList<HtmlNode> VideoCollection(HtmlDocument htmlDocument, HTMLBrowserSearchInfo webSiteSearch)
        {
            IList<HtmlNode> nodeDictionary = null;
            try
            {
                List<XmlNodeSearchAttribute> searchAttribute = null;
                if (webSiteSearch.VideoTag.GetAttribute != null)
                {
                    searchAttribute = new List<XmlNodeSearchAttribute>
                    {
                        new XmlNodeSearchAttribute
                        {
                            Name = webSiteSearch.VideoTag.GetAttribute, 
                            Value = webSiteSearch.VideoTag.ContainsText, 
                            ExactSearch = false, 
                            SearchActivated = true, 
                            RegularExpression = webSiteSearch.VideoTag.RegularExpression 
                        }
                    };
                }

                XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                {
                    Name = webSiteSearch.VideoTag.TagSearch,
                    FindType = FindTypeEnum.TAG_NAME,
                    ExactSearch = true,
                    SearchAttribute = searchAttribute
                };

                HtmlNodeCollection childNodeList = null;
                foreach (HtmlNode childNode in htmlDocument.DocumentNode.ChildNodes)
                {
                    if (childNode.ChildNodes != null && childNode.ChildNodes.Count > 0)
                    {
                        childNodeList = childNode.ChildNodes;
                        break;
                    }
                }

                if (childNodeList != null)
                {
                    HtmlManager.HTMLNodeListByNodeInfo(childNodeList, xmlNodeInfo, ref nodeDictionary);
                    if (nodeDictionary != null && nodeDictionary.Any())
                    {
                        if (webSiteSearch.VideoTag.RegularExpression != null && webSiteSearch.VideoTag.GetAttribute != null)
                        {
                            nodeDictionary = nodeDictionary.Where(f => Regex.IsMatch(f.Attributes[webSiteSearch.VideoTag.GetAttribute].Value, @webSiteSearch.VideoTag.RegularExpression)).ToList();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }


            return nodeDictionary;
        }
        
        private HtmlDocument ConnectToWebSite(HTMLBrowserSearchInfo webSiteSearch, string webSiteUrl)
        {
            string homeUrl = string.Empty;
            HtmlDocument htmlDocument = null;

            try
            {
                htmlDocument = HtmlManager.ConnectToWebSite(webSiteUrl, ref homeUrl);
                webSiteSearch.HomeUrl = homeUrl;
            }
            catch (Exception)
            {
                throw;
            }

            return htmlDocument;
        }

        private void VideoCollection(HTMLBrowserSearchInfo webSiteSearch, SiteHeaderInfo siteHeader, List<VideoInfo> videoList)
        {
            string lastVideoUrl = null;
            SiteInfo site = siteHeader.SiteList.FirstOrDefault(f => f.IdSite == webSiteSearch.IdWebSite);

        GET_MORE_VIDEO:

            int page = 0;
            int oldPage = 0;
            if (site != null)
            {
                webSiteSearch.PageNumber = site.CurrentPage;
            }

            if (webSiteSearch.PageTotalCount > 0)
            {
                page = ((webSiteSearch.MaxShow * webSiteSearch.PageNumber) / webSiteSearch.PageTotalCount) + 1;
                oldPage = ((webSiteSearch.MaxShow * (webSiteSearch.PageNumber - 1)) / webSiteSearch.PageTotalCount) + 1;
            }
            else
            {
                page = 1;
            }

            HtmlDocument htmlDocument = ConnectToWebSite(webSiteSearch, string.Format(webSiteSearch.UrlSearch, webSiteSearch.TextSearch, (webSiteSearch.InitPageNumberZero ? page - 1 : page)));
            IList<HtmlNode> nodeDictionary = VideoCollection(htmlDocument, webSiteSearch);
            if (nodeDictionary != null && nodeDictionary.Any())
            {
                if (videoList == null)
                {
                    videoList = new List<VideoInfo>();
                }

                if (site == null)
                {
                    siteHeader.SiteList.Add(new SiteInfo
                    {
                        IdSite = webSiteSearch.IdWebSite,
                        //PageTotalCount = nodeDictionary.Count,
                        LastVideo = null
                    });
                }
                else
                {
                    //site.PageTotalCount = nodeDictionary.Count;
                }

                // Pone LastVideo en null para evitar validar porque no existe en una nueva pagina
                if (oldPage != page)
                {
                    site.LastVideo = null;
                }

                AddVideo(htmlDocument, nodeDictionary, siteHeader, webSiteSearch, site, videoList);

                //// Si es nulo el ultimo video y no lo encuentra en la lista vuelve a obtener los videos
                //if (site.LastVideo != null && !videoList.Any(a => a.VideoUrl == site.LastVideo))
                //{
                //    int findIndex = videoList.FindLastIndex(i => i.VideoUrl == site.LastVideo);
                //    site.LastVideo = lastVideoUrl = videoList[findIndex].VideoUrl;
                //    videoList.RemoveRange(0, findIndex + 1);

                //    site.CurrentPage++;
                //    goto GET_MORE_VIDEO;
                //}
                //// Si el ultimo video despues de tomar mas videos es diferente de nulo y lo encuentra en la lista vuelve a obtener los videos
                //else if (lastVideoUrl != null && videoList.Any(a => a.VideoUrl == lastVideoUrl))
                //{
                //    int findIndex = videoList.FindLastIndex(i => i.VideoUrl == site.LastVideo);
                //    site.LastVideo = lastVideoUrl = videoList[findIndex].VideoUrl;
                //    videoList.RemoveRange(0, findIndex + 1);

                //    site.CurrentPage++;
                //    goto GET_MORE_VIDEO;
                //}

                // Valida si faltan videos y agrega los restantes
                site.CurrentPage++;
                if (site != null && site.LastVideo != null)
                {
                    int findIndex = videoList.FindLastIndex(i => i.VideoUrl == site.LastVideo);
                    if (findIndex >= 0)
                    {
                        site.LastVideo = lastVideoUrl = videoList[findIndex].VideoUrl;
                        videoList.RemoveRange(0, findIndex);
                        int quantityRest = videoList.Count() - webSiteSearch.MaxShow;

                        if (quantityRest < 0)
                        {
                            //site.CurrentPage++;
                            goto GET_MORE_VIDEO;
                        }
                    }
                }

                //int countSiteHeader = siteHeader.SiteList.Count();

                //if (site != null)
                //{
                //    site.LastVideo = siteHeader.VideoList.Last().VideoUrl;
                //}
            }
        }

        private VideoInfo GetVideoView(HtmlDocument htmlDocument, HTMLBrowserSearchInfo webSiteSearch)
        {
            VideoInfo videoInfo = new VideoInfo();
            IList<HtmlNode> nodeDictionary = VideoCollection(htmlDocument, webSiteSearch);
            if (nodeDictionary != null && nodeDictionary.Any())
            {
                if (webSiteSearch.VideoTag.GetProperty != null && nodeDictionary[0].Attributes[webSiteSearch.VideoTag.GetProperty] != null)
                {
                    videoInfo.VideoUrl = nodeDictionary[0].Attributes[webSiteSearch.VideoTag.GetProperty].Value;
                }
                else
                {
                    videoInfo.VideoUrl = HtmlManager.GetTagText(nodeDictionary[0]);
                }

                videoInfo.VideoUrl = HttpUtility.HtmlDecode(videoInfo.VideoUrl);
            }
            return videoInfo;
        }

        public SiteHeaderInfo RetrieveVideoList(int maxShow, int maxBySite, List<SiteInfo> siteList, List<HTMLBrowserSearchInfo> dictionaryList)
        {
            SiteHeaderInfo siteHeader = new SiteHeaderInfo
            {
                VideoList = new List<VideoInfo>(),
                SiteList = (siteList == null ? new List<SiteInfo>() : siteList)
            };

            if (dictionaryList != null && dictionaryList.Any())
            {
                List<Task> taskList = new List<Task>();
                IEnumerable<HTMLBrowserSearchInfo> dictionarySort = dictionaryList.OrderBy(o => o.Order);
                //int pageNumber = dictionaryList[0].PageNumber;
                //const int blockCount = 1;

                //// Se obtiene el número de bloque actual
                //int blockNumber = (int)Math.Ceiling((double)pageNumber / maxShow);
                //int totalSum = 0;

                foreach (HTMLBrowserSearchInfo webSite in dictionarySort)
                {
                    if (!siteHeader.SiteList.Any(f => f.IdSite == webSite.IdWebSite))
                    {
                        siteHeader.SiteList.Add(new SiteInfo
                        {
                            IdSite = webSite.IdWebSite,
                            LastVideo = null
                        });
                    }
                }

                //while (totalSum <= (pageNumber * maxShow))
                //{
                //    foreach (WebSiteSearchInfo dictionaryData in dictionarySort)
                //    {
                //        totalSum += dictionaryData.MaxShow;
                //        if (totalSum > ((pageNumber - 1) * maxShow) && totalSum <= (pageNumber * maxShow))
                //        {
                //            dictionaryData.BlockNumber = blockCount;
                //        }
                //        else
                //        {
                //            //break;
                //        }
                //    }
                //}

                //int maxSite = Math.Abs(dictionarySort.Sum(s => s.MaxShow) - maxShow);
                //foreach (WebSiteSearchInfo web in dictionarySort)
                //{
                //    if (maxSite > 0)
                //    {
                //        web.MaxShow += maxSite;
                //        maxSite = 0;
                //    }
                //}

                List<List<VideoInfo>> videoCollection = new List<List<VideoInfo>>();
                // var webSiteSearchList = dictionarySort.Where(f => f.BlockNumber == blockCount).OrderBy(o => o.Order);

                foreach (HTMLBrowserSearchInfo dictionaryData in dictionarySort)
                {
                    List<VideoInfo> videoList = new List<VideoInfo>();
                    Task runProcess = ProcessSite(dictionaryData, siteHeader, videoList);
                    videoCollection.Add(videoList);
                    runProcess.Start();
                    taskList.Add(runProcess);
                }
                Task.WaitAll(taskList.ToArray());

                List<VideoInfo> allVideoList = new List<VideoInfo>();

                foreach (List<VideoInfo> videoInfoList in videoCollection)
                {
                    allVideoList.AddRange(videoInfoList);
                }

                allVideoList = allVideoList.GroupBy(g => g.Text).Select(s => s.First()).ToList();
                GetNextVideoListRule(siteHeader, allVideoList, maxShow);
            }

            return siteHeader;
        }

        private void GetNextVideoListRule(SiteHeaderInfo siteHeader, List<VideoInfo> videoCollection, int maxShow)
        {
            // Luis Vera falta terminar este metodo para obtener los videos de manera correcta
            if (videoCollection != null && videoCollection.Any())
            {
                int siteQuantity = videoCollection.Select(s => s.IdSite).Distinct().Count();
                int maxBySite = maxShow / siteQuantity;
                foreach (var videoGroup in videoCollection.GroupBy(g => g.IdSite))
                {
                    int maxBySiteCount = 0;
                    bool videoFinded = false;
                    string firstImageUrl = null;
                    string imageUrl = null;
                    string firstVideoUrl = null;
                    string lastVideoUrl = null;
                    SiteInfo site = siteHeader.SiteList.FirstOrDefault(f => f.IdSite == videoGroup.Key);
                    bool addVideoToCollection = false;
                    site.PageTotalCount = videoGroup.Count(); ;

                    foreach (VideoInfo video in videoGroup)
                    {
                        lastVideoUrl = video.VideoUrl;
                        imageUrl = video.ImageUrl;

                        if (!videoFinded)
                        {
                            if (site != null && site.LastVideo != null)
                            {
                                if (lastVideoUrl != site.LastVideo)
                                {
                                    continue;
                                }
                                else
                                {
                                    videoFinded = true;
                                    continue;
                                }
                            }
                        }

                        if (firstVideoUrl == null)
                        {
                            firstVideoUrl = lastVideoUrl;
                        }

                        if (firstImageUrl == null)
                        {
                            firstImageUrl = imageUrl;
                        }

                        if (site != null && site.FirstVideo != null && firstVideoUrl == site.FirstVideo)
                        {
                            break;
                        }

                        lastVideoUrl = video.VideoUrl;
                        siteHeader.VideoList.Add(video);
                        addVideoToCollection = true;

                        if ((++maxBySiteCount) == maxBySite)
                        {
                            break;
                        }
                    }

                    if (site != null && firstVideoUrl != null && lastVideoUrl != null)
                    {
                        site.FirstVideo = firstVideoUrl;
                        site.LastVideo = lastVideoUrl;
                    }

                    if (!addVideoToCollection)
                    {
                    }
                }
            }
        }

        private Task ProcessSite(HTMLBrowserSearchInfo webSiteSearch, SiteHeaderInfo siteHeader, List<VideoInfo> videoList)//, ChromeOptions options)
        {
            return new Task(() =>
            {
                try
                {
                    VideoCollection(webSiteSearch, siteHeader, videoList);
                }
                catch (Exception)
                {
                    //loggerManager.WriteLog(string.Concat("ProcessSite error: ", exception.Message, "\n"), LoggerManager.eMessageType.ERROR, true);
                }

                ////CloseWebSite(htmlDocument);
            });
        }

        public VideoInfo ViewVideo(HTMLBrowserSearchInfo webSiteSearch)
        {
            HtmlDocument htmlDocument = ConnectToWebSite(webSiteSearch, string.Format(webSiteSearch.UrlSearch, webSiteSearch.TextSearch, webSiteSearch.PageNumber));
            return GetVideoView(htmlDocument, webSiteSearch);
        }

        public SiteHeaderInfo SearchVideo(Dictionary<string, object> dictionaryData, string video, int pageNumber, int maxShow)
        {
            int maxBySite = 0;
            List<ItemInfo> itemList = dictionaryData["SelectedSiteList"].DeserializerObject<List<ItemInfo>>();
            List<SiteInfo> siteList = dictionaryData["SiteCollection"].DeserializerObject<List<SiteInfo>>();

            if (siteList != null && siteList.Any())
            {
                maxBySite = maxShow / siteList.Count();
            }
            else
            {
                maxBySite = maxShow / itemList.Count();
            }

            List<HTMLBrowserSearchInfo> webInfo = new List<HTMLBrowserSearchInfo>();
            if (siteList != null)
            {
                //ResponseInfo<IList<CAT_WEBSITE_SEARCH>> webSiteList = catWebSiteSearchDAO.RetrieveAll();
                //if (webSiteList != null && webSiteList.Data != null && webSiteList.Data.Any())
                //{
                //    foreach (CAT_WEBSITE_SEARCH webSiteSearch in webSiteList.Data.Where(f => f.Active))
                //    {
                //        CFG_WEBSITE_SEARCH_TAG videoTag = webSiteSearch.Cfg_WebSite_Search_Tag_Collection.FirstOrDefault(f => f.Cat_WebSite_Video_Property_Type?.Video_Property_Type_Code == "VIDEO");
                //        CFG_WEBSITE_SEARCH_TAG imageTag = webSiteSearch.Cfg_WebSite_Search_Tag_Collection.FirstOrDefault(f => f.Cat_WebSite_Video_Property_Type?.Video_Property_Type_Code == "IMAGE");
                //        CFG_WEBSITE_SEARCH_TAG viewTag = webSiteSearch.Cfg_WebSite_Search_Tag_Collection.FirstOrDefault(f => f.Cat_WebSite_Video_Property_Type?.Video_Property_Type_Code == "VIEW");
                //        CFG_WEBSITE_SEARCH_TAG durationTag = webSiteSearch.Cfg_WebSite_Search_Tag_Collection.FirstOrDefault(f => f.Cat_WebSite_Video_Property_Type?.Video_Property_Type_Code == "DURATION");
                //        CFG_WEBSITE_SEARCH_TAG titleTag = webSiteSearch.Cfg_WebSite_Search_Tag_Collection.FirstOrDefault(f => f.Cat_WebSite_Video_Property_Type?.Video_Property_Type_Code == "TITLE");

                //        webInfo.Add(new WebSiteSearchInfo
                //        {
                //            IdWebSite = webSiteSearch.ID_WebSite_Search,
                //            UrlSearch = webSiteSearch.Url_Search,
                //            TextSearch = video,
                //            PageNumber = pageNumber,
                //            MaxShow = maxBySite,
                //            Order = webSiteSearch.Order,
                //            BlockNumber = webSiteSearch.Block_Number,
                //            InitPageNumberZero = webSiteSearch.Init_Page_Number == 0,
                //            VideoTag = new TagSearchInfo
                //            {
                //                ContainsText = videoTag?.Contains_Text,
                //                TagSearch = videoTag?.Tag_Search,
                //                AttributeSearch = videoTag?.Attribute_Search,
                //                LevelCount = videoTag != null ? videoTag.Level_Count : 0,
                //                GetProperty = videoTag?.Get_Attribute,
                //                RegularExpression = videoTag?.Regular_Expression,
                //                FindType = (FindTypeEnum)Enum.Parse(typeof(FindTypeEnum), videoTag?.Cat_WebSite_Search_Tag_Type.Search_Tag_Type_Code != null ? videoTag?.Cat_WebSite_Search_Tag_Type?.Search_Tag_Type_Code : FindTypeEnum.TAG_NAME.ToString())
                //            },
                //            ImageTag = new TagSearchInfo
                //            {
                //                TagSearch = imageTag?.Tag_Search,
                //                ContainsText = imageTag?.Contains_Text,
                //                AttributeSearch = imageTag?.Attribute_Search,
                //                LevelCount = imageTag != null ? imageTag.Level_Count : 0,
                //                GetProperty = imageTag?.Get_Attribute,
                //                RegularExpression = imageTag?.Regular_Expression,
                //                FindType = (FindTypeEnum)Enum.Parse(typeof(FindTypeEnum), imageTag?.Cat_WebSite_Search_Tag_Type.Search_Tag_Type_Code != null ? imageTag?.Cat_WebSite_Search_Tag_Type?.Search_Tag_Type_Code : FindTypeEnum.TAG_NAME.ToString())
                //            },
                //            ViewsTag = new TagSearchInfo
                //            {
                //                TagSearch = viewTag?.Tag_Search,
                //                ContainsText = viewTag?.Contains_Text,
                //                AttributeSearch = viewTag?.Attribute_Search,
                //                LevelCount = viewTag != null ? viewTag.Level_Count : 0,
                //                GetProperty = viewTag?.Get_Attribute,
                //                RegularExpression = viewTag?.Regular_Expression,
                //                FindType = (FindTypeEnum)Enum.Parse(typeof(FindTypeEnum), viewTag?.Cat_WebSite_Search_Tag_Type.Search_Tag_Type_Code != null ? viewTag?.Cat_WebSite_Search_Tag_Type?.Search_Tag_Type_Code : FindTypeEnum.TAG_NAME.ToString())
                //            },
                //            DurationTag = new TagSearchInfo
                //            {
                //                TagSearch = durationTag?.Tag_Search,
                //                ContainsText = durationTag?.Contains_Text,
                //                AttributeSearch = durationTag?.Attribute_Search,
                //                LevelCount = durationTag != null ? durationTag.Level_Count : 0,
                //                GetProperty = durationTag?.Get_Attribute,
                //                RegularExpression = durationTag?.Regular_Expression,
                //                FindType = (FindTypeEnum)Enum.Parse(typeof(FindTypeEnum), durationTag?.Cat_WebSite_Search_Tag_Type.Search_Tag_Type_Code != null ? durationTag?.Cat_WebSite_Search_Tag_Type?.Search_Tag_Type_Code : FindTypeEnum.TAG_NAME.ToString())
                //            },
                //            TitleTag = titleTag == null ? null : new TagSearchInfo
                //            {
                //                TagSearch = titleTag?.Tag_Search,
                //                ContainsText = titleTag?.Contains_Text,
                //                AttributeSearch = titleTag?.Attribute_Search,
                //                LevelCount = titleTag != null ? titleTag.Level_Count : 0,
                //                GetProperty = titleTag?.Get_Attribute,
                //                RegularExpression = titleTag?.Regular_Expression,
                //                FindType = (FindTypeEnum)Enum.Parse(typeof(FindTypeEnum), titleTag?.Cat_WebSite_Search_Tag_Type.Search_Tag_Type_Code != null ? titleTag?.Cat_WebSite_Search_Tag_Type?.Search_Tag_Type_Code : FindTypeEnum.TAG_NAME.ToString())
                //            }
                //        });
                //    }

                //    int maxSite = Math.Abs(webInfo.Sum(s => s.MaxShow) - maxShow);
                //    foreach (WebSiteSearchInfo web in webInfo)
                //    {
                //        var site = siteList.FirstOrDefault(f => f.IdSite == web.IdWebSite);
                //        if (site != null)
                //        {
                //            web.PageTotalCount = site.PageTotalCount;
                //        }

                //        if (maxSite > 0)
                //        {
                //            web.MaxShow += maxSite;
                //            maxSite = 0;
                //        }
                //    }

                //    if (itemList != null && itemList.Any() && webInfo != null && webInfo.Any())
                //    {
                //        webInfo = webInfo.Where(f => itemList.Any(a => a.id == f.IdWebSite)).ToList();
                //    }
                //}
            }

            return RetrieveVideoList(maxShow, maxBySite, siteList, webInfo);
        }

        public List<ItemInfo> RetrieveSiteList()
        {
            List<ItemInfo> itemList = new List<ItemInfo>();
            //ResponseInfo<IList<CAT_WEBSITE_SEARCH>> webSiteList = catWebSiteSearchDAO.RetrieveAll();

            //if (webSiteList.Data != null && webSiteList.Data.Any())
            //{
            //    itemList = webSiteList.Data.Where(f => f.Active).Select(s => new ItemInfo
            //    {
            //        id = s.ID_WebSite_Search,
            //        itemName = s.WebSite_Search_Name
            //    }).ToList();
            //}

            return itemList;
        }

        //public IList<CAT_WEBSITE_SEARCH> RetrieveAllSiteList()
        //{
        //    return catWebSiteSearchDAO.RetrieveAll().Data.Where(f => f.Active).ToList();
        //}

        private string QueryCatWebSearch(string webSite, string webSiteCode, string urlHome, string urlSearch)
        {
            const string queryCatWebSearchString = "INSERT INTO CAT_WEBSITE_SEARCH(WebSite_Search_Name, WebSite_Search_Code, Url_Home, Url_Search, Text_Search, Page_Number, Max_Show, [Order], Block_Number, Init_Page_Number"
                + ", Alteration_Counter, ID_User_Created, Creation_Date, Active)\n"
                + " SELECT '{0}', SUBSTRING('{1}',1, 10), '{2}', '{3}', NULL, 0, 10, 1, 1, 0, 1, 0, GETDATE(), 1";

            return string.Format(queryCatWebSearchString, webSite, webSiteCode, urlHome, urlSearch);
        }

        private string QueryCfgWebSearchTag(string webSiteCode, string contains, string regularExpression, string tagSearch, string attributeSearch, string GetProperty, string tagTypeCode, string propertyTypeCode)
        {
            const string queryCfgWebSearchTagString = "INSERT INTO CFG_WEBSITE_SEARCH_TAG (Contains_Text, Regular_Expression, Tag_Search, Attribute_Search, Get_Attribute, Level_Count, ID_WebSite_Video_Property_Type,"
                + "ID_WebSite_Search_Tag_Type, ID_WebSite_Search)\n"
                + "SELECT '{0}', '{1}', '{2}', '{3}', '{4}', 0, \n"
                + "(SELECT TOP 1 ID_WebSite_Video_Property_Type FROM CAT_WEBSITE_VIDEO_PROPERTY_TYPE WHERE Video_Property_Type_Code = '{7}'), \n"
                + "(SELECT TOP 1 ID_WebSite_Search_Tag_Type FROM CAT_WEBSITE_SEARCH_TAG_TYPE WHERE Search_Tag_Type_Code = '{6}'), \n"
                + "(SELECT TOP 1 ID_WebSite_Search FROM CAT_WEBSITE_SEARCH WHERE WebSite_Search_Code = '{5}')";

            return string.Format(queryCfgWebSearchTagString, contains, regularExpression, tagSearch, attributeSearch, GetProperty, webSiteCode, tagTypeCode, propertyTypeCode);
        }

        private string ExtractDomainFromURL(string sURL)
        {
            Regex rg = new Regex(@"://(?<host>([a-z\d][-a-z\d]*[a-z\d]\.)*[a-z][-a-z\d]+[a-z])");
            string url = null;
            if (rg.IsMatch(sURL))
            {
                url = rg.Match(sURL).Result("${host}");

                string[] urlSplit = url.Split('.');

                if (urlSplit.Length == 2)
                {
                    url = urlSplit[0];
                }
                else
                {
                    url = urlSplit[1];
                }

            }

            return url;
        }


        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.youporn.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.xvideos.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.xnxx.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fes.pornhub.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fes.redtube.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.youjizz.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.tube8.com

        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.hardsextube.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.drtuber.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/http%3A%2F%2Fwww.largeporntube.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.porn.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.xtube.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.nuvid.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.nudevista.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.porntube.com
        //http://localhost:5000/api/Portal/GetSiteConfiguration/https%3A%2F%2Fwww.bravoporn.com

        private bool LoadPage(string page, int delay)
        {
            return true;
        }

        private HtmlNode GetWebElement(HtmlDocument htmlDocument, string tag, string by, string getOption,
            string attribute = null, string property = null, string attributeValue = null, string regularExpression = null)
        {
            HtmlNode element = null;

            try
            {
                List<XmlNodeSearchAttribute> searchAttribute = null;
                if (!string.IsNullOrWhiteSpace(attribute))
                {
                    searchAttribute = new List<XmlNodeSearchAttribute>
                    {
                        new XmlNodeSearchAttribute
                        {
                            Name = attribute,
                            Value = attributeValue,
                            ExactSearch = false,
                            SearchActivated = true,
                            RegularExpression = regularExpression
                        }
                    };
                }

                switch (by)
                {
                    case CommonConstants.ID:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.ID,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;

                    case CommonConstants.NAME:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;

                    case CommonConstants.TAG_NAME:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.TAG_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;

                    case CommonConstants.TEXT:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.TAG_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;

                    case CommonConstants.CONTAINS:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.TAG_NAME,
                                ExactSearch = false,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;

                    case CommonConstants.CLASS_NAME:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.CLASS_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;

                    case CommonConstants.CSS_SELECTOR:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.CLASS_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;

                    case CommonConstants.XPATH:
                        {
                            string elementValue = null;

                            TagSearchInfo tagSearchInfo = new TagSearchInfo
                            {
                                ContainsText = tag,
                                RegularExpression = "",
                                TagSearch = "",
                                GetProperty = null,
                                GetAttribute = null,
                                DepthCount = 0,
                                FindType = FindTypeEnum.XPATH
                            };

                            switch(getOption)
                            {
                                case "INNER_TEXT":
                                    tagSearchInfo.GetProperty = "InnerText";
                                    elementValue = HtmlManager.FindPropertyByXPath<string>(tagSearchInfo, htmlDocument.DocumentNode);
                                    break;

                                case "INNER_HTML":
                                    tagSearchInfo.GetProperty = "InnerHtml";
                                    elementValue = HtmlManager.FindPropertyByXPath<string>(tagSearchInfo, htmlDocument.DocumentNode);
                                    break;

                                case "OUTER_HTML":
                                    tagSearchInfo.GetProperty = "OuterHtml";
                                    elementValue = HtmlManager.FindPropertyByXPath<string>(tagSearchInfo, htmlDocument.DocumentNode);
                                    break;

                                case "ATTRIBUTE":
                                    tagSearchInfo.GetAttribute = attribute;
                                    elementValue = HtmlManager.FindAttributeByXPath<string>(tagSearchInfo, htmlDocument.DocumentNode);
                                    break;

                                case "PROPERTY":
                                    tagSearchInfo.GetProperty = property;
                                    elementValue = HtmlManager.FindPropertyByXPath<string>(tagSearchInfo, htmlDocument.DocumentNode);
                                    break;

                                default:
                                    break;
                            }
                        }
                        break;

                    default:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.ID,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            element = HtmlManager.HTMLTag(htmlDocument.DocumentNode.ChildNodes, tag);
                        }
                        break;
                }
            }
            catch (Exception)
            {

            }

            return element;
        }

        private IList<HtmlNode> GetWebElementCollection(HtmlDocument htmlDocument, string tag, string by, 
            string attribute = null, string attributeValue = null, string regularExpression = null)
        {
            IList<HtmlNode> nodeList = null;

            try
            {
                List<XmlNodeSearchAttribute> searchAttribute = null;
                if (!string.IsNullOrWhiteSpace(attribute))
                {
                    searchAttribute = new List<XmlNodeSearchAttribute>
                    {
                        new XmlNodeSearchAttribute
                        {
                            Name = attribute,
                            Value = attributeValue,
                            ExactSearch = false,
                            SearchActivated = true,
                            RegularExpression = regularExpression
                        }
                    };
                }

                switch (by)
                {
                    case CommonConstants.ID:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.ID,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;

                    case CommonConstants.NAME:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;

                    case CommonConstants.TAG_NAME:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.TAG_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;

                    case CommonConstants.TEXT:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.TAG_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;

                    case CommonConstants.CONTAINS:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.TAG_NAME,
                                ExactSearch = false,
                                SearchAttribute = searchAttribute
                            };

                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;

                    case CommonConstants.CLASS_NAME:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.CLASS_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;

                    case CommonConstants.XPATH:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.XPATH,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            //*[@id="title"]/h1/yt-formatted-string
                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;

                    default:
                        {
                            XmlNodeInfo xmlNodeInfo = new XmlNodeInfo
                            {
                                Name = tag,
                                FindType = FindTypeEnum.TAG_NAME,
                                ExactSearch = true,
                                SearchAttribute = searchAttribute
                            };

                            HtmlManager.HTMLNodeListByNodeInfo(htmlDocument.DocumentNode.ChildNodes, xmlNodeInfo, ref nodeList);
                        }
                        break;
                }
            }
            catch (Exception)
            {

            }

            return nodeList;
        }

        private IObjectType ProcessSite()
        {
            IObjectType result = null;
            HTMLBrowserDTO webSiteDTO = processBC.GetProcessedInputValue<HTMLBrowserDTO>();

            if (webSiteDTO != null)
            {
                HTMLBrowserSearchInfo webSiteSearchInfo = new HTMLBrowserSearchInfo();
                HtmlDocument htmlDocument = null;
                string pageFile = webSiteDTO.IsLocalFile ? webSiteDTO.File : webSiteDTO.Page;

                if (webSiteDTO != null && !string.IsNullOrWhiteSpace(pageFile) && (webSiteDTO.CurrentPage || (htmlDocument = ConnectToWebSite(webSiteSearchInfo, pageFile)) != null))
                {
                    try
                    {
                        string objectId = webSiteDTO.Object;
                        string searchType = webSiteDTO.SearchType;

                        if (webSiteDTO.Multiple)
                        {
                            IList<HtmlNode> elementCollection = GetWebElementCollection(htmlDocument, objectId, searchType);
                            if (elementCollection?.Count > 0)
                            {
                                result = new ListObjectType(elementCollection.Cast<HtmlNode>().Select(s => s.InnerHtml).ToList());
                            }
                        }
                        else
                        {
                            HtmlNode element = GetWebElement(htmlDocument, objectId, searchType, webSiteDTO.Get,
                                webSiteDTO.Attribute, webSiteDTO.Property);

                            if (element != null)
                            {
                                result = new TextType(element.InnerHtml);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        StepFail(ex.Message);
                    }
                }
                else
                {
                    // NO SE PUDO CONECTAR AL SITIO O ARCHIVO, FAVOR DE REVISAR LOS CAMPOS
                }
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            return ProcessSite();
        }
    }
}