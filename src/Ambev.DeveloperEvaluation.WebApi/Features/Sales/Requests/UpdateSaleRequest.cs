namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Requests;

public class UpdateSaleRequest
{
    public DateTime Date { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
}
