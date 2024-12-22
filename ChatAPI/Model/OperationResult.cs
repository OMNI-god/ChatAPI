namespace ChatAPI.Model
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? UserId { get; set; }
    }
}
