using Microsoft.EntityFrameworkCore;

namespace Chronolog.Api.Infrastructure.Persistence;

public class ChronologDbContext : DbContext
{
    public ChronologDbContext(DbContextOptions<ChronologDbContext> options) : base(options)
    {

    }


}