using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PluralsightLicense.Service
{
    public class TeamDeveloper
    {
        [Key]
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string DeveloperIds { get; set; }
       
    }
}
