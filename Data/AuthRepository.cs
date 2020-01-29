using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileManagerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FileManagerApi.Data
{
    //this class implements IAuthRepository interface --- means it should be added to startup.cs, so other files can use it
    //this file will be responsible for querying our db via efcore
    //uses hmacsha512 algorithm to generate hash and salt
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;

        //inject the DataContext inside the constructor
        public AuthRepository(DataContext context)
        {
            _context = context;
        } 
        public async Task<User> Login(string username, string password)
        {
            //find user first
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            //check if user was found
            if (user == null)
                return null;

            //verify password
            // returns true or false
            //pass in user typed password -password - 
            //since we are using the user repo, we have access to passwordhas and passwordsalt
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null; //return 401 in controller

            //if successful -- return the user
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            //use the password as the key
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                //this computes the hash from the password
                //will also use the key -passwordSalt - passed inside hmacsha512
                //the resultant hash should be same as the computed one when registering
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                //since the computedhash is a byte array we need to loop over it
                //and compare with each element in the stored hash

                for(int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;

                }

            }

            //if password matches return true
            return true; //the login function wraps up and returns user

        }

        //takes in the user model/entity and password string
        public async Task<User> Register(User user, string password)
        {
            //the password will be saved as a hash
            byte[] passwordHash, passwordSalt; //line **
            //passwordHash and passwordSalt are passed as reference to the one above, not value- to the 
            //CreatePassword method
            //when they are updated in the createPasswordhas method, it will also 
            //be updated inside line ** where it is declared
            CreatePasswordHash(password, out passwordHash, out  passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            //return user
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //hash password
            //when you look at the class definition for hmacsha512
            //the inheritance chain goes down to idisposable
            //and has the dispose method
            //to ensure it is called after this class is done, wrap the var hmac statement
            //inside the 'using' statement.
            //this ensures an instance of the class is created


            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                //anything here will be disposed as soon as we are finished with this class

                //set password salt
                passwordSalt = hmac.Key; //generates a random key
                //set password hash as byte array
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        //checks if user exists in database
        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;
            //return false if user not found
            return false;
        }
    }
}
