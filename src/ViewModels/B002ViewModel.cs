using MetaFrm.Management.Razor.Models;
using MetaFrm.MVVM;

namespace MetaFrm.Management.Razor.ViewModels
{
    /// <summary>
    /// C001ViewModel
    /// </summary>
    public partial class B002ViewModel : BaseViewModel
    {
        /// <summary>
        /// SearchModel
        /// </summary>
        public SearchModel SearchModel { get; set; } = new();

        /// <summary>
        /// SelectResultModel
        /// </summary>
        public List<PermissionsModel> SelectResultModel { get; set; } = new List<PermissionsModel>();

        /// <summary>
        /// C001ViewModel
        /// </summary>
        public B002ViewModel()
        {

        }
    }
}