namespace Saa_Project_BackEnd.ResponseContracts;

public class ContactResponse
{
    public long ContactId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public DateTime AddedOn { get; set; }
}