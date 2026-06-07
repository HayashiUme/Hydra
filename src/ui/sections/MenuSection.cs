using System;
using UnityEngine;

namespace HydraMenu.ui.sections
{
	internal class MenuSection : ISection
	{
		public MenuSection() : base("菜单") { }

		public override void Render()
		{
			Hydra.notifications.DisableNotifications = GUILayout.Toggle(Hydra.notifications.DisableNotifications, "禁用通知");

			GUILayout.Label($"主题颜色: {Styles.primaryColor}");
			Styles.primaryColor = (Styles.UIColors)GUILayout.HorizontalSlider((float)Styles.primaryColor, 0, Styles.ColorValues.Count - 1);

			GUILayout.Label($"菜单透明度: {Styles.menuOpacity * 100:F0}%");
			Styles.menuOpacity = (float)Math.Round(GUILayout.HorizontalSlider(Styles.menuOpacity, 0, 1), 4);

			GUILayout.Label($"界面缩放: {MainUI.scale:F2}x");
			MainUI.scale = (float)Math.Round(GUILayout.HorizontalSlider(MainUI.scale, 0.5f, 2.0f), 2);

			if(GUILayout.Button("应用更改"))
			{
				Styles.ClearCache();
			}
		}
	}
}
