using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using CoffeePotDevice.Services;

namespace CoffeePotDevice.ViewModels
{
  public class MainPageViewModel : ViewModelBase
  {

    #region Play Sounds

    DelegateCommand<String> playSoundCommand;

    public DelegateCommand<String> PlaySoundCommand
       => playSoundCommand ?? (playSoundCommand = new DelegateCommand<String>(async (path) => await PlaySoundAsync(path), (path) => true));

    private async Task PlaySoundAsync(String Path)
    {
      await SoundService.PlayAudioFileAsync(Path);
    }

    #endregion Play Sounds

    #region Navigation Commands

    DelegateCommand<String> navigateCommand;

    public DelegateCommand<String> NavigateCommand
       => navigateCommand ?? (navigateCommand = new DelegateCommand<String>((target) => NavigateToTarget(target), (target) => true));

    private void NavigateToTarget(String Target)
    {
      try
      {
        Type targetType = Type.GetType(Target);
        NavigationService.Navigate(targetType, 0);
      }
      catch
      {
        throw;
      }
    }

    #endregion Navigation Commands
  }
}

