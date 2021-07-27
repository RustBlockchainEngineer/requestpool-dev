using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foundation.Core
{
    public static class Validation
    {
        public const string Empty = @"(?=^$)|";
        public const string Required = @"(?=^.+$)";
        public const string Code = @"(?=^.{0,150}$|^$)";
        public const string ReferenceNumber = @"(?=^.{0,150}$|^$)";
        public const string RevistionNumber = @"(?=^.{0,150}$|^$)";

        public const string SingleName = @"(?=^[A-Za-z\u0621-\u064A0-9 ]{0,50}$|^$)";
        public const string FullName = @"(?=^[A-Za-z\u0621-\u064A0-9 ]{0,75}$|^$)";
        public const string Address = @"(?=^.{5,255}$|^$)";        
        public const string Title = @"(?=^.{0,150}$|^$)";
        public const string Brief = @"(?=^.{0,250}$|^$)";
        public const string Description = @"(?=^.{0,500}$|^$)";
        public const string Article = @"(?=^.{1,}$|^$)";
        public const string Notes = @"(?=^.{0,2000}$|^$)";
        public const string Lookup = @"(?=^.{0,75}$|^$)";
        public const string ShortText = @"(?=^.{0,50}$|^$)";
        public const string StandardText = @"(?=^.{0,125}$|^$)";
        public const string LongText = @"(?=^.{0,250}$|^$)";
        public const string PostCode = @"(?=^.{3,10}$|^$)";
        public const string Latitude = @"(?=^-?([1-8]?[1-9]|[1-9]0)\.{1}\d{1,20}$)|^$";
        public const string Longitude = @"(?=^-?([1-8]?[1-9]|[1-9]0)\.{1}\d{1,20}$)|^$";
        public const string MapCoordinates = @"(?=^-?([1-8]?[1-9]|[1-9]0)\.{1}\d{1,20},-?([1-8]?[1-9]|[1-9]0)\.{1}\d{1,20}$|^$)";
        public const string Digits = @"(?=^[0-9]*$|^$)";
        public const string Number = @"(?=^\-?[0-9]*\.?[0-9]+$|^$)";
        public const string Phone = @"(?=^[\+]?[0-9]{3,15}$|^$)";
        public const string Email = @"(?=^[a-zA-Z0-9]+([_\-\.][a-zA-Z0-9]+)*@[a-zA-Z0-9]{1,63}([\-\.][a-zA-Z0-9]{1,63})*\.[a-zA-Z]{2,63}$|^$)";
        public const string Url = @"(?=^[a-zA-Z0-9]{1,63}([\-\.][a-zA-Z0-9]{1,63})*\.[a-zA-Z]{2,63}$|^$)";
        public const string IsoDate = @"(?=^(\d{4})\D?(0[1-9]|1[0-2])\D?([12]\d|0[1-9]|3[01])$|^$)";
        public const string IsoTime = @"(?=^([01]\d|2[0-3])\D?([0-5]\d)\D?([0-5]\d)?\D?(\d{3})?Z$|^$)";
        public const string IsoDateTime = @"(?=^(\d{4})\D?(0[1-9]|1[0-2])\D?([12]\d|0[1-9]|3[01])(T([01]\d|2[0-3])\D?([0-5]\d)\D?([0-5]\d)?\D?(\d{3})?Z)?$|^$)";

        public const string Username = @"(?=^.{5,50}$)(?=^[a-zA-Z0-9]+([_\-\.][a-zA-Z0-9]+)*(@[a-zA-Z0-9]{1,63}([\-\.][a-zA-Z0-9]{1,63})*\.[a-zA-Z]{2,63})?$|^$)";
        public const string AdminUsername = @"(?=^.{5,50}$)(?=^[a-zA-Z0-9]+([_\-\.][a-zA-Z0-9]+)*(@[a-zA-Z0-9]{1,63}([\-\.][a-zA-Z0-9]{1,63})*\.[a-zA-Z]{2,63})?$|^$)";
        public const string Password = @"(?=^.{5,15}$|^$)";
        public const string AdminPassword = @"(?=^.{5,15}$|^$)";
        public const string NationalId = @"(?=^[0-9]{14}$|^$)";
        public const string PassportNumber = @"(?=^[a-zA-Z0-9]{3,20}$|^$)";

    }
}