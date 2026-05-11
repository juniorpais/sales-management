namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.Requests;

public class CreateCartRequest
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<CreateCartItemRequest> Items { get; set; } = [];
}

public class CreateCartItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
