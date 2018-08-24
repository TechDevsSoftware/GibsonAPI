﻿namespace TechDevs.Accounts
{
    public interface IPasswordHasher
    {
        string HashPassword(AuthUser user, string password);
        bool VerifyHashedPassword(AuthUser user, string hashedPassword, string providedPassword);
    }
}
