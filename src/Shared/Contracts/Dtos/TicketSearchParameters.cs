namespace Contracts.Dtos
{
    /// <summary>
    /// Inherts from SearchParameters
    /// </summary>
    public sealed class TicketSearchParameters : SearchParameters
    {
        public string? Description { get; set; }
        //public override string OrderBy { get; set; } = nameof(TicketDto.Id);
    }
}
