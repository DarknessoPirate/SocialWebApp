using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
   // Helper to fetch strings from appsettings.json
   public class CloudinarySettings
   {
      public required string CloudName { get; set; }
      public required string ApiKey { get; set; }
      public required string ApiSecret { get; set; }
   }
}