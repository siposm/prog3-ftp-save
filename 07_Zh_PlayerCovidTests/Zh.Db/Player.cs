using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Zh.Db
{
    [Table("players")]
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FamilyName { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string Position { get; set; }

        [NotMapped]
        public virtual ICollection<CovidTest> Tests { get; set; }

        public Player()
        {
            Tests = new HashSet<CovidTest>();
        }

        public Player(XElement source) : this()
        {
            if (source != null)
            {
                FamilyName = source.Element("familyName").Value;
                FirstName = source.Element("firstName").Value;
                Position = source.Element("position").Value;
                Code = source.Attribute("code").Value;
            }
        }
        public override string ToString()
        {
            return $"{FamilyName} {FirstName}, {Position}";
        }
    }
}
