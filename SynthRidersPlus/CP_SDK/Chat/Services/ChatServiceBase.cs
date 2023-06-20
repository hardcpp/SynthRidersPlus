﻿using CP_SDK.Chat.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace CP_SDK.Chat.Services
{
    /// <summary>
    /// Generic implementation for IChatService
    /// </summary>
    public class ChatServiceBase
    {
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, string>>   m_OnSystemMessageCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, string>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService>>           m_OnLoginCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService>>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel>>                                        m_OnJoinRoomCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel>>                                        m_OnLeaveRoomCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel>>                                        m_OnRoomStateUpdatedCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, bool, int>>                             m_OnLiveStatusUpdatedCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, bool, int>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, Dictionary<string, IChatResourceData>>> m_OnChannelResourceDataCached
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, Dictionary<string, IChatResourceData>>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser>>                             m_OnChannelFollowCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, int>>                        m_OnChannelBitsCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, int>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, IChatChannelPointEvent>>     m_OnChannelPointsCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, IChatChannelPointEvent>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, IChatSubscriptionEvent>>     m_OnChannelSubscriptionCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, IChatSubscriptionEvent>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, int>>                        m_OnChannelRaidCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatChannel, IChatUser, int>>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatMessage>> m_OnTextMessageReceivedCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, IChatMessage>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, string>> m_OnChatClearedCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, string>>();
        protected ConcurrentDictionary<ChatServiceBase, Action<IChatService, string>> m_OnMessageClearedCallbacks
            = new ConcurrentDictionary<ChatServiceBase, Action<IChatService, string>>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public event Action<IChatService, string> OnSystemMessage
        {
            add     => m_OnSystemMessageCallbacks.AddAction(this, value);
            remove  => m_OnSystemMessageCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService> OnLogin
        {
            add     => m_OnLoginCallbacks.AddAction(this, value);
            remove  => m_OnLoginCallbacks.RemoveAction(this, value);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public event Action<IChatService, IChatChannel> OnJoinChannel
        {
            add     => m_OnJoinRoomCallbacks.AddAction(this, value);
            remove  => m_OnJoinRoomCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel> OnLeaveChannel
        {
            add     => m_OnLeaveRoomCallbacks.AddAction(this, value);
            remove  => m_OnLeaveRoomCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel> OnRoomStateUpdated
        {
            add     => m_OnRoomStateUpdatedCallbacks.AddAction(this, value);
            remove  => m_OnRoomStateUpdatedCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel, bool, int> OnLiveStatusUpdated
        {
            add     => m_OnLiveStatusUpdatedCallbacks.AddAction(this, value);
            remove  => m_OnLiveStatusUpdatedCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel, Dictionary<string, IChatResourceData>> OnChannelResourceDataCached
        {
            add     => m_OnChannelResourceDataCached.AddAction(this, value);
            remove  => m_OnChannelResourceDataCached.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel, IChatUser> OnChannelFollow
        {
            add     => m_OnChannelFollowCallbacks.AddAction(this, value);
            remove  => m_OnChannelFollowCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel, IChatUser, int> OnChannelBits
        {
            add     => m_OnChannelBitsCallbacks.AddAction(this, value);
            remove  => m_OnChannelBitsCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel, IChatUser, IChatChannelPointEvent> OnChannelPoints
        {
            add     => m_OnChannelPointsCallbacks.AddAction(this, value);
            remove  => m_OnChannelPointsCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel, IChatUser, IChatSubscriptionEvent> OnChannelSubscription
        {
            add     => m_OnChannelSubscriptionCallbacks.AddAction(this, value);
            remove  => m_OnChannelSubscriptionCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, IChatChannel, IChatUser, int> OnChannelRaid
        {
            add     => m_OnChannelRaidCallbacks.AddAction(this, value);
            remove  => m_OnChannelRaidCallbacks.RemoveAction(this, value);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public event Action<IChatService, IChatMessage> OnTextMessageReceived
        {
            add     => m_OnTextMessageReceivedCallbacks.AddAction(this, value);
            remove  => m_OnTextMessageReceivedCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, string> OnChatCleared
        {
            add     => m_OnChatClearedCallbacks.AddAction(this, value);
            remove  => m_OnChatClearedCallbacks.RemoveAction(this, value);
        }
        public event Action<IChatService, string> OnMessageCleared
        {
            add     => m_OnMessageClearedCallbacks.AddAction(this, value);
            remove  => m_OnMessageClearedCallbacks.RemoveAction(this, value);
        }
    }
}
