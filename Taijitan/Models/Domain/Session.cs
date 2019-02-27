﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taijitan.Models.Domain
{
    public class Session
    {
        public  int SessionId { get; set; }
        public IEnumerable<Member> Members { get; set; }
        public DateTime Date { get; set; }
        public Formula Formula { get; set; }
        public Teacher Teacher { get; set; }

        public Session(Formula formula,Teacher teacher,IEnumerable<Member> members)
        {
            Members = members;
            Date = DateTime.Now;
            Formula = Formula;
            Teacher = teacher;
        }
        public Session()
        {

        }
    }
}