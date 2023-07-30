using System.Security.Cryptography.X509Certificates;

namespace Store.Models.Books.Request;

public class SaveBookRequest
{
    public Guid? BookId { get; set; }

    public string BookName { get; set; }

    public Guid AuthId { get; set; }

    public string ISBN { get; set; }

    public DateTime PublicationYear { get; set; }
}
