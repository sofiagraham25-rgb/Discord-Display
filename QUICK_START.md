# Discord Ray-Ban Companion App - Quick Start

## 🚀 5-Minute Setup

### Step 1: Get Discord Bot Token
1. Visit https://discord.com/developers/applications
2. Click **New Application** → Name it
3. Go to **Bot** section → **Add Bot**
4. Click **Copy** under TOKEN

### Step 2: Add Bot to Server
1. In Developer Portal, go **OAuth2** → **URL Generator**
2. Select `bot` scope + these permissions:
   - ✅ Read Messages/View Channels
   - ✅ Send Messages
   - ✅ Read Message History
   - ✅ View Server Members
3. Copy the generated URL and open it

### Step 3: Configure App

**Option A - Easy (Development Only)**
1. Open `appsettings.Development.json`
2. Paste your token:
```json
{
  "Discord": {
	"BotToken": "YOUR_TOKEN_HERE"
  }
}
```

**Option B - Secure**
```bash
dotnet user-secrets set "Discord:BotToken" "YOUR_TOKEN_HERE"
```

### Step 4: Run
```bash
dotnet run
```
Open https://localhost:7000

---

## 📱 Using the App

| Button | Action |
|--------|--------|
| **📢 Channels** | Browse server channels |
| **💬 DMs** | View direct messages |
| **👥 Friends** | See member list |
| **🔓 Logout** | Disconnect |

**Send Message**: Type → Press Enter

---

## 🎨 Customizing for Your Glasses

Edit `wwwroot/app.css` - Look for these variables:

```css
:root {
	--glasses-text-primary: #ffffff;      /* Text color */
	--glasses-bg-primary: #1a1a1a;        /* Main background */
	--glasses-accent: #5865F2;            /* Button/highlight color */
	--font-size-base: 12px;               /* Adjust for readability */
}
```

For different screen sizes, update media queries:
```css
@media (max-width: 480px) {
	:root {
		--font-size-base: 11px;  /* Smaller for compact displays */
	}
}
```

---

## ❌ Troubleshooting

| Issue | Fix |
|-------|-----|
| "Invalid token" | Copy token again, verify in Developer Portal |
| Messages not showing | Bot permissions need "Read Message History" |
| App won't start | Check `appsettings.json` for syntax errors |
| Slow on glasses | Reduce `--font-size-base`, check network |

---

## 📂 Key Files

| File | Purpose |
|------|---------|
| `Services/DiscordApiService.cs` | Discord API calls |
| `Components/Pages/DiscordCompanion.razor` | Main app logic |
| `wwwroot/app.css` | Glasses-optimized styling |
| `appsettings.json` | Configuration |

---

## 🔐 Security Reminder

⚠️ **NEVER**:
- Commit bot token to GitHub
- Share token in chat/email
- Use production token for testing

✅ **DO**:
- Use `appsettings.Development.json` (git-ignored)
- Or use `dotnet user-secrets`
- Regenerate token if exposed

---

## 🧪 Testing Without Real Glasses

Use browser DevTools:
1. Press **F12** to open DevTools
2. Click device icon 📱
3. Set size to **400x300px** (Ray-Ban spec)
4. Test touch interactions

---

## 📞 Need Help?

**Discord API Issues**: Check [Discord Docs](https://discord.com/developers/docs)

**App Not Working**: Check troubleshooting in full README

**Ray-Ban Integration**: See Meta Developer docs

---

**Ready?** Run `dotnet run` and start chatting! 🎉
