﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Taijitan.Filters;
using Taijitan.Models.Domain;
using Taijitan.Models.ViewModels;
using System.Web;

namespace Taijitan.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(SessionFilter))]
    [ServiceFilter(typeof(UserFilter))]
    public class SessionController : Controller
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITrainingDayRepository _trainingDayRepository;
        private readonly IFormulaRepository _formulaRepository;
        private readonly INonMemberRepository _nonMemberRepository;
        private readonly IScoreRepository _scoreRepository;

        public SessionController(IUserRepository userRepository, ISessionRepository sessionRepository,
            IFormulaRepository formulaRepository, ITrainingDayRepository trainingDayRepository, INonMemberRepository nonMemberRepository, IScoreRepository scoreRepository)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _trainingDayRepository = trainingDayRepository;
            _formulaRepository = formulaRepository;
            _nonMemberRepository = nonMemberRepository;
            _scoreRepository = scoreRepository;
        }
        [Authorize(Policy = "Teacher")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Formulas"] = GetSelectListFormulas();
            return View(new SessionViewModel());
        }
        [Authorize(Policy = "Teacher")]
        [HttpPost]
        public IActionResult Create(Session session,SessionViewModel svm,User user = null)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<Formula> formulasOfDay = _formulaRepository.GetByTrainingDay(_trainingDayRepository.getById(svm.TrainingDayId));
                IList<Member> members = new List<Member>();
                foreach (var formula in formulasOfDay)
                {
                    foreach (var member in _userRepository.GetByFormula(formula))
                        members.Add(member);
                }
                IEnumerable<Member> membersSession = new List<Member>(members);
                Teacher t = (Teacher)_userRepository.GetByEmail(user.Email);
                session.PutFormulas(formulasOfDay.ToList());
                session.Teacher = t;
                session.Members = membersSession;
                session.TrainingDay = _trainingDayRepository.getById(svm.TrainingDayId);
                if (session.SessionId == svm.SessionId)
                {
                    _sessionRepository.Add(session);
                    _sessionRepository.SaveChanges();
                }
                svm.Change(session);
                return View("Register", svm);
            }
            return RedirectToAction(nameof(Create));
        }
        [HttpGet]
        [Authorize(Policy = "Teacher")]
        public IActionResult Register(Session session, User user = null)
        {
            Session sessionFromR = _sessionRepository.GetById(session.SessionId);
            SessionViewModel svm = new SessionViewModel(sessionFromR);
            svm.SessionTeacher = (Teacher)_userRepository.GetByEmail(user.Email);
            svm.TrainingDay = sessionFromR.TrainingDay;
            return View(svm);
        }
        [Authorize(Policy = "Teacher")]
        [HttpPost]
        public IActionResult AddToPresent(int sessionId, int id, User user)
        {
            if (user == null)
                return NotFound();
            Session CurrentSession = _sessionRepository.GetById(sessionId);
            if (CurrentSession == null)
                return NotFound();
            Member m = (Member)_userRepository.GetById(id);
            if (m == null)
                return NotFound();
            CurrentSession.AddToMembersPresent(m);

            if (m.Formula.Name.Contains("en"))
            {
                m.Scores.Add(new Score { Amount = 5, MemberId = m.UserId, Name = "Aanwezigheid 1/2", Type = "Aanwezigheid" });
            }
            else
            {
                m.Scores.Add(new Score { Amount = 10, MemberId = m.UserId, Name = "Aanwezigheid", Type= "Aanwezigheid"});
            }
            _userRepository.SaveChanges();
            SessionViewModel svm = new SessionViewModel(CurrentSession);
            svm.SessionTeacher = (Teacher)_userRepository.GetByEmail(user.Email);
            svm.TrainingDay = CurrentSession.TrainingDay;
            _sessionRepository.SaveChanges();
            return RedirectToAction("Register", new { id = sessionId });
        }
        [Authorize(Policy = "Teacher")]
        [HttpPost]
        public IActionResult AddToUnconfirmed(int sessionId, int id, User user)
        {
            if (user == null)
                return NotFound();
            Session CurrentSession = _sessionRepository.GetById(sessionId);
            if (CurrentSession == null)
                return NotFound();
            Member m = (Member)_userRepository.GetById(id);
            CurrentSession.AddToMembers(m);
            if (m.Formula.Name.Contains("en"))
            {
                _scoreRepository.Delete(_scoreRepository.GetByAmount(5, m.UserId));
            }
            else
            {
                _scoreRepository.Delete(_scoreRepository.GetByAmount(10, m.UserId));
            }
            _userRepository.SaveChanges();
            SessionViewModel svm = new SessionViewModel(CurrentSession);
            svm.SessionTeacher = (Teacher)_userRepository.GetByEmail(user.Email);
            svm.TrainingDay = CurrentSession.TrainingDay;
            _sessionRepository.SaveChanges();
            return RedirectToAction("Register", new { id = sessionId });
        }
        [HttpPost]
        [Authorize(Policy = "Teacher")]
        public IActionResult AddOtherMember(int id,User user, string searchTerm = "")
        {
            if (user == null)
                return NotFound();
            var session = _sessionRepository.GetById(id);
            if (session == null)
                return NotFound();
            var sessionMembers = session.Members;
            IEnumerable<Member> otherMembers = _userRepository.GetAllMembers();

            foreach (Member m in session.MembersPresent)
            {
                List<Member> hList = sessionMembers.ToList();
                hList.Add(m);
                sessionMembers = hList;
            }
            foreach (Member m in sessionMembers)
            {
                otherMembers = otherMembers.Where(om => om.UserId != m.UserId);
            }

            if (searchTerm != null && !searchTerm.Equals(""))
            {
                otherMembers = otherMembers.Where(u => u.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            SessionViewModel svm = new SessionViewModel(session);
            svm.SessionTeacher = (Teacher)_userRepository.GetByEmail(user.Email);
            svm.TrainingDay = session.TrainingDay;
            ViewData["otherMembers"] = otherMembers.ToList();
            return View(svm);
        }
        [Authorize(Policy = "Teacher")]
        [HttpPost]
        public IActionResult AddNonMember(string firstName, string lastName, string email, int id)
        {
            Session s = _sessionRepository.GetById(id);
            if (s == null)
                return NotFound();
            var nonMember = new NonMember(firstName, lastName, email, id);
            s.AddNonMember(nonMember);
            _sessionRepository.SaveChanges();
            return RedirectToAction("Register", new { id });
        }
        [Authorize(Policy = "Teacher")]
        [HttpPost]
        public IActionResult RemoveNonMember(string firstName, int id)
        {
            Session s = _sessionRepository.GetById(id);
            if (s == null)
                return NotFound();
            NonMember nonMember = s.NonMembers.FirstOrDefault(nm => nm.FirstName.Equals(firstName));
            if (nonMember == null)
                return NotFound();
            s.RemoveNonMember(nonMember);
            _sessionRepository.SaveChanges();
            return RedirectToAction("Register", new { id });
        }
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IActionResult GetSessions()
        {
            ViewData["Sessions"] = _sessionRepository.GetAll();
            return View("SelectSession");
        }
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IActionResult SummaryPresences(int id)
        {
            Session s = _sessionRepository.GetById(id);
            if (s == null)
                return NotFound();
            ViewData["NonMembers"] = s.NonMembers;
            return View(s.MembersPresent);
        }
        private SelectList GetSelectListFormulas()
        {
            return new SelectList(_trainingDayRepository.GetAll().Select(t => new { t.TrainingDayId, t.Name })
                .ToList(), "TrainingDayId", "Name");
        }
    }
}