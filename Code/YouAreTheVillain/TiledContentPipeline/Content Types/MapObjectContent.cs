using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace TiledContentPipeline
{
	public class MapObjectContent
	{
		public string Name = string.Empty;
		public string Type = string.Empty;
		public Rectangle Location;
		public List<Property> Properties = new List<Property>();

		public MapObjectContent(XmlNode node)
		{
			if (node.Attributes["name"] != null)
			{
				Name = node.Attributes["name"].Value;
			}

			if (node.Attributes["type"] != null)
			{
				Type = node.Attributes["type"].Value;
			}

			Location = new Rectangle(
				int.Parse(node.Attributes["x"].Value),
				int.Parse(node.Attributes["y"].Value),
				int.Parse(node.Attributes["width"].Value),
				int.Parse(node.Attributes["height"].Value));

			XmlNode propertiesNode = node["properties"];
			if (propertiesNode != null)
			{
				foreach (XmlNode property in propertiesNode.ChildNodes)
				{
					Properties.Add(new Property
					{
						Name = property.Attributes["name"].Value,
						Value = property.Attributes["value"].Value,
					});
				}
			}
		}
	}
}
