using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dtos;

public record class GameDto(
  int Id, 
  [Required][StringLength(50)] string Name, 
  [Required][StringLength(20)] string Genre,
  [Range(1,100)] decimal Price,
  DateOnly ReleaseDate
  );
