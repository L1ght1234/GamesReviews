using GamesReviews;
using GamesReviews.Infrastructure;
using GamesReviews.Middlewares;
using GamesReviews1;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies(builder.Configuration);

var app = builder.Build();

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Api");
    });
}

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GamesReviewsDbContext>();

    var retryCount = 0;
    var maxRetries = 5;

    while (true)
    {
        try
        {
            dbContext.Database.Migrate();
            break;
        }
        catch (Npgsql.NpgsqlException)
        {
            if (++retryCount >= maxRetries)
                throw;
            Thread.Sleep(2000);
        }
    }
}

app.Run();