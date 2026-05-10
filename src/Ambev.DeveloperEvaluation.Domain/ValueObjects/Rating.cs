namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public class Rating
{
    public decimal Rate { get; private set; }
    public int Count { get; private set; }

    private Rating() { }

    public Rating(decimal rate, int count)
    {
        if (rate < 0 || rate > 5)
            throw new ArgumentOutOfRangeException(nameof(rate), "Rate must be between 0 and 5.");
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");

        Rate = rate;
        Count = count;
    }
}
