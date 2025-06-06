using System;

namespace ProjetDemineur5TTI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int TailleGrille;
            int NombreBombes;
            char[,] grille;
            bool[,] grilleDévoilée;
            bool[,] grilleMarquée;

            Console.Write("Entrez la taille de la grille (ex: 10 pour une grille 10x10) : ");
            TailleGrille = int.Parse(Console.ReadLine());

            Console.Write("Entrez le nombre de bombes : ");
            NombreBombes = int.Parse(Console.ReadLine());

            grille = new char[TailleGrille, TailleGrille];
            grilleDévoilée = new bool[TailleGrille, TailleGrille];
            grilleMarquée = new bool[TailleGrille, TailleGrille];

            InitialiserGrille(TailleGrille, NombreBombes, grille, grilleDévoilée, grilleMarquée);
            bool jeuTerminé = false;
            bool joueurAGagné = false;

            while (!jeuTerminé)
            {
                AfficherGrille(TailleGrille, grilleDévoilée, grille, grilleMarquée);

                Console.Write("Entrez les coordonnées (ligne colonne) ou 'marquer ligne colonne' : ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Entrée invalide. Veuillez entrer les coordonnées.");
                    continue;
                }

                if (input.StartsWith("marquer"))
                {
                    string[] coordonnées = input.Split(' ');
                    if (coordonnées.Length != 3 || !int.TryParse(coordonnées[1], out int ligne) || !int.TryParse(coordonnées[2], out int colonne))
                    {
                        Console.WriteLine("Entrée invalide. Veuillez entrer les coordonnées au format 'marquer ligne colonne'.");
                        continue;
                    }

                    if (ligne < 0 || ligne >= TailleGrille || colonne < 0 || colonne >= TailleGrille)
                    {
                        Console.WriteLine("Coordonnées hors limites. Veuillez entrer des coordonnées valides.");
                        continue;
                    }

                    // Marquer ou dé-marquer la case
                    if (grilleMarquée[ligne, colonne])
                    {
                        grilleMarquée[ligne, colonne] = false; // Dé-marquer
                    }
                    else
                    {
                        grilleMarquée[ligne, colonne] = true; // Marquer
                    }
                }
                else
                {
                    string[] coordonnées = input.Split(' ');

                    if (coordonnées.Length != 2 || !int.TryParse(coordonnées[0], out int ligne) || !int.TryParse(coordonnées[1], out int colonne))
                    {
                        Console.WriteLine("Entrée invalide. Veuillez entrer les coordonnées au format 'ligne colonne'.");
                        continue;
                    }

                    if (ligne < 0 || ligne >= TailleGrille || colonne < 0 || colonne >= TailleGrille)
                    {
                        Console.WriteLine("Coordonnées hors limites. Veuillez entrer des coordonnées valides.");
                        continue;
                    }

                    if (grille[ligne, colonne] == '*')
                    {
                        AfficherGrille(TailleGrille, grilleDévoilée, grille, grilleMarquée);
                        Console.WriteLine("BOOM ! Vous avez touché une bombe. Vous avez perdu !");
                        jeuTerminé = true;
                    }
                    else
                    {
                        RévélerCase(ligne, colonne, TailleGrille, grille, grilleDévoilée, grilleMarquée);
                        joueurAGagné = JoueurAGagné(TailleGrille, NombreBombes, grille, grilleDévoilée);
                        if (joueurAGagné)
                        {
                            AfficherGrille(TailleGrille, grilleDévoilée, grille, grilleMarquée);
                            Console.WriteLine("Félicitations ! Vous avez gagné !");
                            jeuTerminé = true;
                        }
                    }
                }
            }
            Console.WriteLine("Appuyez sur une touche pour quitter.");
            Console.ReadKey();
        }

        // Méthode pour initialiser la grille
        static void InitialiserGrille(int TailleGrille, int NombreBombes, char[,] grille, bool[,] grilleDévoilée, bool[,] grilleMarquée)
        {
            for (int i = 0; i < TailleGrille; i++)
            {
                for (int j = 0; j < TailleGrille; j++)
                {
                    grille[i, j] = '.'; // Représente une case non découverte
                    grilleDévoilée[i, j] = false; // Toutes les cases sont initialement non dévoilées
                    grilleMarquée[i, j] = false; // Aucune case n'est marquée au départ
                }
            }

            Random random = new Random();
            int bombesPlacées = 0;
            while (bombesPlacées < NombreBombes)
            {
                int x = random.Next(0, TailleGrille);
                int y = random.Next(0, TailleGrille);

                if (grille[x, y] != '*')
                {
                    grille[x, y] = '*'; // Représente une bombe
                    bombesPlacées++;
                }
            }

            for (int i = 0; i < TailleGrille; i++)
            {
                for (int j = 0; j < TailleGrille; j++)
                {
                    if (grille[i, j] != '*')
                    {
                        int bombesAdjacentes = CompterBombesAdjacentes(i, j, TailleGrille, grille);
                        if (bombesAdjacentes > 0)
                        {
                            grille[i, j] = bombesAdjacentes.ToString()[0]; // Convertir le nombre en caractère
                        }
                    }
                }
            }
        }

        static int CompterBombesAdjacentes(int x, int y, int TailleGrille, char[,] grille)
        {
            int compte = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue; // Ignorer la case elle-même

                    int nx = x + i;
                    int ny = y + j;

                    if (nx >= 0 && nx < TailleGrille && ny >= 0 && ny < TailleGrille && grille[nx, ny] == '*')
                    {
                        compte++;
                    }
                }
            }
            return compte;
        }

        static void AfficherGrille(int TailleGrille, bool[,] grilleDévoilée, char[,] grille, bool[,] grilleMarquée)
        {
            Console.Write("  ");
            for (int j = 0; j < TailleGrille; j++)
            {
                Console.Write(j + " "); // Afficher les numéros de colonnes
            }
            Console.WriteLine();

            for (int i = 0; i < TailleGrille; i++)
            {
                Console.Write(i + " "); // Afficher les numéros de lignes
                for (int j = 0; j < TailleGrille; j++)
                {
                    if (grilleDévoilée[i, j])
                    {
                        Console.Write(grille[i, j] + " "); // Afficher le contenu de la case dévoilée
                    }
                    else if (grilleMarquée[i, j])
                    {
                        Console.Write("F "); // Afficher 'F' pour les cases marquées
                    }
                    else
                    {
                        Console.Write(". "); // Afficher '.' pour les cases non dévoilées
                    }
                }
                Console.WriteLine();
            }
        }

        static void RévélerCase(int x, int y, int TailleGrille, char[,] grille, bool[,] grilleDévoilée, bool[,] grilleMarquée)
        {
            if (x < 0 || x >= TailleGrille || y < 0 || y >= TailleGrille || grilleDévoilée[x, y] || grilleMarquée[x, y])
            {
                return; // Sortir si les coordonnées sont invalides ou si la case est déjà dévoilée ou marquée
            }

            grilleDévoilée[x, y] = true; // Dévoiler la case

            if (grille[x, y] == '0')
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;
                        RévélerCase(x + i, y + j, TailleGrille, grille, grilleDévoilée, grilleMarquée);
                    }
                }
            }
        }

        static bool JoueurAGagné(int TailleGrille, int NombreBombes, char[,] grille, bool[,] grilleDévoilée)
        {
            int casesNonBombesDévoilées = 0;
            for (int i = 0; i < TailleGrille; i++)
            {
                for (int j = 0; j < TailleGrille; j++)
                {
                    if (grille[i, j] != '*' && grilleDévoilée[i, j])
                    {
                        casesNonBombesDévoilées++;
                    }
                }
            }
            return casesNonBombesDévoilées == (TailleGrille * TailleGrille - NombreBombes);
        }
    }
}