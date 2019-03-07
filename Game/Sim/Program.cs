using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim
{
    class Program
    {
        static void Main(string[] args)
        {
            bool exit = false;
            string option;

            while (exit != true)
            {
                Console.WriteLine("Quieres crear un nuevo jugador? (s/n) ");
                option = Console.ReadLine();

                if (option == "s")
                {
                    joinPlayer();
                }
                else
                {
                    exit = true;
                }
            }
        }

        static void joinPlayer()
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Users\Usuari1\Documents\GitHub\M09-UF3_Practica_1\Game\Game\Game\bin\Debug\CatchGame.exe";
            process.Start();
        }
    }
}
