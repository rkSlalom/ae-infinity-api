# SignalR Testing Guide

## Quick Diagnostic Checklist

### 1. Check Browser Console (Frontend)
Look for these log messages in order:

```
🔑 SignalR starting with token: eyJhbGciOiJIUzI1Ni...
🌐 SignalR Hub URL: http://localhost:5233/hubs/shopping-list
🚀 SignalR attempting to connect...
✅ SignalR Connected! Connection ID: [some-id]
📡 SignalR subscribing to event: ListCreated
📡 SignalR subscribing to event: ListUpdated
📡 SignalR subscribing to event: ListDeleted
📡 SignalR subscribing to event: ListArchived
🔔 ListsDashboard: Subscribing to real-time list events
```

**If you see "NO TOKEN"**: Auth token is not being retrieved correctly
**If connection fails**: Check CORS, backend running, or network issues

### 2. Check Backend Console (API)
When a list is created, you should see:

```
Broadcasting ListCreated event for list {guid} to all connected users
```

### 3. Visual Indicator
Check the top-right of the Lists Dashboard:
- 🟢 Live = Connected ✅
- 🟡 Connecting/Reconnecting = In progress
- 🔴 Offline = Not connected ❌

## Testing Real-Time Updates

### Test 1: Two Browser Windows
1. Open two browser windows (or use incognito)
2. Login with different accounts in each
3. Both should show 🟢 Live indicator
4. Create a list in Window 1
5. Within 2 seconds, the list should appear in Window 2

### Test 2: Console Monitoring
1. Open browser DevTools (F12)
2. Go to Console tab
3. Create a list
4. You should see:
   ```
   📨 SignalR received event: ListCreated {listId: "...", list: {...}, timestamp: "..."}
   ```

## Common Issues & Fixes

### Issue: "NO TOKEN" in console
**Fix**: Check localStorage for `auth_token` key
```javascript
localStorage.getItem('auth_token')
```

### Issue: Connection fails with 401 Unauthorized
**Fix**: Token is invalid or expired - logout and login again

### Issue: Connection fails with 404
**Fix**: Backend not running or wrong URL
- Check: http://localhost:5233/hubs/shopping-list
- Verify: Backend is running on port 5233

### Issue: Connected but no events received
**Fix**: Check backend logs - events might not be broadcasting
```
grep "Broadcasting" logs.txt
```

### Issue: Events received but list not updating
**Fix**: Check frontend event handler logic in ListsDashboard.tsx

## Manual API Test

Test backend broadcasting manually:

```bash
# 1. Login to get token
curl -X POST http://localhost:5233/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"sarah@example.com","password":"Password123!"}'

# 2. Create a list (should broadcast ListCreated event)
curl -X POST http://localhost:5233/api/lists \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -d '{"name":"Test List","description":"Testing SignalR"}'
```

If SignalR is working, all connected clients will receive the event.

