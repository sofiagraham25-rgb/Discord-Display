# 🎯 GET YOUR DISCORD BOT TOKEN - STEP BY STEP

Since the app is now running and fixed, here's exactly how to set it up to work with Discord:

---

## 📋 Step 1: Visit Discord Developer Portal

1. Open your browser
2. Go to: **https://discord.com/developers/applications**
3. Log in with your Discord account
   - If you don't have one, create one first

---

## 🆕 Step 2: Create a New Application

1. Click **"New Application"** button (top right)
2. Give it a name: `Discord Ray-Ban Companion` (or your choice)
3. Agree to the terms
4. Click **"Create"**

You'll now see your application dashboard.

---

## 🤖 Step 3: Add a Bot

1. On the left sidebar, click **"Bot"**
2. Click **"Add Bot"** button
3. Click "Yes, do it!" if prompted

You now have a bot user!

---

## 🔑 Step 4: Get Your Bot Token

1. Under the bot name, click **"Reset Token"** (or "Copy" if token already visible)
2. You'll see a long string - this is your **Bot Token**
3. Click **"Copy"** to copy it to clipboard

⚠️ **IMPORTANT:** Keep this token secret! Never share it or commit to GitHub.

---

## 🎯 Step 5: Set Bot Permissions

1. On the left sidebar, click **"OAuth2"**
2. Click **"URL Generator"**

3. Under "SCOPES", select:
   - ✅ `bot`

4. Under "PERMISSIONS", select:
   - ✅ `Read Messages/View Channels`
   - ✅ `Send Messages`
   - ✅ `Read Message History`
   - ✅ `View Server Members`

5. Scroll to bottom - copy the generated URL
6. Open the URL in a new browser tab
7. Select your test server
8. Click **"Authorize"**
9. Complete any CAPTCHA

**Your bot is now added to your server!** ✅

---

## 📝 Step 6: Configure Your App

Now add the token to your application:

1. Open Visual Studio
2. Find `appsettings.Development.json` in the Solution Explorer
3. Edit it:

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  },
  "Discord": {
	"BotToken": "PASTE_YOUR_TOKEN_HERE"
  }
}
```

4. Replace `PASTE_YOUR_TOKEN_HERE` with your actual token (the long string from Step 4)
5. **Save the file** (Ctrl+S)

---

## 🚀 Step 7: Run the App

1. The app should already be running
2. If not, press **F5** in Visual Studio to start it
3. Your browser should open to: **https://localhost:7000**

---

## 💬 Step 8: Use the App

1. You should see the app UI
2. You might see a login/authentication prompt
3. If prompted, enter your bot token (or it might auto-fill from config)
4. Click **"Connect"**

If successful:
- ✅ You'll see "Loading Discord data..."
- ✅ Then your servers will appear
- ✅ You can browse channels, send messages, etc.

---

## ✅ Success Checklist

After setup, verify:

- [ ] You have a Discord application created
- [ ] You have a bot in that application
- [ ] You copied your bot token
- [ ] Bot is authorized to your server
- [ ] Bot has required permissions
- [ ] Token is in `appsettings.Development.json`
- [ ] App is running
- [ ] You can see your servers in the app
- [ ] You can send messages
- [ ] You can see member status

---

## 🐛 Troubleshooting

### "Invalid Token" Error
- ✅ Copy your token again from Discord Developer Portal
- ✅ Make sure there are no extra spaces
- ✅ Paste it exactly in `appsettings.Development.json`
- ✅ Save the file
- ✅ Restart the app (F5)

### "Bot not showing in server"
- ✅ Make sure you authorized it with the URL from step 5
- ✅ Check it's in your server's member list
- ✅ Verify permissions are set correctly

### "Can't see channels"
- ✅ Make sure bot has "Read Messages" permission
- ✅ Try a different server
- ✅ Check bot is online (Discord app)

### "App won't start"
- ✅ Check `appsettings.Development.json` syntax (valid JSON)
- ✅ Press F5 to rebuild/restart
- ✅ Check error message in Debug output

### "No servers appearing"
- ✅ Bot must be in at least one server
- ✅ Use URL from OAuth2 URL Generator to add bot
- ✅ Make sure bot has guild access

---

## 📱 Testing on Different Screen Sizes

Since this is for Ray-Ban glasses (400x300px):

1. Open Developer Tools (F12)
2. Click device toggle (Ctrl+Shift+M)
3. Set dimensions to: **Width: 400, Height: 300**
4. Test navigation and buttons

The UI should be readable and touch-friendly!

---

## 🔒 Security Reminders

⚠️ **DO NOT:**
- Commit bot token to GitHub
- Share token in Discord/Chat
- Use in public code
- Leave token in Production config

✅ **DO:**
- Keep token in `appsettings.Development.json` (git-ignored)
- Regenerate token if accidentally exposed
- Use environment variables for Production
- Use User Secrets for local testing

---

## 📚 Quick Reference

| Task | Where |
|------|-------|
| Create App | https://discord.com/developers/applications |
| Add Bot | Bot section in app settings |
| Get Token | Bot section → "Copy" button |
| Set Permissions | OAuth2 → URL Generator |
| Add to Server | Use OAuth2 URL |
| Configure | `appsettings.Development.json` |
| Run App | F5 in Visual Studio |
| Test App | https://localhost:7000 |

---

## 🎉 You're All Set!

Once you've completed these steps:

1. ✅ App is running
2. ✅ Bot is connected
3. ✅ You can send Discord messages
4. ✅ Your Ray-Ban companion is ready!

**Happy messaging!** 💬

---

## 📞 Still Having Issues?

1. **Check Errors:**
   - Look at Debug output in Visual Studio
   - Check browser console (F12)

2. **Verify Token:**
   - Go back to Discord Developer Portal
   - Copy token again
   - Make sure no extra spaces

3. **Restart:**
   - Press F5 (or Shift+F5) to rebuild
   - Clear browser cache (Ctrl+Shift+Delete)
   - Try again

4. **Read Guides:**
   - `QUICK_START.md` - Setup overview
   - `DISCORD_COMPANION_README.md` - Features guide
   - `DEVELOPER_GUIDE.md` - Technical details

---

## 🚀 Next Features to Try

Once basic setup works:

- [ ] Browse different servers
- [ ] Send messages to different channels
- [ ] View direct messages
- [ ] See member online status
- [ ] Adjust settings (theme, font size)
- [ ] Test on different screen sizes
- [ ] Deploy to production

---

**Version:** 1.0.0  
**Status:** ✅ Ready to Use  
**Target:** Meta Ray-Ban Smart Glasses  

Happy coding! 🎉
