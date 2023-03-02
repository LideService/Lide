namespace Lide.Core.Model;

public class SignatureRequest
{
    public bool IsBase { get; set; }
    public bool IsGeneric { get; set; }
    public bool IsParameter { get; set; }
    public bool IsReturn { get; set; }
}