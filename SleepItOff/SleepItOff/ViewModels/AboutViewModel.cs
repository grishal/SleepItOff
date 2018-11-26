using System;
using System.Windows.Input;
using Xamarin.Forms;
using SleepItOff.Models;


namespace SleepItOff.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            myBsFunc = new Command(()=>Console.WriteLine(10));
            // TODO check 3 next lines
            AuthPage aut = new AuthPage();
            EventArgs e=new EventArgs();
            signInCommand = new Command(async() => {
                await aut.SigninButton_ClickAsync(this, e);
                Console.WriteLine(aut.response_code);
                });
        }
        public ICommand myBsFunc { get; } 
        public ICommand OpenWebCommand { get; }
        public ICommand signInCommand { get; }
    }
}