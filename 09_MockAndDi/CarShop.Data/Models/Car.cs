using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CarShop.Data
{
    [Table("cars")]
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string Model { get; set; }

        public int? BasePrice { get; set; }

        [NotMapped]
        public virtual Brand Brand { get; set; }

        public int BrandId { get; set; }

        public override string ToString()
        {
            return $"#{Id}: {Model} of Brand #{BrandId}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Car)
            {
                Car other = obj as Car;
                return this.Id == other.Id && 
                    this.Model == other.Model && 
                    this.BasePrice == other.BasePrice && 
                    this.BrandId == other.BrandId;
            } return false;
        }
        public override int GetHashCode()
        {
            return Id;
        }
    }
}
