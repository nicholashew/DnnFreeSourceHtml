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
using System.Collections.Generic;
using System.Linq;

namespace FreeSource.Modules.Html.Components
{
    public interface IHtmlTextRepository
    {
        int Add(HtmlTextInfo htmlText, int MaximumVersionHistory);

        void Delete(HtmlTextInfo htmlText);

        HtmlTextInfo FindById(int itemId);

        IQueryable<HtmlTextInfo> GetAll();

        IQueryable<HtmlTextInfo> GetAll(int moduleId);

        IEnumerable<HtmlTextInfo> GetAll(int moduleId, string locale);
        
        void Update(HtmlTextInfo htmlText);

    }
}