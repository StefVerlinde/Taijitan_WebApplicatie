﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Taijitan.Models.Domain;

namespace Taijitan.Models.UserViewModel
{
    public class EditViewModel
    {
        [Required(ErrorMessage = "Naam is verplicht in te vullen")]
        [Display(Name ="Naam")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Voornaam is verplicht in te vullen")]
        [Display(Name = "Voornaam")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Geboortedatum is verplicht in te vullen")]
        [Display(Name = "Geboortedatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Straatnaam is verplicht in te vullen")]
        [Display(Name = "Straatnaam")]
        public string Street { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "Postcode")]
        [Required(ErrorMessage = "Postcode is verplicht in te vullen")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Stadnaam is verplicht in te vullen")]
        [Display(Name = "Stadnaam")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Land is verplicht in te vullen")]
        [Display(Name="Land")]
        public Country Country { get; set; }

        [Required(ErrorMessage = "Huisnummer is verplicht in te vullen")]
        [Display(Name = "Huisnummer")]
        public string HouseNumber { get; set; }

        [Required(ErrorMessage = "Telefoonnummer is verplicht in te vullen")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"([+]?\d{1,2}[.\s-]?)?(\d{3}[.-]?){2}\d{4}", ErrorMessage = "Ongeldig telefoonnummer")]
        [Display(Name = "Telefoonnummer")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mailadres")]
        public string Email { get; set; }

        //---------------------------------------------------------------------------------
        [Required(ErrorMessage = "Datum van aanmelding mag niet leeg zijn")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum van aanmelding")]
        public DateTime DateRegistred { get; set; }

        [Required(ErrorMessage = "Geslacht mag niet leeg zijn")]
        [Display(Name = "Geslacht")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Nationaliteit mag niet leeg zijn")]
        [Display(Name = "Nationaliteit")]
        public Country Nationality { get; set; }
        
        [Display(Name = "Rijksregisternummer")]
        public string PersonalNationalNumber { get; set; }

        [Required(ErrorMessage = "Geboorteplaats is verplicht in te vullen")]
        [Display(Name = "Geboorteplaats")]
        public string BirthPlace { get; set; }

        [Display(Name = "Vast telefoonnummer")]
        public string LandLineNumber { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mailadres van een ouder")]
        public string MailParent { get; set; }

        [Required(ErrorMessage = "Een gebruiker moet een Rank hebben")]
        [Display(Name = "Kyu/ graad")]
        public Rank Rank { get; set; }




        public EditViewModel(User user)
        {
            Name = user.Name;
            FirstName = user.FirstName;
            DateOfBirth = user.DateOfBirth;
            Street = user.Street;
            PostalCode = user.City?.Postalcode ?? "Not Found";
            CityName = user.City?.Name ?? "Not Found";
            Country = user.Country;
            HouseNumber = user.HouseNumber;
            PhoneNumber = user.PhoneNumber;
            Email = user.Email;
            DateRegistred = user.DateRegistred;
            Gender = user.Gender;
            Nationality = user.Nationality;
            PersonalNationalNumber = user.PersonalNationalNumber;
            BirthPlace = user.BirthPlace;
            LandLineNumber = LandLineNumber;
            MailParent = MailParent;
            Rank = user.Rank;

        }

        public EditViewModel()
        {

        }
    }
}
