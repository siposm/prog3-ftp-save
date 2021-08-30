using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketShop.Db
{
    [Table("venues")]
    public class Venue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [NotMapped]
        public virtual ICollection<Sector> Sectors { get; set; }

        [NotMapped]
        public virtual ICollection<Seller> Sellers { get; set; }

        public Venue()
        {
            Sectors = new HashSet<Sector>();
            Sellers = new HashSet<Seller>();
        }
    }
}
