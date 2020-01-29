using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerApi.Dtos
{
    //this class maps the User model to objects
    //it will be used in the register api
    //to represent incomking json
    //validation can be added here
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "you must specify password between 4 and 8 characters")]
        public string Password { get; set; }
    }
}
