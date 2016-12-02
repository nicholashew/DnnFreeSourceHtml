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
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using DotNetNuke.Entities.Modules;

namespace FreeSource.Modules.Html.Components
{
    /// <summary>
    /// The HtmlTextController is the Controller class for managing HtmlText information the HtmlText module
    /// </summary>
    class HtmlTextController
    {

        #region Constructor

        private readonly IHtmlTextRepository repository;

        public HtmlTextController(IHtmlTextRepository htmlTextRepository)
        {
            Requires.NotNull(htmlTextRepository);

            repository = htmlTextRepository;
        }
        
        public HtmlTextController() : this(HtmlTextRepository.Instance) { }

        #endregion

        #region Utilities

        /// <summary>
        /// GetMaximumVersionHistory retrieves the maximum number of versions to store for a module
        /// </summary>
        /// <param name="PortalID"></param>
        /// <returns></returns>
        public int GetMaximumVersionHistory(int portalId)
        {
            int intMaximumVersionHistory = -1;

            intMaximumVersionHistory = int.Parse(PortalController.GetPortalSetting(Constants.MaximumVersionHistoryPortalSettingName, portalId, "-1"));

            // if undefined at portal level, set portal default
            if (intMaximumVersionHistory == -1)
            {
                intMaximumVersionHistory = Constants.DefaultMaximumVersionHistory;
                // default
                UpdateMaximumVersionHistorySetting(portalId, intMaximumVersionHistory);
            }

            return intMaximumVersionHistory;
        }

        public void UpdateMaximumVersionHistorySetting(int portalId, int maxVersion)
        {
            PortalController.UpdatePortalSetting(portalId, Constants.MaximumVersionHistoryPortalSettingName, maxVersion.ToString());
        }

        public static string ManageRelativePaths(string strHTML, string strUploadDirectory, string strToken, int intPortalID)
        {
            int P = 0;
            int R = 0;
            int S = 0;
            int tLen = 0;
            string strURL = null;
            var sbBuff = new StringBuilder("");

            if (!string.IsNullOrEmpty(strHTML))
            {
                tLen = strToken.Length + 2;
                string uploadDirectory = strUploadDirectory.ToLower();

                //find position of first occurrance:
                P = strHTML.IndexOf(strToken + "=\"", StringComparison.InvariantCultureIgnoreCase);
                while (P != -1)
                {
                    sbBuff.Append(strHTML.Substring(S, P - S + tLen));
                    //keep charactes left of URL
                    S = P + tLen;
                    //save startpos of URL
                    R = strHTML.IndexOf("\"", S);
                    //end of URL
                    if (R >= 0)
                    {
                        strURL = strHTML.Substring(S, R - S).ToLower();
                    }
                    else
                    {
                        strURL = strHTML.Substring(S).ToLower();
                    }

                    if (strHTML.Substring(P + tLen, 10).Equals("data:image", StringComparison.InvariantCultureIgnoreCase))
                    {
                        P = strHTML.IndexOf(strToken + "=\"", S + strURL.Length + 2, StringComparison.InvariantCultureIgnoreCase);
                        continue;
                    }

                    // if we are linking internally
                    if (!strURL.Contains("://"))
                    {
                        // remove the leading portion of the path if the URL contains the upload directory structure
                        string strDirectory = uploadDirectory;
                        if (!strDirectory.EndsWith("/"))
                        {
                            strDirectory += "/";
                        }
                        if (strURL.IndexOf(strDirectory) != -1)
                        {
                            S = S + strURL.IndexOf(strDirectory) + strDirectory.Length;
                            strURL = strURL.Substring(strURL.IndexOf(strDirectory) + strDirectory.Length);
                        }
                        // add upload directory
                        if (!strURL.StartsWith("/")
                            && !String.IsNullOrEmpty(strURL.Trim())) //We don't write the UploadDirectory if the token/attribute has not value. Therefore we will avoid an unnecessary request
                        {
                            sbBuff.Append(uploadDirectory);
                        }
                    }
                    //find position of next occurrance
                    P = strHTML.IndexOf(strToken + "=\"", S + strURL.Length + 2, StringComparison.InvariantCultureIgnoreCase);
                }

                if (S > -1)
                {
                    sbBuff.Append(strHTML.Substring(S));
                }
                //append characters of last URL and behind
            }

            return sbBuff.ToString();
        }

        /// <summary>
        /// FormatHtmlText formats HtmlText content for display in the browser
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="moduleId">The ModuleID</param>
        /// <param name = "content">The HtmlText Content</param>
        /// <param name = "settings">Module Settings</param>
        /// <param name="portalSettings">The Portal Settings.</param>
        /// <param name="page">The Page Instance.</param>
        public static string FormatHtmlText(int moduleId, string content, HtmlModuleSettings settings, PortalSettings portalSettings, Page page)
        {
            // token replace

            if (settings.ReplaceTokens)
            {
                var tr = new HtmlTokenReplace(page)
                {
                    AccessingUser = UserController.Instance.GetCurrentUserInfo(),
                    DebugMessages = portalSettings.UserMode != PortalSettings.Mode.View,
                    ModuleId = moduleId,
                    PortalSettings = portalSettings
                };
                content = tr.ReplaceEnvironmentTokens(content);
            }

            // Html decode content
            content = HttpUtility.HtmlDecode(content);

            // manage relative paths
            content = ManageRelativePaths(content, portalSettings.HomeDirectory, "src", portalSettings.PortalId);
            content = ManageRelativePaths(content, portalSettings.HomeDirectory, "background", portalSettings.PortalId);

            return content;
        }

        #endregion

        #region Uses HtmlTextRepository

        /// <summary>
        /// Get HtmlText by ItemId
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public HtmlTextInfo GetHtmlText(int itemId)
        {
            return repository.FindById(itemId);
        }

        /// <summary>
        /// GetTopHtmlText gets the most recent HtmlTextInfo object for the Module
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="locale"></param>
        /// <param name="isPublished"></param>
        /// <returns></returns>
        public HtmlTextInfo GetTopHtmlText(int moduleId, string locale, bool isPublished)
        {
            return GetHtmlTextList(moduleId, locale).Where(item => item.IsPublished = isPublished).OrderByDescending(item => item.Version).FirstOrDefault();
        }

        public IEnumerable<HtmlTextInfo> GetAllHtmlText()
        {
            return repository.GetAll();
        }

        public IEnumerable<HtmlTextInfo> GetHtmlTextList(int moduleId)
        {
            return repository.GetAll(moduleId);
        }

        public IEnumerable<HtmlTextInfo> GetHtmlTextList(int moduleId, string locale)
        {
            return repository.GetAll(moduleId, locale);
        }

        public void UpdateHtmlText(HtmlTextInfo htmlText, bool blnCreateNewVersion, int maximumVersionHistory)
        {
            Requires.NotNull("htmlText", htmlText);
            //Requires.PropertyNotNegative("htmlText", "ItemId", htmlText.ItemId);
            Requires.PropertyNotNegative("htmlText", "ModuleId", htmlText.ModuleId);

            //flag create new version if ItemId is not valid                                                       
            if (htmlText.ItemId == -1)
            {
                blnCreateNewVersion = true;
            }

            if (blnCreateNewVersion)
            {
                //add new version content
                htmlText.CreatedByUserID = UserController.Instance.GetCurrentUserInfo().UserID;
                htmlText.CreatedOnDate = DateTime.Now;
                htmlText.LastModifiedByUserId = UserController.Instance.GetCurrentUserInfo().UserID;
                htmlText.LastModifiedOnDate = DateTime.Now;
                repository.Add(htmlText, maximumVersionHistory);
            }
            else {
                //update existing content
                htmlText.LastModifiedByUserId = UserController.Instance.GetCurrentUserInfo().UserID;
                htmlText.LastModifiedOnDate = DateTime.Now;
                repository.Update(htmlText);
            }
        }

        public void DeleteHtmlText(HtmlTextInfo htmlText)
        {
            Requires.NotNull(htmlText);
            Requires.PropertyNotNegative("htmlText", "ItemId", htmlText.ItemId);

            repository.Delete(htmlText);
        }
        
        #endregion

    }
}