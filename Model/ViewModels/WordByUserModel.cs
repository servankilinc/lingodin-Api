using Model.Dtos.WordDtos;

namespace Model.ViewModels;

public class WordByUserModel
{
    public WordResponseDto? Word { get; set; }
    public bool IsWordAddedFav { get; set; }
}