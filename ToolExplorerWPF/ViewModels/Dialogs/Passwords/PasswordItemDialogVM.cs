using PasswordLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.ViewModels.Dialogs.Passwords
{
    public partial class PasswordItemDialogVM : ObservableObject
    {
        private PasswordItem _item;

        [ObservableProperty]
        private string _selectedUsername;
        [ObservableProperty]
        private string _selectedPassword;
        [ObservableProperty]
        private string _selectedNote;

        public PasswordItemDialogVM(PasswordItem item = null)
        {
            if (item != null)
            {
                _item = item;
                SelectedUsername = _item.Username;
                SelectedPassword = _item.Password;
                SelectedNote = _item.Note;
            }
        }

        public PasswordItem GetPasswordItem()
        {
            return new PasswordItem()
            {
                Username = SelectedUsername,
                Password = SelectedPassword,
                Note = SelectedNote,
            };
        }
    }
}
