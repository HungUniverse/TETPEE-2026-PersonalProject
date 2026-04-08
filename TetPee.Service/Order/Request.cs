namespace TetPee.Service.Order;

public class Request
{
    public class CreateOrderRequest
    {
        public string Address { get; set; }
        public List<ProductOrderRequest> Product { get; set; }
    }

    public class ProductOrderRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
    
    //Tạo đơn hàng 
    // Setup chuyển khoản thành cng bằng tiền thiệt, để xác nhận đơn hàng đã đc đặt
    // Nếu tạo đơn hàng, mà không chuyển khoản liền, thì đơn hàng sẽ bị hủy trong 15p
    // Để tránh tìn trạng khách đặt hàng rồi ko chuyển khoản, làm ảnh hưởng đến việc quản lí đơn hàng
        
    //Hủy đơn haàng
    
    //Cập nhật đơn hàng
    
    public class SepayWebhookRequest
    {
        public string Gateway { get; set; }
        public DateTime TrasactionDate { get; set; }
        public string AccountNumber { get; set; }
        public string SubAccount { get; set; }
        public string Code { get; set; }
        public string Content { get; set; }
        public string TransferType { get; set; }
        public string Description { get; set; }
        public decimal TransferAmount { get; set; }
        public string ReferenceCode { get; set; }
        public decimal Accumulated { get; set; }
        public long Id { get; set; }
    }
    
    /*
     *'{"gateway":"BIDV",
     * "transactionDate":"2026-04-06 23:41:15",
     * "accountNumber":"8886369921",
     * "subAccount":"96247BENTRAN",
     * "code":"TCMPBf9c3895c14b94583bad78673263",
     * "content":"QR - TCMPBf9c3895c14b94583bad786732631b1ca",
     * "transferType":"in",
     * "description":"BankAPINotify QR - TCMPBf9c3895c14b94583bad786732631b1ca",
     * "transferAmount":2500,
     * "referenceCode":"bc8af415-13e4-4bf9-8352-a8af59df5808",
     * "accumulated":0,
     * "id":48628369}'
     * 
     */
}