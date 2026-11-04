# LUMS Demo — Hệ thống Quản lý Thư viện Đại học (ULMS)

Dự án mẫu để **demo mã nguồn** cho môn *Phát triển dự án phần mềm* — Nhóm 14.
Công nghệ đã chốt: **ASP.NET Core MVC · SQL Server · kiến trúc MVC · 4 pattern: MVC, Repository, Dependency Injection, Dependency Inversion**.

> **Trạng thái:** đã build và chạy thử trên .NET 8 + SQL Server 2022 (Docker): `dotnet build` không lỗi/không cảnh báo, `dotnet test` đạt 13/13. Vẫn nên chạy lại `dotnet build` và `dotnet test` (mục 2) trên máy demo trước buổi trình bày.

---

## 1. Dự án có gì

| Hạng mục (yêu cầu của cô) | Nằm ở đâu |
|---|---|
| **Layout** theo wireframe Figma | `src/LUMS.Web/Views/Shared/` — 4 layout: `_PublicLayout`, `_ReaderLayout` (lồng trong Public), `_StaffLayout`, `_AuthLayout`; CSS tại `wwwroot/css/site.css` (token màu/Inter/lưới 8pt lấy từ Figma và chuyên đề UI/UX) |
| **Màn hình** | Landing `/` · OPAC `/Books` · Chi tiết sách `/Books/Details/{id}` · Đăng nhập `/Account/Login` · 403 · Sách đang mượn `/Reader/MyLoans` · Dashboard Admin `/Admin/Dashboard` · Quản lý kho sách `/Staff/Books` · thanh tab di động (≤ 768px) |
| **Controller trang web** | `src/LUMS.Web/Controllers/` — `Home`, `Books`, `Account`, `Reader`, `Staff`, `Admin`; ViewModel ở `Models/`, View ở `Views/<Controller>/<Action>.cshtml` |
| **API (REST)** | `src/LUMS.Web/Controllers/Api/` — tài liệu Swagger tại `/swagger` (chỉ bật ở môi trường Development, không có link trên giao diện) |
| **CSDL SQL Server** | `src/LUMS.Infrastructure/Data/` — ánh xạ đúng tên bảng/cột trong ERD (Chuyên đề 2), dữ liệu mẫu ở `DbSeeder.cs` |
| **Nghiệp vụ** | `src/LUMS.Core/` — Entity, Service, interface Repository (không phụ thuộc EF Core/ASP.NET) |
| **Kiểm thử đơn vị** | `tests/LUMS.Tests/` — chạy bằng Repository giả, không cần SQL Server |

### API hiện có

| Phương thức | Đường dẫn | Quyền | Use case |
|---|---|---|---|
| GET | `/api/books?keyword=&category=&author=&inStock=&page=&pageSize=` | Công khai | UC-01-01 |
| GET | `/api/books/{id}` | Công khai | UC-01-02 |
| GET | `/api/books/filters` | Công khai | UC-01-01 |
| GET | `/api/loans/mine` | Độc giả | UC-01-03 |
| POST | `/api/loans/{id}/renew` | Độc giả (cần header `X-CSRF-TOKEN`) | UC-01-04 |

Mã lỗi: `409` vi phạm quy tắc nghiệp vụ (quá hạn / có người đặt trước), `404` không tìm thấy, `403` sai chủ sở hữu hoặc sai vai trò, `401` chưa đăng nhập. Nội dung lỗi theo chuẩn ProblemDetails (RFC 7807).

---

## 2. Chạy dự án

**Yêu cầu:** .NET SDK 8.0 trở lên, và một SQL Server (Docker là nhanh nhất).

```bash
# 1) SQL Server bằng Docker
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Lums_Demo_2026!" \
  -p 1433:1433 --name lums-sql -d mcr.microsoft.com/mssql/server:2022-latest
#   Mac chip Apple Silicon: thêm --platform linux/amd64 và bật Rosetta trong Docker Desktop.

# 2) Build + test + chạy
dotnet build
dotnet test
dotnet run --project src/LUMS.Web --launch-profile http
```

Mở <http://localhost:5080>. Lần chạy đầu ở môi trường Development, ứng dụng **tự tạo CSDL `LUMS_Demo` và nạp dữ liệu mẫu** (`Database:AutoCreateAndSeed = true` trong `appsettings.Development.json`).
Dùng SQL Server khác? Sửa `ConnectionStrings:LumsDb` trong `src/LUMS.Web/appsettings.Development.json`.

Nếu dùng .NET SDK khác 8.0 (9, 10…): đổi `TargetFramework` trong `Directory.Build.props`.

### Tài khoản demo (mật khẩu chung `Lums@12345`)

| Vai trò | Đăng nhập bằng | Sau đăng nhập |
|---|---|---|
| Độc giả (Nguyễn Văn A) | `22300123` | Tra cứu; vào *Sách đang mượn* để thử gia hạn |
| Thủ thư (Trần Thị B) | `NV001` | Quản lý kho sách |
| Admin | `admin@lums.edu.vn` | Dashboard + cấu hình quy định |

### Kịch bản demo gợi ý (5 phút)

1. `/` → gõ từ khóa ở ô tìm nhanh → OPAC có lọc theo thể loại/tác giả/tình trạng, phân trang, huy hiệu Còn sách / Sắp hết / Hết sách.
2. Mở một sách → tab *Bản sao có sẵn*.
3. Đăng nhập Độc giả → *Sách đang mượn* có 3 trạng thái: **gia hạn được**, **bị chặn vì có người đặt trước**, **quá hạn (có tiền phạt)**. Bấm *Gia hạn* → gọi `POST /api/loans/{id}/renew`.
4. Gõ thẳng `/swagger` trên thanh địa chỉ (chỉ có ở Development) → thử `GET /api/books` và `POST /api/loans/{id}/renew` (Swagger tự gắn token CSRF).
5. Đăng nhập Admin → Dashboard; đổi *Số ngày mượn tối đa* → *Lưu cấu hình* → dòng mới xuất hiện ở *Nhật ký hệ thống*.
6. Vào `/Admin/Dashboard` bằng tài khoản Độc giả hoặc Thủ thư → trang **403**.

---

## 3. Áp dụng các chuyên đề

### 3.1 Kiến trúc (Chuyên đề 3) — ASP.NET Core MVC

```
LUMS.sln
├─ src/LUMS.Core            ← MODEL: Entities + quy tắc nghiệp vụ + Services + interface Repository
├─ src/LUMS.Infrastructure  ← cài đặt Repository bằng EF Core / SQL Server
├─ src/LUMS.Web             ← CONTROLLER (Controllers/) + VIEW (Views/) + ViewModel (Models/) + composition root
└─ tests/LUMS.Tests
```

Phụ thuộc chỉ đi một chiều: **Web → Core ← Infrastructure** (Core không biết ai).

Quy ước đặt file: **mỗi lớp/interface một file, tên file = tên lớp** (VD `BookService.cs`). Interface của Service tách riêng vào `Services/Interfaces/`. Ngoại lệ có chủ đích: các enum gom trong `Enums/Enums.cs`, exception trong `Exceptions/Exceptions.cs`, DTO gom theo nghiệp vụ trong `Dtos/*Dtos.cs`.

#### Cấu trúc thư mục chi tiết

```
LUMS-Demo/
├─ LUMS.sln
├─ Directory.Build.props                 ← cấu hình chung: net8.0, nullable, implicit usings
│
├─ src/LUMS.Core/                        ← MODEL (nghiệp vụ) — không phụ thuộc EF Core / ASP.NET
│  ├─ Entities/                          ← thực thể = bảng trong ERD, có quy tắc nghiệp vụ
│  │  ├─ Account.cs                         TAI_KHOAN
│  │  ├─ BookTitle.cs                       DAU_SACH
│  │  ├─ Copy.cs                            BAN_SAO
│  │  ├─ Loan.cs                            GIAO_DICH_MUON_TRA — Renew(), GetOverdueDays()…
│  │  ├─ Reservation.cs                     DAT_TRUOC
│  │  ├─ Fine.cs                            PHIEU_PHAT
│  │  ├─ PolicyConfiguration.cs             CAU_HINH_QUY_DINH
│  │  └─ SystemLog.cs                       NHAT_KY_HE_THONG
│  ├─ Enums/Enums.cs                     ← Role, CopyStatus, LoanStatus, ReservationStatus…
│  ├─ Exceptions/Exceptions.cs           ← DomainException (409), NotFoundException (404), ForbiddenException (403)
│  ├─ Dtos/                              ← dữ liệu trả ra cho tầng Web (gom theo nghiệp vụ)
│  │  ├─ BookDtos.cs                        BookSearchCriteria, PagedResult, BookSummaryDto, BookDetailDto…
│  │  ├─ LoanDtos.cs                        LoanDto, RenewLoanResult, ReservationDto
│  │  ├─ AdminDtos.cs                       AdminDashboardDto, PolicyDto, UpdatePolicyRequest…
│  │  └─ AuthDtos.cs                        LoginResult, AuthenticatedUser
│  ├─ Abstractions/                      ← interface hạ tầng mà Core cần (Dependency Inversion)
│  │  ├─ IClock.cs
│  │  ├─ IPasswordHasher.cs
│  │  └─ IUnitOfWork.cs
│  ├─ Repositories/                      ← interface Repository (cài đặt nằm ở Infrastructure)
│  │  ├─ IRepository.cs                     interface chung: GetById / Add / Update / Remove
│  │  ├─ IBookRepository.cs
│  │  ├─ ILoanRepository.cs
│  │  ├─ IReservationRepository.cs
│  │  ├─ IAccountRepository.cs
│  │  ├─ IFineRepository.cs
│  │  ├─ IPolicyRepository.cs
│  │  └─ ISystemLogRepository.cs
│  └─ Services/                          ← nghiệp vụ theo use case (lớp cài đặt)
│     ├─ AuthService.cs                     UC-00-01 Đăng nhập
│     ├─ BookService.cs                     UC-01-01/02 Tra cứu, chi tiết sách
│     ├─ LoanService.cs                     UC-01-03/04 Sách đang mượn, gia hạn
│     ├─ ReservationService.cs              UC-01-05 Danh sách đặt trước
│     ├─ PolicyService.cs                   UC-03-02 Cấu hình quy định
│     ├─ DashboardService.cs                UC-03-03/04 Thống kê, nhật ký
│     └─ Interfaces/                     ← interface của Service — Controller chỉ biết tới các interface này
│        ├─ IAuthService.cs
│        ├─ IBookService.cs
│        ├─ ILoanService.cs
│        ├─ IReservationService.cs
│        ├─ IPolicyService.cs
│        └─ IDashboardService.cs
│
├─ src/LUMS.Infrastructure/              ← cài đặt các interface của Core bằng EF Core + SQL Server
│  ├─ Data/
│  │  ├─ LumsDbContext.cs                   DbContext, 8 DbSet
│  │  ├─ DbSeeder.cs                        dữ liệu mẫu cho demo
│  │  └─ Configurations/                    ánh xạ bảng/cột theo ERD, mỗi bảng một file
│  │     ├─ AccountConfiguration.cs
│  │     ├─ BookTitleConfiguration.cs
│  │     ├─ CopyConfiguration.cs
│  │     ├─ LoanConfiguration.cs
│  │     ├─ ReservationConfiguration.cs
│  │     ├─ FineConfiguration.cs
│  │     ├─ PolicyConfigurationConfiguration.cs
│  │     └─ SystemLogConfiguration.cs
│  ├─ Repositories/
│  │  ├─ EfRepository.cs                    lớp cha chung, cài đặt IRepository<T>
│  │  ├─ BookRepository.cs
│  │  ├─ LoanRepository.cs
│  │  ├─ ReservationRepository.cs
│  │  ├─ AccountRepository.cs
│  │  ├─ FineRepository.cs
│  │  ├─ PolicyRepository.cs
│  │  └─ SystemLogRepository.cs
│  └─ Services/
│     ├─ UnitOfWork.cs                      IUnitOfWork → DbContext.SaveChangesAsync (một giao dịch)
│     ├─ SystemClock.cs                     IClock → giờ hệ thống
│     └─ IdentityPasswordHasher.cs          IPasswordHasher → băm mật khẩu của ASP.NET Identity
│
├─ src/LUMS.Web/                         ← CONTROLLER + VIEW + ViewModel + composition root
│  ├─ Program.cs                            cấu hình MVC, cookie auth, CSRF, Swagger (chỉ Development), seed CSDL
│  ├─ appsettings.json / appsettings.Development.json   chuỗi kết nối, bật tự tạo CSDL khi Development
│  ├─ Properties/launchSettings.json        profile chạy (http://localhost:5080)
│  ├─ Extensions/
│  │  └─ ServiceCollectionExtensions.cs     AddLumsApplication(): nơi duy nhất "nối dây" DI
│  ├─ Controllers/                       ← CONTROLLER trang web (trả về HTML)
│  │  ├─ HomeController.cs                  /, /Error
│  │  ├─ BooksController.cs                 /Books, /Books/Details/{id}
│  │  ├─ AccountController.cs               /Account/Login, /Account/Logout, /Account/Forbidden
│  │  ├─ ReaderController.cs                /Reader/MyLoans                              [Độc giả]
│  │  ├─ StaffController.cs                 /Staff/Books                                 [Thủ thư, Admin]
│  │  ├─ AdminController.cs                 /Admin/Dashboard, POST /Admin/SavePolicy     [Admin]
│  │  └─ Api/                            ← REST API (trả về JSON)
│  │     ├─ ApiExceptionFilterAttribute.cs  exception nghiệp vụ → ProblemDetails 409/404/403
│  │     ├─ BooksApiController.cs           /api/books…
│  │     └─ LoansApiController.cs           /api/loans/mine, /api/loans/{id}/renew   [Độc giả]
│  ├─ Models/                            ← ViewModel: dữ liệu đã gom sẵn cho từng View
│  │  ├─ HomeIndexViewModel.cs              Home/Index
│  │  ├─ ErrorViewModel.cs                  Home/Error
│  │  ├─ BookSearchViewModel.cs             Books/Index và Staff/Books
│  │  ├─ PagerViewModel.cs                  partial _Pager
│  │  ├─ LoginViewModel.cs                  Account/Login (có kiểm tra dữ liệu nhập)
│  │  ├─ MyLoansViewModel.cs                Reader/MyLoans
│  │  ├─ DashboardViewModel.cs              Admin/Dashboard
│  │  └─ PolicyForm.cs                      form cấu hình quy định trên Dashboard
│  ├─ Views/                             ← VIEW: Views/<Controller>/<Action>.cshtml
│  │  ├─ _ViewImports.cshtml                using + tag helper dùng chung
│  │  ├─ _ViewStart.cshtml                  layout mặc định: _PublicLayout
│  │  ├─ Home/        Index.cshtml, Error.cshtml
│  │  ├─ Books/       Index.cshtml, Details.cshtml
│  │  ├─ Account/     Login.cshtml, Forbidden.cshtml, _ViewStart.cshtml (→ _AuthLayout)
│  │  ├─ Reader/      MyLoans.cshtml, _ViewStart.cshtml (→ _ReaderLayout)
│  │  ├─ Staff/       Books.cshtml, _ViewStart.cshtml (→ _StaffLayout)
│  │  ├─ Admin/       Dashboard.cshtml, _ViewStart.cshtml (→ _StaffLayout)
│  │  └─ Shared/
│  │     ├─ _Head.cshtml                    phần <head> dùng chung cho cả 3 layout (title, font, CSS)
│  │     ├─ _PublicLayout.cshtml            khung trang công khai: thanh trên, footer, tab di động
│  │     ├─ _ReaderLayout.cshtml            lồng trong Public, thêm menu bên của Độc giả
│  │     ├─ _StaffLayout.cshtml             khung trang nhân viên (dùng chung cho Staff và Admin)
│  │     ├─ _AuthLayout.cshtml              khung trang đăng nhập
│  │     ├─ _UserMenu.cshtml                menu tài khoản (partial)
│  │     ├─ _BookCard.cshtml                thẻ sách (partial)
│  │     └─ _Pager.cshtml                   phân trang (partial)
│  ├─ Security/                          ← phân quyền và thông tin người đăng nhập
│  │  ├─ AppRoles.cs                        hằng số vai trò: Reader, Librarian, Admin, Staff
│  │  ├─ LumsClaimTypes.cs                  claim riêng (mã SV/NV)
│  │  ├─ LumsClaimsExtensions.cs            User.GetAccountId(), GetFullName(), IsStaff()…
│  │  └─ RoleHome.cs                        trang chuyển tới sau đăng nhập theo vai trò
│  ├─ Presentation/
│  │  └─ DisplayText.cs                     đổi enum → chữ tiếng Việt + màu huy hiệu, định dạng tiền/ngày
│  └─ wwwroot/
│     ├─ css/site.css                       giao diện theo Figma
│     └─ js/site.js                         toast, tab, hộp thoại, nút Gia hạn (gọi API)
│
└─ tests/LUMS.Tests/                     ← kiểm thử đơn vị, không cần SQL Server
   ├─ LoanTests.cs                          quy tắc trong Entity Loan
   ├─ LoanServiceTests.cs                   LoanService chạy với Repository giả
   └─ Fakes/                                cài đặt giả của các interface trong Core
      ├─ FakeClock.cs
      ├─ FakeUnitOfWork.cs
      ├─ FakeLoanRepository.cs
      ├─ FakeReservationRepository.cs
      ├─ FakePolicyRepository.cs
      └─ FakeSystemLogRepository.cs
```

**Muốn tìm gì thì mở đâu:**
- Thêm/sửa một **màn hình**: Controller trong `Web/Controllers/` → ViewModel trong `Web/Models/` → View trong `Web/Views/<Controller>/`.
- Thêm/sửa **quy tắc nghiệp vụ**: `Core/Services/` (khai báo hàm mới ở `Core/Services/Interfaces/`) hoặc trong Entity (`Core/Entities/`).
- Thêm/sửa **câu truy vấn CSDL**: interface ở `Core/Repositories/`, cài đặt ở `Infrastructure/Repositories/`.
- Đăng ký lớp mới vào DI: `Web/Extensions/ServiceCollectionExtensions.cs`.

| Thành phần MVC | Trong dự án |
|---|---|
| **Model** | `LUMS.Core` (Entity, DTO, Service) + `LUMS.Web/Models/` (ViewModel: dữ liệu đã gom sẵn cho từng màn hình) |
| **View** | `Views/<Controller>/<Action>.cshtml`, layout và partial ở `Views/Shared/` |
| **Controller** | `Controllers/*.cs` cho trang web (`HomeController`, `BooksController`, `AccountController`, `ReaderController`, `StaffController`, `AdminController`) — mỗi controller ứng với một nhóm người dùng; `Controllers/Api/*.cs` cho REST API |

Luồng một request: URL `/{controller}/{action}/{id?}` (VD `/Books/Details/3` → `BooksController.Details(3)`) → Controller gọi Service trong Core → đóng gói kết quả vào ViewModel → `return View(model)` → Razor render `Views/Books/Details.cshtml`.

| URL | Controller.Action | Quyền |
|---|---|---|
| `/` | `Home.Index` | Công khai |
| `/Books`, `/Books/Details/{id}` | `Books.Index`, `Books.Details` | Công khai |
| `/Account/Login`, `/Account/Logout`, `/Account/Forbidden` | `Account.*` | Công khai |
| `/Reader/MyLoans` | `Reader.MyLoans` | Độc giả |
| `/Staff/Books` | `Staff.Books` | Thủ thư, Admin |
| `/Admin/Dashboard`, `POST /Admin/SavePolicy` | `Admin.Dashboard`, `Admin.SavePolicy` | Admin |

### 3.2 Bốn pattern

| Pattern | Minh họa trong mã nguồn |
|---|---|
| **MVC** | Xem bảng trên. Controller chỉ nhận request, gọi Service, đưa ViewModel cho View — không có quy tắc nghiệp vụ. |
| **Repository** | `IRepository<T>`, `IBookRepository`, `ILoanRepository`… (Core) ↔ `EfRepository<T>`, `BookRepository`… (Infrastructure). Có thêm **Unit of Work** (`IUnitOfWork`) để mỗi lần gia hạn ghi khoản mượn + nhật ký trong **một giao dịch** (NFR-04). |
| **Dependency Injection** | Mọi lớp nhận phụ thuộc qua constructor, ví dụ `LoanService(ILoanRepository, IReservationRepository, …)`. Container DI khởi tạo — không có `new Repository()` trong mã nghiệp vụ. |
| **Dependency Inversion** | Interface do **Core** định nghĩa, **Infrastructure** cài đặt. Nơi duy nhất "nối dây" là `Extensions/ServiceCollectionExtensions.cs` (`AddLumsApplication`). Hệ quả có thể chứng minh bằng `dotnet test`: `LoanServiceTests` chạy `LoanService` với Repository giả, không cần CSDL. |

Quy tắc nghiệp vụ cũng nằm trong Entity (rich domain model): `Loan.Renew()` kiểm tra quá hạn / có người đặt trước — đúng ví dụ ở Chuyên đề 3 mục 2.5.3.

### 3.3 CSDL (Chuyên đề 2)

`LumsDbContext` + `Data/Configurations/` (mỗi bảng một lớp `...Configuration`) ánh xạ 8 bảng của ERD: `TAI_KHOAN`, `DAU_SACH`, `BAN_SAO`, `GIAO_DICH_MUON_TRA`, `DAT_TRUOC`, `PHIEU_PHAT`, `CAU_HINH_QUY_DINH`, `NHAT_KY_HE_THONG`. Giá trị enum lưu dạng chuỗi trùng Từ điển dữ liệu (`DangMuon`, `SanSang`, `ThuThu`…). Mọi khóa ngoại dùng `Restrict` (không xóa dây chuyền).

### 3.4 UI/UX (Chuyên đề UI/UX + Figma)

Bố cục theo từng frame wireframe; màu `#1E40AF`, huy hiệu xanh/cam/đỏ, font Inter, thanh trên 72px. Có thêm: liên kết "Bỏ qua điều hướng", `focus-visible`, `aria-*` cho tab/phân trang, màu chữ cam đậm hơn wireframe để đạt tương phản AA, và `prefers-reduced-motion`.

### 3.5 Truy vết SRS

| Mã | Nội dung | Hiện thực |
|---|---|---|
| UC-00-01 | Đăng nhập | `AuthService`, `Account/Login` |
| UC-00-06 | Đăng xuất | `Account/Logout` (chỉ nhận POST + token CSRF) |
| UC-00-07 | RBAC, trang 403 | `[Authorize(Roles = AppRoles.Reader / Staff / Admin)]` trên Controller (`Security/AppRoles.cs`), `Account/Forbidden` |
| UC-01-01 / 02 | Tra cứu / chi tiết sách | `BookService`, `/Books`, `/api/books` |
| UC-01-03 | Xem sách đang mượn | `LoanService.GetActiveLoansAsync`, `/Reader/MyLoans` |
| UC-01-04 | Gia hạn trực tuyến | `Loan.Renew`, `LoanService.RenewAsync`, `POST /api/loans/{id}/renew` |
| UC-01-05 | Đặt trước | **Mới hiển thị** danh sách đặt trước (`ReservationService`) |
| UC-03-02 | Cấu hình quy định | `PolicyService`, form ở Dashboard (mỗi lần lưu thêm một dòng mới → giữ lịch sử) |
| UC-03-03 / 04 | Thống kê + nhật ký | `DashboardService` |
| NFR-04 | Toàn vẹn dữ liệu | `IUnitOfWork` |

---

## 4. Khác biệt và chưa làm — nên nói trước với cô

**Khác biệt so với tài liệu**
- **`NVARCHAR` thay vì `VARCHAR`:** ERD ghi `VARCHAR`, nhưng tiếng Việt có dấu cần `NVARCHAR` trong SQL Server. Nên cập nhật lại Chuyên đề 2.
- **Cookie authentication**, chưa dùng JWT: phù hợp web MVC render phía server; JWT là bước nâng cấp nếu cần cho ứng dụng di động.
- Menu nhân viên (Dashboard / Kho sách / OPAC) là phần thêm ngoài wireframe.
- `EnsureCreated` thay cho EF Migrations (chỉ để demo cho nhanh).

**Chưa làm (ngoài phạm vi "dựng layout + 1 luồng chạy thật")**
- Đăng ký (UC-00-02), quên mật khẩu (UC-00-03), khóa tài khoản sau nhiều lần sai (nhánh thay thế của UC-00-01).
- Ghi nhật ký khi truy cập trái phép (nhánh thay thế của UC-00-07); hiện chỉ chuyển sang trang 403.
- Đặt trước sách, yêu thích, lịch sử mượn, thông tin cá nhân.
- Thêm/sửa sách (UC-02-03; hộp thoại chỉ mới là giao diện), xuất Excel (UC-02-05), tạo tài khoản (UC-03-01), mượn/trả tại quầy (UC-02-01/02).
- Ảnh bìa sách thật (đang dùng ô giữ chỗ như wireframe).

**Bước tiếp theo hợp lý:** hiện thực UC-02-03 (thêm/sửa sách) — thêm `IBookService.CreateAsync` + `BookRepository`, nối vào hộp thoại có sẵn — để cô thấy một luồng ghi dữ liệu đi qua đủ 4 pattern.
