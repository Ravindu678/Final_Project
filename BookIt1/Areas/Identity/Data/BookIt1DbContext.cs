using BookIt1.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookIt1.Areas.Admin.Models;
using BookIt1.Models;  // Update this to the correct namespace where your Event model is located


namespace BookIt1.Data;
//namespace BookIt1.Areas.Admin.Models;


public class BookIt1DbContext : IdentityDbContext<BookIt1User>
{
    public BookIt1DbContext(DbContextOptions<BookIt1DbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
 
    }
    public DbSet<Event> Events { get; set; }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

}
