namespace Hanging_Man_Game;

public partial class LandingPage : ContentPage
{
    public LandingPage()
    {
        InitializeComponent();
    }
    private void Start_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new MainPage();
    }
}