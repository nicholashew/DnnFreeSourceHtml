<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Edit.ascx.cs" Inherits="FreeSource.Modules.Html.Edit" %>
<%@ Register TagPrefix="dnn" TagName="label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register TagPrefix="dnnweb" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>
<%@ Register TagPrefix="dnncl" Namespace="DotNetNuke.Web.Client.ClientResourceManagement" Assembly="DotNetNuke.Web.Client" %>

<div class="dnnForm dnnClear dnn-free-html-edit" id="dnnEditBasicSettings">

    <fieldset>

        <asp:PlaceHolder ID="phLocale" runat="server">
            <div class="dnnFormItem">
                <dnn:label ID="lblLocale" runat="server" />
                <asp:DropDownList ID="ddlLocale" runat="server" AutoPostBack="true" />
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phEdit" runat="server">
            <div class="dnnFormItem">
                <dnn:label ID="lblModuleTitle" runat="server" />
                <asp:TextBox ID="txtModuleTitle" runat="server" />
            </div>
            <div class="dnnFormItem">
                <dnn:texteditor id="txtContent" runat="server" height="400" width="100%" ChooseMode="false"></dnn:texteditor>
            </div>
            <div class="dnnFormItem">
                <dnn:label ID="lblSummary" runat="server" />
                <asp:TextBox ID="txtSummary" runat="server" TextMode="MultiLine" Rows="2" />
            </div>
            <asp:HiddenField ID="hfEditor" runat="server" />
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phPreview" runat="server" Visible="false">
            
            <asp:LinkButton ID="btnPreviewBack" runat="server" class="dnnSecondaryAction" resourcekey="btnPreviewBack" />

            <asp:PlaceHolder ID="phPreviewVersion" runat="server">                
                <div class="dnnFormItem">
                    <dnn:label id="plPreviewVersion" runat="server" controlname="lblPreviewVersion" suffix=":" />
                    <asp:Label ID="lblPreviewVersion" runat="server" />
                </div>
            </asp:PlaceHolder>
            <div class="html_preview" id="divPreviewArea" runat="server">
                <div class="SpacingBottom">
                    <h4>
                        <span style="display: block; margin-bottom: 5px;">
                            <asp:Literal ID="litPreviewTitle" runat="server" />
                        </span>
                    </h4>
                    <div>
                        <asp:Literal ID="litPreviewBody" runat="server" />
                    </div>
                    <div class="clear"></div>
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phHistory" runat="server" Visible="false">
            <div class="dnnFormItem">
                <dnn:label id="plMaxVersions" runat="server" controlname="lblMaxVersions" suffix=":" />
                <asp:Label ID="lblMaxVersions" runat="server" />
            </div>

            <table class="table table-striped">
                <thead>
                    <th>
                        <asp:Label ID="lblHeaderVersion" runat="server" resourcekey="lblHeaderVersion" />
                    </th>
                    <th>
                        <asp:Label ID="lblHeaderDate" runat="server" resourcekey="lblHeaderDate" />
                    </th>
                    <th>
                        <asp:Label ID="lblHeaderTitle" runat="server" resourcekey="lblHeaderTitle" />
                    </th>
                    <th>
                        <asp:Label ID="lblHeaderContent" runat="server" resourcekey="lblHeaderContent" />
                    </th>
                    <th>
                        <asp:Label ID="lblHeaderUser" runat="server" resourcekey="lblHeaderUser" />
                    </th>
                    <th>
                        <asp:Label ID="lblHeaderPublished" runat="server" resourcekey="lblHeaderPublished" />
                    </th>
                    <th>
                        <asp:Label ID="lblHeaderAction" runat="server" resourcekey="lblHeaderAction" />
                    </th>
                </thead>
                <tbody>
                    <asp:ListView ID="lvVersions" runat="server">
                        <LayoutTemplate>
                            <tr id="itemPlaceHolder" runat="server"></tr>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <th scope="row">
                                    <asp:Literal ID="litVersion" runat="server" />
                                </th>
                                <td>
                                    <asp:Literal ID="litDate" runat="server" />
                                </td>
                                <td>
                                    <asp:Literal ID="litTitle" runat="server" />
                                </td>
                                <td>
                                    <asp:Literal ID="litContent" runat="server" />
                                </td>
                                <td>
                                    <asp:Literal ID="litDisplayName" runat="server" />
                                </td>
                                <td>
                                    <asp:Literal ID="litPublished" runat="server" />
                                </td>
                                <td>
                                    <dnnweb:DnnImageButton ID="btnPreview" runat="server" CommandName="Preview" class="img-btn" IconKey="View" Text="Preview" resourcekey="VersionsPreview" />
                                    <dnnweb:DnnImageButton ID="btnRemove" runat="server" CommandName="Remove" class="img-btn delete-action" IconKey="ActionDelete" Text="Delete" resourcekey="VersionsRemove" />
                                    <dnnweb:DnnImageButton ID="btnRollback" runat="server" CommandName="RollBack" class="img-btn" IconKey="Restore" Text="Rollback" resourcekey="VersionsRollback" />
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <tr>
                                <td colspan="7"><%=Localization.GetString("NoRecord", LocalResourceFile)%></td>
                            </tr>
                        </EmptyDataTemplate>
                    </asp:ListView>
                </tbody>
            </table>

            <asp:DataPager ID="dpVersions" runat="server" PagedControlID="lvVersions" PageSize="10">
                <Fields>
                    <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="true" ShowPreviousPageButton="true" ShowNextPageButton="false" ButtonCssClass="page-item" />
                    <asp:NumericPagerField ButtonType="Link" NumericButtonCssClass="page-item" CurrentPageLabelCssClass="page-item active" />
                    <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="true" ShowPreviousPageButton="false" ButtonCssClass="page-item" />
                </Fields>
            </asp:DataPager>
        </asp:PlaceHolder>

    </fieldset>

    <div class="actions">
        <ul class="dnnActions dnnClear">
            <li>
                <asp:LinkButton ID="btnSave" runat="server" resourcekey="btnSave" CssClass="dnnPrimaryAction" />
            </li>
            <li>
                <asp:HyperLink ID="hlCancel" runat="server" class="dnnSecondaryAction" resourcekey="cmdCancel" />
            </li>
            <li class="separator"></li>
            <li>
                <asp:LinkButton ID="btnEdit" runat="server" class="dnnSecondaryAction" resourcekey="btnEdit" Visible="true" Enabled="false" />
            </li>
            <li>
                <asp:LinkButton ID="btnPreview" runat="server" class="dnnSecondaryAction" resourcekey="btnPreview" />
            </li>
            <li>
                <asp:LinkButton ID="btnHistory" runat="server" class="dnnSecondaryAction" resourcekey="btnHistory" />
            </li>
        </ul>
    </div>
</div>

<script type="text/javascript">
    /*globals jQuery, window, Sys */
    (function ($, Sys) {
        function dnnEditBasicSettings() {
            $('#dnnEditBasicSettings').dnnPanels();
            $('#dnnEditBasicSettings .dnnFormExpandContent a').dnnExpandAll({ expandText: '<%=Localization.GetString("ExpandAll", LocalResourceFile)%>', collapseText: '<%=Localization.GetString("CollapseAll", LocalResourceFile)%>', targetArea: '#dnnEditBasicSettings' });
        }

        $(document).ready(function () {
            dnnEditBasicSettings();
            dnnBindConfirmationEvent();
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                dnnEditBasicSettings();
            });
        });

    }(jQuery, window.Sys));
</script>
