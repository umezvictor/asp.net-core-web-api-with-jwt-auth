using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerApi.Dtos
{
    //no validation here, api will handle that
    public class UserForLoginDto
    {
        
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
