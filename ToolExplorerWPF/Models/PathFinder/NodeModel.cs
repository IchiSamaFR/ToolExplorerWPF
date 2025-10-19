using AstarLibrary;

namespace ToolExplorerWPF.Models.PathFinder
{
    public partial class NodeModel : ObservableObject
    {
        public Node Node { get; set; }
        public bool IsEndNode
        {
            get
            {
                return Node.IsEndNode;
            }
        }
        public bool IsStartNode
        {
            get
            {
                return Node.IsStartNode;
            }
        }
        public bool PathFound
        {
            get
            {
                return Node.PathFound;
            }
        }

        public bool IsWall
        {
            get
            {
                return Node.IsWall;
            }
            set
            {
                Node.IsWall = value;
            }
        }
        public bool IsChecked
        {
            get
            {
                return Node.IsChecked;
            }
        }

        public Node Previous
        {
            get
            {
                return Node.Previous;
            }
        }
        public Position Pos
        {
            get
            {
                return Node.Pos;
            }
        }
        public Position EndNodePos
        {
            get
            {
                return Node.Pos;
            }
        }

        public float Gcost
        {
            get
            {
                return Node.Gcost;
            }
        }
        public float Hcost
        {
            get
            {
                return Node.Hcost;
            }
        }
        public float Fcost
        {
            get
            {
                return Node.Fcost;
            }
        }

        public void Notify()
        {
            OnPropertyChanged(string.Empty);
        }
    }
}
