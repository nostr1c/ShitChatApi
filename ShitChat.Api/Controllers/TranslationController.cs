using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShitChat.Application.DTOs;
using ShitChat.Application.Translations.Dtos;
using ShitChat.Application.Translations.Requests;
using ShitChat.Application.Translations.Services;

namespace ShitChat.Api.Controllers;

//[Authorize(AuthenticationSchemes = "Bearer")]
[ApiController]
[Route("api/v1/[controller]")]
public class TranslationController : ControllerBase
{
    private readonly ITranslationService _translationService;
    public TranslationController(ITranslationService translationService)
    {
        _translationService = translationService;
    }

    [HttpGet]
    public async Task<ActionResult<GenericResponse<IEnumerable<TranslationDto>>>> GetTranslations()
    {
        var (message, translations) = await _translationService.GetTranslationsAsync();
        
        return Ok(ResponseHelper.Success(message, translations));
    }

    [HttpPost]
    public async Task<ActionResult<GenericResponse<TranslationDto>>> CreateTranslation(CreateTranslationRequest request)
    {
        var (success, message, translation) = await _translationService.CreateTranslationAsync(request);

        if (!success)
            return BadRequest(ResponseHelper.Error<TranslationDto>(message));

        return Ok(ResponseHelper.Success(message, translation, StatusCodes.Status201Created));
    }

    [HttpGet("{translationId}")]
    public async Task<ActionResult<GenericResponse<object>>> GetTranslationByGuid(Guid translationId)
    {
        var (success, message, translation) = await _translationService.GetTranslationByGuidAsync(translationId);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message, null, StatusCodes.Status404NotFound));

        return Ok(ResponseHelper.Success(message, translation));
    }

    [HttpPut("{translationId}")]
    public async Task<ActionResult<GenericResponse<object>>> UpdateTranslation(Guid translationId, [FromBody] UpdateTranslationRequest request)
    {
        var (success, message, translation) = await _translationService.UpdateTranslationAsync(translationId, request);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(ResponseHelper.Success(message, translation));
    }

    [HttpDelete("{translationId}")]
    public async Task<ActionResult<GenericResponse<object>>> DeleteTranslation(Guid translationId)
    {
        var (success, message) = await _translationService.DeleteTranslationAsync(translationId);

        if (!success)
            return BadRequest(ResponseHelper.Error<object>(message));

        return Ok(ResponseHelper.Success(message));
    }

}
