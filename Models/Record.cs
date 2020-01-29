using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerApi.Models
{
    public class Record
    {
        [Key]
        public int Id { get; set; }

        public string UploaderName { get; set; }

        public string Description { get; set; }
    }
}
