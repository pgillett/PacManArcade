using System;
using System.Collections.ObjectModel;
using System.Linq;
using PacManArcadeGame.Helpers;

namespace PacManArcadeGame.GameItems
{
    public class BonusFruit
    {
        public readonly Location Location;
        public Fruit Type;
        public bool ShowAsScore;

        private int _tickCounter;

        public bool ShowAsFruit => _tickCounter > 0 && !ShowAsScore;

        public BonusFruit(Location location)
        {
            Location = location;
            ShowAsScore = false;
        }

        public void SetLevel(int level)
        {
            Type = FruitFromLevel(level);

            FruitList = Enumerable.Range(Math.Max(0, level - 6), Math.Min(7, level + 1))
                .Select(FruitFromLevel)
                .ToList().AsReadOnly();
        }

        private Fruit FruitFromLevel(int level)
        {
            if (level < 2)
            {
                return (Fruit)level;
            }
            if (level > 12)
            {
                return Fruit.Key5000;
            }
            return (Fruit)(level / 2 + 1);
        }

        public ReadOnlyCollection<Fruit> FruitList;

        public void Tick(int coinsEaten)
        {
            if (_tickCounter > 0)
            {
                _tickCounter--;
            }
            else if (coinsEaten == 70 || coinsEaten == 170)
            {
                _tickCounter = 7 * 60;
                ShowAsScore = false;
            }
            else
            {
                ShowAsScore = false;
            }
        }

        public void Eaten()
        {
            _tickCounter = 2 * 70;
            ShowAsScore = true;
        }

        public int Score => Type switch
        {
            Fruit.Cherry100 => 100,
            Fruit.Strawberry300 => 300,
            Fruit.Orange500 => 500,
            Fruit.Bell700 => 700,
            Fruit.Apple1000 => 1000,
            Fruit.Grapes2000 => 2000,
            Fruit.Arcadian3000 => 3000,
            Fruit.Key5000 => 5000,
            _ => 0
        };
    }
}
