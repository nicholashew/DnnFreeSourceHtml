/*
' Copyright (c) 2016 nicholashew@users.noreply.github.com
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;

namespace FreeSource.Modules.Html.Components
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Controller class for Html
    /// 
    /// The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    /// DotNetNuke will poll this class to find out which Interfaces the class implements. 
    /// 
    /// The IPortable interface is used to import/export content from a DNN module
    /// 
    /// The ISearchable interface is used by DNN to index the content of a module
    /// 
    /// The IUpgradeable interface allows module developers to execute code during the upgrade 
    /// process for a module.
    /// 
    /// Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class FeatureController : ModuleSearchBase, IPortable, IUpgradeable
    {
        // feel free to remove any interfaces that you don't wish to use
        // (requires that you also update the .dnn manifest file)

        #region Optional Interfaces

        /// <summary>
        /// Gets the modified search documents for the DNN search engine indexer.
        /// </summary>
        /// <param name="moduleInfo">The module information.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();
            var controller = new HtmlTextController();
            var items = controller.GetHtmlTextList(moduleInfo.ModuleID);
            var repo = new HtmlModuleSettingsRepository();
            var settings = repo.GetSettings(moduleInfo);

            foreach (var item in items)
            {
                if (item.LastModifiedOnDate.ToUniversalTime() <= beginDate.ToUniversalTime() ||
                    item.LastModifiedOnDate.ToUniversalTime() >= DateTime.UtcNow)
                    continue;

                // Get the content & summary
                var strContent = HtmlUtils.Clean(string.Format("{0}<br />{1}", item.Content, item.Summary), false);

                // Get the description string
                var description = strContent.Length <= settings.SearchDescLength ? strContent : HtmlUtils.Shorten(strContent, settings.SearchDescLength, "...");

                var searchDocumnet = new SearchDocument
                {
                    UniqueKey = string.Format("Items:{0}:{1}", moduleInfo.ModuleID, item.ItemId),  // any unique identifier to be able to query for your individual record
                    PortalId = moduleInfo.PortalID,  // the PortalID
                    TabId = moduleInfo.TabID, // the TabID
                    AuthorUserId = item.LastModifiedByUserId, // the person who created the content
                    Title = moduleInfo.ModuleTitle,  // the title of the content, but should be the module title
                    Description = description,  // the description or summary of the content
                    Body = strContent,  // the long form of your content
                    ModifiedTimeUtc = item.LastModifiedOnDate.ToUniversalTime(),  // a time stamp for the search results page
                    CultureCode = moduleInfo.CultureCode, // the current culture code
                    IsActive = true  // allows you to remove the item from the search index (great for soft deletes)
                };

                searchDocuments.Add(searchDocumnet);
            }

            return searchDocuments;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be exported</param>
        /// -----------------------------------------------------------------------------
        public string ExportModule(int moduleId)
        {
            var controller = new HtmlTextController();
            HtmlTextInfo item = controller.GetTopHtmlText(moduleId, System.Threading.Thread.CurrentThread.CurrentCulture.Name, true);
            var sb = new StringBuilder();

            if (item == null)
                return string.Empty;

            sb.Append("<htmlText>");
            //sb.AppendFormat("<ItemId>{0}</ItemId>", item.ItemId);
            //sb.AppendFormat("<ModuleId>{0}</ModuleId>", item.ModuleId);
            //sb.AppendFormat("<Locale>{0}</Locale>", item.Locale);
            sb.AppendFormat("<ModuleTitle>{0}</ModuleTitle>", XmlUtils.XMLEncode(item.ModuleTitle));
            sb.AppendFormat("<Content>{0}</Content>", XmlUtils.XMLEncode(item.Content));
            sb.AppendFormat("<Summary>{0}</Summary>", XmlUtils.XMLEncode(item.Summary));
            //sb.AppendFormat("<Version>{0}</Version>", item.Version);
            //sb.AppendFormat("<IsPublished>{0}</IsPublished>", item.IsPublished);
            //sb.AppendFormat("<CreatedByUserID>{0}</CreatedByUserID>", item.CreatedByUserID);
            //sb.AppendFormat("<CreatedOnDate>{0}</CreatedOnDate>", item.CreatedOnDate);
            //sb.AppendFormat("<LastModifiedByUserId>{0}</LastModifiedByUserId>", item.LastModifiedByUserId);
            //sb.AppendFormat("<LastModifiedOnDate>{0}</LastModifiedOnDate>", item.LastModifiedOnDate);
            sb.Append("</htmlText>");

            return sb.ToString();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be imported</param>
        /// <param name="content">The content to be imported</param>
        /// <param name="version">The version of the module to be imported</param>
        /// <param name="userId">The Id of the user performing the import</param>
        /// -----------------------------------------------------------------------------
        public void ImportModule(int moduleId, string content, string version, int userId)
        {
            ModuleInfo module = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            var controller = new HtmlTextController();
            XmlNode item = DotNetNuke.Common.Globals.GetContent(content, "htmlText");

            if (item == null) return;

            var newItem = new HtmlTextInfo()
            {
                ModuleId = moduleId,
                Locale = System.Threading.Thread.CurrentThread.CurrentCulture.Name,
                ModuleTitle = item.SelectSingleNode("ModuleTitle").InnerText,
                Content = item.SelectSingleNode("Content").InnerText,
                Summary = item.SelectSingleNode("Summary").InnerText,
                Version = -1,
                IsPublished = true, 
                CreatedByUserID = userId,
                CreatedOnDate = DateTime.Now,
                LastModifiedByUserId = userId,
                LastModifiedOnDate = DateTime.Now
            };

            // NOTE: If moving from one installation to another, this user will not exist
            controller.UpdateHtmlText(newItem, true, controller.GetMaximumVersionHistory(module.PortalID));

        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeModule implements the IUpgradeable Interface
        /// </summary>
        /// <param name="version">The current version of the module</param>
        /// -----------------------------------------------------------------------------
        public string UpgradeModule(string version)
        {
            try
            {
                switch (version)
                {
                    case "01.00.00":
                        break;
                }
                return "success";
            }
            catch
            {
                return "failure";
            }
        }

        #endregion
    }
}