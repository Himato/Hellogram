using HelloGram.Core.Models;

namespace HelloGram.Core.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }

        public SettingsViewModel() : this(null)
        {
            
        }

        public SettingsViewModel(ApplicationUser user) : base(NavIndices.Settings, user, true)
        {
        }
    }
}