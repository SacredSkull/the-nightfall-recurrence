# Unity Nightfall Incident Port

If you haven't heard of The Nightfall Incident, it is an
[excellent turn based strategy flash game](http://jayisgames.com/games/spybot-the-nightfall-incident/) 
that lego sponsored a games company to make, way back in 2003.

I hope to emulate it (in meaning of "_to match or surpass, typically by imitation_") offering
extensibility, in terms of levels and programs/enemies. I'm also thinking about an online repo for levels/enemies and other customisations that will be accessible in game.

I'm currently using [Tiled](https://github.com/bjorn/tiled) to store and create levels because it's fairly accessible (therefore allowing user created content) and is quite mature.

## Progress report
A while ago I completely overhauled the project to make it more maintainable and in doing so, I began using a MVC (Model-View-Controller for those not in the web develosphere) framework simply called [MVC](https://bitbucket.org/eduardo_costa/thelab-unity-mvc). Unfortunately I found that framework a little restrictive for my tastes and decided to change. Now I'm using, and have contributed to, a project called [Karma](https://github.com/cgarciae/karma), which was created by a web developer and is more or less exactly what I was looking for. Also, dependency injection. Nice!

The bulk of the initial work I did on moving to the first MVC framework was directly useful for Karma, but inevitably I had to discard some. I last worked on this project a couple of months ago (committed, but not pushed) but have wanted to finish the code for my blog update, which is now complete, until I change my own mind again. This should be my free time programming focus but I'm going into my final year at university, so there may not be much of that (unless I can weave it into an assignment somehow).

## Todo
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
