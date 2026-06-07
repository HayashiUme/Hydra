using HydraMenu.features;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class SpooferSection : ISection
	{
		public SpooferSection() : base("伪装") { }

		public readonly Dictionary<string, int> versions = new Dictionary<string, int>()
		{
			{ $"{Constants.AddressablesVersion} (当前)", Constants.GetBroadcastVersion() },
			{ "16.1.0", 50632950 },
			{ "17.1", 50643450 },
			{ "17.1.2", 50647000 },
			{ "17.2", 50645050 },
			{ "17.2.1", 50652900 },
			{ "17.2.2", 50653700 }
		};

		private int versionSelection = 0;

		public override void Render()
		{
			Spoofer.shouldSpoofVersion = GUILayout.Toggle(Spoofer.shouldSpoofVersion, "启用版本伪装");

			GUILayout.Label($"伪装版本: {versions.ElementAt(versionSelection).Key} ({Spoofer.spoofedVersion})");
			versionSelection = (int)GUILayout.HorizontalSlider(versionSelection, 0, versions.Count - 1);
			Spoofer.spoofedVersion = versions.ElementAt(versionSelection).Value;

			Spoofer.useModdedProtocol = GUILayout.Toggle(Spoofer.useModdedProtocol, "使用 Modded 协议");

			GUILayout.Label($"伪装平台: {Spoofer.spoofedPlatform}");
			Spoofer.spoofedPlatform = (Platforms)GUILayout.HorizontalSlider((float)Spoofer.spoofedPlatform, 0, 10);
		}
	}
}
