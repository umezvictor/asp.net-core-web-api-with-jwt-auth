using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerApi.Models
{
    public class User
    {
        //users table
        //we can do validation here, but note
        //passwordhash and salt are not created by user
        //so, validation  is done is userforregisterdto

        [Key]
        public int Id { get; set; }

        public string Username { get; set; }

        public byte[] PasswordHash { get; set; }
         
        public byte[] PasswordSalt { get; set; }
    }
}
