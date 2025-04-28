using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Mapping;

namespace WebApplication1.Endpoints;

public static class GenresEndpoints
{
  public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("genres");

    group.MapGet("/", async (GameStoreContext dbcontext) =>
      await dbcontext.Genres
                     .Select(genre => genre.ToDto())
                     .AsNoTracking()
                     .ToListAsync()
    );

    return group;
  }
}
