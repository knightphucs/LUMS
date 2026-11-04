using LUMS.Core.Abstractions;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace LUMS.Infrastructure.Data;

public static class DbSeeder
{
    public const string DemoPassword = "Lums@12345";

    public static async Task SeedAsync(LumsDbContext db, IPasswordHasher hasher, IClock clock, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);
        if (await db.Accounts.AnyAsync(ct))
        {
            return;
        }

        var now = clock.Now;
        var today = clock.Today;
        var passwordHash = hasher.Hash(DemoPassword);

        Account NewAccount(string name, string email, string code, Role role, ReaderType? type, AccountStatus status, int createdDaysAgo) => new()
        {
            FullName = name,
            Email = email,
            Code = code,
            PasswordHash = passwordHash,
            Role = role,
            ReaderType = type,
            Status = status,
            CreatedAt = now.AddDays(-createdDaysAgo)
        };

        var admin = NewAccount("Quản trị viên", "admin@lums.edu.vn", "NV000", Role.Admin, null, AccountStatus.HoatDong, 400);
        var librarian = NewAccount("Trần Thị B", "tranthib@lums.edu.vn", "NV001", Role.ThuThu, null, AccountStatus.HoatDong, 3);
        var readerA = NewAccount("Nguyễn Văn A", "22300123@sinhvien.edu.vn", "22300123", Role.DocGia, ReaderType.SinhVien, AccountStatus.HoatDong, 200);
        var readerC = NewAccount("Lê Văn C", "22300456@sinhvien.edu.vn", "22300456", Role.DocGia, ReaderType.SinhVien, AccountStatus.HoatDong, 20);
        var readerD = NewAccount("Phạm D", "22190022@sinhvien.edu.vn", "22190022", Role.DocGia, ReaderType.SinhVien, AccountStatus.Khoa, 10);
        var readerF = NewAccount("Võ Thị F", "22300789@sinhvien.edu.vn", "22300789", Role.DocGia, ReaderType.GiangVien, AccountStatus.HoatDong, 5);

        db.Accounts.AddRange(admin, librarian, readerA, readerC, readerD, readerF);
        await db.SaveChangesAsync(ct);

        db.PolicyConfigurations.Add(new PolicyConfiguration
        {
            MaxLoanDays = 14,
            MaxBooksPerReader = 5,
            MaxReservations = 3,
            FinePerDay = 5000m,
            UpdatedById = admin.Id,
            EffectiveFrom = now.AddDays(-30)
        });

        var books = new List<BookTitle>();
        var bookByCode = new Dictionary<string, BookTitle>();

        BookTitle AddBook(string code, string title, string author, string isbn, string shelf, int copies, int createdDaysAgo,
            string description, string category = "Công nghệ thông tin")
        {
            var book = new BookTitle
            {
                Title = title,
                Author = author,
                Category = category,
                Isbn = isbn,
                Description = description,
                DefaultShelf = shelf,
                CreatedAt = now.AddDays(-createdDaysAgo)
            };
            for (var i = 1; i <= copies; i++)
            {
                book.Copies.Add(new Copy
                {
                    Id = $"LUMS-{code}-{i:000}",
                    Shelf = shelf,
                    Status = CopyStatus.SanSang,
                    AddedAt = book.CreatedAt
                });
            }

            books.Add(book);
            bookByCode[code] = book;
            return book;
        }

        AddBook("CS101", "Lập trình C# Cơ bản", "Nguyễn Văn A", "978-604-01-2345", "A-12", 5, 1,
            "Giáo trình nhập môn C# và .NET: cú pháp, hướng đối tượng, LINQ và làm việc với cơ sở dữ liệu.");
        AddBook("AI401", "Trí tuệ nhân tạo", "Võ Thị F", "978-604-01-2349", "B-01", 1, 2,
            "Tổng quan về tìm kiếm, biểu diễn tri thức, học máy và các ứng dụng của trí tuệ nhân tạo.");
        AddBook("SE301", "Kỹ thuật phần mềm", "Trần Minh E", "978-604-01-2350", "B-02", 3, 3,
            "Quy trình phát triển phần mềm, phân tích yêu cầu, thiết kế kiến trúc, kiểm thử và bảo trì.");
        AddBook("CS310", "Hệ điều hành", "Trần Văn E", "978-604-01-2351", "A-17", 3, 4,
            "Quản lý tiến trình, bộ nhớ, tập tin và đồng bộ hóa trong hệ điều hành hiện đại.");
        AddBook("CS102", "Cấu trúc dữ liệu & Giải thuật", "Nguyễn Văn A", "978-604-01-2346", "A-11", 2, 12,
            "Mảng, danh sách liên kết, cây, đồ thị và các giải thuật sắp xếp, tìm kiếm kinh điển.");
        AddBook("CS201", "Cơ sở dữ liệu SQL", "Nguyễn Văn A", "978-604-01-2347", "A-14", 2, 15,
            "Mô hình quan hệ, chuẩn hóa, T-SQL, chỉ mục và giao dịch trong SQL Server.");
        AddBook("CS205", "Mạng máy tính", "Lê Thị C", "978-604-01-2348", "A-13", 4, 18,
            "Mô hình OSI/TCP-IP, định tuyến, chuyển mạch và bảo mật mạng căn bản.");
        AddBook("MA102", "Toán rời rạc", "Phạm Văn D", "978-604-01-2352", "C-05", 2, 20,
            "Logic mệnh đề, tập hợp, quan hệ, đếm tổ hợp và lý thuyết đồ thị.", "Toán học");
        AddBook("CS320", "Lập trình Web với ASP.NET Core", "Nguyễn Văn A", "978-604-01-2353", "A-18", 3, 25,
            "Razor Pages, MVC, Entity Framework Core, xác thực và xây dựng REST API.");
        AddBook("SE305", "Phân tích và thiết kế hệ thống", "Trần Minh E", "978-604-01-2354", "B-03", 2, 28,
            "Use case, ERD, UML và các phương pháp phân tích thiết kế hướng đối tượng.");
        AddBook("IS410", "An toàn thông tin", "Lê Thị C", "978-604-01-2355", "B-04", 2, 30,
            "Mật mã học, xác thực, kiểm soát truy cập và các mối đe dọa an ninh ứng dụng web.");
        AddBook("AI420", "Học máy cơ bản", "Võ Thị F", "978-604-01-2356", "B-05", 2, 35,
            "Hồi quy, phân loại, phân cụm và đánh giá mô hình học máy.");

        db.BookTitles.AddRange(books);
        await db.SaveChangesAsync(ct);

        var copyById = books.SelectMany(b => b.Copies).ToDictionary(c => c.Id);

        Loan Borrow(string copyId, Account reader, int borrowedDaysAgo, int dueInDays)
        {
            copyById[copyId].Status = CopyStatus.DangDuocMuon;
            return new Loan
            {
                CopyId = copyId,
                ReaderId = reader.Id,
                LibrarianId = librarian.Id,
                BorrowedAt = now.AddDays(-borrowedDaysAgo),
                DueDate = today.AddDays(dueInDays),
                RenewalCount = 0,
                Status = dueInDays < 0 ? LoanStatus.QuaHan : LoanStatus.DangMuon
            };
        }

        var activeLoans = new List<Loan>
        {
            Borrow("LUMS-CS101-002", readerA, 5, 9),
            Borrow("LUMS-CS102-001", readerA, 12, 2),
            Borrow("LUMS-CS205-004", readerA, 17, -3),
            Borrow("LUMS-CS101-004", readerC, 3, 11),
            Borrow("LUMS-CS101-005", readerC, 8, 6),
            Borrow("LUMS-AI401-001", readerC, 4, 10),
            Borrow("LUMS-CS201-002", readerC, 6, 8),
            Borrow("LUMS-MA102-002", readerC, 2, 12),
            Borrow("LUMS-CS102-002", readerC, 7, 7)
        };

        var history = new List<Loan>();
        var perMonth = new[] { 6, 9, 12, 16, 20, 11, 14, 17 };
        var allCopyIds = copyById.Keys.OrderBy(x => x, StringComparer.Ordinal).ToList();
        var historyReaders = new[] { readerA, readerC, readerF };
        var counter = 0;
        for (var m = 0; m < perMonth.Length; m++)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-(perMonth.Length - 1 - m));
            var isCurrentMonth = m == perMonth.Length - 1;
            for (var j = 0; j < perMonth[m]; j++)
            {
                var day = isCurrentMonth ? Math.Min(j, now.Day - 1) : j % 26;
                var borrowedAt = monthStart.AddDays(day).AddHours(9);
                var returned = DateOnly.FromDateTime(borrowedAt.AddDays(10));
                if (returned > today)
                {
                    returned = today;
                }

                history.Add(new Loan
                {
                    CopyId = allCopyIds[counter % allCopyIds.Count],
                    ReaderId = historyReaders[counter % historyReaders.Length].Id,
                    LibrarianId = librarian.Id,
                    BorrowedAt = borrowedAt,
                    DueDate = DateOnly.FromDateTime(borrowedAt.AddDays(14)),
                    ReturnedDate = returned,
                    Status = LoanStatus.DaTra
                });
                counter++;
            }
        }

        db.Loans.AddRange(activeLoans);
        db.Loans.AddRange(history);
        await db.SaveChangesAsync(ct);

        copyById["LUMS-SE301-001"].Status = CopyStatus.DangGiuCho;
        db.Reservations.AddRange(
            new Reservation { BookTitleId = bookByCode["CS102"].Id, ReaderId = readerF.Id, ReservedAt = now.AddDays(-2), QueuePosition = 1, Status = ReservationStatus.DangCho },
            new Reservation { BookTitleId = bookByCode["AI401"].Id, ReaderId = readerD.Id, ReservedAt = now.AddDays(-3), QueuePosition = 1, Status = ReservationStatus.DangCho },
            new Reservation { BookTitleId = bookByCode["AI401"].Id, ReaderId = readerA.Id, ReservedAt = now.AddDays(-1), QueuePosition = 2, Status = ReservationStatus.DangCho },
            new Reservation { BookTitleId = bookByCode["SE301"].Id, ReaderId = readerA.Id, ReservedAt = now.AddDays(-6), QueuePosition = 1, Status = ReservationStatus.SanSangNhanSach, HoldUntil = today.AddDays(3) });

        db.Fines.AddRange(
            new Fine
            {
                LoanId = history[0].Id,
                ReaderId = readerA.Id,
                LibrarianId = librarian.Id,
                Type = FineType.TreHan,
                Amount = 15000m,
                Note = "Trả trễ 3 ngày",
                HandledAt = now.AddMinutes(-5),
                PaymentStatus = PaymentStatus.DaThanhToan
            },
            new Fine
            {
                CopyId = "LUMS-CS205-001",
                ReaderId = readerC.Id,
                LibrarianId = librarian.Id,
                Type = FineType.Hong,
                Amount = 50000m,
                Note = "Rách bìa",
                HandledAt = now.AddMinutes(-30),
                PaymentStatus = PaymentStatus.DaThanhToan
            });

        db.SystemLogs.AddRange(
            new SystemLog { AccountId = admin.Id, Action = "Cập nhật cấu hình: Số ngày mượn tối đa = 14", RelatedObject = "PolicyConfiguration", Timestamp = now.AddHours(-1) },
            new SystemLog { AccountId = librarian.Id, Action = "Xử lý trả sách: LUMS-CS101-002", RelatedObject = "Copy:LUMS-CS101-002", Timestamp = now.AddHours(-2) },
            new SystemLog { AccountId = admin.Id, Action = "Khóa tài khoản: Phạm D", RelatedObject = $"Account:{readerD.Id}", Timestamp = now.AddHours(-20) },
            new SystemLog { AccountId = librarian.Id, Action = "Thêm sách mới: Hệ điều hành (CS310)", RelatedObject = "BookTitle", Timestamp = now.AddHours(-22) });

        await db.SaveChangesAsync(ct);
    }
}
