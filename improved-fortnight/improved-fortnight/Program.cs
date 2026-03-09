using System;

namespace LibrarySystem
{
    public class Author
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new();
    }

    public class Genre
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Title { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new();
    }

    public class Book
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        public List<BookItem> Inventory { get; set; } = new();
    }

    public class BookItem
    {
        public int Id { get; set; }
        [Required]
        public string InventoryNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
    }


    public class LibraryContext : DbContext
    {
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<BookItem> BookItems { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryCodeFirstDb;Trusted_Connection=True;");
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            using (LibraryContext db = new LibraryContext())
            {
                Console.WriteLine("Перевірка бази даних...");
                db.Database.EnsureCreated();

                if (!db.Authors.Any())
                {
                    Console.WriteLine("Заповнення бази тестовими даними...");

                    var author = new Author { Name = "Тарас Шевченко" };
                    var genre = new Genre { Title = "Поезія" };

                    var book = new Book
                    {
                        Title = "Кобзар",
                        Author = author,
                        Genre = genre
                    };

                    db.BookItems.Add(new BookItem { Book = book, InventoryNumber = "INV-001", IsAvailable = true });
                    db.BookItems.Add(new BookItem { Book = book, InventoryNumber = "INV-002", IsAvailable = false });

                    db.SaveChanges();
                    Console.WriteLine("Дані успішно збережено!");
                }

                Console.WriteLine("\n--- Список книг у бібліотеці ---");
                var libraryData = db.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Include(b => b.Inventory);

                foreach (var b in libraryData)
                {
                    Console.WriteLine($"Книга: \"{b.Title}\" | Автор: {b.Author.Name} | Жанр: {b.Genre.Title}");
                    foreach (var item in b.Inventory)
                    {
                        string status = item.IsAvailable ? "В наявності" : "Видана";
                        Console.WriteLine($"  - Примірник: {item.InventoryNumber} [{status}]");
                    }
                }
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}