/*
' Copyright (c) 2016  nicholashew@users.noreply.github.com
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
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security;
using FreeSource.Modules.Html.Components;

namespace FreeSource.Modules.Html
{
    /// <summary>
    /// The Settings ModuleSettingsBase is used to manage the settings for the HTML Module
    /// </summary>
    public partial class Settings : ModuleSettingsBase
    {
        private HtmlModuleSettings _moduleSettings;
        private new HtmlModuleSettings ModuleSettings
        {
            get
            {
                return _moduleSettings ?? (_moduleSettings = new HtmlModuleSettingsRepository().GetSettings(this.ModuleConfiguration));
            }
        }

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            btnCleanAll.Click += OnHtmlTextCleanAllClick;
            btnCleanSelection.Click += OnHtmlTextCleanSelection;
        }

        private void OnHtmlTextCleanAllClick(Object sender, EventArgs e)
        {
            HtmlTextController htc = new HtmlTextController();
            foreach (var htmlText in GetCleanableHtmlTexts())
            {
                htc.DeleteHtmlText(htmlText);
            }
            BindCleanableItems();
        }

        private void OnHtmlTextCleanSelection(Object sender, EventArgs e)
        {
            HtmlTextController htc = new HtmlTextController();
            var cleanableItems = GetCleanableHtmlTexts();
            foreach (ListItem item in cleanableListBox.Items)
            {
                if (item.Selected)
                {
                    var itemToDelete = cleanableItems.Where(x => x.ItemId == int.Parse(item.Value)).SingleOrDefault();
                    if (itemToDelete != null)
                    {
                        htc.DeleteHtmlText(itemToDelete);
                    }
                }
            }
            BindCleanableItems();
        }
        #endregion

        #region Private Functions

        private bool HasAdminRights()
        {
            return (UserInfo.IsSuperUser || PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName));
        }

        private IEnumerable<HtmlTextInfo> GetCleanableHtmlTexts()
        {
            var htmlTextController = new HtmlTextController();

            var allHtmlTexts = htmlTextController.GetAllHtmlText();
            var allModules = ModuleController.Instance.GetModules(PortalId).Cast<ModuleInfo>().ToList();

            var distinctHtmlTextModuleIDs = allHtmlTexts.Select(x => x.ModuleId).Distinct();
            var distinctModulesModuleIDs = allModules.Select(x => x.ModuleID).Distinct();

            var cleanableModuleIDs = distinctHtmlTextModuleIDs.Except(distinctModulesModuleIDs).ToList();

            return allHtmlTexts.Where(x => cleanableModuleIDs.Contains(x.ModuleId));
        }

        private void BindCleanableItems()
        {
            cleanableListBox.Items.Clear();

            foreach (var htmlText in GetCleanableHtmlTexts())
            {
                string text = HtmlUtils.Clean(htmlText.ModuleTitle + " - " + htmlText.Content, false);
                var item = new ListItem(string.Format("{0} ({1}) - {2}", HtmlUtils.Shorten(text, 35, "..."), htmlText.Locale, htmlText.LastModifiedOnDate), htmlText.ItemId.ToString());
                cleanableListBox.Items.Add(item);
            }
        }
        #endregion

        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    chkReplaceTokens.Checked = ModuleSettings.ReplaceTokens;
                    txtSearchDescLength.Text = ModuleSettings.SearchDescLength.ToString();
                    rblEnableFallback.SelectedValue = ModuleSettings.EnableFallback.ToString();

                    if (HasAdminRights())
                    {
                        var htmlTextController = new HtmlTextController();
                        phAdvanceSettings.Visible = true;
                        ddlMaxVersion.DataSource = Enumerable.Range(1, 15);
                        ddlMaxVersion.DataBind();

                        int maxVersion = htmlTextController.GetMaximumVersionHistory(PortalId);
                        if (ddlMaxVersion.Items.FindByValue(maxVersion.ToString()) != null)
                        {
                            ddlMaxVersion.Items.FindByValue(maxVersion.ToString()).Selected = true;
                        }

                        BindCleanableItems();
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                //update module settings
                ModuleSettings.ReplaceTokens = chkReplaceTokens.Checked;
                ModuleSettings.SearchDescLength = int.Parse(txtSearchDescLength.Text);
                ModuleSettings.EnableFallback = bool.Parse(rblEnableFallback.SelectedValue);
                var repo = new HtmlModuleSettingsRepository();
                repo.SaveSettings(this.ModuleConfiguration, ModuleSettings);

                // disable module caching if token replace is enabled
                if (chkReplaceTokens.Checked)
                {
                    ModuleInfo module = ModuleController.Instance.GetModule(ModuleId, TabId, false);
                    if (module.CacheTime > 0)
                    {
                        module.CacheTime = 0;
                        ModuleController.Instance.UpdateModule(module);
                    }
                }

                // advance global settings
                if (HasAdminRights())
                {
                    var htmlTextController = new HtmlTextController();
                    htmlTextController.UpdateMaximumVersionHistorySetting(PortalId, Convert.ToInt32(ddlMaxVersion.SelectedValue));
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion
    }
}