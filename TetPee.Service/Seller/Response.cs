namespace TetPee.Service.Seller;

public class Response
{
    public class GetSellerResponse
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public string Taxcode { get; set; }
        public string CompanyName { get; set; }
    }
    
    
    public class GetSellerByIdResponse
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public string PhoneNumber { get; set; }
        public string Taxcode { get; set; }
        public string? CompanyName { get; set; } = null;
        public string? CompanyAddress { get; set; } = null;
        public string? Address { get; set; } =  null;
    }
}