namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<GetCartItemResult> Items { get; set; } = [];
}

public class GetCartItemResult
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
