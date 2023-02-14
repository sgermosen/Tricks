using sysmed.Controls;

namespace sysmed.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        public DashboardViewModel()
        {
            AppShell.Current.FlyoutHeader = new FlyoutHeaderControl();
        }
    }
}
