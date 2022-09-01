using System.ComponentModel.DataAnnotations;

namespace MetaFrm.Management.Razor.Models
{
    /// <summary>
    /// PermissionsModel
    /// </summary>
    public class PermissionsModel
    {
        /// <summary>
        /// RESPONSIBILITY_ID
        /// </summary>
        public decimal? RESPONSIBILITY_ID { get; set; }

        /// <summary>
        /// NAME
        /// </summary>
        [Required]
        [Display(Name = "Permissions")]
        public string? NAME { get; set; }

        /// <summary>
        /// INACTIVE_DATE
        /// </summary>
        [Display(Name = "Inactive date")]
        public DateTime? INACTIVE_DATE { get; set; } = DateTime.Now.AddYears(100);
    }
}