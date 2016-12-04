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
using System.Reflection;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;

namespace FreeSource.Modules.Html.Components
{
    /// <summary>
    /// HtmlModuleSettings provides a strongly typed list of properties used by 
    /// the HTML module settings.
    /// </summary>
    [Serializable]
    public class HtmlModuleSettings
    {
        public bool ReplaceTokens { get; set; } = false;

        public int SearchDescLength { get; set; } = 100;

        public bool EnableFallback { get; set; } = true;
    }

    /// <summary>
    /// The <see cref="HtmlModuleSettingsRepository"/> used for storing and retrieving <see cref="HtmlModuleSettings"/>
    /// </summary>
    public class HtmlModuleSettingsRepository  //: SettingsRepository<HtmlModuleSettings>
    {

        private readonly IModuleController _moduleController;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlModuleSettingsRepository"/> class.</summary>
        /// </summary>
        public HtmlModuleSettingsRepository()
        {
            _moduleController = ModuleController.Instance;
        }

        public HtmlModuleSettings GetSettings(ModuleInfo moduleContext)
        {
            var settings = new HtmlModuleSettings();                        
            if (moduleContext != null)
            {
                var moduleSettings = moduleContext.ModuleSettings;
                Type type = settings.GetType();
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    string key = Constants.ModuleSettingsPrefix + property.Name;
                    if (moduleSettings.ContainsKey(key))
                    {
                        property.SetValue(settings, Convert.ChangeType(moduleSettings[key], property.PropertyType), null);
                    }
                }
            }
            return settings;
        }

        public void SaveSettings(ModuleInfo moduleContext, HtmlModuleSettings settings)
        {
            Requires.NotNull("moduleContext", moduleContext);
            Requires.NotNull("settings", settings);

            Type type = settings.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                string key = Constants.ModuleSettingsPrefix + property.Name;
                string value = property.GetValue(settings, null).ToString();
                _moduleController.UpdateModuleSetting(moduleContext.ModuleID, key, value);
            }
        }
    }
}