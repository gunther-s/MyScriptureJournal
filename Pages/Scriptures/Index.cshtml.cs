using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyScriptureJournal.Data;
using MyScriptureJournal.Models;

namespace MyScriptureJournal.Pages.Scriptures
{
    public class IndexModel : PageModel
    {
        private readonly MyScriptureJournal.Data.MyScriptureJournalContext _context;

        public IndexModel(MyScriptureJournal.Data.MyScriptureJournalContext context)
        {
            _context = context;
        }

        public IList<Scripture> Scripture { get;set; }
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }
        // Requires using Microsoft.AspNetCore.Mvc.Rendering;
        public SelectList Notes { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ScriptureNotes { get; set; }

        public string BookSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public async Task OnGetAsync(string sortOrder)
        {
            BookSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            IQueryable<string> bookQuery = from m in _context.Scripture
                                            orderby m.Book
                                            select m.Book;

            var scriptures = from m in _context.Scripture
                         select m;
            if (!string.IsNullOrEmpty(SearchString))
            {
                scriptures = scriptures.Where(s => s.Notes.Contains(SearchString));
            }
            Scripture = await _context.Scripture.ToListAsync();

            if (!string.IsNullOrEmpty(ScriptureNotes))
            {
                scriptures = scriptures.Where(x => x.Notes == ScriptureNotes);
            }
            Notes = new SelectList(await bookQuery.Distinct().ToListAsync());

            switch (sortOrder)
            {
                case "name_desc":
                    scriptures = scriptures.OrderByDescending(s => s.Book);
                    break;
                case "Date":
                    scriptures = scriptures.OrderBy(s => s.DateCreated);
                    break;
                case "date_desc":
                    scriptures = scriptures.OrderByDescending(s => s.DateCreated);
                    break;
                default:
                    scriptures = scriptures.OrderBy(s => s.Book);
                    break;
            }

            
            Scripture = await scriptures.ToListAsync();

            
        }
    }
}
