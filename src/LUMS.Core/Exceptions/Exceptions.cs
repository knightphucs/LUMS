namespace LUMS.Core.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}

/// <summary>Ném ra khi thêm đầu sách có ISBN đã tồn tại; kèm Id đầu sách cũ để UI dẫn tới trang chi tiết.</summary>
public class DuplicateIsbnException : DomainException
{
    public DuplicateIsbnException(string isbn, int existingBookId)
        : base($"ISBN {isbn} đã tồn tại (đầu sách #{existingBookId}). Hãy dùng chức năng thêm bản sao cho đầu sách này.")
    {
        Isbn = isbn;
        ExistingBookId = existingBookId;
    }

    public string Isbn { get; }
    public int ExistingBookId { get; }
}
