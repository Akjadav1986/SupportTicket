using Microsoft.EntityFrameworkCore;
using SupportTicketClassifier.Api.Agents;
using SupportTicketClassifier.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITicketClassificationAgent, RuleBasedTicketClassificationAgent>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger is enabled for this learning/demo project.
// For production projects, keep this inside: if (app.Environment.IsDevelopment()) { ... }
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Support Ticket Classifier API v1");
    options.RoutePrefix = "swagger";
});

app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.UseHttpsRedirection();
app.UseCors("AngularClient");
app.UseAuthorization();
app.MapControllers();
app.Run();
