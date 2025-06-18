# SNAP GAME MOD TEMPLATE
# DOCUMENTATION
-- USE UNITY 6 (6000.0.40F1), OTHERWISE PROJECT MAY DIFFER FROM THE ORIGINAL. --


# 📂 WHERE TO PUT MY MOD FILES 📂
Put all your mod files into the "Mod" folder in the assets.
-Prefabs: all the prefabs used in your mod.
-Scripts: all the scripts used in your mod.
-Models: all the models used in your mod (Preferably in .fbx format).
-Materials&Textures: all the materials and the textures used in your mod.

# 🕹️ HOW TO RUN A SESSION 🕹️ 
1. Build the game.
2. Run the instance in the Unity Editor and run your game build.
3. Press "Server" to host a room without being a player OR press "Host" to host a room while also being a player (Preferably in the Unity Editor). Optionally, customize the IP on which server will be running.
4. Press "Client" on the instance you want to be client on. Optionally, customize the IP of the server to which you want to connect.

# 🥎 WHERE TO GET COMMON NETWORK OBJECTS 🥎 
All the common objects are in the "Objects" folder in the assets.
To spawn the object during the game - you need to drag object's prefab into the scene in the unity editor, it will be automaticly spawned across the network (Server, host and clients), if you want to avoid it being spawned across the network and exists only locally - switch off the script "SpawnAlongNetwork.cs".

# ❓HOW TO CREATE YOUR OWN NETWORK OBJECT❓ 
1. Create a folder named after your object in the "Objects" inside your "Mod" folder in the assets.
2. Drag all the neccessary assets for your object.
3. Add components NetworkObject and NetworkTransform.
4. Tick off the unneccessary synchronisations in the NetworkTransform.
5. Optionally, add the collider and the rigidbody to your object.
6. Optionally, add your scripts (Not "MonoBehaviour", but "NetworkBehaviour").
7. Optionally, add "SpawnAlongNetwork" script to the object.

#  💬 HOW TO USE CHAT 💬 
Chat was not added in the mod template yet, it will come later.

# 📄 ADDITIONAL DOCS 📄 
Unity 6: https://docs.unity.com/en-us
Unity Netcode for GameObjects: https://docs-multiplayer.unity3d.com/netcode/current/installation/index.html
C#: https://learn.microsoft.com/en-us/dotnet/csharp/
