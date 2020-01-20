using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EFCoreDeadlockRetryBug
{
    public class Child
    {
        [Key]
        public int Id { get; set; }
        public Parent Parent { get; set; }
    }
}
