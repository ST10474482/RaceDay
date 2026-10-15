using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Tests
{
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _session = new();

        public bool IsAvailable => true;

        public string Id => "TestSessionId";

        public IEnumerable<string> Keys => _session.Keys;

        public void Clear()
        {
            _session.Clear();
        }

        public Task CommitAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _session[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            if (_session.TryGetValue(key, out var storedValue))
            {
                value = storedValue;
                return true;
            }

            value = null!;
            return false;
        }
    }
}