// File: Ogur.Infrastructure/Events/InMemoryEventBus.cs
// Project: Ogur.Infrastructure
// Namespace: Ogur.Infrastructure.Events

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ogur.Abstractions;
using Ogur.Abstractions.Events;

namespace Ogur.Infrastructure.Events;

/// <summary>
/// In-memory event bus implementation using broadcast channels.
/// Thread-safe, supports multiple subscribers with optional filtering.
/// </summary>
public sealed class InMemoryEventBus : IEventBus
{
    private readonly ILogger<InMemoryEventBus> _logger;
    private readonly ConcurrentBag<Subscription> _subscriptions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryEventBus"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Publishes an event to all subscribers.
    /// </summary>
    /// <param name="type">Event type identifier.</param>
    /// <param name="message">Event message.</param>
    public void Publish(string type, string message)
    {
        var evt = ApplicationEvent.Info(type, message);
        
        _logger.LogTrace("Publishing event: {Type}", type);
        
        int delivered = 0;
        foreach (var sub in _subscriptions)
        {
            if (sub.Matches(type))
            {
                sub.Channel.Writer.TryWrite(evt);
                delivered++;
            }
        }
        
        _logger.LogTrace("Event {Type} delivered to {Count} subscribers", type, delivered);
    }

    /// <summary>
    /// Subscribes to events matching the optional filter.
    /// </summary>
    /// <param name="filter">Optional event type filter.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Async enumerable of events.</returns>
    public async IAsyncEnumerable<ApplicationEvent> Subscribe(
        string? filter, 
        [EnumeratorCancellation] CancellationToken ct)
    {
        var channel = Channel.CreateUnbounded<ApplicationEvent>();
        var subscription = new Subscription(filter, channel);
        
        _subscriptions.Add(subscription);
        _logger.LogDebug("New subscription registered (filter: {Filter})", filter ?? "all");
        
        try
        {
            while (await channel.Reader.WaitToReadAsync(ct))
            {
                while (channel.Reader.TryRead(out var evt))
                {
                    yield return evt;
                }
            }
        }
        finally
        {
            _logger.LogDebug("Subscription unregistered (filter: {Filter})", filter ?? "all");
        }
    }

    private sealed class Subscription
    {
        public string? Filter { get; }
        public Channel<ApplicationEvent> Channel { get; }

        public Subscription(string? filter, Channel<ApplicationEvent> channel)
        {
            Filter = filter;
            Channel = channel;
        }

        public bool Matches(string eventType)
        {
            if (Filter is null) return true;
            
            if (Filter.EndsWith(".*"))
            {
                var prefix = Filter[..^2];
                return eventType.StartsWith(prefix, StringComparison.Ordinal);
            }
            
            return string.Equals(Filter, eventType, StringComparison.Ordinal);
        }
    }
}