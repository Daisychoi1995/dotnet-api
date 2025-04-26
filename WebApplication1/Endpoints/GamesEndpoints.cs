using System;
using WebApplication1.Dtos;

namespace WebApplication1.Endpoints;

public static class GamesEndpoints
{
  const string GetGameEndpointName = "GetGame";

  private static readonly List<GameDto> games = [
      new (1, "Pixel Racers", "Racing", 29.99m, new DateOnly(2023, 7, 15)),
      new (2, "Mystic Realms", "RPG", 49.99m, new DateOnly(2022, 11, 5)),
      new (3, "Block Builder Pro", "Simulation", 19.99m, new DateOnly(2024, 3, 10))
];

  public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
  {

    var group = app.MapGroup("/games")   
                   .WithParameterValidation();

    // GET /games
    group.MapGet("/", () => games);

    // GET /games/id
    group.MapGet("/{id}", (int id) => {

      GameDto? game = games.Find(game => game.Id == id);

      return game is null ? Results.NotFound() : Results.Ok(game);
        })
      .WithName(GetGameEndpointName);

    // POST /games
    group.MapPost("/", (CreateGameDto newGame) => {
      GameDto game = new (
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price, 
        newGame.ReleaseDate
      );
      games.Add(game);
      return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
    })
    .WithParameterValidation();

    // PUT /games/id
    group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) => {
      var index = games.FindIndex(game => game.Id == id);

      if(index == -1) {
        return Results.NotFound();
      }
      games[index] = new GameDto(
        id, 
        updatedGame.Name,
        updatedGame.Genre,
        updatedGame.Price,
        updatedGame.ReleaseDate
      );

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
