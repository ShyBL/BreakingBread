# BreakingBread
<video src="https://github.com/ShyBL/BreakingBread/assets/78258828/517429dd-eb93-4e96-884e-b133007503a0" width=180><video/>

## Overview

This project is a Unity C# idle mobile game that serves as a showcase of programming architecture principles, particularly focusing on the application of SOLID principles. 
The game utilizes third-party packages such as Firebase for backend functionalities, and DoTween for smooth animations.

### Prerequisites

This project uses Unity (version 2022.3.2f1), and the Firebase SDK (version 11.6). 
For other packages, see the manifest file.

## Features

### Showcase of SOLID principles in game architecture

IdleBakery exemplifies SOLID principles in its design:
- **Single Responsibility Principle:** Core logic resides in managers, separating it from gameplay logic in components.
- **Liskov’s Substitution Principle:** Features are divided into subclasses or interfaces, ensuring interchangeable components.
- **Interface Segregation Principle:** The interface is designed to be specific to the needs of its clients, avoiding unnecessary dependencies.

### Idle gameplay mechanics

- **Upgrade Manager:** Holds data about main gameplay features, generating baked goods based on manual or per-tick generation.
- **Research Manager:** Amplifies generation through a multiplier or cap on off-screen production.
- **Milestone Manager:** Manages current milestones, awarding special currency.

### Firebase integration for backend services

- **Config Manager:** Utilizes Remote Configuration to fetch values for gameplay managers.
- **Monitor Manager and Analytics Manager:** Wrappers for the Crashlytics and Analytics modules.

#### Smooth animations using DoTween
#### Editor tools inspect non-MonoBehaviour objects during runtime.
#### Menu tools for streamlining production workflows.


## Tech Design Board

![Idle Bakery Tech Design](https://i.imgur.com/QUthGjT.jpg)

