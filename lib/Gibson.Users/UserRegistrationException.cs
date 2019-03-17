using System;
using System.Collections.Generic;
using Gibson.Common.Models;

namespace Gibson.Users
{
    public class UserRegistrationException : Exception
    {
        public List<string> RegistrationErrors { get; set; }

        public UserRegistrationException(UserRegistration userRegistration)
        {
            UserRegistration = userRegistration;
        }
        public UserRegistrationException(UserRegistration userRegistration, List<string> errors, string message) : base(message)
        {
            UserRegistration = userRegistration;
            RegistrationErrors = errors;
        }
        public UserRegistrationException(UserRegistration userRegistration, List<string> errors, string message, Exception inner) : base(message, inner)
        {
            UserRegistration = userRegistration;
            RegistrationErrors = errors;
        }

        public UserRegistration UserRegistration { get; }
    }
}