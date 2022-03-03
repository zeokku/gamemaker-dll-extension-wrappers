# Gamemaker DLL extension wrappers
C# and C++ wrappers to develop Gamemaker DLL extensions easily

Don't forget to define `RegisterCallbacks(int, int, int, int)` in your extension's functions in Gamemaker Studio.

# C#
Just drop `.cs` file into project, then import namespace `using Gamemaker.Internal;`

Map can be edited with [] notation or via `AddKeyValuePair()` method. Brackets allow any value type (incl. other DsMaps), floating and integral (up to 32-bit) types will be stored as doubles, booleans will turn into 0/1, `null` will be empty string, other DsMaps will be stored as their ID number (double), everything else, the `ToString()` method will be called on, **including 64-bit integers**. So for complex object types it's handy to define `ToString()` method which results with a JSON representation which can be then parsed back in Gamemaker if needed.

`DsMap.Dispatch` has several overloads, so you can not only define callback type but also result type (default is `_type_` key) as string or number, which can be useful for determining what kind of information is returned.

In the end use [DLLExport](https://github.com/3F/DllExport) to generate export signatures.


## Example:

```c#
DsMap result = new DsMap((int)EAsyncSteamType.leaderboardDownload);

result["success"] = true;

DsMap entries = new DsMap();
result["entries"] = entries;

result.Dispatch(EGmsEventType.EVENT_ASYNC_STEAM);
```

# C++
Add `gamemaker.cpp` and `gamemaker.h` to your project, then `#include "gamemaker.h"` in your project and `using namespace gamemaker;`

It's pretty similar to C# version but being a little outdated, so there's some inconsistency, so result types are defined in constructor and there're no brackets editing, only `set()` method

## Example:
```c++
ds_map report("_result_", result_status);
report.set("value", (double)10);
bool dispatch_res = report.dispatch(gml_event_type::social);
```
