﻿using NetCoreBE.Api.Infrastructure.Persistence;

namespace CSRO.Server.Services
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task Seed(int count = 10);
    }

    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly IRepository<Ticket> _repository;
        private ApiDbContext _context;

        public TicketRepository(IRepository<Ticket> repository, ApiDbContext context, IApiIdentity apiIdentity, IDateTimeService dateTimeService) : base(context, apiIdentity, dateTimeService)
        {
            _repository = repository;            
            _context = context;            
        }

        public override Task<List<Ticket>> GetList()
        {
            //return base.GetList();
            //var exist = await _repository.GetFilter(a => a.VersionFull == version);
            //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            //return _repository.GetListFilter(a => a.IsDeleted != true);
            return _repository.GetList();
        }

        public Task Seed(int count = 10)
        {
            var list = new List<Ticket>();
            for (int i = 1; i <= count; i++)
            {
                var ticket = new Ticket
                {
                    //Id = Guid.Empty.ToString(),
                    Description = $"Description {i}",
                    RequestedFor = $"RequestedFor {i}",
                    IsOnBehalf = i % 2 == 0,
                    CreatedBy = "Seed",
                };
                //ticket.Id = ticket.Id.Remove(0, 1);
                //ticket.Id = ticket.Id.Insert(0, i.ToString());
                list.Add(ticket);
            }
            _repository.AddRange(list);
            return _repository.SaveChangesAsync();
        }   
    }
}
