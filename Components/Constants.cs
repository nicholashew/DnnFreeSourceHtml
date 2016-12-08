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

namespace FreeSource.Modules.Html.Components
{
    public class Constants
    {
        public const string ModuleSettingsPrefix = "FreeSource_HtmlText_";
        public const string MaximumVersionHistoryPortalSettingName = "FreeSource_Html_MaximumVersionHistory";
        public const int DefaultMaximumVersionHistory = 5;

        public class ModulePermissionKey
        {
            public const string Edit = "EDIT_CONTENT";
            public const string ViewHistory = "VIEW_HISTORY";
            public const string Delete = "DELETE_CONTENT";
            public const string Rollback = "ROLLBACK_CONTENT";
        }        
    }
}