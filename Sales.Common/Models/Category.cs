using Newtonsoft.Json;
using Sales.Common.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string Description { get; set; }

    [Display(Name = "Image")]
    public string ImagePath { get; set; }

    //para que ste campo no sea tenido en cuenta en JSON 
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; }

    public string ImageFullPath
    {
        get
        {
            if (string.IsNullOrEmpty(this.ImagePath))
            {
                return "noproduct";
            }

            return $"https://salesbackend.azurewebsites.net{this.ImagePath.Substring(1)}";
        }
    }
}
