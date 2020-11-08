using System;
using System.Collections.Generic;

namespace DotnetExercises.Models
{
    public partial class Students
    {
        public long StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public long? Age { get; set; }
    }
}
