using Core.Utils;
using Core.Utils.Pagination;
using Microsoft.AspNetCore.Http;
using Model.Dtos.WordDtos;
using Model.Entities;
using Model.ViewModels;

namespace Business.Abstract;

public interface IWordService
{
    // 1) Related Entity Methods
    Task<WordResponseDto> GetWordAsync(Guid wordId);
    Task<Paginate<Word>> GetAllWordsAsync(FSPModel fSP);
    Task<WordResponseDto> InsertWordAsync(WordCreateDto wordCreateDto);
    Task<WordResponseDto> UpdateWordAsync(WordUpdateDto wordUpdateDto);
    Task<WordResponseDto> ChangeCategoryOfWordAsync(CategoryWordRequestDto requestDto);
    Task DeleteWordAsync(Guid wordId);
    Task<WordResponseDto> UpdateImageAsync(IFormFile file, Guid wordId);
    Task<WordResponseDto> DeleteImageAsync(Guid wordId, string url);


    // 2) Interaction Methods
    //Task<WordByUserModel> GetWordForUserAsync(Guid wordId, Guid userId);
    Task<ICollection<WordResponseDto>> GetWordsByCategoryAsync(Guid categoryId);
    Task<ICollection<WordByUserModel>> GetWordsByCategoryForUserAsync(Guid categoryId, Guid userId);
    Task<ICollection<WordResponseDto>> GetFavoriteWordsForUserAsync(Guid userId);
    Task<ICollection<WordByUserModel>> GetLearnedWordsForUserAsync(Guid userId);

    // 3) Favorite Methods
    Task AddWordAsFavoriteAsync(FavoriteWordRequestDto favoriteWordRequestDto);
    Task RemoveWordFromFavoritesAsync(FavoriteWordRequestDto favoriteWordRequestDto);

    // 4) Learning Methods
    Task AddWordAsLearnedAsync(LearningWordRequestDto learningWordRequestDto);
    Task RemoveWordFromLearnedAsync(LearningWordRequestDto learningWordRequestDto);
}