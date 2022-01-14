using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    [Table("Spoj")]
    public class Spoj
    {
        [Key]
        public int ID { get; set; }

        [Range(5,10)]
        public int Ocena { get; set;}

         public virtual IspitniRok IspitniRok { get; set; }

        [JsonIgnore]
        public virtual Student Student { get; set; }

        public virtual Predmet Predmet { get; set; }
        
    }
}