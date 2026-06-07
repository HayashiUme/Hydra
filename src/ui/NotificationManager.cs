using System;
using System.Collections.Generic;
using UnityEngine;

namespace HydraMenu.ui
{
	public class NotificationManager : MonoBehaviour
	{
		public List<Notification> notifications = new List<Notification>();
		public bool DisableNotifications = false;

		// 增大通知框以容纳多行长文本
		public static Vector2 BoxSize
		{
			get { return new Vector2(330, 130) * MainUI.scale; }
		}

		public static Vector2 BoxHeaderSize
		{
			get { return new Vector2(BoxSize.x, 20 * MainUI.scale); }
		}

		public static Vector2 BoxContentPadding
		{
			get { return new Vector2(10, 5) * MainUI.scale; }
		}

		public static Vector2 BoxContentSize
		{
			get { return new Vector2(BoxSize.x - BoxContentPadding.x * 2, BoxSize.y - BoxHeaderSize.y - BoxSliderSize.y - BoxContentPadding.y * 2); }
		}

		public static Vector2 BoxSliderSize
		{
			get { return new Vector2(BoxSize.x, 16 * MainUI.scale); }
		}

		// 文字自动换行的 GUIStyle (缓存，避免每帧创建)
		private static GUIStyle _labelStyle;
		private static GUIStyle LabelStyle
		{
			get
			{
				if(_labelStyle == null)
				{
					_labelStyle = new GUIStyle(GUI.skin.label);
					_labelStyle.wordWrap = true;
					_labelStyle.alignment = TextAnchor.UpperLeft;
				}
				_labelStyle.fontSize = (int)(13 * MainUI.scale);
				return _labelStyle;
			}
		}

		private static GUIStyle _headerStyle;
		private static GUIStyle HeaderStyle
		{
			get
			{
				if(_headerStyle == null)
				{
					_headerStyle = new GUIStyle(GUI.skin.box);
					_headerStyle.alignment = TextAnchor.MiddleLeft;
					_headerStyle.fontStyle = FontStyle.Bold;
				}
				_headerStyle.fontSize = (int)(13 * MainUI.scale);
				return _headerStyle;
			}
		}

		public void Update()
		{
			int notificaions = Math.Min(GetMaxNotifications(), notifications.Count);

			for(int i = 0; i < notificaions; i++)
			{
				Notification notification = notifications[i];
				notification.lifetime += Time.deltaTime;

				if(notification.HasExpired)
				{
					notifications.RemoveAt(i);

					// 移除了一个元素，索引和上限都要递减
					i--;
					notificaions--;
					continue;
				}
			}
		}

		public void OnGUI()
		{
			if(DisableNotifications) return;

			// 确保字体大小正确，即使主菜单没打开
			GUI.skin.label.fontSize = (int)(13 * MainUI.scale);

			int notificaions = Math.Min(GetMaxNotifications(), notifications.Count);

			for(byte i = 0; i < notificaions; i++)
			{
				RenderNotification(i, notifications[i]);
			}
		}

		private void RenderNotification(byte position, Notification notification)
		{
			float boxX = Screen.width - BoxSize.x;
			float boxY = Screen.height - (int)(BoxSize.y * (position + 1));

			// 外框
			GUI.Box(new Rect(boxX, boxY, BoxSize.x, BoxSize.y), notification.title, HeaderStyle);

			// 文字区域 — 使用带 wordWrap 的样式
			float contentX = boxX + BoxContentPadding.x;
			float contentY = boxY + BoxHeaderSize.y + BoxContentPadding.y;
			GUI.Label(new Rect(contentX, contentY, BoxContentSize.x, BoxContentSize.y), notification.message, LabelStyle);

			// 进度条 — 在框的底部
			float sliderY = boxY + BoxSize.y - BoxSliderSize.y;
			float remaining = Math.Max(0, notification.ttl - notification.lifetime);
			GUI.HorizontalSlider(new Rect(boxX, sliderY, BoxSize.x, BoxSliderSize.y), remaining, 0, notification.ttl);
		}

		public int GetMaxNotifications()
		{
			return (Screen.height / 2) / (int)BoxSize.y;
		}

		public void Send(string title, string message, float ttl = 10)
		{
			Hydra.Log.LogMessage($"[Notification] [{title}] {message}");

			if(DisableNotifications) return;

			Notification notification = new Notification(title, message, ttl);
			notifications.Add(notification);
		}

		public void ClearNotifications()
		{
			notifications.Clear();
		}
	}
}
