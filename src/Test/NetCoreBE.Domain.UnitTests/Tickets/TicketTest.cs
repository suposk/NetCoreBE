namespace NetCoreBE.Domain.UnitTests.Tickets;

public class TicketTest : BaseTest
{

    [Fact]
    public void Create_Should_Create_Init_History()
    {
        //areange
        var request = Ticket.Create(TicketData.Ticket.Id, TicketData.Ticket.TicketType, TicketData.Ticket.Notes.FirstOrDefault(), TicketData.Ticket.CreatedBy);

        //act
        //request.Create(UtcNow);

        //assert
        request.Status.Should().Be(StatusEnum.Submited.ToString());
        request.TicketHistoryList.Should().HaveCount(1);
        request.TicketHistoryList.First().Operation.Should().Be(StatusEnum.Submited.ToString());

        //request.TicketHistoryList.First().CreatedAt.Should().Be(UtcNow);
    }

    [Fact]
    public void Init_Should_Create_Init_History()
    {
        //areange
        var request = TicketData.Ticket;

        //act
        request.Init(TicketData.Ticket.Notes.FirstOrDefault(), UtcNow);

        //assert
        request.Status.Should().Be(StatusEnum.Submited.ToString());
        request.TicketHistoryList.Should().HaveCount(2);
        request.TicketHistoryList.First().Operation.Should().Be(StatusEnum.Submited.ToString());
        request.TicketHistoryList.Single(a => a.Operation.Equals(Ticket.CONST.InitStatus)).Should().NotBeNull();
        
        //request.TicketHistoryList.First().CreatedAt.Should().Be(UtcNow);
    }

}
