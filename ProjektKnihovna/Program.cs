using System.Text.RegularExpressions;
using Newtonsoft.Json;

public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int Year { get; set; }
    public List<string> Genres { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Book(string title, string author, string isbn, int year, List<string> genres)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        Year = year;
        Genres = genres ?? new List<string>();
    }
    

    public override string ToString()
    {
        return $"{Title}, {Author}, {Year}, Genres: {Genres}, ISBN: {ISBN}, Available: {IsAvailable}";
    }
    public void ReturnBook()
    {
        IsAvailable = true;
    }
}

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string LibraryCardNumber { get; set; }

    public User(string firstName, string lastName, string libraryCardNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        LibraryCardNumber = libraryCardNumber;
    }
    
    public void ShowCurrentLoans(List<Loan> loans)
    {
        Console.WriteLine("");
        
        var currentLoans = loans.Where(l => l.Borrower.LibraryCardNumber == this.LibraryCardNumber && l.ReturnedDate == null).ToList();

        if (currentLoans.Count == 0)
        {
            Console.WriteLine("You have no current loans.");
        }
        else
        {
            Console.WriteLine($"Current loans for {this.FirstName} {this.LastName}:");
            foreach (var loan in currentLoans)
            {
                Console.WriteLine($"  Book: {loan.BorrowedBook.Title}, Borrowed on: {loan.BorrowedDate}");
            }
        }
    }


    public override string ToString()
    {
        return $"{FirstName} {LastName}, Card: {LibraryCardNumber}";
    }
}

public class Loan
{
    public Book BorrowedBook { get; set; }
    public User Borrower { get; set; }
    public DateTime BorrowedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public List<string> LoanHistory { get; set; } = new List<string>();

    public Loan(Book book, User user)
    {
        BorrowedBook = book;
        Borrower = user;
        BorrowedDate = DateTime.Now;
    }

    public void ReturnBook()
    {
        // Nastavení data vrácení
        ReturnedDate = DateTime.Now;

        // Změna dostupnosti knihy na úrovni vypůjčené knihy
        BorrowedBook.IsAvailable = true;
        // Přidání záznamu o vrácení knihy do historie
        LoanHistory.Add($"Returned Book: {BorrowedBook.Title} by {Borrower.FirstName} {Borrower.LastName}, Returned Date: {ReturnedDate}");
    }


    public override string ToString()
    {
        return $"Book: {BorrowedBook.Title}, Borrower: {Borrower.FirstName} {Borrower.LastName}, Borrowed: {BorrowedDate}, Returned: {ReturnedDate?.ToString() ?? "Not yet"}";
    }
}

public class Library
{
    public List<Book> Books { get; set; } = new List<Book>();
    public List<User> Users { get; set; } = new List<User>();
    public List<Loan> Loans { get; set; } = new List<Loan>();

    private const string BooksFile = "books.json";
    private const string UsersFile = "users.json";
    private const string LoansFile = "loans.json";
    
    public void ShowLoanHistory()
    {
        if (Loans.Count == 0)
        {
            Console.WriteLine("No loans found.");
            return;
        }

        foreach (var loan in Loans)
        {
            // Displaying the borrowed book's title and the borrow date
            Console.WriteLine($"\nLoan history for book: {loan.BorrowedBook.Title}");
            Console.WriteLine($"  Borrowed by: {loan.Borrower.FirstName} {loan.Borrower.LastName}");
            Console.WriteLine($"  Borrowed on: {loan.BorrowedDate}");

            if (loan.LoanHistory == null || loan.LoanHistory.Count == 0)
            {
                Console.WriteLine("  No history found for this loan.");
            }
            else
            {
                foreach (var record in loan.LoanHistory)
                {
                    Console.WriteLine("  " + record);
                }
            }

            // Show the return date if it exists, otherwise show that the book hasn't been returned yet.
            if (loan.ReturnedDate.HasValue)
            {
                Console.WriteLine($"  Returned on: {loan.ReturnedDate.Value}");
            }
            else
            {
                Console.WriteLine("  Book not returned yet.");
            }
        }
    }


    public List<Book> SearchByGenre(List<string> genres)
    {
        return Books.Where(book => genres.All(searchGenre =>
            book.Genres.Any(bookGenre => bookGenre.Contains(searchGenre, StringComparison.OrdinalIgnoreCase))))
        .ToList();
    }



    public List<string> Genres { get; set; } = new List<string>
    {
        "Fiction",
        "Non-fiction",
        "Science Fiction",
        "Fantasy",
        "Mystery",
        "Biography",
        "History",
        "Children's",
        "Romance",
        "Horror",
        "Thriller",
        "Adventure",
        "Self-help",
        "Philosophy",
        "Psychology",
        "Poetry",
        "Drama",
        "Graphic Novel",
        "Travel",
        "Religious"
    };
    public Library()
    {
        LoadData();
    }

    public void SearchBookByName(string name)
    {
        var results = Books.FindAll(b => b.Title.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
        DisplaySearchResults(results);
    }

    public void SearchBookByReleaseDate(int year)
    {
        var results = Books.FindAll(b => b.Year == year);
        DisplaySearchResults(results);
    }

    public void SearchBooksByGenre(List<string> searchGenres)
    {
        var matchingBooks = Books.Where(b => b.Genres.Any(genre => searchGenres.Contains(genre, StringComparer.OrdinalIgnoreCase))).ToList();

        if (matchingBooks.Count == 0)
        {
            Console.WriteLine("No books found for the specified genre(s).");
        }
        else
        {
            Console.WriteLine("Books matching the specified genre(s):");
            foreach (var book in matchingBooks)
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, Year: {book.Year}, Genres: {string.Join(", ", book.Genres)}, ISBN: {book.ISBN}, Available: {book.IsAvailable}");
            }
        }
    }


    private void DisplaySearchResults(List<Book> results)
    {
        if (results.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }

        Console.WriteLine("\nSearch Results:");
        foreach (var book in results)
        {
            Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, Year: {book.Year}, Genres: {string.Join(", ", book.Genres)}, ISBN: {book.ISBN}, Available: {book.IsAvailable}");
        }
    }



    public void SearchBookByTitle(string titlePart)
    {
        var foundBooks = Books.FindAll(b => b.Title.IndexOf(titlePart, StringComparison.OrdinalIgnoreCase) >= 0);

        if (foundBooks.Count > 0)
        {
            Console.WriteLine("\nBooks found:");
            foreach (var book in foundBooks)
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, Year: {book.Year}, Genres: {string.Join(", ", book.Genres)}, ISBN: {book.ISBN}, Available: {book.IsAvailable}");
            }
        }
        else
        {
            Console.WriteLine($"No books found with title containing '{titlePart}'.");
        }
    }
    public void SearchBookByAuthor(string authorPart)
    {
        var foundBooks = Books.FindAll(b => b.Author.IndexOf(authorPart, StringComparison.OrdinalIgnoreCase) >= 0);

        if (foundBooks.Count > 0)
        {
            Console.WriteLine("\nBooks by author found:");
            foreach (var book in foundBooks)
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, Year: {book.Year}, Genres: {string.Join(", ", book.Genres)}, ISBN: {book.ISBN}, Available: {book.IsAvailable}");
            }
        }
        else
        {
            Console.WriteLine($"No books found by author containing '{authorPart}'.");
        }
    }

    public void SearchBookByReleaseYear(int year)
    {
        var foundBooks = Books.FindAll(b => b.Year == year);

        if (foundBooks.Count > 0)
        {
            Console.WriteLine($"\nBooks released in {year}:");
            foreach (var book in foundBooks)
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, Year: {book.Year}, Genres: {string.Join(", ", book.Genres)}, ISBN: {book.ISBN}, Available: {book.IsAvailable}");
            }
        }
        else
        {
            Console.WriteLine($"No books found from the year {year}.");
        }
    }

    private void DisplayGenresTable(List<string> genres)
    {
        int rows = 4;
        int columns = 5;
        int totalGenres = genres.Count;

        Console.WriteLine("Available Genres (5x4 Table):");
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int genreIndex = row * columns + col;

                if (genreIndex < totalGenres)
                {
                    Console.Write($"{genres[genreIndex],-20}");
                }
                else
                {
                    Console.Write($"{string.Empty,-20}");
                }
            }
            Console.WriteLine();
        }
    }

    public void SearchBookByGenre()
    {
        DisplayGenresTable(Genres);

        Console.Write("\nEnter genres to search (comma-separated): ");
        List<string> selectedGenres = Console.ReadLine().Split(',').Select(g => g.Trim()).ToList();

        var results = SearchByGenre(selectedGenres);

        if (results.Count == 0)
        {
            Console.WriteLine("No books found with those genres.");
        }
        else
        {
            Console.WriteLine("Books found:");
            DisplaySearchResults(results);
        }
    }





    // Add Book
    public void AddBook(Book book)
    {
        if (!Regex.IsMatch(book.ISBN, @"^\d{3}-\d{10}$"))
        {
            Console.WriteLine("Invalid ISBN format. Correct format: XXX-XXXXXXXXXX");
            return;
        }
        Books.Add(book);
        SaveData();
    }

    // Remove Book
    public void RemoveBook(string isbn)
    {
        var book = Books.Find(b => b.ISBN == isbn);
        if (book != null)
        {
            Books.Remove(book);
            SaveData();
            Console.WriteLine("Book removed successfully.");
        }
        else
        {
            Console.WriteLine("Book not found.");
        }
    }

    public void EditBookByName(string bookName)
    {
        // Find books that match the entered name (case-insensitive search)
        var matchingBooks = Books.FindAll(b => b.Title.IndexOf(bookName, StringComparison.OrdinalIgnoreCase) >= 0);

        if (matchingBooks.Count == 0)
        {
            Console.WriteLine("No books found with the given name.");
            return;
        }

        // If multiple books match, ask the user to pick one
        if (matchingBooks.Count > 1)
        {
            Console.WriteLine("\nMultiple books found. Please choose a book to edit:");
            for (int i = 0; i < matchingBooks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {matchingBooks[i].Title} by {matchingBooks[i].Author}, ISBN: {matchingBooks[i].ISBN}");
            }

            Console.Write("Choose a book (enter number): ");
            string choice = Console.ReadLine();
            if (int.TryParse(choice, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= matchingBooks.Count)
            {
                EditBookDetails(matchingBooks[selectedIndex - 1]);  // Call method to edit the selected book
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
        else
        {
            // Only one match, directly edit this book
            EditBookDetails(matchingBooks[0]);
        }
    }

    // This method handles editing the book details
    private void EditBookDetails(Book book)
    {
        Console.WriteLine("\nEditing Book:");
        Console.WriteLine($"Current Title: {book.Title}");
        Console.WriteLine($"Current Author: {book.Author}");
        Console.WriteLine($"Current ISBN: {book.ISBN}");
        Console.WriteLine($"Current Year: {book.Year}");
        Console.WriteLine($"Current Genres: {string.Join(", ", book.Genres)}");

        // Ask for new values, and keep the original if nothing is entered
        Console.Write("Enter new title (leave blank to keep current): ");
        string newTitle = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            book.Title = newTitle;
        }

        Console.Write("Enter new author (leave blank to keep current): ");
        string newAuthor = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newAuthor))
        {
            book.Author = newAuthor;
        }

        Console.Write("Enter new year of publication (leave blank to keep current): ");
        string newYearStr = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newYearStr) && int.TryParse(newYearStr, out int newYear))
        {
            book.Year = newYear;
        }

        // Display available genres and allow user to choose multiple
        Console.WriteLine("\nAvailable Genres:");
        for (int i = 0; i < Genres.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Genres[i]}");
        }

        Console.Write("Enter new genres by selecting numbers separated by commas (leave blank to keep current genres): ");
        string genresInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(genresInput))
        {
            List<string> newGenres = genresInput.Split(',')
                                                .Select(g => g.Trim())
                                                .Where(g => int.TryParse(g, out int idx) && idx >= 1 && idx <= Genres.Count)
                                                .Select(g => Genres[int.Parse(g) - 1])
                                                .ToList();

            if (newGenres.Any())
            {
                book.Genres = newGenres;
            }
        }

        Console.WriteLine("Book updated successfully.");
        SaveData();  // Save the updated book data to file
    }



    // Add User
    public void AddUser(User user)
    {
        if (!Regex.IsMatch(user.LibraryCardNumber, @"^\d{2}-\d{4}$"))
        {
            Console.WriteLine("Invalid library card format. Correct format: XX-XXXX");
            return;
        }
        Users.Add(user);
        SaveData();
    }

    // Remove User
    public void RemoveUser(string libraryCardNumber)
    {
        var user = Users.Find(u => u.LibraryCardNumber == libraryCardNumber);
        if (user != null)
        {
            Users.Remove(user);
            SaveData();
            Console.WriteLine("User removed successfully.");
        }
        else
        {
            Console.WriteLine("User not found.");
        }
    }

    // Loan Book
    public void LoanBook(string isbn, string libraryCardNumber)
    {
        var book = Books.Find(b => b.ISBN == isbn && b.IsAvailable);
        var user = Users.Find(u => u.LibraryCardNumber == libraryCardNumber);

        if (book == null)
        {
            Console.WriteLine("Error: Book not found, or book is not available.");
            return;
        }
        else if (user == null)
        {
            Console.WriteLine("Error: User not found.");
            return;
        }
        if (user != null && book != null)
        {
            book.IsAvailable = false;
            Loans.Add(new Loan(book, user));
            SaveData();
            Console.WriteLine("Book loaned successfully.");
        }
    }
    
 public void LoanBookByName(string libraryCardNumber)
{
    Console.Write("Enter the name of the book you want to loan: ");
    string bookTitle = Console.ReadLine();

    // Find books with matching titles that are available
    var matchingBooks = Books.Where(b => b.Title.Contains(bookTitle, StringComparison.OrdinalIgnoreCase) && b.IsAvailable).ToList();

    if (matchingBooks.Count == 0)
    {
        Console.WriteLine("Error: No available books found with that title.");
        return;
    }
    
    Book selectedBook = null;
    if (matchingBooks.Count > 1)
    {
        Console.WriteLine("Multiple books with that title are available. Please select the book to loan:");
        for (int i = 0; i < matchingBooks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {matchingBooks[i].Title}, ISBN: {matchingBooks[i].ISBN}");
        }

        int choice;
        int invalidAttempts = 0;
        while (invalidAttempts < 2)  // Limit to two invalid attempts
        {
            Console.Write("Enter the number of the book you want to loan: ");
            string choiceInput = Console.ReadLine();
            if (int.TryParse(choiceInput, out choice) && choice > 0 && choice <= matchingBooks.Count)
            {
                selectedBook = matchingBooks[choice - 1];
                break;
            }
            else
            {
                Console.WriteLine("Invalid selection. Please enter a valid number from the list.");
                invalidAttempts++;
            }
        }

        // If no valid book was selected, exit the method
        if (selectedBook == null)
        {
            Console.WriteLine("Operation canceled due to invalid inputs.");
            return;
        }
    }
    else
    {
        // If only one book matches, select it directly
        selectedBook = matchingBooks[0];
    }

    // Find the user by library card number
    var user = Users.Find(u => u.LibraryCardNumber == libraryCardNumber);
    if (user == null)
    {
        Console.WriteLine("Error: User not found.");
        return;
    }

    // Loan the book if both book and user are valid
    selectedBook.IsAvailable = false;
    Loans.Add(new Loan(selectedBook, user));
    SaveData();
    Console.WriteLine($"Book '{selectedBook.Title}' loaned successfully to {user.FirstName} {user.LastName}.");
}




    // Return Book
    public void ReturnBook(string isbn)
    {
        var loan = Loans.Find(l => l.BorrowedBook.ISBN == isbn && l.ReturnedDate == null);
        if (loan == null)
        {
            Console.WriteLine("Error: Loan not found.");
            return;
        }

        loan.ReturnBook();
        SaveData();
    }
    
    public void ReturnBookByName(string libraryCardNumber)
{
    Console.Write("Enter the name of the book you want to return: ");
    string bookTitle = Console.ReadLine();

    var matchingLoans = Loans.Where(l => 
        l.BorrowedBook.Title.Contains(bookTitle, StringComparison.OrdinalIgnoreCase) &&
        l.Borrower.LibraryCardNumber == libraryCardNumber &&
        l.ReturnedDate == null).ToList();

    if (matchingLoans.Count == 0)
    {
        Console.WriteLine("Error: No active loans found with that title for this user.");
        return;
    }

    Loan selectedLoan = null;
    if (matchingLoans.Count > 1)
    {
        Console.WriteLine("Multiple loans found with that title. Please select the loan to return:");
        for (int i = 0; i < matchingLoans.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {matchingLoans[i].BorrowedBook.Title}, ISBN: {matchingLoans[i].BorrowedBook.ISBN}, Loan Date: {matchingLoans[i].BorrowedDate}");
        }

        int choice;
        int invalidAttempts = 0;
        while (invalidAttempts < 2)
        {
            Console.Write("Enter the number of the loan you want to return: ");
            string choiceInput = Console.ReadLine();
            if (int.TryParse(choiceInput, out choice) && choice > 0 && choice <= matchingLoans.Count)
            {
                selectedLoan = matchingLoans[choice - 1];
                break;
            }
            else
            {
                Console.WriteLine("Invalid selection. Please enter a valid number from the list.");
                invalidAttempts++;
            }
        }

        if (selectedLoan == null)
        {
            Console.WriteLine("Operation canceled due to invalid inputs.");
            return;
        }
    }
    else
    {
        selectedLoan = matchingLoans[0];
    }

    selectedLoan.ReturnBook();
    SaveData();
    Console.WriteLine($"Book '{selectedLoan.BorrowedBook.Title}' returned successfully by {selectedLoan.Borrower.FirstName} {selectedLoan.Borrower.LastName}.");
}



    // Show Books
    public void ShowBooks()
    {
        if (Books.Count == 0)
        {
            Console.WriteLine("No books found.");
        }
        else
        {
            foreach (var book in Books)
            {
                Console.WriteLine($"Title: {book.Title}, Author: {book.Author}, Year: {book.Year}, Genres: {string.Join(", ", book.Genres)}, ISBN: {book.ISBN}, Available: {book.IsAvailable}");
            }
        }
    }

    // Show Loans
    public void ShowLoans()
    {
        foreach (var loan in Loans)
        {
            Console.WriteLine(loan);
        }
    }

    // Clear all Books and Loans
    public void ClearBooks()
    {
        Books.Clear();
        Loans.Clear();
        SaveData();
        Console.WriteLine("All books and loans cleared.");
    }

    // Clear all Users
    public void ClearUsers()
    {
        Users.Clear();
        SaveData();
        Console.WriteLine("All users cleared.");
    }

    // Clear All Data
    public void ClearAllData()
    {
        Books.Clear();
        Users.Clear();
        Loans.Clear();
        SaveData();
        Console.WriteLine("All data cleared.");
    }

    // Load data from files
    public void LoadData()
    {
        if (File.Exists(BooksFile))
        {
            var booksData = File.ReadAllText(BooksFile);
            Books = !string.IsNullOrEmpty(booksData)
                    ? JsonConvert.DeserializeObject<List<Book>>(booksData)
                    : new List<Book>();
        }

        if (File.Exists(UsersFile))
        {
            var usersData = File.ReadAllText(UsersFile);
            Users = !string.IsNullOrEmpty(usersData)
                    ? JsonConvert.DeserializeObject<List<User>>(usersData)
                    : new List<User>();
        }

        if (File.Exists(LoansFile))
        {
            var loansData = File.ReadAllText(LoansFile);
            Loans = !string.IsNullOrEmpty(loansData)
                    ? JsonConvert.DeserializeObject<List<Loan>>(loansData)
                    : new List<Loan>(); // Kontrola a inicializace prázdného seznamu
        }
    }


    // Save data to files
    public void SaveData()
    {
        File.WriteAllText(BooksFile, JsonConvert.SerializeObject(Books, Formatting.Indented));
        File.WriteAllText(UsersFile, JsonConvert.SerializeObject(Users, Formatting.Indented));
        File.WriteAllText(LoansFile, JsonConvert.SerializeObject(Loans, Formatting.Indented));
    }

    public void ShowUsers()
    {
        if (Users.Count == 0)
        {
            Console.WriteLine("No users found.");
        }
        else
        {
            foreach (var user in Users)
            {
                Console.WriteLine(user);
            }
        }
    }

}

public class Program
{
    static void Main()
    {
        var library = new Library();
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("1. Login as Librarian");
            Console.WriteLine("2. Login as User");
            Console.WriteLine("3. Login as Administrator");
            Console.WriteLine("4. Search for Books");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    LibrarianMenu(library);
                    break;
                case "2":
                    UserMenu(library);
                    break;
                case "3":
                    AdministratorMenu(library);
                    break;
                case "4":
                    SearchMenu(library);
                    break;
                case "5":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // Librarian Menu
    static void LibrarianMenu(Library library)
    {
        bool librarianRunning = true;
        while (librarianRunning)
        {
            Console.WriteLine("\n--- Librarian Menu ---");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Remove Book");
            Console.WriteLine("3. Show Books");
            Console.WriteLine("4. Show Loans");
            Console.WriteLine("5. Add User");
            Console.WriteLine("6. Show Users");
            Console.WriteLine("7. Show Loan History");
            Console.WriteLine("8. Search Books");
            Console.WriteLine("9. Return to Main Menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddBook(library);
                    break;
                case "2":
                    RemoveBook(library);
                    break;
                case "3":
                    library.ShowBooks();
                    break;
                case "4":
                    library.ShowLoans();
                    break;
                case "5":
                    AddUser(library);
                    break;
                case "6":
                    library.ShowUsers();
                    break;
                case "7":
                    library.ShowLoanHistory();
                    break;
                case "8":
                    ShowSearchMenu(library);
                    break;
                case "9":
                    librarianRunning = false;
                    break; 
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // User Menu
    static void UserMenu(Library library)
    {
        Console.Write("Enter your library card number (format XX-XXXX): ");
        string cardNumber = Console.ReadLine();

        var user = library.Users.Find(u => u.LibraryCardNumber == cardNumber);
        if (user == null)
        {
            Console.WriteLine("Invalid card number or user not found.");
            return;
        }

        bool userRunning = true;
        while (userRunning)
        {
            Console.WriteLine("\n--- User Menu ---");
            Console.WriteLine("1. Loan Book");
            Console.WriteLine("2. Return Book");
            Console.WriteLine("3. Show Books");
            Console.WriteLine("4. Show Current Loans");
            Console.WriteLine("5. Delete My Account");
            Console.WriteLine("6. Return to Main Menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    library.LoanBookByName(cardNumber);
                    break;
                case "2":
                    library.ReturnBookByName(cardNumber);
                    break;
                case "3":
                    library.ShowBooks();
                    break;
                case "4":
                    user.ShowCurrentLoans(library.Loans); // Pass the list of loans to the method
                    break;
                case "5":
                    DeleteAccount(library, cardNumber);
                    userRunning = false;
                    break;
                case "6":
                    userRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // Administrator Menu
    static void AdministratorMenu(Library library)
    {
        bool adminRunning = true;
        while (adminRunning)
        {
            Console.WriteLine("\n--- Administrator Menu ---");
            Console.WriteLine("1. Book Management");
            Console.WriteLine("2. User Management");
            Console.WriteLine("3. Library Management");
            Console.WriteLine("4. Return to Main Menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    BookManagement(library);
                    break;
                case "2":
                    UserManagement(library);
                    break;
                case "3":
                    LibraryManagement(library);
                    break;
                case "4":
                    adminRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // Book Management for Admin
    static void BookManagement(Library library)
    {
        bool bookManagementRunning = true;
        while (bookManagementRunning)
        {
            Console.WriteLine("\n--- Book Management ---");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Remove Book");
            Console.WriteLine("3. Edit Book by Name");
            Console.WriteLine("4. Loan Book");
            Console.WriteLine("5. Return Book");
            Console.WriteLine("6. Show Books");
            Console.WriteLine("7. Show Loans");
            Console.WriteLine("8. Return to Admin Menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddBook(library);
                    break;
                case "2":
                    RemoveBook(library);
                    break;
                case "3": // Option to edit a book by name in librarian's menu
                    Console.Write("Enter part of the book's name to edit: ");
                    string editBookName = Console.ReadLine();
                    library.EditBookByName(editBookName);
                    break;
                case "4":
                    string cardNumber = string.Empty;
                    bool isValid = false;
                    int invalidInputCount = 0;

                    while (!isValid)
                    {
                        Console.Write("Enter your library card number (format xx-xxxx): ");
                        cardNumber = Console.ReadLine();

                        // Check if the input matches the format xx-xxxx
                        if (string.IsNullOrWhiteSpace(cardNumber))
                        {
                            invalidInputCount++;
                            Console.WriteLine("Card number cannot be empty. Please try again.");

                            if (invalidInputCount >= 2)
                            {
                                Console.WriteLine("You have entered an empty input twice. Exiting...");
                                break;
                            }
                        }
                        else if (cardNumber.Length == 7 && cardNumber[2] == '-' && 
                                 cardNumber.Substring(0, 2).All(char.IsDigit) && 
                                 cardNumber.Substring(3, 4).All(char.IsDigit))
                        {
                            isValid = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid card number format. Please enter in the format xx-xxxx.");
                            invalidInputCount++;
                            if (invalidInputCount >= 2)
                            {
                                Console.WriteLine("You have entered an invalid card number twice. Exiting...");
                                break;
                            }
                        }
                    }

                    if (isValid)
                    {
                        library.LoanBookByName(cardNumber);    
                    }
                    break;
                case "5":
                    string cardNumberTwo = string.Empty;
                    bool isValidTwo = false;
                    int invalidInputCountTwo = 0;

                    while (!isValidTwo)
                    {
                        Console.Write("Enter your library card number (format xx-xxxx): ");
                        cardNumberTwo = Console.ReadLine();

                        // Check if the input matches the format xx-xxxx
                        if (string.IsNullOrWhiteSpace(cardNumberTwo))
                        {
                            invalidInputCountTwo++;
                            Console.WriteLine("Card number cannot be empty. Please try again.");

                            if (invalidInputCountTwo >= 2)
                            {
                                Console.WriteLine("You have entered an empty input twice. Exiting...");
                                break;
                            }
                        }
                        else if (cardNumberTwo.Length == 7 && cardNumberTwo[2] == '-' && 
                                 cardNumberTwo.Substring(0, 2).All(char.IsDigit) && 
                                 cardNumberTwo.Substring(3, 4).All(char.IsDigit))
                        {
                            isValidTwo = true;
                        }
                        else
                        {
                            Console.WriteLine("Invalid card number format. Please enter in the format xx-xxxx.");
                            invalidInputCountTwo++;
                            if (invalidInputCountTwo >= 2)
                            {
                                Console.WriteLine("You have entered an invalid card number twice. Exiting...");
                                break;
                            }
                        }
                    }

                    if (isValidTwo)
                    {
                        library.ReturnBookByName(cardNumberTwo);    
                    }
                    break;
                case "6":
                    library.ShowBooks();
                    break;
                case "7":
                    library.ShowLoans();
                    break;
                case "8":
                    bookManagementRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // User Management for Admin
    static void UserManagement(Library library)
    {
        bool userManagementRunning = true;
        while (userManagementRunning)
        {
            Console.WriteLine("\n--- User Management ---");
            Console.WriteLine("1. Add User");
            Console.WriteLine("2. Remove User");
            Console.WriteLine("3. Edit User Information");
            Console.WriteLine("4. Show Users");
            Console.WriteLine("5. Return to Admin Menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddUser(library);
                    break;
                case "2":
                    RemoveUser(library);
                    break;
                case "3":
                    EditUser(library);
                    break;
                case "4":
                    library.ShowUsers();
                    break;
                case "5":
                    userManagementRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    // Library Management for Admin
    static void LibraryManagement(Library library)
    {
        bool libraryManagementRunning = true;
        while (libraryManagementRunning)
        {
            Console.WriteLine("\n--- Library Management ---");
            Console.WriteLine("1. Clear All Books and Loans");
            Console.WriteLine("2. Clear All Users");
            Console.WriteLine("3. Clear All Data");
            Console.WriteLine("4. Load Data from File");
            Console.WriteLine("5. Return to Admin Menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ConfirmAndClearBooks(library);
                    break;
                case "2":
                    ConfirmAndClearUsers(library);
                    break;
                case "3":
                    ConfirmAndClearAllData(library);
                    break;
                case "4":
                    library.LoadData();
                    Console.WriteLine("Data loaded from files.");
                    break;
                case "5":
                    libraryManagementRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }
    
    static void SearchMenu(Library library)
    {
        bool searching = true;
    
        while (searching)
        {
            Console.WriteLine("\n--- Search Menu ---");
            Console.WriteLine("1. Search by Title");
            Console.WriteLine("2. Search by Author");
            Console.WriteLine("3. Search by Release Year");
            Console.WriteLine("4. Search by Genre");
            Console.WriteLine("5. Back to Main Menu");
            Console.Write("Choose an option: ");
            string searchChoice = Console.ReadLine();

            switch (searchChoice)
            {
                case "1":
                    Console.Write("Enter part of the book title to search: ");
                    string titlePart = Console.ReadLine();
                    library.SearchBookByTitle(titlePart);
                    break;
                case "2":
                    Console.Write("Enter part of the author's name to search: ");
                    string authorPart = Console.ReadLine();
                    library.SearchBookByAuthor(authorPart);
                    break; 
                case "3":
                    Console.Write("Enter the release year to search: ");
                    int year;
                    if (int.TryParse(Console.ReadLine(), out year))
                    {
                        library.SearchBookByReleaseYear(year);
                    }
                    else
                    {
                        Console.WriteLine("Invalid year format.");
                    }
                    break;
                case "4":
                    library.SearchBookByGenre();
                    break;
                case "5":
                    searching = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }
    private static void ShowSearchMenu(Library library)
    {
        bool searching = true;

        while (searching)
        {
            Console.WriteLine("\n--- Search Menu ---");
            Console.WriteLine("1. Search Book by Name");
            Console.WriteLine("2. Search Book by Author");
            Console.WriteLine("3. Search Book by Release Date");
            Console.WriteLine("4. Search Book by Genre");
            Console.WriteLine("5. Back to Librarian Menu");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter book name: ");
                    string bookName = Console.ReadLine();
                    library.SearchBookByName(bookName);
                    break;
                case "2":
                    Console.Write("Enter author name: ");
                    string authorName = Console.ReadLine();
                    library.SearchBookByAuthor(authorName);
                    break;
                case "3":
                    Console.Write("Enter release year: ");
                    if (int.TryParse(Console.ReadLine(), out int releaseYear))
                    {
                        library.SearchBookByReleaseDate(releaseYear);
                    }
                    else
                    {
                        Console.WriteLine("Invalid year format.");
                    }
                    break;
                case "4":
                    library.SearchBookByGenre();
                    break;
                case "5":
                    searching = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }


    // Additional Helper Functions

    static void AddBook(Library library)
    {
        Console.Write("Enter book title: ");
        string title = Console.ReadLine();

        Console.Write("Enter author: ");
        string author = Console.ReadLine();

        Console.Write("Enter ISBN (format XXX-XXXXXXXXXX): ");
        string isbn = Console.ReadLine();

        Console.Write("Enter year of publication: ");
        int year = int.Parse(Console.ReadLine());
        
        Console.WriteLine("\nAvailable Genres:");
        for (int i = 0; i < library.Genres.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {library.Genres[i]}");
        }

        Console.Write("Enter genres (separate multiple genres with commas): ");
        List<string> genres = Console.ReadLine().Split(',').Select(g => g.Trim()).ToList();

        library.AddBook(new Book(title, author, isbn, year, genres));
        Console.WriteLine("Book added successfully.");
    }

    static void RemoveBook(Library library)
    {
        Console.Write("Enter ISBN of the book to remove: ");
        string isbn = Console.ReadLine();
        library.RemoveBook(isbn);
    }

    static void AddUser(Library library)
    {
        Console.Write("Enter first name: ");
        string firstName = Console.ReadLine();

        Console.Write("Enter last name: ");
        string lastName = Console.ReadLine();

        Console.Write("Enter library card number (format XX-XXXX): ");
        string cardNumber = Console.ReadLine();

        library.AddUser(new User(firstName, lastName, cardNumber));
        Console.WriteLine("User added successfully.");
    }

    static void RemoveUser(Library library)
    {
        Console.Write("Enter library card number (format XX-XXXX): ");
        string cardNumber = Console.ReadLine();
        library.RemoveUser(cardNumber);
    }

    static void EditUser(Library library)
    {
        Console.Write("Enter library card number of user to edit: ");
        string cardNumber = Console.ReadLine();
        var user = library.Users.Find(u => u.LibraryCardNumber == cardNumber);

        if (user == null)
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.Write("Enter new first name (leave blank to keep unchanged): ");
        string newFirstName = Console.ReadLine();

        Console.Write("Enter new last name (leave blank to keep unchanged): ");
        string newLastName = Console.ReadLine();

        Console.Write("Enter new library card number (leave blank to keep unchanged): ");
        string newCardNumber = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(newFirstName)) user.FirstName = newFirstName;
        if (!string.IsNullOrWhiteSpace(newLastName)) user.LastName = newLastName;
        if (!string.IsNullOrWhiteSpace(newCardNumber)) user.LibraryCardNumber = newCardNumber;

        library.SaveData();
        Console.WriteLine("User information updated successfully.");
    }

    static void LoanBook(Library library, string cardNumber = null)
    {
        Console.Write("Enter ISBN of the book to loan: ");
        string isbn = Console.ReadLine();

        if (string.IsNullOrEmpty(cardNumber))
        {
            Console.Write("Enter library card number: ");
            cardNumber = Console.ReadLine();
        }

        library.LoanBook(isbn, cardNumber);
        
    }

    static void ReturnBook(Library library)
    {
        Console.Write("Enter ISBN of the book to return: ");
        string isbn = Console.ReadLine();

        library.ReturnBook(isbn);
        Console.WriteLine("Book returned successfully.");
        
    }

    static void DeleteAccount(Library library, string cardNumber)
    {
        Console.WriteLine("Are you sure you want to delete your account? Type 'yes' to confirm.");
        string confirmation = Console.ReadLine();

        if (confirmation.ToLower() == "yes")
        {
            library.RemoveUser(cardNumber);
            Console.WriteLine("Your account has been deleted.");
        }
        else
        {
            Console.WriteLine("Account deletion cancelled.");
        }
    }

    static void ConfirmAndClearBooks(Library library)
    {
        Console.WriteLine("Are you sure you want to clear all books and loans? Type 'yes' to confirm.");
        string confirmation = Console.ReadLine();

        if (confirmation.ToLower() == "yes")
        {
            library.ClearBooks();
        }
        else
        {
            Console.WriteLine("Clearing books and loans cancelled.");
        }
    }

    static void ConfirmAndClearUsers(Library library)
    {
        Console.WriteLine("Are you sure you want to clear all users? Type 'yes' to confirm.");
        string confirmation = Console.ReadLine();

        if (confirmation.ToLower() == "yes")
        {
            library.ClearUsers();
        }
        else
        {
            Console.WriteLine("Clearing users cancelled.");
        }
    }

    static void ConfirmAndClearAllData(Library library)
    {
        Console.WriteLine("Are you sure you want to clear all data? Type 'yes' to confirm.");
        string confirmation = Console.ReadLine();

        if (confirmation.ToLower() == "yes")
        {
            library.ClearAllData();
        }
        else
        {
            Console.WriteLine("Clearing all data cancelled.");
        }
    }
}
