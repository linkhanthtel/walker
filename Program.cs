using System;
using System.Collections.Generic;

class Program
{
    static int score = 0;
    static int level = 1;
    static readonly int coinsRequired = 5;
    static int playerHealth = 100;
    static List<(int x, int y)> coins = new List<(int x, int y)>();
    static List<(int x, int y)> walls = new List<(int x, int y)>();
    static List<(int x, int y, int dx, int dy)> enemies = new List<(int x, int y, int dx, int dy)>();
    static Random random = new Random();

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        int playerX = 2;
        int playerY = 2;
        char playerSymbol = 'L';
        char coinSymbol = '$';
        char wallSymbol = '+';
        char enemySymbol = 'X';
        bool isGameRunning = true;

        Console.WriteLine("Welcome to the Walking Game!");
        Console.WriteLine("L - Player | $ - Coins | + - Walls | X - Enemies");
        Console.WriteLine("Collect coins to advance to next level.");
        Console.WriteLine("Press any key to start...");
        Console.WriteLine("Press Q to quit.");
        Console.ReadKey(true);

        InitializeLevel();

        while (isGameRunning)
        {
            Console.Clear();
            // Draw game info
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Level: {level} | Score: {score} | Health: {playerHealth} | Coins needed: {coinsRequired - coins.Count}");

            // Draw enemies
            foreach (var enemy in enemies)
            {
                Console.SetCursorPosition(enemy.x, enemy.y + 2);
                Console.Write(enemySymbol);
            }

            // Draw walls
            foreach (var wall in walls)
            {
                Console.SetCursorPosition(wall.x, wall.y + 2);
                Console.Write(wallSymbol);
            }

            // Draw coins
            foreach (var coin in coins)
            {
                Console.SetCursorPosition(coin.x, coin.y + 2);
                Console.Write(coinSymbol);
            }

            // Draw player
            Console.SetCursorPosition(playerX, playerY + 2);
            Console.Write(playerSymbol);

            // Handle input
            var key = Console.ReadKey(true).Key;
            int newX = playerX;
            int newY = playerY;

            switch (key)
            {
                case ConsoleKey.LeftArrow: newX--; break;
                case ConsoleKey.RightArrow: newX++; break;
                case ConsoleKey.UpArrow: newY--; break;
                case ConsoleKey.DownArrow: newY++; break;
                case ConsoleKey.Q: isGameRunning = false; break;
            }

            // Check collision with walls
            bool canMove = true;
            foreach (var wall in walls)
            {
                if (wall.x == newX && wall.y == newY)
                {
                    canMove = false;
                    break;
                }
            }

            // Update position if valid
            if (canMove && newX > 0 && newX < Console.WindowWidth - 1 && 
                newY > 0 && newY < Console.WindowHeight - 3)
            {
                playerX = newX;
                playerY = newY;
            }

            // Check coin collection
            for (int i = coins.Count - 1; i >= 0; i--)
            {
                if (coins[i].x == playerX && coins[i].y == playerY)
                {
                    coins.RemoveAt(i);
                    score += 10 * level;
                }
            }

            // Check level completion
            if (coins.Count == 0)
            {
                level++;
                InitializeLevel();
                playerX = 2;
                playerY = 2;
            }

            // Move enemies
            MoveEnemies();

            // Check enemy collision
            foreach (var enemy in enemies)
            {
                if (enemy.x == playerX && enemy.y == playerY)
                {
                    playerHealth -= 25;
                    if (playerHealth <= 0)
                    {
                        isGameRunning = false;
                        break;
                    }
                }
            }
        }

        Console.Clear();
        Console.WriteLine($"Game Over! Final Score: {score}");
        Console.WriteLine($"You reached level {level}");
    }

    static void MoveEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];
            int newX = enemy.x + enemy.dx;
            int newY = enemy.y + enemy.dy;

            // Bounce off walls
            if (newX <= 0 || newX >= Console.WindowWidth - 1)
            {
                enemy.dx *= -1;
                newX = enemy.x + enemy.dx;
            }
            if (newY <= 0 || newY >= Console.WindowHeight - 4)
            {
                enemy.dy *= -1;
                newY = enemy.y + enemy.dy;
            }

            enemies[i] = (newX, newY, enemy.dx, enemy.dy);
        }
    }

    static void InitializeLevel()
    {
        coins.Clear();
        walls.Clear();

        // Add coins
        for (int i = 0; i < coinsRequired; i++)
        {
            int x, y;
            do
            {
                x = random.Next(1, Console.WindowWidth - 1);
                y = random.Next(1, Console.WindowHeight - 4);
            } while (coins.Exists(c => c.x == x && c.y == y));
            coins.Add((x, y));
        }

        // Add walls (more walls for higher levels)
        int wallCount = 5 + (level * 2);
        for (int i = 0; i < wallCount; i++)
        {
            int x, y;
            do
            {
                x = random.Next(1, Console.WindowWidth - 1);
                y = random.Next(1, Console.WindowHeight - 4);
            } while (coins.Exists(c => c.x == x && c.y == y) || 
                     walls.Exists(w => w.x == x && w.y == y) ||
                     (x == 2 && y == 2)); // Don't place wall at starting position
            walls.Add((x, y));
        }

        // Add enemies (more enemies for higher levels)
        enemies.Clear();
        int enemyCount = 2 + level;
        for (int i = 0; i < enemyCount; i++)
        {
            int x, y;
            do
            {
                x = random.Next(5, Console.WindowWidth - 5);
                y = random.Next(5, Console.WindowHeight - 7);
            } while (coins.Exists(c => c.x == x && c.y == y) || 
                     walls.Exists(w => w.x == x && w.y == y));
            
            int dx = random.Next(2) * 2 - 1; // -1 or 1
            int dy = random.Next(2) * 2 - 1; // -1 or 1
            enemies.Add((x, y, dx, dy));
        }
    }
}
