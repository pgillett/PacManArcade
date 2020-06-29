# PacMan arcade replica

PacMan in C# with Windows and Blazor (WASM) front end.

Play at https://pgillett.github.io/pacmanplay

C - Credit
1 - Start (after credit)
Cursor keys - movement

P - Pause
R - Reset
F - Fast forward (skip frames)
I - Invincible
T - Tick (when paused)
L - Level skip

While working with Kev and David on https://github.com/YorkCodeDojo/PacMan (a C# PacMan development livestream with an emphasis on test driven design) I decided to see to work on this side project to replicate the arcade machine as closely as possible.

As I had a background in 8-bit development, it was a enjoyable challenge.

It's fairly complete, though the sound and intermission cut-scenes are missing. The attract playing mode could do with some work, but thanks to David for the AI code ideas that replaced my original listed moves.

It's not the neatest code and could do with some serious refactoring, but my ultimate goal, with limited time resources, was for it to play/look right.

Although it's loosely coupled to the original format, it is driven from the Board.txt and LevelSetup.cs file while can be swapped for a different layout (even a different size).