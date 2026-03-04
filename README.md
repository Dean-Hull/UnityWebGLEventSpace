# UnityWebGLEventSpace

The Unity WebGL 3D interactive environment component of the [LeadXP Virtual Event Platform](https://github.com/Dean-Hull/virtual-event-platform). Renders an explorable virtual booth space embedded directly in the browser via WebGL, communicating with the Next.js host application through a JavaScript bridge.

## Unity Version
<img src="https://img.shields.io/badge/Unity-6000.0.54f1-blue.svg?style=flat&logo=unity" alt="Unity-6000.0.54f1">

## Features

- Interactive 3D Environment: Orbit, pan, and zoom a virtual event floor using touch and mouse input
- Speaker Booths: Clickable speaker objects that trigger navigation in the host app
- Camera Focus: Smooth camera transitions to focus on selected booths
- Browser Bridge: Two-way communication with the surrounding Next.js app via `jslib` interop
- WebGL Build: Optimised export for embedding in a web page as an `<iframe>` or Unity WebGL loader

## Architecture

This project acts as the 3D viewport embedded inside the LeadXP platform. When a user clicks a speaker booth:

1. `Speaker.OnMouseDown` fires → calls `BrowserBridge.ObjectClicked(id)`
2. `BrowserBridge` calls the `NotifyObjectClicked` extern, invoking a JavaScript function in the host page
3. The Next.js host receives the event and navigates to the selected speaker's profile

## Project Structure

```
Assets/
└── Scripts/
    ├── BrowserInteraction/
    │   └── BrowserBridge.cs       # JS interop — sends click events to the host page
    ├── Camera/
    │   ├── CameraController.cs    # Orbit / pan / zoom with touch & mouse
    │   └── CameraClickEvent.cs    # Base class; focuses camera on a clicked object
    ├── Interaction/
    │   └── ClickHandler.cs        # Generic click → BrowserBridge
    ├── Managers/
    │   └── SelectionManager.cs    # Singleton registry of active Speaker instances
    └── Speaker/
        └── Speaker.cs             # Registers with SelectionManager; handles booth clicks
```

## Getting Started

### Prerequisites

- Unity 6000.0.54f1 (Unity 6)
- WebGL Build Support module installed

### Opening the Project

1. Open Unity Hub
2. Click Add -> Add project from disk
3. Select the `UnityWebGLEventSpace` folder
4. Open the `Assets/Scenes/App.unity` scene

### Building for WebGL

1. File -> Build Settings
2. Select WebGL platform (install the module if prompted)
3. Click Switch Platform
4. Click Build and output to `Builds/`

The resulting build can be served alongside the Next.js app or hosted on a CDN, with the URL set via `NEXT_PUBLIC_BLOB_URL` in the platform's `.env.local`.

## Coding Conventions

| Type      | Convention                                                    |
|-----------|---------------------------------------------------------------|
| Public    | `PascalCase`                                                  |
| Private   | `_camelCase`                                                  |
| Internal  | `camelCase`                                                   |
| Constant  | `SCREAMING_SNAKE_CASE`                                        |
| Interface | `IInterface`                                                  |
| Enum      | Type: `PascalCase` · State: `SCREAMING_SNAKE_CASE`            |

---