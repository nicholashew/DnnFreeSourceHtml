<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="FreeSource.Modules.Html.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>

<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("BasicSettings")%></a></h2>
<fieldset>
    <div class="dnnFormItem">
        <dnn:label id="plReplaceTokens" controlname="chkReplaceTokens" runat="server" />
        <asp:CheckBox ID="chkReplaceTokens" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:label id="plSearchDescLength" runat="server" controlname="txtSearchDescLength" />
        <asp:TextBox ID="txtSearchDescLength" runat="server" />
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtSearchDescLength"
            Display="Dynamic" CssClass="dnnFormMessage dnnFormError" ValidationExpression="^\d+$" resourcekey="valSearchDescLength.ErrorMessage" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblEnableFallback" runat="server" />
        <asp:RadioButtonList ID="rblEnableFallback" runat="server" CssClass="dnnFormRadioButtons">
            <asp:ListItem Value="True" resourcekey="rblEnableFallback_True"></asp:ListItem>
            <asp:ListItem Value="False" resourcekey="rblEnableFallback_False"></asp:ListItem>
        </asp:RadioButtonList>
    </div>
</fieldset>

<asp:PlaceHolder ID="phAdvanceSettings" runat="server" Visible="false">
    <h2 id="dnnSitePanel-AdvanceSettings" class="dnnFormSectionHead"><a href=""><%=LocalizeString("AdvancedSettings")%></a></h2>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblMaxVersion" runat="server" />
            <asp:DropDownList ID="ddlMaxVersion" runat="server" />
        </div>
        <hr />
        <div class="dnnFormItem">
            <dnn:Label ID="lblCleanup" runat="server" />
            <asp:ListBox ID="cleanableListBox" runat="server" Width="450px" Rows="14" SelectionMode="Multiple" />
            <div class="actions" style="float: right;">
                <ul class="dnnActions dnnClear">
                    <li>
                        <asp:LinkButton ID="btnCleanSelection" runat="server" resourcekey="btnCleanSelection" CssClass="dnnSecondaryAction" /></li>
                    <li>
                        <asp:LinkButton ID="btnCleanAll" runat="server" resourcekey="btnCleanAll" CssClass="dnnPrimaryAction" /></li>
                </ul>
            </div>
        </div>
    </fieldset>
</asp:PlaceHolder>

<script type="text/javascript">
    (function ($, Sys) {
        $(document).ready(function () {

            var setupCleanup = function () {

                var yesText = '<%= Localization.GetSafeJSString("Yes.Text", Localization.SharedResourceFile) %>';
                var noText = '<%= Localization.GetSafeJSString("No.Text", Localization.SharedResourceFile) %>';
                var titleText = '<%= Localization.GetSafeJSString("Confirm.Text", Localization.SharedResourceFile) %>';

                $('#<%= btnCleanAll.ClientID %>').dnnConfirm({
                    text: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("DeleteAll")) %>',
                    yesText: yesText,
                    noText: noText,
                    title: titleText
                });

                $('#<%= btnCleanSelection.ClientID %>').dnnConfirm({
                    text: '<%= DotNetNuke.UI.Utilities.ClientAPI.GetSafeJSString(LocalizeString("DeleteSelection")) %>',
                    yesText: yesText,
                    noText: noText,
                    title: titleText
                });
            };

            setupCleanup();

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                setupCleanup();
            });

        });
    }(jQuery, window.Sys));
</script>
