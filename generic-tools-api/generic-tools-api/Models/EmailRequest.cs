namespace GenericToolsAPI.Models
{
    public class EmailRequest
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<string> Emails { get; set; }
    }
}
