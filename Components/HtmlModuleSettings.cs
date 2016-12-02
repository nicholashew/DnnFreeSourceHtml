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
using DotNetNuke.Entities.Modules.Settings;

namespace FreeSource.Modules.Html.Components
{
    /// <summary>
    /// An example implementation of the <see cref="ModuleSettingAttribute"/>
    /// </summary>
    /// <remarks>
    /// HtmlModuleSettings provides a strongly typed list of properties used by 
    /// the HTML module.  Settings will automatically be serialized and deserialized
    /// for storage in the underlying settings table.
    /// </remarks>
    [Serializable]
    public class HtmlModuleSettings
    {
        [ModuleSetting(Prefix = "FreeSource_HtmlText_")]
        public bool ReplaceTokens { get; set; } = false;
        
        [ModuleSetting(Prefix = "FreeSource_HtmlText_")]
        public int SearchDescLength { get; set; } = 100;

        [ModuleSetting(Prefix = "FreeSource_HtmlText_")]
        public bool EnableFallback { get; set; } = true;
    }

    /// <summary>
    /// The <see cref="SettingsRepository{T}"/> used for storing and retrieving <see cref="HtmlModuleSettings"/>
    /// </summary>
    public class HtmlModuleSettingsRepository : SettingsRepository<HtmlModuleSettings>
    {
    }
}