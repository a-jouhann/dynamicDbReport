namespace DynamicDbReport.DTO.Models.Public;

public class PublicActionResponse
{
    public bool SuccessAction { get; set; } = false;
    public ErrorExceptionResponse ErrorException { get; set; }
    public DateTime ResponseTime { get; set; } = DateTime.Now;
}
