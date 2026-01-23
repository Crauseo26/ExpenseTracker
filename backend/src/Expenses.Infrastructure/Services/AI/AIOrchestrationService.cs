using Expenses.Application.DTOs;
using Expenses.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Expenses.Infrastructure.Services.AI;

public class AIOrchestrationService : IAIOrchestrationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AIOrchestrationService> _logger;
    private readonly string _baseUrl;
    private readonly int _timeoutSeconds;
    private readonly string _apiKey;

    public AIOrchestrationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AIOrchestrationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["AIService:BaseUrl"] ?? "http://localhost:5000";
        _timeoutSeconds = int.TryParse(configuration["AIService:TimeoutSeconds"], out var timeout) 
            ? timeout 
            : 30;
        _apiKey = configuration["AIService:ApiKey"] ?? string.Empty;

        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-Service-Token", _apiKey);
        }
    }

    public async Task<AIProposalResponseDto> ProcessInputAsync(
        Guid userId,
        string inputType,
        string normalizedContent,
        List<string> availableAccounts,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new AIProcessingRequest
            {
                RawText = normalizedContent,
                InputType = inputType,
                AvailableAccounts = availableAccounts,
                Metadata = new AIRequestMetadata
                {
                    ReceivedAt = DateTime.UtcNow.ToString("o")
                }
            };

            _logger.LogInformation(
                "Sending AI processing request for user {UserId}, input type {InputType}, with {AccountCount} available accounts",
                userId,
                inputType,
                availableAccounts.Count);

            var response = await _httpClient.PostAsJsonAsync(
                "/process-text",
                request,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "AI service returned status code {StatusCode}",
                    response.StatusCode);
                return CreateEmptyResponse();
            }

            var aiResponse = await response.Content.ReadFromJsonAsync<AIProcessingResponse>(
                cancellationToken: cancellationToken);

            if (aiResponse == null)
            {
                _logger.LogWarning("AI service returned null response");
                return CreateEmptyResponse();
            }

            _logger.LogInformation(
                "AI processing completed. Received {ProposalCount} proposals with overall confidence {Confidence}",
                aiResponse.Proposals.Count,
                aiResponse.OverallConfidence);

            return MapToDto(aiResponse);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "Failed to connect to AI service at {BaseUrl}. Service may be unavailable.",
                _baseUrl);
            return CreateEmptyResponse();
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(
                ex,
                "AI service request timed out after {TimeoutSeconds} seconds",
                _timeoutSeconds);
            return CreateEmptyResponse();
        }
        catch (JsonException ex)
        {
            _logger.LogError(
                ex,
                "Failed to deserialize AI service response");
            return CreateEmptyResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while processing AI request");
            return CreateEmptyResponse();
        }
    }

    private static AIProposalResponseDto CreateEmptyResponse()
    {
        return new AIProposalResponseDto
        {
            Proposals = new List<ExpenseProposalDto>(),
            OverallConfidence = 0.0
        };
    }

    private static AIProposalResponseDto MapToDto(AIProcessingResponse response)
    {
        return new AIProposalResponseDto
        {
            Proposals = response.Proposals.Select(p => new ExpenseProposalDto
            {
                AccountId = Guid.Empty,
                Amount = p.Amount,
                Currency = p.Currency,
                Description = p.Description,
                ExpenseType = p.ExpenseType,
                PurchaseDate = DateTime.TryParse(p.PurchaseDate, out var date) ? date : DateTime.UtcNow,
                Confidence = p.Confidence,
                SuggestedAccountName = p.Metadata.SuggestedAccount
            }).ToList(),
            OverallConfidence = response.OverallConfidence
        };
    }
}
