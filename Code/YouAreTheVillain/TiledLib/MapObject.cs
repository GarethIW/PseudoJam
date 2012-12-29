using Microsoft.Xna.Framework;

namespace TiledLib
{
	/// <summary>
	/// An arbitrary object placed on an ObjectLayer.
	/// </summary>
	public class MapObject
	{
		/// <summary>
		/// Gets the name of the object.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the type of the object.
		/// </summary>
		public string Type { get; private set; }

		/// <summary>
		/// Gets the location of the object.
		/// </summary>
		public Rectangle Location { get; private set; }

		/// <summary>
		/// Gets a list of the object's properties.
		/// </summary>
		public PropertyCollection Properties { get; private set; }

		internal MapObject(string name, string type, Rectangle location, PropertyCollection properties)
		{
			Name = name;
			Type = type;
			Location = location;
			Properties = properties;
		}
	}
}
