using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
   public class Group
   {
      [Key]
      public required string Name { get; set; }
      public ICollection<Connection> Connections { get; set; } = [];
   }
}