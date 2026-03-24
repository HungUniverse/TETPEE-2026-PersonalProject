namespace TetPee.Service.User;

public class Response
{
    public class GetUserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? ImageUrl { get; set; } = null;
        public string? PhoneNumber { get; set; }  = null;
        public string? Address { get; set; } =  null;
        public string Role { get; set; } 
        
    }
    
   
}