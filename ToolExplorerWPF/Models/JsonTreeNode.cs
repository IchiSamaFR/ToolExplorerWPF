using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolExplorerWPF.Models
{
    public class JsonTreeNode
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ObservableCollection<JsonTreeNode> Children { get; set; } = new ObservableCollection<JsonTreeNode>();
    }
}
