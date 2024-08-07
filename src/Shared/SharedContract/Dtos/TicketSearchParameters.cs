namespace SharedContract.Dtos
{
    /// <summary>
    /// Inherts from SearchParameters
    /// </summary>
    public sealed class TicketSearchParameters : SearchParameters
    {
        /// <summary>
        /// oldtiket paramter. TODO delete
        /// </summary>
        public string? Description { get; set; }

        public string? TicketType { get; set; }
        public string? Status { get; set; }
        public string? Note { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedAt { get; set; }
        //public override string OrderBy { get; set; } = nameof(TicketDto.Id);
    }
}
