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
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using FreeSource.Modules.Html.Components;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Security;

namespace FreeSource.Modules.Html
{
    /// <summary>
    /// The EditHtml PortalModuleBase is used to manage Html
    /// </summary>
    public partial class Edit : HtmlModuleBase
    {

        private readonly HtmlTextController _htmlTextController = new HtmlTextController();

        private int _topHtmlTextItemId = -1; //temp variable for lookup current active item during listview databound

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            hlCancel.NavigateUrl = Globals.NavigateURL();

            ddlLocale.SelectedIndexChanged += ddlLocale_SelectedIndexChanged;
            btnSave.Click += btnSave_Click;
            btnEdit.Click += btnEdit_Click;
            btnPreview.Click += btnPreview_Click;
            btnPreviewBack.Click += btnPreviewBack_Click;
            btnHistory.Click += btnHistory_Click;
            lvVersions.ItemDataBound += lvVersions_ItemDataBound;
            lvVersions.ItemCommand += lvVersions_ItemCommand;
            lvVersions.PagePropertiesChanging += lvVersions_PagePropertiesChanging;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (!Page.IsPostBack)
                {
                    //bind language dropdown
                    ddlLocale.DataSource = LocaleController.Instance.GetLocales(PortalId).Values;
                    ddlLocale.DataTextField = "Text";
                    ddlLocale.DataValueField = "Code";
                    ddlLocale.DataBind();
                    ddlLocale.SelectedValue = CurrentLocaleCode;

                    //load current locale content
                    var htmlText = _htmlTextController.GetTopHtmlText(ModuleId, CurrentLocaleCode, true);
                    DisplayContent(htmlText);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void ddlLocale_SelectedIndexChanged(object sender, EventArgs e)
        {
            var topHtmlText = _htmlTextController.GetTopHtmlText(ModuleId, ddlLocale.SelectedValue, true);

            if (phEdit.Visible)
            {
                //Edit
                DisplayContent(topHtmlText);
                //setEditContent(topHtmlText); this method will be call from DisplayContent
            }
            else if (phPreview.Visible)
            {
                //Preview
                DisplayPreview(topHtmlText, false);
                setEditContent(topHtmlText);
            }
            else if (phHistory.Visible)
            {
                //Versions
                DisplayVersions();
                setEditContent(topHtmlText);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string localeCode = ddlLocale.SelectedValue;
                var htmlText = _htmlTextController.GetTopHtmlText(ModuleId, localeCode, true);
                if (htmlText == null)
                {
                    htmlText = new HtmlTextInfo();
                    htmlText.ItemId = -1;
                    htmlText.ModuleId = ModuleId;
                    htmlText.Locale = localeCode;
                }

                if (phEdit.Visible)
                {
                    htmlText.ModuleTitle = txtModuleTitle.Text.Trim();
                    htmlText.Content = txtContent.Text.Trim();
                    htmlText.Summary = txtSummary.Text.Trim();
                }
                else {
                    var editContent = new JavaScriptSerializer().Deserialize<HtmlTextInfo>(hfEditor.Value);
                    if (editContent != null)
                    {
                        htmlText.ModuleTitle = editContent.ModuleTitle;
                        htmlText.Content = editContent.Content;
                        htmlText.Summary = editContent.Summary;
                    }
                }

                //for remark only, not display on public view
                htmlText.IsPublished = true; //always set to true, since not implementing workflow feature now

                _htmlTextController.UpdateHtmlText(htmlText, true, _htmlTextController.GetMaximumVersionHistory(PortalId));
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Page, "Error occurred: ", exc.Message, ModuleMessage.ModuleMessageType.RedError);
                return;
            }

            // redirect back to portal
            Response.Redirect(Globals.NavigateURL(), true);
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            //get from hidden value
            var editContent = new JavaScriptSerializer().Deserialize<HtmlTextInfo>(hfEditor.Value);

            //if null, get from top item
            if (editContent == null)
                editContent = _htmlTextController.GetTopHtmlText(ModuleId, ddlLocale.SelectedValue, true);

            DisplayContent(editContent);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            HtmlTextInfo editContent = null;

            if (phEdit.Visible)
            {
                editContent = new HtmlTextInfo()
                {
                    Locale = ddlLocale.SelectedValue,
                    ModuleTitle = txtModuleTitle.Text.Trim(),
                    Content = txtContent.Text.Trim(),
                    Summary = txtSummary.Text.Trim()
                };
                hfEditor.Value = new JavaScriptSerializer().Serialize(editContent);
            }
            else {
                editContent = new JavaScriptSerializer().Deserialize<HtmlTextInfo>(hfEditor.Value);
            }

            DisplayPreview(editContent, false);
        }

        protected void btnPreviewBack_Click(object sender, EventArgs e)
        {
            if (phPreviewVersion.Visible)
            {
                //back to history view
                btnHistory_Click(null, null);
            }
            else {
                //back to edit view
                btnEdit_Click(null, null);
            }
        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            if (phEdit.Visible)
            {
                var editContent = new HtmlTextInfo()
                {
                    Locale = ddlLocale.SelectedValue,
                    ModuleTitle = txtModuleTitle.Text.Trim(),
                    Content = txtContent.Text.Trim(),
                    Summary = txtSummary.Text.Trim()
                };
                hfEditor.Value = new JavaScriptSerializer().Serialize(editContent);
            }

            DisplayVersions();
        }

        protected void lvVersions_ItemDataBound(object sender, System.Web.UI.WebControls.ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                HtmlTextInfo htmlText = (HtmlTextInfo)e.Item.DataItem;
                Literal litVersion = (Literal)e.Item.FindControl("litVersion");
                Literal litDate = (Literal)e.Item.FindControl("litDate");
                Literal litTitle = (Literal)e.Item.FindControl("litTitle");
                Literal litContent = (Literal)e.Item.FindControl("litContent");
                Literal litDisplayName = (Literal)e.Item.FindControl("litDisplayName");
                Literal litPublished = (Literal)e.Item.FindControl("litPublished");
                ImageButton btnRemove = (ImageButton)e.Item.FindControl("btnRemove");
                ImageButton btnPreview = (ImageButton)e.Item.FindControl("btnPreview");
                ImageButton btnRollback = (ImageButton)e.Item.FindControl("btnRollback");

                litVersion.Text = htmlText.Version.ToString();

                DateTime lastModifiedDate = htmlText.LastModifiedOnDate > htmlText.CreatedOnDate ? htmlText.LastModifiedOnDate : htmlText.CreatedOnDate;
                litDate.Text = lastModifiedDate.ToString("dd/MM/yyyy hh:mm:ss");

                litTitle.Text = htmlText.ModuleTitle;
                litContent.Text = HtmlUtils.Shorten(HtmlUtils.Clean(htmlText.Content, false), 80, "...");

                int lastModifiedBy = htmlText.LastModifiedOnDate > htmlText.CreatedOnDate ? htmlText.LastModifiedByUserId : htmlText.CreatedByUserID;
                var objUser = DotNetNuke.Entities.Users.UserController.GetUserById(PortalId, lastModifiedBy);
                litDisplayName.Text = objUser != null ? objUser.DisplayName : "Default";

                litPublished.Text = htmlText.IsPublished.ToString();

                //set command args   
                btnRemove.CommandArgument = htmlText.ItemId.ToString();
                btnPreview.CommandArgument = htmlText.ItemId.ToString();
                btnRollback.CommandArgument = htmlText.ItemId.ToString();

                //hide remove button for Non-Admin
                string deleteMsg = Localization.GetString("DeleteVersion.Confirm", LocalResourceFile)
                    .Replace("[VERSION]", htmlText.Version.ToString())
                    .Replace("[LOCALE]", htmlText.Locale);
                btnRemove.OnClientClick = "return confirm(\"" + deleteMsg + "\");";
                btnRemove.Visible = (UserInfo.IsSuperUser || PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName));

                //hide rollback button for current active item
                if (_topHtmlTextItemId == -1)
                {
                    var item = _htmlTextController.GetTopHtmlText(ModuleId, ddlLocale.SelectedValue, true);
                    if (item != null)
                        _topHtmlTextItemId = item.ItemId;
                }
                string rollbackMsg = Localization.GetString("RollbackVersion.Confirm", LocalResourceFile)
                        .Replace("[VERSION]", htmlText.Version.ToString())
                        .Replace("[LOCALE]", htmlText.Locale);
                btnRollback.OnClientClick = "return confirm(\"" + rollbackMsg + "\");";
                btnRollback.Visible = (htmlText.ItemId != _topHtmlTextItemId);                
            }
        }

        protected void lvVersions_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            try
            {
                HtmlTextInfo htmlText = _htmlTextController.GetHtmlText(int.Parse(e.CommandArgument.ToString()));

                switch (e.CommandName.ToLower())
                {
                    case "remove":
                        if (UserInfo.IsSuperUser || PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))
                        {
                            _htmlTextController.DeleteHtmlText(htmlText);
                            DisplayVersions();
                        }
                        else
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(Page, "Error occurred: ", LocalizeString("AccessDenied.Text"), ModuleMessage.ModuleMessageType.RedError);
                        break;
                    case "rollback":
                        //clone a new version as rollback, just like git, we are not rewriting the history.
                        HtmlTextInfo newVersion = new HtmlTextInfo()
                        {
                            ItemId = -1,
                            ModuleId = htmlText.ModuleId,
                            Locale = htmlText.Locale,
                            ModuleTitle = htmlText.ModuleTitle,
                            Content = htmlText.Content,
                            Summary = htmlText.Summary,
                            IsPublished = true
                        };
                        _htmlTextController.UpdateHtmlText(newVersion, true, _htmlTextController.GetMaximumVersionHistory(PortalId));
                        DisplayVersions();
                        setEditContent(newVersion);
                        break;
                    case "preview":
                        DisplayPreview(htmlText, true);
                        break;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void lvVersions_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            dpVersions.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            DisplayVersions();
        }

        #endregion

        #region Private Methods

        private void setEditContent(HtmlTextInfo htmlText)
        {
            htmlText = htmlText == null ? new HtmlTextInfo() : htmlText;
            txtModuleTitle.Text = htmlText.ModuleTitle;
            txtContent.Text = FormatContent(htmlText.Content);
            txtSummary.Text = htmlText.Summary;
            hfEditor.Value = new JavaScriptSerializer().Serialize(htmlText);
        }

        protected void DisplayContent(HtmlTextInfo htmlText)
        {
            phLocale.Visible = true;
            setEditContent(htmlText);
            phEdit.Visible = true;
            phPreview.Visible = false;
            phHistory.Visible = false;
            btnSave.Enabled = true;
            btnEdit.Enabled = false;
            btnPreview.Enabled = true;
            btnHistory.Enabled = true;
        }

        private void DisplayPreview(HtmlTextInfo htmlText, bool historyPreview)
        {
            phLocale.Visible = false;
            if (historyPreview && htmlText.Version > 0)
            {
                phPreviewVersion.Visible = true;
                lblPreviewVersion.Text = htmlText.Version.ToString();
            }
            else {
                phPreviewVersion.Visible = false;
            }

            litPreviewTitle.Text = htmlText.ModuleTitle;
            litPreviewBody.Text = HtmlTextController.FormatHtmlText(ModuleId, htmlText.Content, Settings, PortalSettings, Page);
            phEdit.Visible = false;
            phPreview.Visible = true;
            phHistory.Visible = false;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnPreview.Enabled = false;
            btnHistory.Enabled = true;
        }

        protected void DisplayVersions()
        {
            phLocale.Visible = true;
            lblMaxVersions.Text = _htmlTextController.GetMaximumVersionHistory(PortalId).ToString();

            var versions = _htmlTextController.GetHtmlTextList(ModuleId, ddlLocale.SelectedValue);
            lvVersions.DataSource = versions.OrderByDescending(x => x.Version).ToList();
            lvVersions.DataBind();

            phEdit.Visible = false;
            phPreview.Visible = false;
            phHistory.Visible = true;
            btnSave.Enabled = false;
            btnEdit.Enabled = true;
            btnPreview.Enabled = false;
            btnHistory.Enabled = false;
        }
        #endregion

        #region Private Functions

        /// <summary>
        ///   Formats the content to make it html safe.
        /// </summary>
        /// <param name = "htmlText">Content of the HTML.</param>
        /// <returns></returns>
        private string FormatContent(string htmlText)
        {
            var strContent = HttpUtility.HtmlDecode(htmlText);
            strContent = HtmlTextController.ManageRelativePaths(strContent, PortalSettings.HomeDirectory, "src", PortalId);
            strContent = HtmlTextController.ManageRelativePaths(strContent, PortalSettings.HomeDirectory, "background", PortalId);
            return HttpUtility.HtmlEncode(strContent);
        }

        #endregion
    }
}