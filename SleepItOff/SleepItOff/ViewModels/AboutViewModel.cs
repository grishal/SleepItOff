using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace SleepItOff.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
            myBsFunc = new Command(()=>Console.WriteLine(10));
        }
        public ICommand myBsFunc { get; } 
        public ICommand OpenWebCommand { get; }
    }
}