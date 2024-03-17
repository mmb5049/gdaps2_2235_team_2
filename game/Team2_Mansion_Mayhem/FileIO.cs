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
        private const string FilePath = "scores.txt"; // Store scores to get highscore

        public static int GetHighScore()
        {
            int highScore = 0;

            try
            {
                if (File.Exists(FilePath))
                {
                    string[] lines = File.ReadAllLines(FilePath);
                    if (lines.Length > 0)
                        highScore = int.Parse(lines[0]); // Assume the first line is the high score
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading high score: {ex.Message}");
            }

            return highScore;
        }

        public static void SaveScore(int score)
        {
            try
            {
                // Check if file exists, otherwise create a new file
                if (!File.Exists(FilePath))
                {
                    using (StreamWriter sw = File.CreateText(FilePath))
                    {
                        sw.WriteLine(score); // Write the score to the file
                    }
                }
                else
                {
                    // Read current high score
                    int existingHighScore = GetHighScore();

                    // Compare with the new score and update if necessary
                    if (score > existingHighScore)
                    {
                        using (StreamWriter sw = new StreamWriter(FilePath, false))
                        {
                            sw.WriteLine(score); // Write the new high score to the file
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving score: {ex.Message}");
            }
        }
    }
}
