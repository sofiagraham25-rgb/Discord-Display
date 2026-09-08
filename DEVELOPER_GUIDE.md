# Discord Ray-Ban Companion App - Developer Guide

## Architecture Overview

### Project Structure

```
Discord web app/
├── 📁 Components/
│   ├── Pages/
│   │   ├── DiscordCompanion.razor         [Main app - auth, view routing]
│   │   ├── Settings.razor                 [User preferences]
│   │   ├── Home.razor                     [Landing page]
│   │   └── Error.razor                    [Error boundary]
│   ├── ChannelView.razor                  [Channel/message browsing]
│   ├── DirectMessageView.razor            [DM interface]
│   ├── FriendsView.razor                  [Member list/presence]
│   ├── MessageInput.razor                 [Message input box]
│   ├── CallIndicator.razor                [Active call display]
│   ├── Layout/
│   │   ├── MainLayout.razor               [App shell]
│   │   └── ReconnectModal.razor           [Reconnect UI]
│   ├── Account/                           [ASP.NET Identity components]
│   ├── App.razor                          [Router & error boundary]
│   ├── Routes.razor                       [Route configuration]
│   └── _Imports.razor                     [Global imports]
├── 📁 Services/
│   └── DiscordApiService.cs               [Discord API wrapper]
├── 📁 Data/
│   ├── ApplicationDbContext.cs            [EF Core DbContext]
│   ├── ApplicationUser.cs                 [User identity model]
│   └── Migrations/                        [DB migrations]
├── 📁 Properties/
│   ├── launchSettings.json                [Debug profiles]
│   └── serviceDependencies.json           [Azure services config]
├── 📁 wwwroot/
│   └── app.css                            [Glasses-optimized styles]
├── Program.cs                             [App startup & DI setup]
├── Discord web app.csproj                 [Project file]
├── appsettings.json                       [Configuration]
├── appsettings.Development.json           [Dev-specific config]
└── appsettings.RayBan.json                [Ray-Ban specific config]
```

---

## Core Components Explained

### 1. DiscordCompanion.razor (Main App)

**Purpose**: Main application logic, authentication, and view routing

**Key Features**:
- Token authentication flow
- View state management (channels/dms/friends)
- Guild and channel loading
- Message sending/receiving

**State Variables**:
```csharp
_token              // Discord bot token
CurrentView         // Active view: "channels" | "dms" | "friends"
Guilds              // List of servers
SelectedGuild       // Currently selected server
CurrentMessages     // Messages in active channel
PrivateChannels     // Direct message channels
```

**Key Methods**:
```csharp
AuthenticateAsync()      // Validates token and loads initial data
LoadGuildsAsync()        // Fetches user's servers
SelectGuildAsync()       // Loads channels for a guild
SelectChannelAsync()     // Loads messages for a channel
SendMessageAsync()       // Posts message to Discord
SetViewAsync()           // Switches between views
```

### 2. DiscordApiService.cs (API Integration)

**Purpose**: Handles all Discord API calls via HttpClient

**Architecture**:
- Singleton service pattern
- Dependency-injected HttpClient
- Bearer token authentication
- Error handling and null-coalescing

**Key Endpoints**:

| Method | Discord Endpoint | Purpose |
|--------|------------------|---------|
| `GetCurrentUserAsync()` | `GET /users/@me` | Get bot/user info |
| `GetGuildsAsync()` | `GET /users/@me/guilds` | List servers |
| `GetGuildChannelsAsync()` | `GET /guilds/{id}/channels` | Get server channels |
| `GetChannelMessagesAsync()` | `GET /channels/{id}/messages` | Fetch message history |
| `SendMessageAsync()` | `POST /channels/{id}/messages` | Send message |
| `GetPrivateChannelsAsync()` | `GET /users/@me/channels` | List DMs |
| `GetGuildMembersAsync()` | `GET /guilds/{id}/members` | Get member list |

**Usage Pattern**:
```csharp
DiscordApi.SetToken(botToken);           // Authenticate
var guilds = await DiscordApi.GetGuildsAsync();  // Fetch data
```

### 3. View Components

#### ChannelView.razor
- Displays guild list (horizontal)
- Shows text channels for selected guild
- Renders message history
- Includes message input

#### DirectMessageView.razor
- Lists active DM channels
- Shows conversation with selected user
- Handles message display
- Integrated message input

#### FriendsView.razor
- Member list with status indicators
- Shows online/idle/DND/offline status
- Refresh button to reload members
- Limited to 50 members for performance

#### MessageInput.razor
- Single-line text input
- Enter to send
- Button to trigger send
- Clears after sending

---

## Styling System

### CSS Architecture

**3-Layer Approach**:

1. **Variables** (`:root`) - Define all colors, sizes
2. **Layout** - Grid/Flex structure for glasses
3. **Components** - Specific styles for buttons, messages, etc.

### Key CSS Variables

```css
:root {
	/* Colors - Discord Theme */
	--discord-primary: #5865F2;         /* Button highlight */
	--discord-online: #43B581;          /* Online status */
	--discord-dnd: #F04747;             /* Do Not Disturb */

	/* Glasses Theme */
	--glasses-bg-primary: #1a1a1a;      /* Main background */
	--glasses-bg-secondary: #2a2a2a;    /* Cards, panels */
	--glasses-text-primary: #ffffff;    /* Main text */
	--glasses-text-secondary: #b0b0b0;  /* Muted text */
	--glasses-accent: #5865F2;          /* Highlights */

	/* Sizing - Adjust for screen size */
	--font-size-base: 12px;
	--font-size-small: 10px;
	--font-size-tiny: 9px;
	--spacing-xs: 4px;
	--spacing-sm: 8px;
	--spacing-md: 12px;
	--spacing-lg: 16px;
}
```

### Responsive Breakpoints

```css
/* Mobile/Glasses - < 480px */
@media (max-width: 480px) {
	:root {
		--font-size-base: 11px;
		--spacing-xs: 3px;
	}
}

/* Tablet - 480px to 768px */
@media (min-width: 480px) and (max-width: 768px) {
	/* Scale up slightly */
}

/* Desktop - > 768px */
@media (min-width: 768px) {
	/* Standard sizing */
}
```

### Component Classes

**Layout**:
- `.glasses-container` - Main wrapper
- `.glasses-header` - Status bar (32px)
- `.glasses-content` - Scrollable content area
- `.glasses-footer` - Navigation bar (40px)

**Messages**:
- `.message-list` - Container for messages
- `.message-item` - Individual message
- `.message-header` - Author name + timestamp
- `.message-content` - Message text

**Navigation**:
- `.nav-button` - Bottom nav buttons
- `.channel-item` / `.dm-item` - Selectable items
- `.channel-badge` - Unread count indicator

---

## Async Data Flow

### Message Loading Flow

```
User clicks channel
	   ↓
SelectChannelAsync()
	   ↓
DiscordApiService.GetChannelMessagesAsync()
	   ↓
API: GET /channels/{id}/messages
	   ↓
Parse JsonElement array
	   ↓
Reverse array (oldest first)
	   ↓
Set CurrentMessages state
	   ↓
UI re-renders ChannelView
```

### Message Sending Flow

```
User types + presses Enter
	   ↓
SendMessageAsync()
	   ↓
DiscordApiService.SendMessageAsync()
	   ↓
API: POST /channels/{id}/messages {content}
	   ↓
On success → SelectChannelAsync() [refresh]
	   ↓
New messages appear in view
```

---

## Configuration

### appsettings.json

```json
{
  "Discord": {
	"BotToken": "",                  // Set in Development.json or secrets
	"ApiBase": "https://discord.com/api/v10"
  },
  "ConnectionStrings": {
	"DefaultConnection": "Server=...;Database=discord_companion;..."
  },
  "RayBan": {
	"ScreenWidth": 400,              // Typical Ray-Ban width
	"ScreenHeight": 300,             // Typical height
	"MaxMessagesPerLoad": 20,        // API efficiency
	"RefreshIntervalMs": 5000,       // Polling interval
	"EnableVoiceIndicators": true
  }
}
```

### Secrets Management

**Development** (local only):
```json
/* appsettings.Development.json (git-ignored) */
{
  "Discord": {
	"BotToken": "YOUR_TOKEN_HERE"
  }
}
```

**Production** (environment variable):
```bash
export DISCORD__BOTTOKEN="YOUR_TOKEN_HERE"
```

---

## Adding New Features

### Example: Adding Emoji Reactions

**1. Extend DiscordApiService**:
```csharp
public async Task<bool> AddReactionAsync(string channelId, string messageId, string emoji)
{
	var url = $"{DiscordApiBase}/channels/{channelId}/messages/{messageId}/reactions/{emoji}/@me";
	var response = await _httpClient.PutAsync(url, null);
	return response.IsSuccessStatusCode;
}
```

**2. Create ReactionPicker.razor**:
```razor
<div class="reaction-picker">
	@foreach (var emoji in new[] { "👍", "❤️", "😂", "🎉" })
	{
		<button @onclick="@(() => OnReact.InvokeAsync(emoji))">@emoji</button>
	}
</div>

@code {
	[Parameter]
	public EventCallback<string> OnReact { get; set; }
}
```

**3. Update MessageItem in views**:
```razor
<MessageInput ... />
<ReactionPicker OnReact="@((emoji) => AddReactionAsync(messageId, emoji))" />
```

### Example: Adding User Search

**1. Service method**:
```csharp
public async Task<List<JsonElement>> SearchMembersAsync(string guildId, string query, int limit = 10)
{
	var response = await _httpClient.GetAsync(
		$"{DiscordApiBase}/guilds/{guildId}/members/search?query={query}&limit={limit}");
	// Parse and return...
}
```

**2. Component**:
```razor
<input type="search" @bind="_searchTerm" @oninput="SearchAsync" placeholder="Find member..." />
<div class="search-results">
	@foreach (var member in SearchResults)
	{
		<div @onclick="@(() => SelectMember(member))">
			@member.GetProperty("user").GetProperty("username").GetString()
		</div>
	}
</div>
```

---

## Performance Optimization

### 1. Message Loading

**Problem**: Loading 100 messages = slow on glasses

**Solution**:
```csharp
// Limit to recent messages
const int DefaultLimit = 20;
var messages = await DiscordApi.GetChannelMessagesAsync(channelId, limit: 20);

// Implement lazy loading for "load more"
var older = await DiscordApi.GetChannelMessagesAsync(channelId, 
	before: oldestMessageId, limit: 20);
```

### 2. Memory Usage

**Problem**: JsonElement objects consume memory

**Solution**:
```csharp
// Clone only needed fields
public class MessageDto 
{
	public string Author { get; set; }
	public string Content { get; set; }
	public DateTime Timestamp { get; set; }
}

// Convert JsonElement to DTO
var dto = new MessageDto 
{
	Author = jsonMsg.GetProperty("author").GetProperty("username").GetString(),
	Content = jsonMsg.GetProperty("content").GetString(),
	Timestamp = DateTime.Parse(jsonMsg.GetProperty("timestamp").GetString())
};
```

### 3. Rendering Performance

**Problem**: Re-rendering all messages on update

**Solution**: Use `@key` directive
```razor
@foreach (var msg in Messages)
{
	<Message @key="msg.Id" Message="msg" />
}
```

### 4. Network Bandwidth

**Problem**: Images/avatars slow down on glasses

**Solution**: Disable avatars by default
```csharp
// In appsettings.RayBan.json
"ShowAvatars": false
```

---

## Debugging Tips

### Enable Discord API Logging

Add to Program.cs:
```csharp
builder.Services.AddLogging(builder => 
	builder.AddDebug().SetMinimumLevel(LogLevel.Debug));
```

### Inspect JSON Responses

```csharp
var response = await _httpClient.GetAsync(url);
var json = await response.Content.ReadAsStringAsync();
Console.WriteLine(json);  // Log to debug output
```

### Breakpoint in Async Methods

```csharp
private async Task SelectChannelAsync(JsonElement channel)
{
	IsLoading = true;
	// Breakpoint here works, but async context matters
	SelectedChannel = channel;
	var messages = await DiscordApi.GetChannelMessagesAsync(...);
	// Breakpoint here after await
	StateHasChanged();  // Force re-render
}
```

### Test Locally on Different Viewports

```bash
# Chrome DevTools Device Emulation
# 1. F12 → Device Toggle (Ctrl+Shift+M)
# 2. Set to custom 400x300
# 3. Test touch interactions

# Or use responsively.app for side-by-side testing
```

---

## Testing Strategy

### Unit Testing Services

```csharp
[TestClass]
public class DiscordApiServiceTests
{
	[TestMethod]
	public async Task GetGuildsAsync_WithValidToken_ReturnsGuilds()
	{
		// Arrange
		var mockHttp = new HttpClientMock();
		var service = new DiscordApiService(mockHttp, config);

		// Act
		var guilds = await service.GetGuildsAsync();

		// Assert
		Assert.IsNotNull(guilds);
		Assert.IsTrue(guilds.Count > 0);
	}
}
```

### Component Testing

```csharp
[TestClass]
public class ChannelViewTests
{
	[TestMethod]
	public async Task OnGuildSelect_LoadsChannels()
	{
		// Use bunit or TestKit
		// Mock DiscordApiService
		// Verify component state updates
	}
}
```

---

## Deployment

### To Azure App Service

```bash
# Build
dotnet build --configuration Release

# Publish
dotnet publish -c Release -o ./publish

# Deploy to Azure
az webapp deployment source config-zip \
	--resource-group myRG \
	--name myApp \
	--src publish.zip
```

### To Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10 AS build
WORKDIR /src
COPY ["Discord web app.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Discord web app.dll"]
```

---

## Future Roadmap

- [ ] WebSocket for real-time updates (replace polling)
- [ ] Offline message queue
- [ ] Encryption for stored tokens
- [ ] Custom themes/color schemes
- [ ] Shortcuts/quick actions
- [ ] Voice message playback
- [ ] Server audio transcription
- [ ] AR overlay (Meta Glasses specific)
- [ ] Gesture controls
- [ ] Voice commands

---

## Resources

- **Discord API**: https://discord.com/developers/docs
- **Blazor Docs**: https://learn.microsoft.com/aspnet/core/blazor
- **ASP.NET Core**: https://learn.microsoft.com/aspnet/core
- **Entity Framework Core**: https://learn.microsoft.com/ef/core

---

**Last Updated**: 2024
**Version**: 1.0.0
**Target**: .NET 10 + Blazor Server
