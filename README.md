# EvOLuTIoN
Evolutionary pathfinding in Unity using Genetic Algorithm


YOUTUBE VIDEO: https://www.youtube.com/watch?v=yCz87N6qaRs
<br><br>

![](gifs/genalg1.gif)
![](gifs/genalg3.gif)
![](gifs/genalgpic.png)


## Getting Started

If you don't want to use Unity you can just download the build version from here:
```
https://drive.google.com/open?id=1QLUNkrD71T5zEbFOk7X2CuQWUCFs-gjK
```
Otherwise just open the folder in Unity as a project in order for it to work.

## Usage

You can change levels by pressing `Shift + 1/2/3`

The green cube is the best player from previous generation.

#### The Game Menu allows you to play with a couple of parameters:
- Number of players
- Game speed - up to x15 so you can train them almost instantly
- Jumping ability
- Rotation
- Movement speed
- Maximum Speed


#### The right combination of parameters is needed for effectiveness of the algorithm
```
For example, turning on rotation and increasing movement speed might make impossible 
for cubes to find the goal. In a situation like that, the physics engine makes their moves a bit random so 
they can't memorise the way correctly
```

## The Code

You can find the algorithm scripts in `Assets>>Scripts>>AlgorithmScripts`

Each player spawns with an instance of `Player.cs` script that controls his behaviour.

`Population.cs` contains the Genetic Algorithm applied by NaturalSelection() function.
```

    void NaturalSelection()
    {
        SetChampion();  //finds the best player from generation

        CalculateFitness();
        CalculateFitnessSum();

        CopyBrain(Players[0], champion);    //best player is always reborn in next generation unchanged as player0

        for (int i = 1; i < playerNum; i++)
        {
            GameObject parent = SelectParent();
            CopyBrain(Players[i], parent);
            Mutate(Players[i]);
        }
        
    }
```

Each player has only one parent, no Crossover is used.

#### Other mechanics:
- Incremental learning is applied, which means each player has a certain lifespan which is increased each 5 generations. This is to give them time to master their first moves before moving on to the next ones.
- When the first player reaches the goal, the minStep variable is assigned which is the minimum of steps he needed to take to reach the goal. From now on, if any player takes more steps then minStep, he will die. This makes them optimise their way so they get to the goal faster each generation.
- The jumping is enabled just by giving players the abilty to be mutated on Y axis (they're mutated only on X and Z by default).

