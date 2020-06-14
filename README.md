[![License: MIT](https://img.shields.io/badge/License-MIT-greed.svg)](LICENSE)

## Features
- City limits:
    - Create city boundary mesh.
    - Create void effect beyond city boundary.
    - Smooth terrain beyond city boundary.

----

## Dependencies
- [Utilities](https://github.com/Besjan/Utilities)
- [MessagePack](https://github.com/neuecc/MessagePack-CSharp)
- [Geo](https://gist.github.com/Besjan/64b8ddbfd74d9ed7fc438c502bd7d257)
- [High Definition RP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@9.0/manual/index.html)
    - Particle System Shader Samples
- [Archimatix Pro](https://assetstore.unity.com/packages/tools/modeling/archimatix-pro-59733)
- [Odin - Inspector and Serializer](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)

----

## Notes
- If there are conflicts with "System.Runtime.CompilerServices.Unsafe.dll" from Unity Collections package, copy the later from ".../Library/PackageCache" to "Packages" and delete "System.Runtime.CompilerServices.Unsafe.dll".
