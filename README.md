# Support Ticket Classification Agent

A full-stack starter project for classifying customer support tickets using **.NET 9 Web API**, **Entity Framework Core**, **MS SQL Server**, and **Angular 21 UI**.

The current classification agent is **rule-based**. It is designed behind an interface so you can later replace it with **Azure OpenAI**, **Semantic Kernel**, **RAG**, or another AI/ML classifier without changing the API controller workflow.

---

## 1. Technology Stack

| Layer | Technology |
|---|---|
| Backend API | .NET 9 Web API |
| ORM | Entity Framework Core 9 |
| Database | Microsoft SQL Server / LocalDB |
| Frontend | Angular 21 standalone components |
| API Documentation | Swagger / OpenAPI |
| Classification Engine | Rule-based agent using keyword scoring |
| Future AI Upgrade | Azure OpenAI / Semantic Kernel ready architecture |

---

## 2. Project Structure

```text
support-ticket-classification-agent/
├── SupportTicketClassifier.Api/
│   ├── Agents/
│   │   ├── ITicketClassificationAgent.cs
│   │   └── RuleBasedTicketClassificationAgent.cs
│   ├── Controllers/
│   │   └── TicketsController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Dtos/
│   │   ├── ClassificationResultDto.cs
│   │   ├── CreateTicketRequest.cs
│   │   └── TicketResponse.cs
│   ├── Models/
│   │   ├── Ticket.cs
│   │   ├── TicketClassification.cs
│   │   └── TicketStatus.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── SupportTicketClassifier.Api.csproj
│
├── support-ticket-ui/
│   ├── src/app/
│   │   ├── models/
│   │   │   └── ticket.model.ts
│   │   ├── services/
│   │   │   └── ticket.service.ts
│   │   ├── pages/
│   │   │   ├── ticket-create/
│   │   │   └── ticket-list/
│   │   ├── app.routes.ts
│   │   └── app.config.ts
│   ├── package.json
│   └── angular.json
│
├── database.sql
└── README.md
```

---

## 3. Prerequisites

Install these tools before running the project:

| Tool | Purpose |
|---|---|
| .NET SDK 9 | Run Web API and EF Core commands |
| SQL Server or SQL Server LocalDB | Store tickets and classifications |
| Node.js | Run Angular UI |
| Angular CLI | Run Angular 21 application |
| Visual Studio Code / Visual Studio | Development IDE |

Check versions:

```bash
dotnet --version
node --version
npm --version
ng version
```

Install Angular CLI globally if needed:

```bash
npm install -g @angular/cli
```

Install EF Core CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

If EF tool is already installed, update it:

```bash
dotnet tool update --global dotnet-ef
```

---

## 4. Backend Configuration

Backend project path:

```text
SupportTicketClassifier.Api
```

Main configuration file:

```text
SupportTicketClassifier.Api/appsettings.json
```

Default connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SupportTicketClassifierDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 4.1 Use SQL Server LocalDB

Use the default connection string if you have SQL Server LocalDB installed:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=SupportTicketClassifierDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

### 4.2 Use Full SQL Server Instance

Example for Windows authentication:

```json
"DefaultConnection": "Server=localhost;Database=SupportTicketClassifierDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Example for SQL authentication:

```json
"DefaultConnection": "Server=localhost;Database=SupportTicketClassifierDb;User Id=sa;Password=YourStrongPassword;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Do not commit real production passwords into source control. Use user secrets or environment variables for real applications.

---

## 5. Backend Run Steps

Open terminal in the API folder:

```bash
cd SupportTicketClassifier.Api
```

Restore packages:

```bash
dotnet restore
```

Create EF Core migration:

```bash
dotnet ef migrations add InitialCreate
```

Create/update database:

```bash
dotnet ef database update
```

Run the API on the port expected by Angular:

```bash
dotnet run --urls "https://localhost:7001"
```

Open Swagger:

```text
https://localhost:7001/swagger
```

If HTTPS certificate warning appears, run:

```bash
dotnet dev-certs https --trust
```

---

## 6. Angular UI Configuration

Frontend project path:

```text
support-ticket-ui
```

API URL configuration file:

```text
support-ticket-ui/src/app/services/ticket.service.ts
```

Current API URL:

```ts
private readonly apiUrl = 'https://localhost:7001/api/tickets';
```

If your Web API runs on another port, update this URL.

Example:

```ts
private readonly apiUrl = 'https://localhost:5001/api/tickets';
```

---

## 7. Angular Run Steps

Open another terminal in the Angular project folder:

```bash
cd support-ticket-ui
```

Install packages:

```bash
npm install
```

Run Angular:

```bash
npm start
```

Open UI:

```text
http://localhost:4200
```

---

## 8. CORS Configuration

CORS is configured in `Program.cs` so Angular can call the API from `http://localhost:4200`.

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

Middleware order:

```csharp
app.UseHttpsRedirection();
app.UseCors("AngularClient");
app.UseAuthorization();
app.MapControllers();
```

If Angular runs on another port, update the CORS origin.

Example:

```csharp
policy.WithOrigins("http://localhost:4300")
```

---

## 9. Database Design

The project uses two main tables.

### 9.1 Tickets Table

Stores the original support ticket information.

| Column | Description |
|---|---|
| Id | Primary key |
| Title | Ticket title |
| Description | Full customer issue description |
| CustomerEmail | Customer email address |
| ProductName | Optional product/module name |
| Status | Ticket status, stored as string enum |
| CreatedAtUtc | Ticket created date/time in UTC |

### 9.2 TicketClassifications Table

Stores the classification result generated by the agent.

| Column | Description |
|---|---|
| Id | Primary key |
| TicketId | Foreign key to Tickets table |
| Category | Detected ticket category |
| Priority | Detected priority |
| RoutedTeam | Suggested support team |
| Confidence | Classification confidence score |
| Reason | Human-readable reason |
| TagsJson | Matched tags/keywords stored as JSON |
| ClassifiedAtUtc | Classification date/time in UTC |

Relationship:

```text
Ticket 1 ---- 0..1 TicketClassification
```

Meaning: one ticket can have one classification result.

---

## 10. EF Core Configuration

EF Core DbContext file:

```text
SupportTicketClassifier.Api/Data/AppDbContext.cs
```

Important configuration:

```csharp
modelBuilder.Entity<Ticket>(entity =>
{
    entity.ToTable("Tickets");

    entity.Property(x => x.Status)
          .HasConversion<string>()
          .HasMaxLength(30);

    entity.Property(x => x.CreatedAtUtc)
          .HasDefaultValueSql("SYSUTCDATETIME()");

    entity.HasOne(x => x.Classification)
          .WithOne(x => x.Ticket)
          .HasForeignKey<TicketClassification>(x => x.TicketId)
          .OnDelete(DeleteBehavior.Cascade);
});
```

This means:

- `TicketStatus` enum is stored as a readable string in SQL Server.
- `CreatedAtUtc` gets default UTC value.
- Deleting a ticket also deletes its classification.
- Each ticket has only one classification row.

---

## 11. API Endpoints

Base URL:

```text
https://localhost:7001/api/tickets
```

### 11.1 Get All Tickets

```http
GET /api/tickets
```

Returns all tickets with classification result.

### 11.2 Get Ticket By Id

```http
GET /api/tickets/{id}
```

Example:

```http
GET /api/tickets/1
```

### 11.3 Create and Classify Ticket

```http
POST /api/tickets
```

Request body:

```json
{
  "title": "Payment failed and invoice not generated",
  "description": "Customer paid using card but invoice is missing and payment status shows failed.",
  "customerEmail": "customer@example.com",
  "productName": "Billing Portal"
}
```

Expected response example:

```json
{
  "id": 1,
  "title": "Payment failed and invoice not generated",
  "description": "Customer paid using card but invoice is missing and payment status shows failed.",
  "customerEmail": "customer@example.com",
  "productName": "Billing Portal",
  "status": "Classified",
  "createdAtUtc": "2026-07-07T00:00:00Z",
  "classification": {
    "category": "Billing",
    "priority": "High",
    "routedTeam": "Billing Team",
    "confidence": 0.87,
    "reason": "Ticket classified as Billing because it matched related keyword(s).",
    "tags": ["billing", "card", "failed", "invoice", "payment"]
  }
}
```

### 11.4 Reclassify Existing Ticket

```http
POST /api/tickets/{id}/classify
```

Example:

```http
POST /api/tickets/1/classify
```

This runs the classification agent again and updates the classification row.

---

## 12. Application Workflow

### 12.1 Main Workflow

```text
User opens Angular UI
        ↓
User fills ticket form
        ↓
Angular calls POST /api/tickets
        ↓
.NET Web API validates request DTO
        ↓
API creates Ticket entity
        ↓
EF Core saves ticket in SQL Server
        ↓
API calls ITicketClassificationAgent
        ↓
RuleBasedTicketClassificationAgent analyzes title + description + product name
        ↓
Agent returns category, priority, routed team, confidence, tags, reason
        ↓
API creates TicketClassification entity
        ↓
EF Core saves classification result in SQL Server
        ↓
API returns TicketResponse DTO
        ↓
Angular displays classification result
```

### 12.2 Reclassification Workflow

```text
User clicks Reclassify button in Angular ticket list
        ↓
Angular calls POST /api/tickets/{id}/classify
        ↓
API loads ticket with existing classification
        ↓
API calls classification agent again
        ↓
Agent recalculates classification
        ↓
API updates TicketClassification row
        ↓
EF Core saves changes
        ↓
Angular refreshes the updated ticket row
```

---

## 13. Classification Agent Workflow

Agent interface:

```text
SupportTicketClassifier.Api/Agents/ITicketClassificationAgent.cs
```

Implementation:

```text
SupportTicketClassifier.Api/Agents/RuleBasedTicketClassificationAgent.cs
```

The API depends on the interface:

```csharp
ITicketClassificationAgent classificationAgent
```

Dependency injection registration:

```csharp
builder.Services.AddScoped<ITicketClassificationAgent, RuleBasedTicketClassificationAgent>();
```

### 13.1 How the Rule-Based Agent Works

The agent follows these steps:

```text
1. Combine ticket title, description, and product name into one text.
2. Convert text to lowercase.
3. Compare text with predefined category keywords.
4. Give score to each category based on matched keywords.
5. Select category with highest score.
6. Detect priority from critical/high priority keywords.
7. Select routed team based on category.
8. Calculate confidence score.
9. Build tags from matched keywords.
10. Return final ClassificationResultDto.
```

### 13.2 Supported Categories

| Category | Example Keywords | Routed Team |
|---|---|---|
| Billing | invoice, payment, refund, subscription, card | Billing Team |
| Technical | api, server, database, timeout, deployment | Engineering Team |
| Security | hacked, breach, malware, phishing, unauthorized | Security Team |
| Account | password, login, locked, reset, profile | Account Support Team |
| Bug | bug, crash, exception, error, failed | Engineering Team |
| FeatureRequest | feature, enhancement, suggestion, add option | Product Team |
| General | fallback when no strong match | L1 Support Team |

### 13.3 Priority Detection

| Priority | Example Keywords / Condition |
|---|---|
| Critical | production down, system down, data loss, security breach, hacked, outage |
| High | urgent, asap, blocked, payment failed, login failed, cannot login |
| Medium | long description / moderate context |
| Low | default priority |

---

## 14. Angular UI Workflow

Angular has two main screens.

### 14.1 Create Ticket Screen

Route:

```text
/tickets/new
```

Component:

```text
src/app/pages/ticket-create/ticket-create.component.ts
```

User enters:

- Title
- Description
- Customer Email
- Product Name

Then Angular calls:

```ts
this.ticketService.createTicket(this.request)
```

This sends POST request to:

```text
https://localhost:7001/api/tickets
```

After success, Angular displays:

- Ticket ID
- Status
- Category
- Priority
- Routed Team
- Confidence
- Reason
- Tags

### 14.2 Ticket List Screen

Route:

```text
/tickets
```

Component:

```text
src/app/pages/ticket-list/ticket-list.component.ts
```

Angular calls:

```ts
this.ticketService.getTickets()
```

This sends GET request to:

```text
https://localhost:7001/api/tickets
```

The screen shows all tickets and has a **Reclassify** button.

---

## 15. Request and Response DTOs

### 15.1 CreateTicketRequest

```csharp
public sealed class CreateTicketRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string CustomerEmail { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProductName { get; set; }
}
```

### 15.2 ClassificationResultDto

```csharp
public sealed class ClassificationResultDto
{
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string RoutedTeam { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public string Reason { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
}
```

---

## 16. Manual SQL Script

The file `database.sql` is included only as a reference.

Recommended approach:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Use `database.sql` only if you want to manually inspect or create the database structure.

---

## 17. How to Add a New Category

Open:

```text
SupportTicketClassifier.Api/Agents/RuleBasedTicketClassificationAgent.cs
```

Add new category keywords:

```csharp
["Network"] = ["dns", "vpn", "latency", "packet loss", "firewall"]
```

Update routed team logic:

```csharp
"Network" => "Network Support Team",
```

Then run the API again.

---

## 18. How to Replace Rule-Based Agent with Azure OpenAI Later

Current registration:

```csharp
builder.Services.AddScoped<ITicketClassificationAgent, RuleBasedTicketClassificationAgent>();
```

Future registration:

```csharp
builder.Services.AddScoped<ITicketClassificationAgent, AzureOpenAiTicketClassificationAgent>();
```

Because the controller depends on `ITicketClassificationAgent`, controller code does not need to change.

Future AI-agent flow:

```text
Ticket text
   ↓
Prompt template
   ↓
Azure OpenAI / Semantic Kernel
   ↓
Structured JSON output
   ↓
ClassificationResultDto
   ↓
Save result using EF Core
```

Recommended JSON output shape from AI:

```json
{
  "category": "Billing",
  "priority": "High",
  "routedTeam": "Billing Team",
  "confidence": 0.92,
  "reason": "The ticket mentions payment failure and missing invoice.",
  "tags": ["payment", "invoice", "billing"]
}
```

---

## 19. Common Problems and Fixes

### Problem: Angular cannot call API

Check these points:

1. API is running.
2. API URL in `ticket.service.ts` is correct.
3. CORS origin in `Program.cs` allows `http://localhost:4200`.
4. HTTPS certificate is trusted.

Fix HTTPS certificate:

```bash
dotnet dev-certs https --trust
```

### Problem: Database connection failed

Check:

1. SQL Server / LocalDB is installed.
2. Connection string is correct.
3. Database user has permission.
4. SQL Server service is running.

### Problem: `dotnet ef` command not found

Install EF Core tool:

```bash
dotnet tool install --global dotnet-ef
```

Then close and reopen terminal.

### Problem: Swagger is not opening

Make sure API is running in development mode and use:

```text
https://localhost:7001/swagger
```

### Problem: Angular package issue

Delete `node_modules` and reinstall:

```bash
rm -rf node_modules package-lock.json
npm install
```

On Windows PowerShell:

```powershell
Remove-Item -Recurse -Force node_modules
Remove-Item -Force package-lock.json
npm install
```

---

## 20. Testing Samples

### Billing Ticket

```json
{
  "title": "Payment failed and invoice not generated",
  "description": "Customer paid using card but invoice is missing and payment status shows failed.",
  "customerEmail": "customer@example.com",
  "productName": "Billing Portal"
}
```

Expected:

```text
Category: Billing
Priority: High
Team: Billing Team
```

### Security Ticket

```json
{
  "title": "Possible account hacked",
  "description": "Customer says unauthorized login happened and MFA was disabled.",
  "customerEmail": "security-user@example.com",
  "productName": "Customer Portal"
}
```

Expected:

```text
Category: Security
Priority: Critical
Team: Security Team
```

### Feature Request Ticket

```json
{
  "title": "Add export to Excel option",
  "description": "Customer wants a new functionality to export reports to Excel.",
  "customerEmail": "product-user@example.com",
  "productName": "Reports"
}
```

Expected:

```text
Category: FeatureRequest
Priority: Low
Team: Product Team
```

---

## 21. Gujarati Workflow Summary

આ project માં user Angular UI માં support ticket create કરે છે. Angular API ને request મોકલે છે. .NET 9 Web API પહેલા ticket ને SQL Server database માં save કરે છે. પછી API `ITicketClassificationAgent` service ને call કરે છે. Agent ticket ના title, description અને product name પરથી category, priority, routed team, confidence અને tags detect કરે છે. ત્યાર પછી classification result `TicketClassifications` table માં save થાય છે. છેલ્લે API Angular ને full response આપે છે અને Angular UI classification result display કરે છે.

Main idea:

```text
Ticket Create → Save in DB → Agent Classify → Save Classification → Show Result in UI
```

---

## 22. Recommended Next Improvements

Good next features for learning and real-world usage:

1. Add ticket status update API.
2. Add assigned agent/user table.
3. Add comments/history table.
4. Add authentication using JWT.
5. Add role-based authorization for Admin, Support Agent, Manager.
6. Add Azure OpenAI classification agent.
7. Add Semantic Kernel plugin/function calling.
8. Add RAG using historical tickets and vector database.
9. Add dashboard with category and priority charts.
10. Add background service for reclassification or SLA escalation.

---

## 23. Quick Start Commands

Backend:

```bash
cd SupportTicketClassifier.Api
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run --urls "https://localhost:7001"
```

Frontend:

```bash
cd support-ticket-ui
npm install
npm start
```

Open:

```text
API Swagger: https://localhost:7001/swagger
Angular UI:  http://localhost:4200
```
