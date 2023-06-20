﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CP_SDK.Chat.Interfaces
{
    /// <summary>
    /// Chat service interface
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// The display name of the service(s)
        /// </summary>
        string DisplayName { get; }
        /// <summary>
        /// Side handle of each message color
        /// </summary>
        Color AccentColor { get; }

        /// <summary>
        /// Channels
        /// </summary>
        ReadOnlyCollection<(IChatService, IChatChannel)> Channels { get; }

        /// <summary>
        /// On system message
        /// </summary>
        event Action<IChatService, string> OnSystemMessage;
        /// <summary>
        /// Callback that occurs when a successful login to the provided streaming service occurs
        /// </summary>
        event Action<IChatService> OnLogin;

        /// <summary>
        /// Callback that occurs when the user joins a chat channel
        /// </summary>
        event Action<IChatService, IChatChannel> OnJoinChannel;
        /// <summary>
        /// Callback that occurs when the user leaves a chat channel
        /// </summary>
        event Action<IChatService, IChatChannel> OnLeaveChannel;
        /// <summary>
        /// Callback that occurs when a chat channel receives updated info
        /// </summary>
        event Action<IChatService, IChatChannel> OnRoomStateUpdated;
        /// <summary>
        /// Callback that occurs when a chat channel receives VideoPlayback info
        /// </summary>
        event Action<IChatService, IChatChannel, bool, int> OnLiveStatusUpdated;
        /// <summary>
        /// Fired once all known resources have been cached for this channel
        /// </summary>
        event Action<IChatService, IChatChannel, Dictionary<string, IChatResourceData>> OnChannelResourceDataCached;
        /// <summary>
        /// Callback that occurs when a follow event is received
        /// </summary>
        event Action<IChatService, IChatChannel, IChatUser> OnChannelFollow;
        /// <summary>
        /// Callback that occurs when a bits event is received
        /// </summary>
        event Action<IChatService, IChatChannel, IChatUser, int> OnChannelBits;
        /// <summary>
        /// Callback that occurs when a points event is received
        /// </summary>
        event Action<IChatService, IChatChannel, IChatUser, IChatChannelPointEvent> OnChannelPoints;
        /// <summary>
        /// Callback that occurs when a subscription event is received
        /// </summary>
        event Action<IChatService, IChatChannel, IChatUser, IChatSubscriptionEvent> OnChannelSubscription;
        /// <summary>
        /// Callback that occurs when a raid event is received
        /// </summary>
        event Action<IChatService, IChatChannel, IChatUser, int> OnChannelRaid;

        /// <summary>
        /// Callback that occurs when a text message is received
        /// </summary>
        event Action<IChatService, IChatMessage> OnTextMessageReceived;
        /// <summary>
        /// Callback that occurs when a users chat is cleared. If null, that means the entire chat was cleared; otherwise the argument is a user id.
        /// </summary>
        event Action<IChatService, string> OnChatCleared;
        /// <summary>
        /// Callback that occurs when a specific chat message is cleared. Argument is the message id to be cleared.
        /// </summary>
        event Action<IChatService, string> OnMessageCleared;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Start the service
        /// </summary>
        void Start();
        /// <summary>
        /// Stop the service
        /// </summary>
        void Stop();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Recache emotes
        /// </summary>
        void RecacheEmotes();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Web page HTML content
        /// </summary>
        /// <returns></returns>
        string WebPageHTMLForm();
        /// <summary>
        /// Web page HTML content
        /// </summary>
        /// <returns></returns>
        string WebPageHTML();
        /// <summary>
        /// Web page javascript content
        /// </summary>
        /// <returns></returns>
        string WebPageJS();
        /// <summary>
        /// Web page javascript content
        /// </summary>
        /// <returns></returns>
        string WebPageJSValidate();
        /// <summary>
        /// On web page post data
        /// </summary>
        /// <param name="p_PostData">Post data</param>
        void WebPageOnPost(Dictionary<string, string> p_PostData);

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sends a text message to the specified IChatChannel
        /// </summary>
        /// <param name="p_Channel">The chat channel to send the message to</param>
        /// <param name="p_Message">The text message to be sent</param>
        void SendTextMessage(IChatChannel p_Channel, string p_Message);

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Is connected
        /// </summary>
        /// <returns></returns>
        bool IsConnectedAndLive();
        /// <summary>
        /// Get primary channel name
        /// </summary>
        /// <returns></returns>
        string PrimaryChannelName();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Join temp channel with group identifier
        /// </summary>
        /// <param name="p_GroupIdentifier">Group identifier</param>
        /// <param name="p_ChannelName">Name of the channel</param>
        /// <param name="p_Prefix">Messages prefix</param>
        /// <param name="p_CanSendMessage">Can send message</param>
        void JoinTempChannel(string p_GroupIdentifier, string p_ChannelName, string p_Prefix, bool p_CanSendMessage);
        /// <summary>
        /// Leave temp channel
        /// </summary>
        /// <param name="p_ChannelName">Name of the channel</param>
        void LeaveTempChannel(string p_ChannelName);
        /// <summary>
        /// Is in temp channel
        /// </summary>
        /// <param name="p_ChannelName">Channel name</param>
        /// <returns></returns>
        bool IsInTempChannel(string p_ChannelName);
        /// <summary>
        /// Leave all temp channel by group identifier
        /// </summary>
        /// <param name="p_GroupIdentifier"></param>
        void LeaveAllTempChannel(string p_GroupIdentifier);
    }
}
