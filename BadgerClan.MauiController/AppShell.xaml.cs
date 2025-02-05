using BadgerClan.MauiController.Views;

namespace BadgerClan.MauiController
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("TeamPage", typeof(TeamPage));
        }
    }
}
