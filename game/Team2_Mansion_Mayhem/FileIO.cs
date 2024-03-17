using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Team2_Mansion_Mayhem
{
    internal class FileIO
    {
        private const string HighScoreFileName = "highscore.txt"; // Add file and change path
        private const string StatsFileName = "stats.txt"; // Add file and change path

        // Read the highscore
        public static int ReadHighScore()
        {
            int highScore = 0;

            // Check if the highscore file exists
            if (File.Exists(HighScoreFileName))
            {
                // Read the contents and convert text to an integer
                string scoreText = File.ReadAllText(HighScoreFileName);
                int.TryParse(scoreText, out highScore);
            }

            return highScore;
        }

        // Write the highscore
        public static void WriteHighScore(int score)
        {
            if (File.Exists(HighScoreFileName))
            {
                string scoreText = File.ReadAllText(HighScoreFileName);
                int highScore = 0;
                int.TryParse(scoreText, out highScore);

                // Check if the new score is higher than the existing highscore
                if (score > highScore)
                {
                    // Write the new highscore
                    File.WriteAllText(HighScoreFileName, score.ToString());
                }
            }
            else
            {
                // If the file doesn't exist, create a new one and write the new highscore
                File.WriteAllText(HighScoreFileName, score.ToString());
            }
        }

        // Save the player and enemies' stats
        public static void SaveStats(string stats)
        {
            // Write the stats
            File.WriteAllText(StatsFileName, stats);
        }

        // Read and return stats from the stats file
        public static (string name, int health, int defense, int damage, int speed) ReadStats()
        {
            string name = "";
            int health = 0, defense = 0, damage = 0, speed = 0;

            // Format of Stats file:
            // Line int: name,defense,damage,speed
            //
            // Example:
            // Line 0: Player,100,50,10,5

            try
            {
                using (StreamReader sr = new StreamReader(StatsFileName))
                {
                    string line;

                    // Read each line
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Split the line into parts by commas
                        string[] parts = line.Split(',');
                        if (parts.Length == 5)
                        {
                            // Assign parts to corresponding variable
                            name = parts[0].Trim();
                            health = int.Parse(parts[1].Trim());
                            defense = int.Parse(parts[2].Trim());
                            damage = int.Parse(parts[3].Trim());
                            speed = int.Parse(parts[4].Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error reading stats file: " + ex.Message);
            }

            return (name, health, defense, damage, speed);
        }
    }
}
