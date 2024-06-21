using AutoMapper;
using Business.Abstract;
using Business.CacheKeys;
using Core.CrossCuttingConcerns;
using Core.Exceptions;
using Core.Utils;
using Core.Utils.Caching;
using Core.Utils.Pagination;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model.Dtos.WordDtos;
using Model.Entities;
using Model.ViewModels;
using Newtonsoft.Json;

namespace Business.Concrete;

[BusinessExceptionHandler]
public class WordService : IWordService
{
    private readonly IWordDal _wordDal;
    private readonly IFavoriteDal _favoriteDal;
    private readonly ILearningDal _learningDal;
    private readonly ICacheService _cacheService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;
    private const string WordGroup = "Word-Cache-Group-Key";
    private const string CategoryUserGroup = "Category-User-Cache-Group-Key";
    private const string WordListByCategory = "WordListByCategoryCacheKey";
    private const string AllWordList = "AllWordListCacheKey";
    private const string WordInfoById = "WordInfoByIdCacheKey";

    public WordService(IWordDal wordDal, IFavoriteDal favoriteDal, ILearningDal learningDal, ICacheService cacheService, IFileService fileService, IMapper mapper)
    {
        _wordDal = wordDal;
        _favoriteDal = favoriteDal;
        _learningDal = learningDal;
        _cacheService = cacheService;
        _fileService = fileService;
        _mapper = mapper;
    }


    // 1) Related Entity Methods ...
    [Cache(WordInfoById, [WordGroup])]
    public async Task<WordResponseDto> GetWordAsync(Guid wordId)
    {
        if (wordId == Guid.Empty) throw new ArgumentNullException(nameof(wordId));
        Word word = await _wordDal.GetAsync(filter: w => w.Id == wordId);
        WordResponseDto wordResponse = _mapper.Map<WordResponseDto>(word);
        return wordResponse;
    }


    //[Cache(AllWordList, [WordGroup])]
    public async Task<Paginate<Word>> GetAllWordsAsync(FSPModel fsp) // ICollection<WordResponseDto>
    {
        var wordList = await _wordDal.GetPaginatedListByDynamicAsync(
            dynamicQuery: fsp.DynamicQuery!,
            index: fsp.PagingRequest!.Page,
            size: fsp.PagingRequest!.PageSize);
        return wordList;

        //ICollection<WordResponseDto> mappedList = wordList.Select(w => _mapper.Map<WordResponseDto>(w)).ToList();
        //return mappedList;
    }


    [CacheRemoveGroup([WordGroup, CategoryUserGroup])]
    [Validation(typeof(WordCreateDto))]
    public async Task<WordResponseDto> InsertWordAsync(WordCreateDto wordCreateDto)
    {
        Word word = _mapper.Map<Word>(wordCreateDto);
        Word insertedWord = await _wordDal.AddAsync(word);
        WordResponseDto mappedWord = _mapper.Map<WordResponseDto>(insertedWord);
        return mappedWord;
    }


    [CacheRemoveGroup([WordGroup, CategoryUserGroup])]
    public async Task DeleteWordAsync(Guid wordId)
    {
        if (wordId == Guid.Empty) throw new ArgumentNullException(nameof(wordId));
        var wordToDelete = new Word { Id = wordId };
        await _wordDal.DeleteAsync(wordToDelete);
    }


    [CacheRemoveGroup([WordGroup])]
    [Validation(typeof(WordUpdateDto))]
    public async Task<WordResponseDto> UpdateWordAsync(WordUpdateDto wordUpdateDto)
    {
        Word existingWord = await _wordDal.GetAsync(filter: w => w.Id == wordUpdateDto.Id);
        if (existingWord == null) throw new BusinessException("Data(existing) for update Not Found !");
        _mapper.Map(source: wordUpdateDto, destination: existingWord);
        Word updatedWord = await _wordDal.UpdateAsync(existingWord);
        WordResponseDto mappedWord = _mapper.Map<WordResponseDto>(updatedWord);
        return mappedWord;
    }


    [CacheRemoveGroup([WordGroup])]
    [Validation(typeof(CategoryWordRequestDto))]
    public async Task<WordResponseDto> ChangeCategoryOfWordAsync(CategoryWordRequestDto requestDto)
    {
        Word existingWord = await _wordDal.GetAsync(filter: w => w.Id == requestDto.WordId);
        if (existingWord == null) throw new BusinessException("Data(existing) for update Not Found !");
        if (existingWord.CategoryId == requestDto.CategoryId)
            return _mapper.Map<WordResponseDto>(existingWord);
        existingWord.CategoryId = requestDto.CategoryId;
        Word updatedWord = await _wordDal.UpdateAsync(existingWord);
        WordResponseDto mappedWord = _mapper.Map<WordResponseDto>(updatedWord);
        return mappedWord;
    }


    [CacheRemoveGroup([WordGroup])]
    public async Task<WordResponseDto> UpdateImageAsync(IFormFile file, Guid wordId)
    {
        if (wordId == Guid.Empty) throw new ArgumentNullException(nameof(wordId));
        if (file == null) throw new ArgumentNullException(nameof(file));

        Word existingWord = await _wordDal.GetAsync(filter: w => w.Id == wordId);
        if (existingWord == null) throw new BusinessException("Data(existing) for update Not Found !");

        string imageUrl = await _fileService.UploadAsync(
           file: file,
           containerName: "images",
           blobDir: "word",
           customFileName: $"img-{wordId}"
        );
        existingWord.HasImage = true;
        existingWord.Image = imageUrl;
        Word updatedWord = await _wordDal.UpdateAsync(existingWord);
        WordResponseDto mappedWord = _mapper.Map<WordResponseDto>(updatedWord);
        return mappedWord;
    }


    [CacheRemoveGroup([WordGroup])]
    public async Task<WordResponseDto> DeleteImageAsync(Guid wordId, string url)
    {
        if (wordId == Guid.Empty) throw new ArgumentNullException(nameof(wordId));
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));

        string fileName = Path.GetFileName(url);
        string temp = wordId.ToString();
        if (fileName.Contains(temp) == false) throw new BusinessException("Url and word did not match!");

        Word existingWord = await _wordDal.GetAsync(filter: w => w.Id == wordId);
        if (existingWord == null) throw new BusinessException("Data(existing) for update Not Found !");

        var result = await _fileService.DeleteAsync(containerName: "images", blobFilename: $"word/{fileName}");
        if (result == false) throw new BusinessException("File has not been deleted!");
        
        existingWord.HasImage = false;
        existingWord.Image = string.Empty;
        Word updatedWord = await _wordDal.UpdateAsync(existingWord);
        WordResponseDto mappedWord = _mapper.Map<WordResponseDto>(updatedWord);
        return mappedWord;
    }


    // 2) Interaction Methods ...
    //[Cache("WordInfoForUserCacheKey", [wordCacheGroup])]
    //public async Task<WordByUserModel> GetWordForUserAsync(Guid wordId, Guid userId)
    //{
    //    if (wordId == Guid.Empty) throw new ArgumentNullException(nameof(wordId));
    //    if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));
    //    Word word = await _wordDal.GetAsync(
    //        filter: w => w.Id == wordId,
    //        include: w => w.Include(i=> i.Favorites)!
    //    );
    //    WordResponseDto wordResponse = _mapper.Map<WordResponseDto>(word);
    //    WordByUserModel wordByUserModel = new WordByUserModel()
    //    {
    //        Word = wordResponse,
    //        IsWordAddedFav = word.Favorites == null ? false : word.Favorites.Any(w => w.UserId == userId)
    //    };
    //    return wordByUserModel;
    //}


    [Cache(WordListByCategory, [WordGroup])]
    public async Task<ICollection<WordResponseDto>> GetWordsByCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));
        var list = await _wordDal.GetAllAsync(filter: w => w.CategoryId == categoryId);
        var mappedList = list.Select(w => _mapper.Map<WordResponseDto>(w)).ToList();
        return mappedList;
    }


    public async Task<ICollection<WordByUserModel>> GetWordsByCategoryForUserAsync(Guid categoryId, Guid userId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        string _cacheKey = WordCacheKeys.WordListByCategoryForUser(categoryId, userId);
        var resultCache = _cacheService.GetFromCache(_cacheKey);
        if (resultCache.IsSuccess)
        {
            var data = JsonConvert.DeserializeObject<ICollection<WordByUserModel>>(resultCache.Source!);
            if (data != null) return data;
        }

        var list = await _wordDal.GetAllAsync(
            filter: w => w.CategoryId == categoryId,
            include: w => w.Include(i => i.Favorites)!
        );
        var model = list.Select(w => 
            new WordByUserModel()
            { 
                Word = _mapper.Map<WordResponseDto>(w),
                IsWordAddedFav = w.Favorites == null ? false : w.Favorites.Any(f => f.UserId == userId)
            }
        ).ToList();
        
        _cacheService.AddToCache(_cacheKey, [WordCacheKeys.WordGroup, WordCacheKeys.WordUserGroup(userId)], model); // purpose: related user do any favorite process dont remove all cached data just this user...
        return model;
    }


    public async Task<ICollection<WordResponseDto>> GetFavoriteWordsForUserAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        // -------- cache on before --------
        string _cacheKey = WordCacheKeys.FavoriteWordListForUser(userId);
        var resultCache = _cacheService.GetFromCache(_cacheKey);
        if (resultCache.IsSuccess)
        {
            var data = JsonConvert.DeserializeObject<ICollection<WordResponseDto>>(resultCache.Source!);
            if (data != null) return data;
        }
        // -------- cache on before --------

        var list = await _favoriteDal.GetAllAsync(
            filter: f => f.UserId == userId,
            include: f => f.Include(f => f.Word)!
        );
        var mappedList = list.Select(f => _mapper.Map<WordResponseDto>(f.Word)).ToList();

        // -------- cache on success --------
        _cacheService.AddToCache(_cacheKey, [WordCacheKeys.WordGroup, WordCacheKeys.WordUserGroup(userId)], mappedList);
        // -------- cache on success --------

        return mappedList;
    }

    public async Task<ICollection<WordResponseDto>> GetFavoriteWordsForUserByCategoryAsync(Guid categoryId, Guid userId)
    {
        if (categoryId == Guid.Empty) throw new ArgumentNullException(nameof(categoryId));
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        // -------- cache on before --------
        string _cacheKey = WordCacheKeys.FavoriteWordListForByCategoryUser(categoryId, userId);
        var resultCache = _cacheService.GetFromCache(_cacheKey);
        if (resultCache.IsSuccess)
        {
            var data = JsonConvert.DeserializeObject<ICollection<WordResponseDto>>(resultCache.Source!);
            if (data != null) return data;
        }
        // -------- cache on before --------

        var list = await _favoriteDal.GetAllAsync(
            filter: f => f.UserId == userId && f.Word!.CategoryId == categoryId,
            include: f => f.Include(f => f.Word)!
        );
        var mappedList = list.Select(f => _mapper.Map<WordResponseDto>(f.Word)).ToList();

        // -------- cache on success --------
        _cacheService.AddToCache(_cacheKey, [WordCacheKeys.WordGroup, WordCacheKeys.WordUserGroup(userId)], mappedList);
        // -------- cache on success --------

        return mappedList;
    }


    public async Task<ICollection<WordByUserModel>> GetLearnedWordsForUserAsync(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

        // -------- cache on before --------
        string _cacheKey = WordCacheKeys.LearnedWordListForUser(userId);
        var resultCache = _cacheService.GetFromCache(_cacheKey);
        if (resultCache.IsSuccess)
        {
            var data = JsonConvert.DeserializeObject<ICollection<WordByUserModel>>(resultCache.Source!);
            if (data != null) return data;
        }
        // -------- cache on before --------

        var list = await _learningDal.GetAllAsync(
            filter: l => l.UserId == userId,
            include: l => l.Include(l => l.Word).ThenInclude(x=> x!.Favorites)!
        );
        var model = list.Select(l =>
            new WordByUserModel()
            {
                Word = l.Word == null ? null : _mapper.Map<WordResponseDto>(l.Word),
                IsWordAddedFav = (l.Word == null || l.Word.Favorites == null) ? false : 
                    l.Word.Favorites.Any(w => w.UserId == userId)
            }
        ).ToList();

        // -------- cache on success --------
        _cacheService.AddToCache(_cacheKey, [WordCacheKeys.WordGroup, WordCacheKeys.WordUserGroup(userId)], model);
        // -------- cache on success --------

        return model;
    }


    // 3) Favorite Methods ...
    [Validation(typeof(FavoriteWordRequestDto))]
    public async Task AddWordAsFavoriteAsync(FavoriteWordRequestDto favoriteWordRequestDto)
    {
        Favorite objToInsert = new Favorite()
        {
            UserId = favoriteWordRequestDto.UserId,
            WordId = favoriteWordRequestDto.WordId
        };
        await _favoriteDal.AddAsync(objToInsert);

        _cacheService.RemoveCacheGroupKeys([WordCacheKeys.WordUserGroup(objToInsert.UserId)]);
    }


    [Validation(typeof(FavoriteWordRequestDto))]
    public async Task RemoveWordFromFavoritesAsync(FavoriteWordRequestDto favoriteWordRequestDto)
    {
        Favorite objToDelete = new Favorite()
        {
            UserId = favoriteWordRequestDto.UserId,
            WordId = favoriteWordRequestDto.WordId
        };
        await _favoriteDal.DeleteAsync(objToDelete);
         
        _cacheService.RemoveCacheGroupKeys([WordCacheKeys.WordUserGroup(objToDelete.UserId)]);
    }


    // 4) Learning Methods ...
    [Validation(typeof(LearningWordRequestDto))]
    public async Task AddWordAsLearnedAsync(LearningWordRequestDto learningWordRequestDto)
    {
        Learned objToInsert = new Learned()
        {
            UserId = learningWordRequestDto.UserId,
            WordId = learningWordRequestDto.WordId
        };
        await _learningDal.AddAsync(objToInsert);

        _cacheService.RemoveFromCache(WordCacheKeys.LearnedWordListForUser(objToInsert.UserId));
        _cacheService.RemoveFromCache(CategoryCacheKeys.AllCategoryListForUser(objToInsert.UserId)); 
        _cacheService.RemoveCacheGroupKeys([CategoryCacheKeys.CategoryUserGroupByUserId(objToInsert.UserId)]);
    }


    [Validation(typeof(LearningWordRequestDto))]
    public async Task RemoveWordFromLearnedAsync(LearningWordRequestDto learningWordRequestDto)
    {
        Learned objToDelete = new Learned()
        {
            UserId = learningWordRequestDto.UserId,
            WordId = learningWordRequestDto.WordId
        };
        await _learningDal.DeleteAsync(objToDelete);

        _cacheService.RemoveFromCache(WordCacheKeys.LearnedWordListForUser(objToDelete.UserId));
        _cacheService.RemoveFromCache(CategoryCacheKeys.AllCategoryListForUser(objToDelete.UserId));
        _cacheService.RemoveCacheGroupKeys([CategoryCacheKeys.CategoryUserGroupByUserId(objToDelete.UserId)]);
    }
}