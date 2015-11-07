using System.Xml;

namespace NCoverCop
{
    public class XmlNodeHelper
    {
        public static int GetIntAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null ? int.Parse(node.Attributes[name].Value) : 0;
        }

        public static string GetStringAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null ? node.Attributes[name].Value : "";
        }

        public static bool GetBoolAttribute(string name, XmlNode node)
        {
            return node.Attributes[name] != null && node.Attributes[name].Value != "false";
        }
    }
}