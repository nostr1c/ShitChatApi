using Microsoft.EntityFrameworkCore;
using ShitChat.Application.Translations.Dtos;
using ShitChat.Application.Translations.Requests;
using ShitChat.Domain.Entities;
using ShitChat.Infrastructure.Data;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Translations.Services;

public class TranslationService : ITranslationService
{
    private readonly AppDbContext _dbContext;
    public TranslationService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(TranslationActionResult, IEnumerable<TranslationDto>)> GetTranslationsAsync()
    {
        var translations = await _dbContext.Translations
            .AsNoTracking()
            .Select(x => new TranslationDto
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.Value,
            }).ToListAsync();

        return (TranslationActionResult.SuccessGotTranslations, translations);
    }

    public async Task<(bool, TranslationActionResult, TranslationDto?)> CreateTranslationAsync(CreateTranslationRequest request)
    {
        var exists = await _dbContext.Translations
            .AnyAsync(x => x.Name == request.Name);

        if (exists)
            return (false, TranslationActionResult.ErrorTranslationNameAlreadyExists, null);

        var translation = new Translation
        {
            Name = request.Name,
            Value = request.Value
        };

        _dbContext.Translations.Add(translation);

        await _dbContext.SaveChangesAsync();

        var translationDto = new TranslationDto
        {
            Id = translation.Id,
            Name = translation.Name,
            Value = translation.Value
        };

        return (true, TranslationActionResult.SuccessCreatedTranslation, translationDto);
    }

    public async Task<(bool, TranslationActionResult, TranslationDto?)> GetTranslationByGuidAsync(Guid translationId)
    {
        var translation = await _dbContext.Translations
            .Where(x => x.Id == translationId)
            .Select(x => new TranslationDto
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.Value
            }).SingleOrDefaultAsync();

        if (translation == null)
            return (false, TranslationActionResult.ErrorTranslationNotFound, null);

        return (true, TranslationActionResult.SuccessGotTranslation, translation);
    }

    public async Task<(bool, TranslationActionResult, TranslationDto?)> UpdateTranslationAsync(Guid translationId, UpdateTranslationRequest request)
    {
        var translation = await _dbContext.Translations
            .Where(x => x.Id == translationId)
            .SingleOrDefaultAsync();

        if (translation == null)
            return (false, TranslationActionResult.ErrorTranslationNotFound, null);

        translation.Name = request.Name;
        translation.Value = request.Value;

        await _dbContext.SaveChangesAsync();

        var translationDto = new TranslationDto
        {
            Id = translationId,
            Name = translation.Name,
            Value = translation.Value
        };

        return (true, TranslationActionResult.SuccessUpdatedTranslation, translationDto);
    }

    public async Task<(bool, TranslationActionResult)> DeleteTranslationAsync(Guid translationId)
    {
        var translation = await _dbContext.Translations
            .Where(x => x.Id == translationId)
            .SingleOrDefaultAsync();

        if (translation == null)
            return (false, TranslationActionResult.ErrorTranslationNotFound);

        _dbContext.Translations.Remove(translation);

        await _dbContext.SaveChangesAsync();

        return (true, TranslationActionResult.SuccessDeletedTranslation);

    }
}
