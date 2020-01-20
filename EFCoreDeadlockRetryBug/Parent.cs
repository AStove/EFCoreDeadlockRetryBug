using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFCoreDeadlockRetryBug
{
    public class Parent
    {
        [Key]
        public int Id { get; set; }
        public List<Child> Children { get; set; }
        public string HairColor { get; set; }
    }
}
