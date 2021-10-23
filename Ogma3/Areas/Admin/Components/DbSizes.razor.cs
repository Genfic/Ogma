using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Ogma3.Areas.Admin.Models;
using Ogma3.Data;

namespace Ogma3.Areas.Admin.Components;

public partial class DbSizes : ComponentBase
{
    [Inject]
    private IDbContextFactory<ApplicationDbContext> ContextFactory { get; set; }

    private List<TableInfo> DbSize { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await using var ctx = await ContextFactory.CreateDbContextAsync();
        DbSize = await ctx.TableInfos.FromSqlRaw(@"
            SELECT 
                table_name as name, 
                pg_relation_size(quote_ident(table_name)) as size
            FROM information_schema.tables 
            WHERE table_schema = 'public' AND table_name NOT LIKE '\_\_%'
            ORDER BY size DESC
        ").ToListAsync();
    }

    private SortBy _sortBy = SortBy.Size;
    private SortDir _sortDir = SortDir.Desc;

    protected void SortName()
    {
        _sortBy = SortBy.Name;
        if (_sortDir == SortDir.Asc)
        {
            _sortDir = SortDir.Desc;
            DbSize = DbSize.OrderByDescending(x => x.Name).ToList();
        }
        else
        {
            _sortDir = SortDir.Asc;
            DbSize = DbSize.OrderBy(x => x.Name).ToList();
        }
    }

    protected void SortSize()
    {
        _sortBy = SortBy.Size;
        if (_sortDir == SortDir.Asc)
        {
            _sortDir = SortDir.Desc;
            DbSize = DbSize.OrderByDescending(x => x.Size).ToList();
        }
        else
        {
            _sortDir = SortDir.Asc;
            DbSize = DbSize.OrderBy(x => x.Size).ToList();
        }
    }
    
    private enum SortBy { Name, Size }
    private enum SortDir { Asc, Desc }
}