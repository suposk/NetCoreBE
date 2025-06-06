﻿using SharedCommon;
using System.Text;

namespace NetCoreBE.Domain.Entities;

public enum StatusEnum
{
    None,
    Draft, 
    Submited,
    Cancelled,
    InProgress,
    WaitingForInfo,
    Closed
}

public enum TicketTypeEnum
{
    None,
    Access,
    Problem,
    Question,
}


public class Ticket : EntityBase
{
    public static class CONST
    {
        public const string InitStatus = $"Init{nameof(Entities.StatusEnum.Submited)}";
    }


    public static Ticket EmptyTicket = Create("1", nameof(TicketTypeEnum.None), "", null);

    private Ticket() { }

    private Ticket(string? id,
                   string? ticketType,
                   string? note,
                   string? status,
                   string? createdBy,
                   List<TicketHistory>? ticketHistoryList)
    {
        Id = id ?? StringHelper.GetStringGuidExt();
        TicketType = ticketType;
        //cousing issues, ctro exeption
        if (string.IsNullOrWhiteSpace(ticketType))
            throw new ArgumentException("ticketType must be provided");
        Notes ??= new();
        if (note.IsNotNullOrEmptyExt())
            Notes.Add(note);
        Status = status;
        CreatedBy = createdBy;
        TicketHistoryList = ticketHistoryList ?? new();
    }

    [MaxLength(50)]
    public string? TicketType { get; private set; }

    //[MaxLength(500)]
    //public string? Note { get; set; }
    public List<string> Notes { get; private set; } = new();

    [MaxLength(50)]
    public string? Status { get; private set; }

    public StatusEnum? StatusEnum
    {
        get
        {
            if (string.IsNullOrEmpty(Status))
                return null;
            _ = Enum.TryParse<StatusEnum>(Status, out var status);
            return status;
        }
    }

    public List<TicketHistory> TicketHistoryList { get; set; } = new();

    public static Ticket Create(string? id, string? ticketType, string? note, string? createdBy)
    {
        //var ticket = new Ticket(id, ticketType, note, status: StatusTicketType.Submited.ToString(), createdBy, null);
        if (ticketType.IsNullOrEmptyExt())
            throw new ArgumentException("ticketType must be provided");
        var ticket = new Ticket() { Id = id, TicketType = ticketType, Status = Entities.StatusEnum.Submited.ToString(), CreatedBy = createdBy, TicketHistoryList = new() };
        if (note.IsNotNullOrEmptyExt())
            ticket.Notes.Add(note!);
        ticket.AddHistory(ticket.Status, null, DateTime.UtcNow);
        return ticket;
    }

    public ResultCom Init(string? note, DateTime utc)
    {
        if (CanAddUpdate(Status) is false) //exidently can be called multiple times
            return ResultCom.Failure($"Can not add history. Current status is {Status}");

        Id ??= StringHelper.GetStringGuidExt();
        if (StatusEnum is not Entities.StatusEnum.Submited || StatusEnum is not Entities.StatusEnum.Draft)
            Status = Entities.StatusEnum.Submited.ToString();

        if (note.IsNotNullOrEmptyExt())
            Notes.Add(note!);
        AddHistory(CONST.InitStatus, nameof(Init), utc);
        CreatedAt = utc;
        return ResultCom.Success();
    }

    public ResultCom Update(string? status, string? note, DateTime utc)
    {
        if (status is null && note is null)
            return ResultCom.Failure($"Can not add history. Current status is {Status}");

        if (CanAddUpdate(status) is false)
            return ResultCom.Failure("Can not update");

        StringBuilder sb = new();
        if (status.IsNotNullOrEmptyExt() && Status != status)
        {
            sb.Append($"Status changed to {status}");
            Status = status;
        }
        if (note.NotNullOrEmptyExt())
            sb.Append($"Note added: {note}");

        if (sb.Length == 0)
            return ResultCom.Failure("Nothing to update");

        //Note += "\n" + note; //add note
        if (note.IsNotNullOrEmptyExt())
        {
            Notes ??= new();
            Notes.Add(note!);            
        }
        ModifiedAt = utc;
        //status changes added
        var operation = string.Equals(Status, status) ? $"{Status} Update" : "Update";
        AddHistory(operation, sb.ToString(), utc);
        return ResultCom.Success();
    }

    private void AddHistory(string operation, string? details, DateTime utc)
    {
        var ticketHistory = TicketHistory.Create(ticketId: Id, operation, CreatedBy, details, utc);
        TicketHistoryList.Add(ticketHistory);
    }

    public static bool CanAddUpdate(string? status) => status is null || status != nameof(Entities.StatusEnum.Closed);
}
