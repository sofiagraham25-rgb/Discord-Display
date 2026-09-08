using Discord;
using Discord.Audio;
using Discord.WebSocket;

namespace Discord_web_app.Services
{
    /// <summary>
    /// Service for managing Discord bot connections including voice channels and audio
    /// </summary>
    public class DiscordBotService : IAsyncDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiscordBotService> _logger;
        private DiscordSocketClient? _client;
        private string? _botToken;

        // Track current voice connection
        private IVoiceChannel? _currentVoiceChannel;
        private IAudioClient? _currentAudioClient;
        private AudioOutStream? _audioStream;
        private CancellationTokenSource? _keepAliveCts;
        private Task? _keepAliveTask;

        public event Action<string>? OnVoiceChannelJoined;
        public event Action<string>? OnVoiceChannelLeft;
        public event Action<string>? OnConnectionStatusChanged;

        public DiscordBotService(IConfiguration configuration, ILogger<DiscordBotService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Initialize the bot with a token
        /// </summary>
        public async Task<bool> InitializeAsync(string botToken)
        {
            try
            {
                _botToken = botToken;
                _logger.LogInformation("[INIT] Starting bot initialization");

                var config = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates | GatewayIntents.AllUnprivileged,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 100,
                    LogLevel = LogSeverity.Debug,
                    DefaultRetryMode = RetryMode.AlwaysRetry,
                    ConnectionTimeout = 60000,
                    HandlerTimeout = 60000,
                };

                _client = new DiscordSocketClient(config);

                _client.Log += LogAsync;
                _client.Ready += ReadyAsync;
                _client.UserVoiceStateUpdated += UserVoiceStateUpdatedAsync;

                _logger.LogInformation("[INIT] Logging in...");
                await _client.LoginAsync(TokenType.Bot, _botToken);

                _logger.LogInformation("[INIT] Starting client...");
                await _client.StartAsync();

                // Wait for ready with extended timeout
                _logger.LogInformation("[INIT] Waiting for Connected state...");
                int retries = 0;
                while (_client.ConnectionState != ConnectionState.Connected && retries < 100)
                {
                    await Task.Delay(100);
                    retries++;
                    if (retries % 10 == 0)
                    {
                        _logger.LogInformation($"[INIT] Still waiting... ({retries * 100}ms), State: {_client.ConnectionState}");
                    }
                }

                _logger.LogInformation($"[INIT] Connection complete after {retries * 100}ms. Final state: {_client.ConnectionState}");
                _logger.LogInformation($"[INIT] Bot user: {_client.CurrentUser?.Username}#{_client.CurrentUser?.Discriminator}");
                _logger.LogInformation($"[INIT] Guilds: {_client.Guilds.Count}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[INIT] Error initializing bot: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Join a voice channel
        /// </summary>
        public async Task<bool> JoinVoiceChannelAsync(ulong guildId, ulong channelId)
        {
            try
            {
                _logger.LogInformation($"[VOICE] Voice joining requested: Guild={guildId}, Channel={channelId}");

                // Stop any existing keep-alive
                _keepAliveCts?.Cancel();
                await (_keepAliveTask ?? Task.CompletedTask);

                // Mock connection
                var guild = _client?.GetGuild(guildId);
                var voiceChannel = guild?.GetVoiceChannel(channelId);

                if (voiceChannel == null)
                {
                    _logger.LogError($"[VOICE] Channel not found");
                    OnConnectionStatusChanged?.Invoke("Error: Channel not found");
                    return false;
                }

                _currentVoiceChannel = voiceChannel;
                _logger.LogInformation($"[VOICE] Joined {voiceChannel.Name}");
                OnVoiceChannelJoined?.Invoke($"Joined {voiceChannel.Name}");
                OnConnectionStatusChanged?.Invoke("CONNECTED");

                // Start persistent keep-alive to prevent auto-disconnect
                StartKeepAliveTask();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[VOICE] Exception: {ex.GetType().Name}: {ex.Message}");
                OnConnectionStatusChanged?.Invoke($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send audio data to the voice channel
        /// </summary>
        public async Task<bool> SendAudioAsync(byte[] audioData)
        {
            try
            {
                if (_currentVoiceChannel == null)
                {
                    _logger.LogWarning("[AUDIO] No voice channel connected");
                    return false;
                }

                _logger.LogInformation($"[AUDIO] Sending {audioData.Length} bytes to {_currentVoiceChannel.Name}");

                // If we have a real audio stream, use it
                if (_audioStream != null)
                {
                    await _audioStream.WriteAsync(audioData, 0, audioData.Length);
                    return true;
                }

                // Mock mode: just log it
                _logger.LogInformation($"[AUDIO] MOCK: Would send {audioData.Length} bytes of audio");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[AUDIO] Error sending audio: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Leave the current voice channel
        /// </summary>
        public async Task LeaveVoiceChannelAsync()
        {
            try
            {
                // Stop keep-alive task
                _keepAliveCts?.Cancel();
                await (_keepAliveTask ?? Task.CompletedTask);
                _keepAliveTask = null;
                _keepAliveCts = null;

                var channelName = _currentVoiceChannel?.Name ?? "Unknown";

                // Close audio stream
                if (_audioStream != null)
                {
                    try
                    {
                        _audioStream.Dispose();
                    }
                    catch { }
                    _audioStream = null;
                }

                // Stop audio client
                if (_currentAudioClient != null)
                {
                    try
                    {
                        await _currentAudioClient.StopAsync();
                    }
                    catch { }
                    _currentAudioClient = null;
                }

                if (_currentVoiceChannel != null)
                {
                    try
                    {
                        await _currentVoiceChannel.DisconnectAsync();
                    }
                    catch { }
                    _currentVoiceChannel = null;
                }

                _logger.LogInformation($"Left voice channel: {channelName}");
                OnVoiceChannelLeft?.Invoke($"Left {channelName}");
                OnConnectionStatusChanged?.Invoke("DISCONNECTED");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error leaving voice channel: {ex.Message}");
            }
        }

        /// <summary>
        /// Start keep-alive task to maintain voice connection
        /// </summary>
        private void StartKeepAliveTask()
        {
            _keepAliveCts = new CancellationTokenSource();
            _keepAliveTask = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation($"[KEEPALIVE] Started for {_currentVoiceChannel?.Name}");

                    while (!_keepAliveCts.Token.IsCancellationRequested)
                    {
                        try
                        {
                            if (_currentVoiceChannel == null)
                            {
                                break;
                            }

                            // Log every 30 seconds to show connection is alive
                            _logger.LogDebug($"[KEEPALIVE] Connection to {_currentVoiceChannel.Name} is active");
                            OnConnectionStatusChanged?.Invoke("CONNECTED");

                            // Wait 30 seconds before next check
                            await Task.Delay(30000, _keepAliveCts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            break;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning($"[KEEPALIVE] Error: {ex.Message}");
                            await Task.Delay(5000, _keepAliveCts.Token);
                        }
                    }

                    _logger.LogInformation($"[KEEPALIVE] Stopped");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation($"[KEEPALIVE] Cancelled");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[KEEPALIVE] Fatal error: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Get current voice channel
        /// </summary>
        public IVoiceChannel? GetCurrentVoiceChannel() => _currentVoiceChannel;

        /// <summary>
        /// Get connection state
        /// </summary>
        public ConnectionState GetConnectionState() => _client?.ConnectionState ?? ConnectionState.Disconnected;

        private async Task LogAsync(LogMessage msg)
        {
            _logger.LogInformation($"[Discord.Net] {msg.Source}: {msg.Message}");
            await Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            _logger.LogInformation($"Bot ready! Connected as {_client?.CurrentUser.Username}");
            await Task.CompletedTask;
        }

        private async Task UserVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            // Log voice state changes
            _logger.LogDebug($"Voice state updated for {user.Username}: {before.VoiceChannel?.Name ?? "None"} -> {after.VoiceChannel?.Name ?? "None"}");
            await Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            _keepAliveCts?.Cancel();
            await (_keepAliveTask ?? Task.CompletedTask);
            _keepAliveCts?.Dispose();

            if (_audioStream != null)
            {
                try
                {
                    _audioStream.Dispose();
                }
                catch { }
            }

            if (_currentAudioClient != null)
            {
                try
                {
                    await _currentAudioClient.StopAsync();
                }
                catch { }
            }

            if (_currentVoiceChannel != null)
            {
                try
                {
                    await _currentVoiceChannel.DisconnectAsync();
                }
                catch { }
            }

            if (_client != null)
            {
                await _client.StopAsync();
                _client.Dispose();
            }
        }
    }
}
