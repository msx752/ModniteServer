# Modnite Server
Modnite Server is a private server for Fortnite Battle Royale. Since Fortnite Battle Royale is an online PvP game, there is no way to mod the game without disrupting other players. With Modnite Server, players can host their own server and be free to mod the game without ruining the game for everyone else.

We ❤️ Fortnite and hope this creates an exciting new experience for the game.

![Server screenshot](/docs/screenshot.png)
![Lobby screenshot](/docs/Version6_10.png)

# Milestones
✔️ Enable editing of game files and disable TLS

✔️ Reach the lobby

▶️ IN PROGRESS: Get into a match alone

➖ Very limited multiplayer support

# Releases
Go to [Releases](https://github.com/ModniteNet/ModniteServer/releases) for compiled binaries.

# FAQs
### What can I do with a private server?
At this time, Modnite Server lets you:
* Get to the lobby
* Equip anything you want

This is a very early preview. There's not much you can do at this time.

### How do I get Fortnite to connect to my server?
[Follow this guide](https://www.modnite.net/guides/easy-way-connect-to-a-private-server-using-modnite-patcher-r8/).

### How do I create accounts?
By default, Modnite Server will automatically create new accounts. Simply login with the email `username@modnite.net`. If the username does not exist, a new account will be created.

For example, logging in as `wumbo@modnite.net` will create a new account with the display name set to `wumbo`.
![Login example](/docs/login.PNG)

Alternatively, you may use the `create` command. For example, `create player123 hunter2`.

### Isn't Epic Games testing a custom matchmaking system?
Sure, but that's not really the point of this project. We want to enable players to mod the game without interfering with other players. Given the PvP nature of Battle Royale, it's unlikely Epic Games would allow modding at all.

### How do I change the item shop?
To mitigate clickbait and scams, access to the item shop and V-Bucks store is restricted. Modnite Server maintains a perpetual item shop consisting of only default skins with all prices set to 0 V-Bucks. We may eventually open-source the item shop feature, but that won't happen any time soon.

### Will you ever add support for Save the World?
Save the World is off limits until it becomes free.

### How can I contribute?
Visit the [Reverse Engineering forum on Modnite.net](https://www.modnite.net/forum/17-reverse-engineering/) to participate. Set `LogHttp` and `Log404` to `true` in `config.json` to enable logging of HTTP requests which will help you implement controllers and routes for the missing features. Pull requests are also welcome.
