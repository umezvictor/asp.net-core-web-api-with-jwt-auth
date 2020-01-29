using FileManagerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerApi.Data
{
    //this is part of repository pattern implementation
    //lists the methods to be implemented
    //the implementation will be outside this file
    //in any file tha implements this interface
    public interface IAuthRepository
    {
        //method for registering a new user
        Task<User> Register(User user, string password);

        //method to login user
        Task<User> Login(string username, string password);

        //method to check if user exists or not
        Task<bool> UserExists(string username);
    }
}
