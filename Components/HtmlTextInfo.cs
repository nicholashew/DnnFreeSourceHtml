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
using System.Web.Caching;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Content;

namespace FreeSource.Modules.Html.Components
{
    [Serializable]
    [TableName("FreeSource_HtmlText")]    
    [PrimaryKey("ItemId", AutoIncrement = true)]    
    [Cacheable("FreeSource_HtmlText_Items", CacheItemPriority.Default, 20)]    
    [Scope("ModuleId")]
    public class HtmlTextInfo
    {

        public int ItemId { get; set; }

        public int ModuleId { get; set; }

        public string Locale { get; set; }

        public string ModuleTitle { get; set; } 

        public string Content { get; set; }

        public string Summary { get; set; }

        public int Version { get; set; }
        
        public bool IsPublished { get; set; }

        public int CreatedByUserID { get; set; }

        public DateTime CreatedOnDate { get; set; }

        public int LastModifiedByUserId { get; set; }

        public DateTime LastModifiedOnDate { get; set; }
    }
}
