namespace ChatAPI.Model.DTO
{
    public class SearchUserResponseDTO
    {
        public List<UserData> SearchedUsers { get; set; }
    }
    public class UserData
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}