using Hazel;
using UnityEngine;
using InnerNet;

namespace HydraMenu.routines
{
    public class MeetingSpamRoutine : IRoutine
    {
        public MeetingSpamRoutine() : base("Meeting Spam") { }

        public float interval = 2.0f;
        private float timeElapsed = 0f;
        private bool shouldCallMeeting = true;

        public override void Run()
        {
            if(PlayerControl.LocalPlayer == null) return;
            if(PlayerControl.LocalPlayer.Data == null) return;

            timeElapsed += Time.deltaTime;
            if(timeElapsed < interval) return;

            if(shouldCallMeeting)
            {
                if(MeetingHud.Instance == null && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                    Hydra.Log.LogInfo("[MeetingSpam] 发起紧急会议");
                    shouldCallMeeting = false;
                }
            }
            else
            {
                if(MeetingHud.Instance != null)
                {
                    // 强制关闭会议: 发送 CloseMeeting RPC
                    MessageWriter w = AmongUsClient.Instance.StartRpcImmediately(
                        MeetingHud.Instance.NetId, (byte)RpcCalls.CloseMeeting, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(w);
                    Hydra.Log.LogInfo("[MeetingSpam] 关闭会议");
                }
                shouldCallMeeting = true;
            }

            timeElapsed = 0f;
        }

        public override void OnEnable()
        {
            if(PlayerControl.LocalPlayer == null)
            {
                Hydra.notifications.Send("强制结束", "需要在游戏中才能使用此功能。", 10);
                Enabled = false;
                return;
            }
            shouldCallMeeting = (MeetingHud.Instance == null);
            Hydra.notifications.Send("强制结束", $"自动强制结束已启用，间隔 {interval:F1}秒", 5);
        }

        public override void OnDisconnect()
        {
            Hydra.notifications.Send("强制结束", "离开游戏，自动强制结束已禁用。", 10);
            Enabled = false;
        }
    }
}
