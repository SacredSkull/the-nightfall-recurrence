#Unity Nightfall Incident Port

If you haven't heard of The Nightfall Incident, it is an
[excellent turn based strategy flash game](http://jayisgames.com/games/spybot-the-nightfall-incident/) 
that lego sponsored a games company to make, way back in 2003.

I hope to emulate it (in meaning of "_to match or surpass, typically by imitation_") offering
extensibility, in terms of levels and programs/enemies. I'm also thinking about an online repo for levels/enemies and other customisations that will be accessible in game.

I'm currently using [Tiled](https://github.com/bjorn/tiled) to store and create levels because it's fairly accessible (therefore allowing user created content) and is quite mature.



##Todo
- Storage/metadata
  - ~~Programs~~ (software tools)
  - ~~Vanilla program information~~ (stats, attacks, etc.)
  - ~~Levels~~
  - ~~Vanilla map features~~ (Credits with different values aren't currently possible because of [this issue](https://github.com/bjorn/tiled/issues/31) - I may consider modifiying Tiled or the XML to achieve this)
  - Vanilla levels (the levels from the original)
  - Saving progress
- Rendering
  - ~~Map geometry~~
  - ~~Programs~~
- AI
  - ~~Movement~~ (A*)
  - ~~Turn decision making~~
- GUI
  - Single level
    - ~~Grids~~
    - Overlay (attack range, etc.)
    -
  - Level selection