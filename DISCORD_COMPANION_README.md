# Discord Ray-Ban Glasses Companion App

A Blazor-based companion app for Meta Ray-Ban display glasses that lets you access Discord features with a glass-optimized interface.

## Features

### Core Discord Features
- **Channels**: Browse and read messages from server channels
- **Direct Messages**: View and send direct messages to other users
- **Friends/Members**: See online status and presence of server members
- **Voice/Video Calls**: Indicators for active calls
- **Message Notifications**: Real-time updates for new messages

### Ray-Ban Glasses Optimized
- **Minimal UI**: Compact, focused interface for small displays
- **Touch/Voice Ready**: Simple navigation with minimal scrolling
- **Low Latency**: Fast message loading and sending
- **Accessibility**: High contrast colors, readable fonts for AR display
- **Battery Efficient**: Optimized CSS and minimal animations

## Project Structure

```
Discord web app/
├── Components/
│   ├── Pages/
│   │   ├── DiscordCompanion.razor       # Main app component
│   │   └── Home.razor
│   ├── ChannelView.razor                 # Channel browsing component
│   ├── DirectMessageView.razor           # Direct messages component
│   ├── FriendsView.razor                 # Member list/presence component
│   ├── MessageInput.razor                # Message input component
│   ├── Layout/
│   ├── Account/                          # Identity components (unchanged)
│   └── _Imports.razor
├── Services/
│   └── DiscordApiService.cs              # Discord API integration
├── Data/                                 # Entity Framework Core
├── Properties/
├── wwwroot/
│   └── app.css                           # Glasses-optimized styles
├── Program.cs
└── appsettings.json
```

## Setup Instructions

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026 or VS Code
- Discord Bot Token (see below)
- Meta Ray-Ban display glasses or browser for testing

### 1. Clone and Open Project
```bash
cd "Discord web app"
```

### 2. Create a Discord Bot

1. Go to [Discord Developer Portal](https://discord.com/developers/applications)
2. Click "New Application"
3. Name it "Ray-Ban Companion" (or similar)
4. Go to "Bot" section and click "Add Bot"
5. Under TOKEN, click "Copy" to copy your bot token
6. **Important**: Keep this token secure and never commit to version control

### 3. Configure Bot Permissions

In the Developer Portal:
1. Go to OAuth2 > URL Generator
2. Select these scopes: `bot`
3. Select these permissions:
   - `Read Messages/View Channels`
   - `Send Messages`
   - `Read Message History`
   - `View Server Members`
4. Copy the generated URL and open it to add the bot to your test server

### 4. Store Bot Token

**Option A: Local Development (Recommended)**
Create `appsettings.Development.json`:
```json
{
  "Discord": {
	"BotToken": "YOUR_BOT_TOKEN_HERE"
  }
}
```

**Option B: User Secrets (More Secure)**
```bash
dotnet user-secrets set "Discord:BotToken" "YOUR_BOT_TOKEN_HERE"
```

### 5. Build and Run
```bash
dotnet build
dotnet run
```

Navigate to `https://localhost:7000` (or your configured port)

## Usage

### Starting the App
1. Launch the application
2. Enter your Discord Bot Token when prompted
3. App will load your servers and direct messages

### Navigation
- **📢 Channels**: Browse server channels
- **💬 DMs**: View direct messages
- **👥 Friends**: See online members
- **🔓 Logout**: Disconnect and reset

### Sending Messages
1. Type message in the input field at the bottom
2. Press Enter or click Send arrow
3. Message appears in the chat

## CSS Customization

The `wwwroot/app.css` file is optimized for small displays with:

- **CSS Variables**: Easy theming via `:root`
  - `--glasses-bg-primary` / `--glasses-bg-secondary`: Dark backgrounds
  - `--glasses-text-primary` / `--glasses-text-secondary`: Text colors
  - `--glasses-accent`: Highlight color (Discord blue)
  - Font size variables for scaling to different displays

- **Classes for Components**:
  - `.glasses-container`: Main layout wrapper
  - `.glasses-header`: Status bar with user info
  - `.glasses-footer`: Navigation bar
  - `.message-list` / `.message-item`: Message display
  - `.channel-list` / `.channel-item`: Channel browsing
  - `.dm-list` / `.dm-item`: Direct message list
  - `.message-input-area`: Bottom input section

### Adapting for Different Screen Sizes
Edit the media queries at the bottom of `app.css`:
```css
@media (max-width: 480px) {
	:root {
		--font-size-base: 11px;
		--font-size-small: 9px;
		/* Adjust as needed */
	}
}
```

## API Integration (DiscordApiService.cs)

The service provides methods for:

### User & Authentication
- `SetToken(token)`: Configure bot token
- `ValidateTokenAsync()`: Check if token is valid
- `GetCurrentUserAsync()`: Get logged-in user info

### Servers & Channels
- `GetGuildsAsync()`: List all servers user is in
- `GetGuildChannelsAsync(guildId)`: Get channels for a server
- `GetPrivateChannelsAsync()`: Get direct message channels

### Messages
- `GetChannelMessagesAsync(channelId, limit)`: Fetch message history
- `SendMessageAsync(channelId, content)`: Send a message

### Members
- `GetGuildMembersAsync(guildId, limit)`: Get member list with presence

## Security Considerations

⚠️ **Important**:
- Never commit bot tokens to version control
- Use environment variables or user secrets for sensitive data
- Consider rate limiting for API calls
- Validate user input before sending to Discord
- Use HTTPS in production

## Troubleshooting

### "Bot token invalid"
- Verify token is copied correctly
- Check token hasn't been regenerated in Developer Portal
- Ensure bot is added to your test server

### Messages not loading
- Check bot has "Read Messages" and "Read Message History" permissions
- Verify channel is text-based (not voice)
- Check Discord API rate limits (max 50 requests/sec)

### App not responding on mobile/glasses
- Check viewport meta tags in App.razor
- Reduce message load limit in `GetChannelMessagesAsync`
- Test on actual device vs. browser zoom

### Styling issues on glasses display
- Adjust `--spacing-xs`, `--spacing-sm`, `--font-size-base` in app.css
- Test with browser DevTools device emulation
- Consider reducing animation complexity

## Performance Tips

1. **Limit Message History**: Fetch only last 10-20 messages
2. **Lazy Load Members**: Only fetch when "Friends" tab opened
3. **Cache Guild Data**: Consider caching server/channel list
4. **Minimize Images**: Avatars and embeds can drain battery
5. **Debounce Scroll**: Implement scroll event throttling

## Future Enhancements

- [ ] Voice/Video call indicators with click-to-join
- [ ] User avatars (scaled down for battery)
- [ ] Emoji reactions and quick reactions
- [ ] Typing indicators
- [ ] Message reactions
- [ ] Search functionality
- [ ] Push notifications
- [ ] Offline message queue
- [ ] WebSocket for real-time updates
- [ ] Custom status messages

## Environment Variables

If using environment-based configuration:

```bash
# Windows
set DISCORD_BOT_TOKEN=your_token_here

# Linux/Mac
export DISCORD_BOT_TOKEN=your_token_here
```

Then in `appsettings.json`:
```json
{
  "Discord": {
	"BotToken": "${DISCORD_BOT_TOKEN}"
  }
}
```

## Testing

For local development without actual glasses:

1. Use browser DevTools device emulation
2. Set viewport to ~400x300px (Ray-Ban glass resolution)
3. Test touch interactions on mobile browser
4. Verify performance on lower-end device throttling

## Resources

- [Discord Developer Portal](https://discord.com/developers/applications)
- [Discord API Documentation](https://discord.com/developers/docs)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor)
- [Meta Ray-Ban Smart Glasses Docs](https://developers.meta.com/docs/smart-glasses)

## License

Built with Blazor and .NET 10 for modern web experiences on wearable devices.

## Support

For issues or feature requests related to:
- **Discord API**: See [Discord Developer Support](https://discord.gg/discord-developers)
- **This App**: Check troubleshooting section above
- **Ray-Ban Integration**: See Meta Developer documentation
