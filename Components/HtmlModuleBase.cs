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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using FreeSource.Modules.Html.Components;

namespace FreeSource.Modules.Html
{
    public class HtmlModuleBase : PortalModuleBase
    {
        public int ItemId
        {
            get
            {
                var qs = Request.QueryString["tid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }

        /// <summary>
        /// Get the current locale code for current request to the portal.
        /// </summary>
        public string CurrentLocaleCode
        {
            get
            {
                return LocaleController.Instance.GetCurrentLocale(PortalId).Code;
            }
        }

        private HtmlModuleSettings _settings;
        public new HtmlModuleSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    var repo = new HtmlModuleSettingsRepository();
                    _settings = repo.GetSettings(this.ModuleConfiguration);
                }
                return _settings;
            }
            set { _settings = value; }
        }
    }
}