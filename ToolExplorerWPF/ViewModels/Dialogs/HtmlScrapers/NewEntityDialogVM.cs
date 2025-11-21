using HtmlScraperLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.ViewModels.Dialogs.HtmlScrapers
{
    public partial class NewEntityDialogVM : ObservableObject
    {
        [ObservableProperty]
        private AEntity _selectedEntityType;

        [ObservableProperty]
        private List<AEntity> _entityTypes = new List<AEntity>()
        {
            new SelectEntity(),
            new LoopEntity(),
            new AttributeEntity(),
            new TextEntity()
        };

        public NewEntityDialogVM()
        {
            SelectedEntityType = EntityTypes.First();
        }
    }
}
