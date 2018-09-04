
namespace Sales.Common.Models
{

using System;
using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        //longitud del producto
        [StringLength(50)]
       
        public string Description { get; set; }
        //para multilinea
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }
        //formato para el precio
        [DisplayFormat(DataFormatString ="{0:C2}",ApplyFormatInEditMode =false)]
        public Decimal Price { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Publish On")]
        //para mostrar el dtpicket
        [DataType(DataType.Date)]
        public DateTime PublishOn { get; set; }

        //NotMapped le dice al Entity framework que no lo mapee a la BD
        [NotMapped]
        public byte[] ImageArray { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.ImagePath))
                {
                    return "noproduct";
                }

                //funciona para el movil si se toma o se agrega pero para la parte web no
                return $"https://salesapimectoys.azurewebsites.net/{this.ImagePath.Substring(1)}";

                //si se restablece https://salesbackendmectoys.azurewebsites.net/ si funciona para la parte
                //web pero cuando se toma foto del mobil no se va a ver
            }

        }
        public override string ToString()
        {
            return this.Description; 
        }
    }
}
