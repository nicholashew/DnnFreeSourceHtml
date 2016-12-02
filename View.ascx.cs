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
using System.Web.UI;
using FreeSource.Modules.Html.Components;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

namespace FreeSource.Modules.Html
{
    /// <summary>
    /// The HtmlModule Class provides the UI for displaying the Html
    /// </summary>
    public partial class View : HtmlModuleBase, IActionable
    {
        private bool EditorEnabled;

        #region Event Handlers

        /// <summary>
        /// Page_Init runs when the control is initialized
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            lblContent.UpdateLabel += lblContent_UpdateLabel;
            EditorEnabled = PortalSettings.InlineEditorEnabled;
            try
            {
                //Add an Action Event Handler to the Skin
                AddActionHandler(ModuleAction_Click);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                // edit in place.
                EditorEnabled = (EditorEnabled && IsEditable && PortalSettings.UserMode == PortalSettings.Mode.Edit && !Settings.ReplaceTokens);
                
                // toolbar visibility
                if (!IsPostBack && EditorEnabled)
                {
                    foreach (DNNToolBarButton button in editorDnnToobar.Buttons)
                    {
                        button.ToolTip = Localization.GetString(button.ToolTip + ".ToolTip", LocalResourceFile);
                    }
                }
                else {                    
                    editorDnnToobar.Visible = false;
                }

                // get content
                var htc = new HtmlTextController();                
                HtmlTextInfo htmlText = null;                
                string contentString = "";
                string contentTitle = "";
                string localeCode = CurrentLocaleCode;
                bool useDefaultLanguage = true;

                // try get localize content if enabled
                if (CanLocalize(localeCode))
                {
                    htmlText = htc.GetTopHtmlText(ModuleId, localeCode, true);
                    useDefaultLanguage = (htmlText != null || (htmlText == null && !Settings.EnableFallback)) ? false : true;
                }

                // default or fallback
                if (useDefaultLanguage)
                    htmlText = htc.GetTopHtmlText(ModuleId, PortalSettings.DefaultLanguage, true);

                // set variables
                if (htmlText != null)
                {
                    contentString = htmlText.Content;
                    contentTitle = htmlText.ModuleTitle;
                }
                else
                {
                    // add some help text when no content and in edit mode
                    if (!IsPostBack && PortalSettings.UserMode == PortalSettings.Mode.Edit)
                        contentString = Localization.GetString(EditorEnabled ? "AddContentFromToolBar.Text" : "AddContentFromActionMenu.Text", LocalResourceFile);
                    else
                        ContainerControl.Visible = false; // hide the module if no content and in view mode
                }                               
                
                // add content to module
                lblContent.Controls.Add(new LiteralControl(HtmlTextController.FormatHtmlText(ModuleId, contentString, Settings, PortalSettings, Page)));
                lblContent.EditEnabled = EditorEnabled;

                // override title to module
                if (!String.IsNullOrEmpty(contentTitle))
                {
                    ModuleConfiguration.ModuleTitle = contentTitle;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// lblContent_UpdateLabel allows for inline editing of content
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void lblContent_UpdateLabel(object source, DNNLabelEditEventArgs e)
        {
            try
            {
                // verify security 
                if ((!new PortalSecurity().InputFilter(e.Text, PortalSecurity.FilterFlag.NoScripting).Equals(e.Text)))
                {
                    throw new SecurityException();
                }
                else if (EditorEnabled && IsEditable && PortalSettings.UserMode == PortalSettings.Mode.Edit)
                {
                    // get content
                    var htmlTextController = new HtmlTextController();
                    HtmlTextInfo htmlText = htmlTextController.GetTopHtmlText(ModuleId, CurrentLocaleCode, true);
                    if (htmlText == null)
                    {
                        htmlText = new HtmlTextInfo();
                        htmlText.ItemId = -1;
                        // use current display title for new item only
                        htmlText.ModuleTitle = ModuleContext.Configuration.ModuleTitle; 
                    }

                    // set content attributes
                    htmlText.ModuleId = ModuleId;
                    htmlText.Locale = CurrentLocaleCode;
                    htmlText.Content = Server.HtmlEncode(e.Text);
                    htmlText.IsPublished = true;

                    // save the content with new version
                    htmlTextController.UpdateHtmlText(htmlText, true, htmlTextController.GetMaximumVersionHistory(PortalId));

                    //refresh module cache
                    ModuleController.SynchronizeModule(ModuleId);
                }
                else
                {
                    throw new SecurityException();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// ModuleAction_Click handles all ModuleAction events raised from the skin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModuleAction_Click(object sender, ActionEventArgs e)
        {
            if (e.Action.Url.Length > 0)
            {
                Response.Redirect(e.Action.Url, true);
            }
        }

        #endregion

        #region Methods

        protected bool CanLocalize(string localeCode)
        {
            Locale enabledLanguage = null;
            bool isLanguageEnabled = LocaleController.Instance.GetLocales(ModuleContext.PortalId).TryGetValue(localeCode, out enabledLanguage);
            bool isDefaultLanguage = (localeCode == PortalSettings.DefaultLanguage);

            //return PortalSettings.ContentLocalizationEnabled && isLanguageEnabled && !isDefaultLanguage;
            return isLanguageEnabled && !isDefaultLanguage;
        }

        #endregion

        #region Optional Interfaces

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), Localization.GetString("EditModule", LocalResourceFile), ModuleActionType.AddContent, "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }

        #endregion
    }
}