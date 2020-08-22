namespace Cuku.City
{
	using Cuku.ScriptableObject;
	using Sirenix.OdinInspector;

    public class EnvironmentConfig : SerializedScriptableObject
    {
		[PropertySpace(20), Title("City"), Required, InlineEditor]
		public StringSO CityName;

		[PropertySpace, InlineEditor, Required]
		[InfoBox("Minimum and maximum terrain heights.", InfoMessageType.None)]
		public Vector2SO TerrainHeightRange;
	}
}
