// TodoItem model
public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsComplete { get; set; }
    public string UserId { get; set; } // foreign key to User table
    public User User { get; set; } // navigation property
}

// User model (inherits from IdentityUser)
public class User : IdentityUser
{
    public ICollection<TodoItem> TodoItems { get; set; }
}

// DbContext class
public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TodoItem>()
            .HasOne<User>(t => t.User)
            .WithMany(u => u.TodoItems)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

// TodoItem service interface
public interface ITodoItemService
{
    Task<List<TodoItem>> GetTodoItemsAsync(string userId);
    Task<TodoItem> GetTodoItemAsync(int id);
    Task CreateTodoItemAsync(TodoItem item);
    Task UpdateTodoItemAsync(TodoItem item);
    Task DeleteTodoItemAsync(int id);
}

// TodoItem service implementation
public class TodoItemService : ITodoItemService
{
    private readonly ApplicationDbContext _context;

    public TodoItemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TodoItem>> GetTodoItemsAsync(string userId)
    {
        return await _context.TodoItems
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<TodoItem> GetTodoItemAsync(int id)
    {
        return await _context.TodoItems.FindAsync(id);
    }

    public async Task CreateTodoItemAsync(TodoItem item)
    {
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTodoItemAsync
