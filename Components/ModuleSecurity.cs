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

using System.ComponentModel;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using DotNetNuke.UI.Modules;
using DotNetNuke.Common;
using DotNetNuke.Security;

namespace FreeSource.Modules.Html.Components
{
    public class ModuleSecurity
    {
        private readonly bool _canEdit;
        private readonly bool _canDelete;
        private readonly bool _canRollback;
        private readonly bool _canViewHistory;

        public ModuleSecurity(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                return;

            var mp = moduleInfo.ModulePermissions;
            _canEdit = ModulePermissionController.HasModulePermission(mp, Constants.ModulePermissionKey.Edit);
            _canViewHistory = ModulePermissionController.HasModulePermission(mp, Constants.ModulePermissionKey.ViewHistory);
            _canDelete = ModulePermissionController.HasModulePermission(mp, Constants.ModulePermissionKey.Delete);
            _canRollback = ModulePermissionController.HasModulePermission(mp, Constants.ModulePermissionKey.Rollback);
        }

        public ModuleSecurity(int moduleId, int tabId) : this(ModuleController.Instance.GetModule(moduleId, tabId, true))
        {
        }

        public ModuleSecurity(ModuleInstanceContext context) : this(context.ModuleId, context.TabId)
        {
        }

        public bool CanEditContent
        {
            get { return _canEdit; }
        }

        public bool CanViewHistory
        {
            get { return _canViewHistory; }
        }

        public bool CanDeleteContent
        {
            get { return _canDelete; }
        }

        public bool CanRollbackContent
        {
            get { return _canRollback; }
        }

        public bool IsAllowedToEditContent()
        {
            return _canEdit || IsAdministrator();
        }

        public bool IsAllowedToViewHistory()
        {
            return _canViewHistory || IsAdministrator();
        }

        public bool IsAllowedToDeleteContent()
        {
            return _canDelete || IsAdministrator();
        }

        public bool IsAllowedToRollbackContent()
        {
            return _canRollback || IsAdministrator();
        }

        public static bool IsAdministrator()
        {
            var administratorRoleName = Globals.GetPortalSettings().AdministratorRoleName;
            return PortalSecurity.IsInRole(administratorRoleName);
        }

    }
}