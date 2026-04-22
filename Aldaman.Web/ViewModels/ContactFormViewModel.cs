namespace Aldaman.Web.ViewModels;

public class ContactFormViewModel
{
    public string Name { get; set; } = string.Empty;
    public string EmailOrPhone { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
