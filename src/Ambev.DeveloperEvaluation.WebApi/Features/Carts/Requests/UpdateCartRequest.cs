namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.Requests;

public class UpdateCartRequest
{
    public DateTime Date { get; set; }
    public List<UpdateCartItemRequest> Items { get; set; } = [];
}

public class UpdateCartItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
