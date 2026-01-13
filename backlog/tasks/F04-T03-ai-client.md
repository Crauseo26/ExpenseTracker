# F04-T03: Implement AI Service Client

## Task Metadata
- **Feature**: F04 — ExpenseInput & AI Processing Pipeline
- **Task ID**: F04-T03
- **Agent**: Backend-Infrastructure-Agent
- **Status**: In Progress
- **Branch**: feature/F04-T03-ai-client
- **Dependencies**: F04-T02 (IAIOrchestrationService interface)

---

## Objective
Implement the concrete AI service client (`AIOrchestrationService`) in the Infrastructure layer that communicates with the external Python-based AI service via HTTP.

---

## Context
- The `IAIOrchestrationService` interface already exists in the Application layer
- The `ProcessExpenseInputCommandHandler` consumes this interface
- We need the concrete implementation that performs HTTP communication with the AI service
- The Python AI service does not exist yet, but the client must be ready to connect

---

## Requirements

### 1. Implement AIOrchestrationService
- Create `AIOrchestrationService` class in Infrastructure layer
- Implement `IAIOrchestrationService` interface
- Use `IHttpClientFactory` for HTTP communication
- Handle HTTP request/response serialization per spec

### 2. HTTP Client Configuration
- Use typed HttpClient or IHttpClientFactory
- Configure base URL from `appsettings.json` (section: `AIService`)
- Set appropriate timeouts and headers
- Handle connection failures gracefully

### 3. Request/Response Contract
Per `specs/07_ai_pipeline.md`, implement:

**Request:**
```json
{
  "normalizedText": "string",
  "inputType": "TEXT | IMAGE | NOTIFICATION",
  "userId": "UUID",
  "metadata": {
    "source": "string",
    "receivedAt": "ISO-8601"
  }
}
```

**Response:**
```json
{
  "proposals": [
    {
      "accountId": "UUID",
      "amount": 1090.00,
      "currency": "UYU",
      "description": "McDonalds",
      "expenseType": "SPORADIC | REPETITIVE",
      "purchaseDate": "YYYY-MM-DD",
      "confidence": 0.92
    }
  ],
  "overallConfidence": 0.90
}
```

### 4. Error Handling
- Handle `HttpRequestException` (service unavailable)
- Handle timeout exceptions
- Handle deserialization errors
- Return meaningful error messages to caller
- Do NOT throw exceptions for connection failures (return null or empty result)

### 5. Configuration
Add to `appsettings.json`:
```json
{
  "AIService": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 30
  }
}
```

### 6. Dependency Injection
- Register service in `InfrastructureServiceCollectionExtensions`
- Register as scoped service
- Configure HttpClient with base address from configuration

---

## Implementation Checklist

- [ ] Create `Services/AIOrchestrationService.cs` in Infrastructure layer
- [ ] Implement `IAIOrchestrationService.ProcessInputAsync` method
- [ ] Create request/response DTOs for HTTP contract
- [ ] Configure IHttpClientFactory with named or typed client
- [ ] Add AIService configuration section to appsettings.json
- [ ] Handle HTTP errors (connection, timeout, deserialization)
- [ ] Register service in DI container
- [ ] Verify build succeeds
- [ ] Test error handling (service unavailable scenario)

---

## Acceptance Criteria

1. ✅ `AIOrchestrationService` implements `IAIOrchestrationService`
2. ✅ Uses `IHttpClientFactory` for HTTP communication
3. ✅ Base URL configured from `appsettings.json`
4. ✅ Request/response serialization matches spec contract
5. ✅ Connection failures handled gracefully (no unhandled exceptions)
6. ✅ Service registered in DI container
7. ✅ Build succeeds with no warnings or errors
8. ✅ Code follows technical conventions (14_technical_conventions.md)

---

## Technical Notes

### HTTP Client Best Practices
- Use IHttpClientFactory to avoid socket exhaustion
- Configure retry policies if needed (optional for MVP)
- Set User-Agent header for observability
- Log request/response for debugging

### Error Handling Strategy
Since the Python service doesn't exist yet:
- Catch `HttpRequestException` and return null or empty proposals
- Log the error for debugging
- Allow `ProcessExpenseInputCommandHandler` to mark ExpenseInput as ERROR

### Future Considerations
- Add retry logic with Polly
- Add circuit breaker pattern
- Add request/response logging middleware
- Add telemetry and metrics

---

## References
- `specs/07_ai_pipeline.md` - AI Pipeline Architecture
- `specs/11_engineering_guardrails.md` - Layering rules
- `specs/14_technical_conventions.md` - Code style and naming
- `Expenses.Application.Interfaces.IAIOrchestrationService` - Interface contract
- `Expenses.Application.Commands.ProcessExpenseInputCommandHandler` - Consumer

---

## Agent Notes
- This is Infrastructure layer work (external service integration)
- Follow strict layering: Infrastructure → Application interface
- Do not add domain logic in this service
- Keep it simple: HTTP request/response only
- Error handling is critical since service doesn't exist yet
