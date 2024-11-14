using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ToolExplorerWPF.Models
{
    public class JsonTreeModel
    {

        public static ObservableCollection<JsonTreeNode> LoadFromJson(string json)
        {
            var treeNodes = new ObservableCollection<JsonTreeNode>();
            var jsonNode = JsonNode.Parse(json);
            if (jsonNode != null)
            {
                ParseJsonNode(jsonNode, treeNodes);
            }
            return treeNodes;
        }

        private static void ParseJsonNode(JsonNode node, ObservableCollection<JsonTreeNode> treeNodes, string nodeName = null)
        {
            if (node is JsonObject jsonObject)
            {
                var newNode = new JsonTreeNode { Name = nodeName ?? "object" };
                foreach (var property in jsonObject)
                {
                    ParseJsonNode(property.Value, newNode.Children, property.Key);
                }
                treeNodes.Add(newNode);
            }
            else if (node is JsonArray jsonArray)
            {
                var arrayNode = new JsonTreeNode { Name = nodeName ?? "array" };
                foreach (var item in jsonArray)
                {
                    ParseJsonNode(item, arrayNode.Children);
                }
                treeNodes.Add(arrayNode);
            }
            else if (node is JsonValue jsonValue)
            {
                treeNodes.Add(new JsonTreeNode { Name = nodeName, Value = jsonValue.ToString() });
            }
        }
    }
}
