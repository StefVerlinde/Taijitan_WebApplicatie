﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taijitan.Models.Domain;

namespace Taijitan.Data.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;
        private  readonly DbSet<Session> _sessions;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
            _sessions = context.Sessions;
        }

        public void Add(Session session)
        {
            _sessions.Add(session);
        }

        public void Delete(Session session)
        {
            _sessions.Remove(session);
        }
        public IEnumerable<Session> GetAll()
        {
            return _sessions.Include(s => s.Members).Include(s => s.MembersPresent).Include(s => s.NonMembers).AsNoTracking().ToList();
        }

        public Session GetById(int id)
        {
            
            return _sessions.Include(s => s.Members).ThenInclude(m => m.City).Include(s => s.MembersPresent).ThenInclude(m => m.City).Include(s => s.TrainingDay).Include(s => s.NonMembers).SingleOrDefault(s => s.SessionId == id);
        }

        public IEnumerable<Session> GetByUser(int id)
        {
            return _sessions.Where(s => s.Members.Any(m => m.UserId == id)).ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
