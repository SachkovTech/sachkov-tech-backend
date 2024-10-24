﻿using CSharpFunctionalExtensions;
using NotificationService.HelperClasses;
using System.Text.RegularExpressions;

namespace NotificationService.Entities
{
    public class Email : ValueObject
    {
        private const string REGEX = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
        public string value { get; }

        private Email(string email)
        {
            value = email;
        }

        public static Result<Email, Error> Create(string email)
        {
            Regex regex = new Regex(REGEX);

            if (regex.IsMatch(email) == false)
                return Error.Validation($"Specified email address is invalid! : {email}");

            return new Email(email);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return value;
        }

    }
}
