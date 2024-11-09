using Microsoft.EntityFrameworkCore;
using Convofy.Models;

namespace Convofy.Services;
public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public required DbSet<User> Users { get; set; }
}