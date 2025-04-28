using System;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos;
using WebApplication1.Entities;
using WebApplication1.Mapping;

namespace WebApplication1.Endpoints;

public static class GamesEndpoints
{
  const string GetGameEndpointName = "GetGame";

  private static readonly List<GameSummaryDto> games = [
      new (1, "Pixel Racers", "Racing", 29.99m, new DateOnly(2023, 7, 15)),
      new (2, "Mystic Realms", "RPG", 49.99m, new DateOnly(2022, 11, 5)),
      new (3, "Block Builder Pro", "Simulation", 19.99m, new DateOnly(2024, 3, 10))
];

  public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
  {

    var group = app.MapGroup("/games")   
                   .WithParameterValidation();

    // GET /games
    group.MapGet("/", (GameStoreContext dbContext) => 
      dbContext.Games
               .Include(game => game.Genre)
               .Select(game => game.ToGameSummaryDto())
               .AsNoTracking()
      );

    // GET /games/id
    group.MapGet("/{id}", (int id, GameStoreContext dbContext) => {

      Game ? game = dbContext.Games.Find(id);

      return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
        })
      .WithName(GetGameEndpointName);

    // POST /games
    group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) => 
    {
      Game game = newGame.ToEntity();
      
      dbContext.Games.Add(game);
      dbContext.SaveChanges();

      return Results.CreatedAtRoute(
        GetGameEndpointName, new { id = game.Id }, 
        game.ToGameDetailsDto());
    })
    .WithParameterValidation();

    // PUT /games/id
    group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) => 
    {
      var existingGame = dbContext.Games.Find(id);

      if(existingGame is null) {
        return Results.NotFound();
      }
      
      dbContext.Entry(existingGame)
               .CurrentValues
               .SetValues(updatedGame.ToEntity(id));

      dbContext.SaveChanges();

      return Results.NoContent();
    });

    // DELETE /games/id
    group.MapDelete("/{id}", (int id) => {
      games.RemoveAll(game => game.Id == id);

      return Results.NoContent();
});
  return group;
  }
}
