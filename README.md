# Gamemaker DLL extension wrappers
C# and C++ wrappers to develop Gamemaker DLL extensions easily

Don't forget to define `RegisterCallbacks(int, int, int, int)` in your extension's functions in Gamemaker Studio.

# C#
Just drop `.cs` file into project, then import namespace `using static Gamemaker.Internal;`

Map can be edited with [] notation or via `AddKeyValuePair()` method. Brackets allow any value type, floating and integral (up to 32-bit) types will be stored as doubles, booleans will turn into 0/1, everything else, the `ToString()` method will be called on, **including 64-bit integers**. So for complex object types it's handy to define `ToString()` method which results with a JSON representation which can be then parsed back in Gamemaker if needed.

`DsMap.Dispatch` comes with an optional argument to define `_type` property's value, which can be useful for determining what kind of information is returned.

In the end use [DLLExport](https://github.com/3F/DllExport) to generate export signatures.


## Example:

```c#
DsMap map = new DsMap();

map["id"] = user.Id;
map["name"] = user.Username;
map["discriminator"] = user.Discriminator;
map["nitro"] = hasNitro;

map.Dispatch("discord_init");
```

# C++
