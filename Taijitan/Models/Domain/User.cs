﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taijitan.Models.Domain
{
    [JsonObject(MemberSerialization.OptIn)]
    public class User
    {
        [JsonProperty]
        public int UserId { get; set; }
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Street { get; set; }
        public City City { get; set; }
        public Country Country { get; set; }
        public string HouseNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Rank Rank { get; set; }
        public ICollection<Score> Scores { get; set; }



        public DateTime DateRegistred { get; set; }
        public Gender Gender { get; set; }
        public Country Nationality { get; set; }
        public string PersonalNationalNumber { get; set; }//rijksregisternummer
        public string BirthPlace { get; set; }
        public string LandlineNumber { get; set; }//vaste telefoonlijn --> niet verplicht
        public string MailParent { get; set; }//--> niet verplicht

        public IEnumerable<Comment> Comments { get; set; }

        #region Methods
        public virtual void Change(string name, string firstName, string street, City city, Country country, string houseNumber, string phoneNumber, Gender gender, Country nationality, string birthPlace, Rank rank, string landlineNumber = "Niet gekend", string mailParent = "niet gekend" ) {
            Name = name;
            FirstName = firstName;
            DateOfBirth = DateOfBirth;
            Street = street;
            City = city;
            Country = country;
            HouseNumber = houseNumber;
            PhoneNumber = phoneNumber;
            Gender = gender;
            Nationality = nationality;
            BirthPlace = birthPlace;
            LandlineNumber = landlineNumber;
            MailParent = mailParent;
            Rank = rank;
        }
        
        public string GetRole()
        {
            return GetType().ToString().Split(".")[3];
        }
        public bool IsRole(string type)
        {
            return GetRole().Equals(type, StringComparison.CurrentCultureIgnoreCase);
        } 
        #endregion
    }
}
