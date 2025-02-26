namespace API.DTOs
{
   public class PhotoDTO
   {
      public int Id { get; set; }
      public required string Url { get; set; }
      public bool IsMain { get; set; }
      public bool IsBackground { get; set; }


   }
}