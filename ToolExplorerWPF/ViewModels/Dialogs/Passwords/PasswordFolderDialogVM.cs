using PasswordLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.ViewModels.Dialogs.Passwords
{
    public partial class PasswordFolderDialogVM : ObservableObject
    {
        private PasswordFolder _folder;

        [ObservableProperty]
        private string _selectedName;

        public PasswordFolderDialogVM(PasswordFolder folder = null)
        {
            if(folder != null)
            {
                _folder = folder;
                SelectedName = _folder.Name;
            }
        }

        public PasswordFolder GetPasswordFolder()
        {
            return new PasswordFolder()
            {
                Name = SelectedName
            };
        }
    }
}
