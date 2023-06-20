﻿using CP_SDK.Chat.Interfaces;

namespace CP_SDK.Chat.Models.Twitch
{
    public struct TwitchEmote : IChatEmote
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public string Uri { get; internal set; }
        public int StartIndex { get; internal set; }
        public int EndIndex { get; internal set; }
        public Animation.EAnimationType Animation { get; internal set; }
        public int Bits { get; internal set; }
        public string Color { get; internal set; }
    }
}
