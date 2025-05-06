
using CandidateHub.Application.Interfaces.Services;
using CandidateHub.Application.Services;
using CandidateHub.Domain.Interfaces.Repos;
using CandidateHub.Domain.Interfaces.Services;
using CandidateHub.Infrastructure.Data;
using CandidateHub.Infrastructure.Repos;
using CandidateHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace CandidateHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<CandidateHubDbContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICandidateService, CandidateService>();
            builder.Services.AddScoped<ICacheService, CacheService>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
