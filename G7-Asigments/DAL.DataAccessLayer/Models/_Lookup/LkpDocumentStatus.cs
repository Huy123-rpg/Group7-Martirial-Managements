namespace DAL.DataAccessLayer.Models._Lookup;

public class LkpDocumentStatus
{
    public byte StatusId { get; set; }
    public string StatusCode { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public string? DocumentType { get; set; }
}
