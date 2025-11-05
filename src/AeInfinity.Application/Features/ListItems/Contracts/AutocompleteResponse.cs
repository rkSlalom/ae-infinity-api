namespace AeInfinity.Application.Features.ListItems.Contracts;

public class AutocompleteResponse
{
    public List<AutocompleteSuggestionDto> Suggestions { get; set; } = new();
}

public class AutocompleteSuggestionDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int Frequency { get; set; }
}

