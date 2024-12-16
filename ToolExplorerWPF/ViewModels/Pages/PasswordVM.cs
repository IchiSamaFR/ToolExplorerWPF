using Microsoft.Win32;
using PasswordLibrary;
using System.Collections.ObjectModel;
using ToolExplorerWPF.ViewModels.Dialogs.Passwords;
using ToolExplorerWPF.Views.Dialogs.Passwords;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace ToolExplorerWPF.ViewModels.Pages
{
    public partial class PasswordVM : ObservableObject, INavigationAware
    {
        private bool _isInitialized = false;
        private PasswordContainer _container;
        private readonly IContentDialogService _contentDialogService;

        // Ouverture
        [ObservableProperty]
        private bool _isChoosing;
        [ObservableProperty]
        private bool _isLoading;
        [ObservableProperty]
        private bool _isLoaded;

        // Type d'ouverture
        [ObservableProperty]
        private bool _isCreation;
        [ObservableProperty]
        private string _passwordFile;

        [ObservableProperty]
        private string _password;

        // Passwords
        [ObservableProperty]
        private ObservableCollection<PasswordFolder> _passwordFolders;
        [ObservableProperty]
        private PasswordFolder _selectedPasswordFolder;

        [ObservableProperty]
        private ObservableCollection<PasswordItem> _passwordItems;
        [ObservableProperty]
        private PasswordItem _selectedPasswordItem;

        // Password Edition
        [ObservableProperty]
        private string _selectedUsername;
        [ObservableProperty]
        private string _selectedPassword;
        [ObservableProperty]
        private string _selectedNote;

        #region Initialisation
        public void OnNavigatedTo()
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        public void OnNavigatedFrom()
        {
        }

        public PasswordVM(IContentDialogService contentDialogService)
        {
            _contentDialogService = contentDialogService;
        }

        private void InitializeViewModel()
        {
            _isInitialized = true;
            IsChoosing = true;
            IsLoading = false;
            IsLoaded = false;
        }
        #endregion

        #region Connexion
        [RelayCommand]
        public void ReturnMain()
        {
            _container = null;
            Password = string.Empty;
            IsChoosing = true;
            IsLoading = false;
            IsCreation = false;
        }

        [RelayCommand]
        public void CreatePasswordFile()
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "PasswordFile",
                DefaultExt = ".pass",
                Filter = "Text documents (.pass)|*.pass"
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            Password = string.Empty;
            PasswordFile = saveFileDialog.FileName;
            IsChoosing = false;
            IsLoading = true;
            IsCreation = true;
        }

        [RelayCommand]
        public void LoadPasswordFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                FileName = "PasswordFile",
                DefaultExt = ".pass",
                Filter = "Text documents (.pass)|*.pass"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            Password = string.Empty;
            PasswordFile = openFileDialog.FileName;
            IsChoosing = false;
            IsLoading = true;
        }

        [RelayCommand]
        public void ValidatePassword()
        {
            _container = new PasswordContainer(Password);
            if (IsCreation)
            {
                _container.Folders = GetDefaultFolders();
                _container.Save(PasswordFile);
            }

            if (_container.Load(PasswordFile))
            {
                IsLoading = false;
                IsLoaded = true;
                LoadPasswordFolders(_container.Folders);
            }
            else
            {
                //Pass incorrect
            }
        }
        private List<PasswordFolder> GetDefaultFolders()
        {
            return new List<PasswordFolder>
            {
                new PasswordFolder { Name = "Default", Passwords = {
                        new PasswordItem { Username = "Admin", Password = "Admin", Note = "Default password" },
                        new PasswordItem { Username = "User", Password = "User", Note = "Default password" }
                    } },
                new PasswordFolder { Name = "Important", Passwords = {
                        new PasswordItem { Username = "ImportantAdmin", Password = "Admin", Note = "Default password" },
                        new PasswordItem { Username = "ImportantUser", Password = "User", Note = "Default password" }
                    } }
            };
        }
        #endregion

        #region Password Folder
        partial void OnSelectedPasswordFolderChanged(PasswordFolder value)
        {
            OpenPasswordFolder(value);
        }
        private void LoadPasswordFolders(List<PasswordFolder> lst)
        {
            PasswordFolders = new ObservableCollection<PasswordFolder>(_container.Folders);
        }
        [RelayCommand]
        public void OpenPasswordFolder(PasswordFolder passwordFolder)
        {
            LoadPasswordItems(passwordFolder?.Passwords ?? new List<PasswordItem>());
        }
        [RelayCommand]
        public async void AddPasswordFolder()
        {
            var content = new PasswordFolderDialog();
            var vm = new PasswordFolderDialogVM();
            content.DataContext = vm;

            var result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Add folder",
                    Content = content,
                    PrimaryButtonText = "Add",
                    CloseButtonText = "Cancel"
                }
            );

            switch (result)
            {
                case ContentDialogResult.Primary:
                    var newFolder = vm.GetPasswordFolder();
                    _container.Folders.Add(newFolder);
                    _container.Save(PasswordFile);

                    LoadPasswordFolders(_container.Folders);
                    break;
                case ContentDialogResult.Secondary:
                case ContentDialogResult.None:
                default:
                    break;
            }
        }
        [RelayCommand]
        public async void UpdatePasswordFolder(PasswordFolder passwordFolder)
        {
            if(passwordFolder == null)
            {
                return;
            }
            var content = new PasswordFolderDialog();
            var vm = new PasswordFolderDialogVM(passwordFolder);
            content.DataContext = vm;

            var result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Update folder",
                    Content = content,
                    PrimaryButtonText = "Update",
                    CloseButtonText = "Cancel",
                }
            );

            switch (result)
            {
                case ContentDialogResult.Primary:
                    var newFolder = vm.GetPasswordFolder();
                    passwordFolder.Name = newFolder.Name;
                    _container.Save(PasswordFile);

                    LoadPasswordFolders(_container.Folders);
                    break;
                case ContentDialogResult.Secondary:
                case ContentDialogResult.None:
                default:
                    break;
            }
        }
        [RelayCommand]
        public async void RemovePasswordFolder(PasswordFolder passwordFolder)
        {
            if (passwordFolder == null)
            {
                return;
            }

            var result = await _contentDialogService.ShowSimpleDialogAsync(
                new SimpleContentDialogCreateOptions()
                {
                    Title = "Update folder",
                    Content = $"Are you sure you want to delete \"{passwordFolder.Name}\" ?",
                    PrimaryButtonText = "Remove",
                    CloseButtonText = "Cancel",
                }
            );

            switch (result)
            {
                case ContentDialogResult.Primary:
                    _container.Folders.Remove(passwordFolder);
                    SelectedPasswordFolder = null;
                    LoadPasswordFolders(_container.Folders);
                    break;
                case ContentDialogResult.Secondary:
                case ContentDialogResult.None:
                default:
                    break;
            }


            LoadPasswordFolders(_container.Folders);
        }
        #endregion

        #region Password File
        partial void OnSelectedPasswordItemChanged(PasswordItem value)
        {
            OpenPasswordItem(value);
        }

        private void LoadPasswordItems(List<PasswordItem> lst)
        {
            PasswordItems = new ObservableCollection<PasswordItem>(lst);
            SelectedPasswordItem = null;
        }
         
        [RelayCommand]
        public void OpenPasswordItem(PasswordItem passwordItem)
        {
            SelectedUsername = passwordItem?.Username;
            SelectedPassword = passwordItem?.Password;
            SelectedNote = passwordItem?.Note;
        }
        [RelayCommand]
        public void AddPasswordItem(PasswordItem passwordItem)
        {

        }
        [RelayCommand]
        public void RemovePasswordItem(PasswordItem passwordItem)
        {

        }

        [RelayCommand]
        public void CopyUsernameToClipboard(PasswordItem passwordItem)
        {
            Clipboard.SetText(passwordItem.Username);
        }
        [RelayCommand]
        public void CopyPasswordToClipboard(PasswordItem passwordItem)
        {
            Clipboard.SetText(passwordItem.Password);
        }
        #endregion
    }

}