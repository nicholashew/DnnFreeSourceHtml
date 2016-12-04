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
using DotNetNuke.Common;
using DotNetNuke.Data;
using DotNetNuke.Framework;

namespace FreeSource.Modules.Html.Components
{
    public class HtmlTextRepository : ServiceLocator<IHtmlTextRepository, HtmlTextRepository>, IHtmlTextRepository
    {
        protected override Func<IHtmlTextRepository> GetFactory()
        {
            return () => new HtmlTextRepository();
        }

        public int Add(HtmlTextInfo htmlText, int MaximumVersionHistory)
        {
            Requires.NotNull("htmlText", htmlText);
            Requires.PropertyNotNegative("htmlText", "ModuleId", htmlText.ModuleId);
            Requires.PropertyNotNullOrEmpty("htmlText", "Locale", htmlText.Locale);

            //set version
            var topVersion = GetAll(htmlText.ModuleId, htmlText.Locale).OrderByDescending(x => x.Version).FirstOrDefault();
            htmlText.Version = topVersion != null ? topVersion.Version + 1 : 1;

            //proceed
            using (IDataContext db = DataContext.Instance())
            {
                try
                {
                    db.BeginTransaction();

                    var rep = db.GetRepository<HtmlTextInfo>();

                    //insert
                    rep.Insert(htmlText);

                    //purge version history
                    rep.Delete("WHERE ModuleID = @0 AND Locale = @1 AND Version <= (@2 - @3)",
                        htmlText.ModuleId, htmlText.Locale, htmlText.Version, MaximumVersionHistory);

                    db.Commit();
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    throw;
                }
            }

            return htmlText.ItemId;
        }

        public void Delete(HtmlTextInfo htmlText)
        {
            Requires.NotNull("htmlText", htmlText);
            Requires.PropertyNotNegative("htmlText", "ItemId", htmlText.ItemId);

            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<HtmlTextInfo>();
                rep.Delete(htmlText);
            }
        }

        public HtmlTextInfo FindById(int itemId)
        {
            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<HtmlTextInfo>();
                return rep.GetById(itemId);
            }
        }

        public IQueryable<HtmlTextInfo> GetAll()
        {
            IQueryable<HtmlTextInfo> htmlTexts = null;

            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<HtmlTextInfo>();
                htmlTexts = rep.Get().AsQueryable();
            }

            return htmlTexts;
        }

        public IQueryable<HtmlTextInfo> GetAll(int moduleId)
        {
            Requires.NotNegative("moduleId", moduleId);

            return GetAll().Where(x => x.ModuleId == moduleId);
        }

        public IEnumerable<HtmlTextInfo> GetAll(int moduleId, string locale)
        {
            Requires.NotNegative("moduleId", moduleId);
            Requires.NotNullOrEmpty("locale", locale);

            return GetAll().Where(x => x.ModuleId == moduleId && x.Locale == locale);
        }

        public void Update(HtmlTextInfo htmlText)
        {
            Requires.NotNull("htmlText", htmlText);
            Requires.PropertyNotNegative("htmlText", "ItemId", htmlText.ItemId);
            Requires.PropertyNotNegative("htmlText", "ModuleId", htmlText.ModuleId);

            using (IDataContext db = DataContext.Instance())
            {
                var rep = db.GetRepository<HtmlTextInfo>();
                rep.Update(htmlText);
            }
        }
    }
}