﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Microsoft.AspNetCore.SignalR.Redis.Tests
{
    public class TestConnectionMultiplexer : IConnectionMultiplexer
    {
        public string ClientName => throw new NotImplementedException();

        public string Configuration => throw new NotImplementedException();

        public int TimeoutMilliseconds => throw new NotImplementedException();

        public long OperationCount => throw new NotImplementedException();

        public bool PreserveAsyncOrder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsConnected => true;

        public bool IncludeDetailInExceptions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int StormLogThreshold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<RedisErrorEventArgs> ErrorMessage
        {
            add { }
            remove { }
        }

        public event EventHandler<ConnectionFailedEventArgs> ConnectionFailed
        {
            add { }
            remove { }
        }

        public event EventHandler<InternalErrorEventArgs> InternalError
        {
            add { }
            remove { }
        }

        public event EventHandler<ConnectionFailedEventArgs> ConnectionRestored
        {
            add { }
            remove { }
        }

        public event EventHandler<EndPointEventArgs> ConfigurationChanged
        {
            add { }
            remove { }
        }

        public event EventHandler<EndPointEventArgs> ConfigurationChangedBroadcast
        {
            add { }
            remove { }
        }

        public event EventHandler<HashSlotMovedEventArgs> HashSlotMoved
        {
            add { }
            remove { }
        }

        private ISubscriber _subscriber = new TestSubscriber();

        public void BeginProfiling(object forContext)
        {
            throw new NotImplementedException();
        }

        public void Close(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public Task CloseAsync(bool allowCommandsToComplete = true)
        {
            throw new NotImplementedException();
        }

        public bool Configure(TextWriter log = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ConfigureAsync(TextWriter log = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ProfiledCommandEnumerable FinishProfiling(object forContext, bool allowCleanupSweep = true)
        {
            throw new NotImplementedException();
        }

        public ServerCounters GetCounters()
        {
            throw new NotImplementedException();
        }

        public IDatabase GetDatabase(int db = -1, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public EndPoint[] GetEndPoints(bool configuredOnly = false)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(string host, int port, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(string hostAndPort, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(IPAddress host, int port)
        {
            throw new NotImplementedException();
        }

        public IServer GetServer(EndPoint endpoint, object asyncState = null)
        {
            throw new NotImplementedException();
        }

        public string GetStatus()
        {
            throw new NotImplementedException();
        }

        public void GetStatus(TextWriter log)
        {
            throw new NotImplementedException();
        }

        public string GetStormLog()
        {
            throw new NotImplementedException();
        }

        public ISubscriber GetSubscriber(object asyncState = null)
        {
            return _subscriber;
        }

        public int HashSlot(RedisKey key)
        {
            throw new NotImplementedException();
        }

        public long PublishReconfigure(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<long> PublishReconfigureAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public void RegisterProfiler(IProfiler profiler)
        {
            throw new NotImplementedException();
        }

        public void ResetStormLog()
        {
            throw new NotImplementedException();
        }

        public void Wait(Task task)
        {
            throw new NotImplementedException();
        }

        public T Wait<T>(Task<T> task)
        {
            throw new NotImplementedException();
        }

        public void WaitAll(params Task[] tasks)
        {
            throw new NotImplementedException();
        }
    }

    public class TestSubscriber : ISubscriber
    {
        // _globalSubscriptions represents the Redis Server you are connected to.
        // So when publishing from a TestSubscriber you fake sending through the server by grabbing the callbacks
        // from the _globalSubscriptions and inoking them inplace.
        private static ConcurrentDictionary<RedisChannel, List<Action<RedisChannel, RedisValue>>> _globalSubscriptions =
            new ConcurrentDictionary<RedisChannel, List<Action<RedisChannel, RedisValue>>>();

        private ConcurrentDictionary<RedisChannel, Action<RedisChannel, RedisValue>> _subscriptions =
            new ConcurrentDictionary<RedisChannel, Action<RedisChannel, RedisValue>>();

        public ConnectionMultiplexer Multiplexer => throw new NotImplementedException();

        public EndPoint IdentifyEndpoint(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisChannel channel, CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected(RedisChannel channel = default)
        {
            throw new NotImplementedException();
        }

        public TimeSpan Ping(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public long Publish(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            if (_globalSubscriptions.TryGetValue(channel, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(channel, message);
                }
            }

            return handlers != null ? handlers.Count : 0;
        }

        public async Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            await Task.Yield();
            return Publish(channel, message, flags);
        }

        public void Subscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
        {
            _globalSubscriptions.AddOrUpdate(channel, _ => new List<Action<RedisChannel, RedisValue>> { handler }, (_, list) =>
            {
                list.Add(handler);
                return list;
            });
            _subscriptions.AddOrUpdate(channel, handler, (_, __) => handler);
        }

        public Task SubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler, CommandFlags flags = CommandFlags.None)
        {
            Subscribe(channel, handler, flags);
            return Task.CompletedTask;
        }

        public EndPoint SubscribedEndpoint(RedisChannel channel)
        {
            throw new NotImplementedException();
        }

        public bool TryWait(Task task)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            _subscriptions.TryRemove(channel, out var handle);
            if (_globalSubscriptions.TryGetValue(channel, out var list))
            {
                list.Remove(handle);
            }
        }

        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            throw new NotImplementedException();
        }

        public Task UnsubscribeAsync(RedisChannel channel, Action<RedisChannel, RedisValue> handler = null, CommandFlags flags = CommandFlags.None)
        {
            Unsubscribe(channel, handler, flags);
            return Task.CompletedTask;
        }

        public void Wait(Task task)
        {
            throw new NotImplementedException();
        }

        public T Wait<T>(Task<T> task)
        {
            throw new NotImplementedException();
        }

        public void WaitAll(params Task[] tasks)
        {
            throw new NotImplementedException();
        }
    }
}
